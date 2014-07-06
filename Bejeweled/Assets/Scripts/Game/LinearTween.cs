using UnityEngine;
using System;
using System.Collections.Generic;

public class LinearTween : Tween {
  private Transform target;
  private Vector3 start;
  private Vector3 finish;
  private Vector3 direction;
  private float speed;
  
  public override bool Finished {
    get { return target.position == finish; }
  }
  
  public LinearTween(Transform target, Vector3 finish, float speed)
  : this(target, target.position, finish, speed) {
  }
  
  private LinearTween(Transform target, Vector3 start, Vector3 finish, float speed) {
    this.target = target;
    this.start = start;
    this.finish = finish;
    this.speed = speed;
    this.direction = (finish - start).normalized;
  }
  
  public override void Update() {
    if (Finished) {
      return;
    }
    
    float distanceThisFrame = speed * Time.deltaTime;
    if ((finish - target.position).magnitude <= distanceThisFrame) {
      target.position = finish;
    } else {
      target.position += direction * distanceThisFrame;
    }
    
    if (Finished) {
      Finish(this);
    }
  }
  
  public override Tween Reverse() {
    return new LinearTween(target, finish, start, speed);
  }
}

