using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace brainer
{

  public class CapacityHitpoints : BrainerLogicCapacity
  {

    public float startingHealth = 1f; // [0,1]
    public float health = 1f;

    public override void setupCapacity()
    {
      health = startingHealth;
    }
    public override void updateCapacity()
    {
    }

    public float getHealth()
    {
      return health;
    }

    public void takeHit(float pwr)
    {
      health -= pwr;
      if (health < 0f) health = 0f;

      Debug.Log(brain.tr.name + " took <b>" + pwr + " dmg</b> and now has <b>" + health + " hp</b>");

    }

  }

}