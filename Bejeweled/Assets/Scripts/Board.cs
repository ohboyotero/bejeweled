using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour, InputCoordinator {

  // The gem prefab that should be instantiated for new gems.
  public Gem gemPrefab;

  // The sprites to be used for each type of gem, indexed by gem type enum.
  public Sprite[] gemSprites;

  // The number of rows of gems in the board.
  public int rows;

  // The number of columns of gems in the board.
  public int columns;

  // The game's timer object.
  public Timer timer;

  // The game's scorer object.
  public Scorer scorer;

  // Spacing between gems, in pixels.
  private float spacing;

  // The gems that make up the board.
  private Gem[,] gems;

  // The first click of a gem swap interaction.
  private Point firstClick;

  // All active gem tweens currently in progress.
  //
  // DO NOT ADD DIRECTLY TO THIS SET. Use the private method AddTween() instead.
  private HashSet<Tween> tweens;

  // Whether or not play is currently active.
  private bool boardActive;

  private const float MOVE_SPEED = 4f;

  void Start() {
    boardActive = true;
    gems = new Gem[rows, columns];
    spacing = -transform.position.x / 4;
    tweens = new HashSet<Tween>();
    firstClick = null;
    FillRandomAssShit();

    timer.Expired += HandleTimerExpired;
    timer.TimerStart();
  }

  void Update() {
    if (tweens.Count > 0) {
      List<Tween> finishedTweens = new List<Tween>();
      foreach (Tween tween in tweens) {
        tween.Update();
        if (tween.Finished) {
          finishedTweens.Add(tween);
        }
      }
      foreach (Tween tween in finishedTweens) {
        tweens.Remove(tween);
      }
    } else {
      if (HandleMatches()) {
        RefillBoard();
      }
    }
  }

  public void NotifyClick(Gem target) {
    if (!ShouldAcceptInput()) {
      Debug.Log("Ignoring input because we have active tweens.");
      return;
    }

    for (int i = 0; i < gems.GetLength(0); ++i) {
      for (int j = 0; j < gems.GetLength(1); ++j) {
        if (gems[i, j] == target) {
          if (firstClick == null) {
            firstClick = new Point(i, j);
          } else {
            AttemptSwap(firstClick.x, firstClick.y, i, j);
          }
          return;
        }
      }
    }

    Debug.LogError("Couldn't find gem corresponding to click.");
  }

  private void HandleTimerExpired(float secondsRemaining) {
    boardActive = false;

    // TODO: handle game over.
  }
  
  private bool ShouldAcceptInput() {
    return tweens.Count == 0 && boardActive;
  }
    
  private void AttemptSwap(int firstX, int firstY, int secondX, int secondY) {
    // If the two click locations are neighbors (total manhattan distance is exactly one),
    // attempt the swap.
    if (Mathf.Abs(firstX + firstY - secondX - secondY) == 1) {
      Gem first = gems[firstX, firstY];
      Gem second = gems[secondX, secondY];

      // No matter what, we want to tween the attempt so the UX is responsive. In other words,
      // even for a failed swap attempt, the gem should move to the user-desired position (though
      // we'll just move it right back to indicate you can't do that).

      Tween firstTween = new LinearTween(first.transform, second.transform.position, MOVE_SPEED);
      Tween secondTween = new LinearTween(second.transform, first.transform.position, MOVE_SPEED);

      // We only actually update game state if it's a valid move.
      if (IsValidSwap(firstX, firstY, secondX, secondY)) {
        Debug.Log("Swapped!");
        gems[secondX, secondY] = first;
        gems[firstX, firstY] = second;
        tweens.Add(firstTween);
        tweens.Add(secondTween);
      } else {
        // If it's not a valid move, add more tweens to undo the initial tweens once they've
        // completed.
        Debug.Log("adding on finished anti-tweens");
        MultiTween roundTripTween = new MultiTween();
        roundTripTween.AddTween(firstTween);
        roundTripTween.AddTween(firstTween.Reverse());
        tweens.Add(roundTripTween);

        roundTripTween = new MultiTween();
        roundTripTween.AddTween(secondTween);
        roundTripTween.AddTween(secondTween.Reverse());
        tweens.Add(roundTripTween);
      }
    }

    firstClick = null;
  }

  // Returns true if swapping the first and second gems results in a valid move. This method
  // assumes that the current game state in gems[] does NOT reflect this hypothetical swap.
  private bool IsValidSwap(int firstX, int firstY, int secondX, int secondY) {
    // Here we rely on knowledge of what was swapped. We only need to check for combos around the
    // two gems that moved. Swap the gems at check to either side.
    Gem first = gems[firstX, firstY];
    Gem second = gems[secondX, secondY];
    gems[secondX, secondY] = first;
    gems[firstX, firstY] = second;

    // Check the row and column of one gem.
    bool isValid = CheckSwappedGem(firstX, firstY);
    if (!isValid) {
      isValid = CheckSwappedGem(secondX, secondY);
    }

    // Undo the hypothetical swap.
    gems[firstX, firstY] = first;
    gems[secondX, secondY] = second;

    return isValid;
  }

  private bool CheckSwappedGem(int x, int y) {
    int numSameType = 1;
    Gem.Type type = gems[x, y].type;

    // First check before and after the gem in the row.
    for (int i = -1; i > -3; --i) {
      if (i + x < 0 || gems[x + i, y].type != type) {
        break;
      }
      ++numSameType;
    }
    for (int i = 1; i < 3; ++i) {
      if (i + x >= columns || gems[x + i, y].type != type) {
        break;
      }
      ++numSameType;
    }

    // Then check before and after the gem in the row.
    for (int i = -1; i > -3; --i) {
      if (i + y < 0 || gems[x, y + i].type != type) {
        break;
      }
      ++numSameType;
    }
    for (int i = 1; i < 3; ++i) {
      if (i + y >= rows || gems[x, y + i].type != type) {
        break;
      }
      ++numSameType;
    }

    return numSameType >= 3;
  }

  private bool HandleMatches() {
    bool matchFound = false;

    // First pass to find all the matches and add to a set of locations to be removed.
    HashSet<Point> pointsToRemove = new HashSet<Point>();
    matchFound = CheckColumnMatches(pointsToRemove);
    matchFound |= CheckRowMatches(pointsToRemove);

    // Update score.
    scorer.AddCombo(pointsToRemove.Count);

    // Second pass actually removes the affected gems.
    foreach (Point point in pointsToRemove) {
      Destroy(gems[point.x, point.y].gameObject);
      gems[point.x, point.y] = null;
    }

    return matchFound;
  }

  private bool CheckColumnMatches(HashSet<Point> pointsToRemove) {
    bool matchFound = false;
    for (int x = 0; x < gems.GetLength(0); ++x) {
      Gem.Type? lastTypeSeen = null;
      int consecutiveGems = 1;
      for (int y = 0; y < gems.GetLength(1); ++y) {
        if (gems[x, y].type == lastTypeSeen) {
          ++consecutiveGems;
          
          // If this is the last of a combo, remove all the affected gems.
          if (consecutiveGems >= 3 &&
            (y == gems.GetLength(1) - 1 || gems[x, y + 1].type != lastTypeSeen)) {
            matchFound = true;
            for (int i = 0; i < consecutiveGems; ++i) {
              pointsToRemove.Add(new Point(x, y - i));
            }
          }
        } else {
          consecutiveGems = 1;
        }
                
        lastTypeSeen = gems[x, y].type;
      }
    }

    return matchFound;
  }

  private bool CheckRowMatches(HashSet<Point> pointsToRemove) {
    bool matchFound = false;
    for (int y = 0; y < gems.GetLength(1); ++y) {
      Gem.Type? lastTypeSeen = null;
      int consecutiveGems = 1;
      for (int x = 0; x < gems.GetLength(1); ++x) {
        if (gems[x, y].type == lastTypeSeen) {
          ++consecutiveGems;
          
          // If this is the last of a combo, remove all the affected gems.
          if (consecutiveGems >= 3 &&
            (x == gems.GetLength(0) - 1 || gems[x + 1, y].type != lastTypeSeen)) {
            matchFound = true;
            for (int i = 0; i < consecutiveGems; ++i) {
              pointsToRemove.Add(new Point(x - i, y));
            }
          }
        } else {
          consecutiveGems = 1;
        }
                
        lastTypeSeen = gems[x, y].type;
      }
    }

    return matchFound;
  }

  private void RefillBoard() {
    // Go up through each column, find holes, apply tweens to gems that need to drop, and generate
    // new gems to come in from above.
    for (int x = 0; x < gems.GetLength(0); ++x) {
      int holes = 0;
      Vector3 drop = new Vector3();
      for (int y = gems.GetLength(1) - 1; y >= 0; --y) {
        Gem gem = gems[x, y];
        if (gem == null) {
          ++holes;
          drop.y += spacing;
        } else if (holes > 0) {
          tweens.Add(new LinearTween(gem.transform, gem.transform.position - drop, MOVE_SPEED));
          gems[x, y + holes] = gem;
          gems[x, y] = null;
        }
      }
      
      // Now generate new gems to fill the holes.
      for (int y = holes - 1; y >= 0; --y) {
        // CreateNewGem() puts the gem in its final position. We move it up off the screen
        // and tween it back in.
        Gem newGem = CreateNewGem(GetRandomGemType(), x, y, transform.position);
        Vector3 finalPosition = newGem.transform.position;
        newGem.transform.position = finalPosition + drop;
        tweens.Add(new LinearTween(newGem.transform, finalPosition, MOVE_SPEED));
      }
    }
  }

  private static Gem.Type GetRandomGemType() {
    int typeIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Gem.Type)).Length - 1);
    return (Gem.Type)typeIndex;
  }

  private void FillRandomAssShit() {
    Vector3 upperLeft = transform.position;

    List<Gem.Type> possibleTypes = new List<Gem.Type>();
    for (int x = 0; x < columns; ++x) {
      for (int y = 0; y < rows; ++y) {
        PopulatePossibleTypes(x, y, possibleTypes);
        int typeIndex = UnityEngine.Random.Range(0, possibleTypes.Count - 1);
        CreateNewGem(possibleTypes[typeIndex], x, y, upperLeft);
        possibleTypes.Clear();
      }
    }
  }

  private Gem CreateNewGem(Gem.Type type, int x, int y, Vector3 upperLeft) {
    Vector3 position = upperLeft;
    position.x += x * spacing + (spacing / 2);
    position.y -= y * spacing + (spacing / 2);

    Gem gem = (Gem)Instantiate(gemPrefab, position, Quaternion.identity);
    gem.inputCoordinator = this;
    gem.type = type;
    SpriteRenderer gemRenderer = gem.GetComponent<SpriteRenderer>();
    gemRenderer.sprite = gemSprites[(int)type];

    gems[x, y] = gem;
    return gem;
  }

  private void PopulatePossibleTypes(int x, int y, List<Gem.Type> possibleTypes) {
    // TODO: This is terrible and slow. It probably doesn't matter, but fix it anyway.
    foreach (Gem.Type type in Enum.GetValues(typeof(Gem.Type))) {
      possibleTypes.Add(type);
    }
    if (x > 0 && gems[x - 1, y]) {
      possibleTypes.Remove(gems[x - 1, y].type);
    }
    if (x < (columns - 1) && gems[x + 1, y]) {
      possibleTypes.Remove(gems[x + 1, y].type);
    }
    if (y > 0 && gems[x, y - 1]) {
      possibleTypes.Remove(gems[x, y - 1].type);
    }
    if (y < (rows - 1) && gems[x, y + 1]) {
      possibleTypes.Remove(gems[x, y + 1].type);
    }
  }
}
