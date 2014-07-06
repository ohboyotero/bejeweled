using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
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
    down = true;
    spriteRenderer.sprite = downSprite;
  }

  void OnMouseUp() {
    down = false;
    if (hover) {
      spriteRenderer.sprite = hoverSprite;
    } else {
      spriteRenderer.sprite = upSprite;
    }
  }

  void OnMouseEnter() {
    hover = true;
    if (!down) {
      spriteRenderer.sprite = hoverSprite;
    }
  }

  void OnMouseExit() {
    hover = false;
    spriteRenderer.sprite = upSprite;
  }
}
