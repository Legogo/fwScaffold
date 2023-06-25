using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// based on mueblo's work
/// </summary>

namespace brainer
{
  using brainer.capacity;

  abstract public class CapacityAttack : BrainerLogicCapacity
  {

    public BoxCollider2D getCollider() => null;

  }
}