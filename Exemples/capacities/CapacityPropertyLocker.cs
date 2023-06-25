using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Allow to manage a list of type that will indicates that a specific property is locked
/// 
/// 0 == both direction
/// 1 == can't move right
/// -1 == can't move left
/// 
/// null == free
/// </summary>

namespace brainer
{
  public class CapacityPropertyLocker
  {

    List<GameObject> list = new List<GameObject>();
    List<int> direction = new List<int>();

    public void addLock(GameObject locker, int dir = 0)
    {
      if (list.Contains(locker)) return;

      list.Add(locker);
      direction.Add(dir);

      //Debug.Log("added lock " + locker.name+", dir == "+dir);
    }

    public void removeLock(GameObject locker)
    {
      if (!list.Contains(locker)) return;

      int idx = list.IndexOf(locker);
      list.RemoveAt(idx);
      direction.RemoveAt(idx);

      //Debug.Log("removed lock " + locker.name);
    }

    public bool isLocked() { return list.Count > 0; }
    public int? getLockDirection()
    {
      //Debug.Log("locker count ? " + list.Count+" / "+direction.Count);

      bool left = false;
      bool right = false;

      for (int i = 0; i < list.Count; i++)
      {
        if (direction[i] < 0) left = true;
        else if (direction[i] > 0) right = true;
        else left = right = true;
      }

      if (left && !right) return -1;
      if (right && !left) return 1;
      if (left && right) return 0;

      return null;
    }
  }

}
