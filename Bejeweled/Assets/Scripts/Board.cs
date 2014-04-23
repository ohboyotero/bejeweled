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

        Gem newGem = (Gem)Instantiate(gemPrefab, position, Quaternion.identity);
        int typeIndex = Random.Range(0, 6);
        newGem.type = (Gem.Type)typeIndex;
        SpriteRenderer gemRenderer = newGem.GetComponent<SpriteRenderer>();
        gemRenderer.sprite = gemSprites[typeIndex];

        gems[row, column] = newGem;
      }
    }
  }
}
