using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using brainer.capacity;

public class CapacityMovementPlatformer : CapacityMovement2D
{
  protected override Vector2 solveMotion()
  {
    return Vector2.zero;
  }

  public void addVelocity(float h, float v)
  {

  }
}
