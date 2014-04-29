using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour, InputCoordinator {

  public Gem gemPrefab;

  public Sprite[] gemSprites;

  public int rows;

  public int columns;

  private float spacing;

  private Gem[,] gems;

  private Point firstClick;

  private HashSet<Tween> tweens;

  private const float MOVE_SPEED = 3f;

  void Start() {
    gems = new Gem[rows, columns];
    spacing = -transform.position.x / 4;
    tweens = new HashSet<Tween>();
    firstClick = null;
    FillRandomAssShit();
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

  private void AttemptSwap(int firstX, int firstY, int secondX, int secondY) {
    if (Mathf.Abs(firstX + firstY - secondX - secondY) == 1) {
      Gem first = gems[firstX, firstY];
      Gem second = gems[secondX, secondY];
      gems[secondX, secondY] = first;
      gems[firstX, firstY] = second;

      tweens.Add(new Tween(first.transform, second.transform.position, MOVE_SPEED));
      tweens.Add(new Tween(second.transform, first.transform.position, MOVE_SPEED));
    }

    firstClick = null;
  }

  private bool HandleMatches() {
    bool matchFound = false;

    // First pass to find all the matches and add to a set of locations to be removed.
    HashSet<Point> pointsToRemove = new HashSet<Point>();
    matchFound = CheckColumnMatches(pointsToRemove) || CheckRowMatches(pointsToRemove);

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
    
  private bool ShouldAcceptInput() {
    return tweens.Count == 0;
  }

  private void FillRandomAssShit() {
    Vector3 upperLeft = transform.position;

    List<Gem.Type> possibleTypes = new List<Gem.Type>();
    for (int x = 0; x < columns; ++x) {
      for (int y = 0; y < rows; ++y) {
        Vector3 position = upperLeft;
        position.x += x * spacing + (spacing / 2);
        position.y -= y * spacing + (spacing / 2);

        Gem newGem = (Gem)Instantiate(gemPrefab, position, Quaternion.identity);
        newGem.inputCoordinator = this;

        PopulatePossibleTypes(x, y, possibleTypes);
        int typeIndex = UnityEngine.Random.Range(0, possibleTypes.Count - 1);
        newGem.type = possibleTypes[typeIndex];
        SpriteRenderer gemRenderer = newGem.GetComponent<SpriteRenderer>();
        gemRenderer.sprite = gemSprites[(int)newGem.type];

        gems[x, y] = newGem;
        possibleTypes.Clear();
      }
    }
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
