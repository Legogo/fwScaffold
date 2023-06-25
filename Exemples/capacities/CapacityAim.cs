using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that allow to gather information about direction of CharacterLogic owner
/// And right stick angle (based on direction)
/// 
/// solve : direction, angle
/// sends : onAimPress, onAimRelease virtuals on matching events
/// </summary>

namespace fwp.scaffold.capacity
{
    using fwp.scaffold;
    
    abstract public class CapacityAim : BrainerLogicCapacity
    {
        //protected InputKeyAim _inputAim;

        protected CapacityMovement2D _move;
        protected float _angle = 0f;

        public override void setupCapacity()
        {
            _move = brain.getCapacity<CapacityMovement2D>();
        }

        public override void updateCapacity()
        {
            _angle = getStickAngle();

            if (pressed_aim()) onAimPress();
            else if (release_aim()) onAimRelease();
        }

        abstract protected void onAimPress();
        abstract protected void onAimRelease();

        abstract protected Vector2 getJoystickVector();
        abstract protected bool pressed_aim();
        abstract protected bool release_aim();

        private float getStickAngle()
        {
            float angle = 0f;

            //override based on joystick angle
            Vector2 joyVector = getJoystickVector();
            Vector2 dir = _move.getHorizontalDirection() < 0 ? Vector3.left : Vector3.right;

            if (joyVector.magnitude > 0.25f)
            {
                angle = ComputeSignedAngle(dir, joyVector);
            }
            else
            {
                //default is in front
                angle = dir.x < 0 ? 180f : 0f;
            }

            return angle;
        }

        private float ComputeSignedAngle(Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle(from, to);

            //if (from.x < 0) angle = -angle;

            Vector3 sign = Vector3.Cross(from, to);

            if (sign.z < 0) angle = -angle;

            if (from.x < 0) angle += 180f;

            //Debug.Log(from);
            //Debug.Log(angle + " , " + sign);

            return angle;
        }

    }

}
