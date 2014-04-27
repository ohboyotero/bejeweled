using UnityEngine;
using System;

public class Tween {
  private Transform target;
  private Vector3 finish;
  private Vector3 direction;
  private float speed;

  public bool Finished {
    get { return target.position == finish; }
  }

  public Tween(Transform target, Vector3 finish, float speed) {
    this.target = target;
    this.finish = finish;
    this.speed = speed;
    this.direction = (finish - target.position).normalized;
  }

  public void Update() {
    if (Finished) {
      return;
    }

    float distanceThisFrame = speed * Time.deltaTime;
    if ((finish - target.position).magnitude < distanceThisFrame) {
      target.position = finish;
    } else {
      target.position += direction * distanceThisFrame;
    }
  }
}

