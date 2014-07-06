using UnityEngine;
using System.Collections.Generic;

public class MultiTween : Tween {

  private LinkedList<Tween> tweens;
  private LinkedListNode<Tween> currentNode;
  private bool executionStarted;

  public override bool Finished {
    get { return currentNode == null; }
  }

  public MultiTween() {
    tweens = new LinkedList<Tween>();
    currentNode = null;
    executionStarted = false;
  }

  public override void Update() {
    executionStarted = true;

    if (!Finished) {
      Tween currentTween = currentNode.Value;
      currentTween.Update();
      if (currentTween.Finished) {
        currentNode = currentNode.Next;
        if (Finished) {
          Finish(this);
        }
      }
    }
  }

  public override Tween Reverse() {
    MultiTween reverseTween = new MultiTween();
    LinkedListNode<Tween> current = tweens.Last;
    while (current != null) {
      reverseTween.AddTween(current.Value.Reverse());
      current = current.Previous;
    }

    return reverseTween;
  }

  // Returns true if successfully added. Only possible to add more tweens before tweening starts.
  public bool AddTween(Tween tween) {
    if (executionStarted) {
      return false;
    }

    tweens.AddLast(tween);
    if (currentNode == null) {
      currentNode = tweens.First;
    }
    return true;
  }
}
