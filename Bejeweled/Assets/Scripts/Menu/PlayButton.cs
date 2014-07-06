using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayButton : MonoBehaviour {

  private Vector3 downCoord;

  // The maximum number of pixels that the mouse can move between mousedown and mouseup and still
  // be considered a click.
  private const float MAX_MOUSE_DRIFT = 3;

  void OnMouseDown() {
    downCoord = Input.mousePosition;
  }

  void OnMouseUp() {
    if ((Input.mousePosition - downCoord).magnitude < MAX_MOUSE_DRIFT) {
      LoadGameScene();
    }
  }

  private void LoadGameScene() {
    Application.LoadLevel("Game");
  }
}
