using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonRollover : MonoBehaviour {

  public Sprite upSprite;
  public Sprite hoverSprite;
  public Sprite downSprite;

  private SpriteRenderer spriteRenderer;
  private bool hover;
  private bool down;

  void Start() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    hover = false;
    down = false;
  }
	
  void OnMouseDown() {
    Debug.Log("Down");
    down = true;
    spriteRenderer.sprite = downSprite;
  }

  void OnMouseUp() {
    Debug.Log("Up");
    down = false;
    if (hover) {
      spriteRenderer.sprite = hoverSprite;
    } else {
      spriteRenderer.sprite = upSprite;
    }
  }

  void OnMouseEnter() {
    Debug.Log("Enter");
    hover = true;
    if (!down) {
      spriteRenderer.sprite = hoverSprite;
    }
  }

  void OnMouseExit() {
    Debug.Log("Exit");
    hover = false;
    spriteRenderer.sprite = upSprite;
  }
}
