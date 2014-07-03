using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Tween {
  public delegate void TweenEvent(Tween tween);
  public event TweenEvent OnFinish;

  public abstract bool Finished {
    get;
  }

  public abstract void Update();

  public abstract Tween Reverse();

  // To be invoked by subclasses since they don't have direct access to the delegate.
  protected void Finish(Tween tween) {
    if (OnFinish != null) {
      OnFinish(tween);
    }
  }
}

