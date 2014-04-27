using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour, InputCoordinator {

  public Gem gemPrefab;
  public Sprite[] gemSprites;
  public int rows;
  public int columns;

  private float spacing;
  private Gem[,] gems;
  private int[] firstClick;
  private HashSet<Tween> tweens;

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
            firstClick = new int[2];
            firstClick[0] = i;
            firstClick[1] = j;
          } else {
            AttemptSwap(firstClick[0], firstClick[1], i, j);
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

      tweens.Add(new Tween(first.transform, second.transform.position, 2));
      tweens.Add(new Tween(second.transform, first.transform.position, 2));
    }

    firstClick = null;
  }

  private bool ShouldAcceptInput() {
    return tweens.Count == 0;
  }

  private void FillRandomAssShit() {
    Vector3 upperLeft = transform.position;

    for (int x = 0; x < columns; ++x) {
      for (int y = 0; y < rows; ++y) {
        Vector3 position = upperLeft;
        position.x += x * spacing + (spacing / 2);
        position.y -= y * spacing + (spacing / 2);

        Gem newGem = (Gem)Instantiate(gemPrefab, position, Quaternion.identity);
        newGem.inputCoordinator = this;
        int typeIndex = Random.Range(0, 6);
        newGem.type = (Gem.Type)typeIndex;
        SpriteRenderer gemRenderer = newGem.GetComponent<SpriteRenderer>();
        gemRenderer.sprite = gemSprites[typeIndex];

        gems[x, y] = newGem;
      }
    }
  }
}
