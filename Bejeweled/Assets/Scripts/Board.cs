using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

  public Gem gemPrefab;
  public Sprite[] gemSprites;
  public int rows;
  public int columns;

  private float spacing;

  private Gem[,] gems;

  void Start() {
    gems = new Gem[rows, columns];
    spacing = -transform.position.x / 4;
    FillRandomAssShit();
  }

  void Update() {

  }

  private void FillRandomAssShit() {
    Vector3 upperLeft = transform.position;

    for (int row = 0; row < rows; ++row) {
      for (int column = 0; column < columns; ++column) {
        Vector3 position = upperLeft;
        position.x += column * spacing + (spacing / 2);
        position.y -= row * spacing + (spacing / 2);
        gems[row, column] = (Gem)Instantiate(gemPrefab, position, Quaternion.identity);
      }
    }
  }
}
