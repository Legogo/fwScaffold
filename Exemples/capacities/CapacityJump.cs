using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace brainer.capacity
{
  abstract public class CapacityJump : BrainerLogicCapacity
  {
    protected CapacityCollision2D _collision;
    protected CapacityMovementPlatformer _move;

    bool grounded = false;
    bool jumping = false;

    public Action onJump;

    public override void setupCapacity()
    {
      _collision = brain.getCapacity<CapacityCollision2D>();
      _move = brain.getCapacity<CapacityMovementPlatformer>();
    }

    abstract protected bool pressJump();

    public override void updateCapacity()
    {
      base.updateCapacity();

      grounded = _collision.isGrounded();

      if (jumping && grounded)
      {
        //Debug.Log("-------------------------- <b>LAND</b> "+Time.frameCount);
        jumping = false;
      }

      if (pressJump()) solveJump();
    }

    [ContextMenu("jump!")]
    public void solveJump()
    {
      bool isGrounded = _collision.isGrounded();

      //Debug.Log("<color=red>================================================</color>");
      //Debug.Log(Time.frameCount+" , <b>JUMP !</b> grounded ? "+isGrounded + " , jump pwr ? " + getJumpPower());

      if (onJump != null) onJump();

      if (isGrounded)
      {
        jumping = true;
        //Debug.Log(" ------------------------ <b>JUMP</b> "+Time.frameCount);
        _move.addVelocity(0f, getJumpPower());

        soundPlayJump();
      }

    }

    virtual public float getJumpPower()
    {
      return 1f;
    }

    virtual public void soundPlayJump()
    {
    }
  }

}
