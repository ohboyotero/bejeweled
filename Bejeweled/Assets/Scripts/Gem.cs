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

  // Use this for initialization
  void Start() {
  
  }
  
  // Update is called once per frame
  void Update() {
  
  }

  void OnMouseDown() {
    inputCoordinator.NotifyClick(this);
  }
}
