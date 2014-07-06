using UnityEngine;
using System;

public interface InputCoordinator {
  void NotifyClick(Gem target);
  void NotifyDrag(Gem target, Point direction);
}
