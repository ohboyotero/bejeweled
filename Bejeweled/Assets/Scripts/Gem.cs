using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {

  public enum Type {
    GREEN,
    PURPLE,
    RED,
    ORANGE,
    YELLOW,
    BLUE
  }

  public Type type;
  public InputCoordinator inputCoordinator;

  private Vector3? downPosition;

  void Start() {
    downPosition = null;
  }

  void Update() {
  
  }

  void OnMouseDown() {
    downPosition = Input.mousePosition;
  }

  void OnMouseUp() {
    if (!downPosition.HasValue) {
      return;
    }

    Vector3 currentPosition = Input.mousePosition;
    if (downPosition.Value == currentPosition) {
      inputCoordinator.NotifyClick(this);
    } else {
      float dx = currentPosition.x - downPosition.Value.x;
      float dy = currentPosition.y - downPosition.Value.y;
      Point direction = new Point(0, 0);
      if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
        if (dx > 0) {
          direction.x = 1;
        } else {
          direction.x = -1;
        }
      } else {
        if (dy > 0) {
          direction.y = -1;
        } else {
          direction.y = 1;
        }
      }

      inputCoordinator.NotifyDrag(this, direction);
    }
  }

}
