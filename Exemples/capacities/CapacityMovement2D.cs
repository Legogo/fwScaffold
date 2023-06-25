using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace brainer.capacity
{
    using fwp.scaffold;
    using fwp.halpers;

    /// <summary>
    /// basic 2D movement
    /// must NOT BE specific to platformer OR topdown
    /// </summary>
    abstract public class CapacityMovement2D : BrainerLogicCapacity, iScaffLog
    {
        public struct MovementData
        {
            public Vector2 origin; // origin before movement
            public Vector2 destination; // destination RESULT
            public Vector2 delta; // dest - origin

            public void reset()
            {
                origin.x = origin.x = 0f;
                destination.y = destination.y = 0f;
                delta.x = delta.y = 0f;
            }
        }

        public struct MovementMotion
        {
            public Vector2 direction;
            public float speed;

            public void reset()
            {
                direction.x = direction.y = 0f;
                speed = 0f;
            }
        }

        protected Transform _pivot;
        protected CapacityCollision2D _collision;

        public MovementData movementInfo;
        public MovementMotion motionInfo;

        public override void setupCapacity()
        {
            base.setupCapacity();

            _pivot = brain.tr;

            _collision = brain.getCapacity<CapacityCollision2D>();
            if (_collision == null) Debug.LogWarning("movement has no data for collision");
        }

        public override void restartCapacity()
        {
            base.restartCapacity();

            movementInfo.reset();
        }

        /// <summary>
        /// solve input
        /// </summary>
        abstract protected Vector2 solveMotion();

        virtual protected void solveInputs()
        { }

        sealed public override void updateCapacity()
        {
            base.updateCapacity();

            solveInputs();

            Vector2 inputMotion = solveMotion();

            //Debug.Log("motion ? "+inputMotion);

            motionInfo.direction = inputMotion.normalized;
            motionInfo.speed = inputMotion.magnitude;

            //dat
            movementInfo.origin = _pivot.position;

            //actual step, using speed & dlt time
            Vector2 step = getMotionStep();
            movementInfo.destination = checkStepCollision(step);

            //dat
            movementInfo.delta = movementInfo.destination - movementInfo.origin;

            //Debug.Log(movementInfo.delta);

            _pivot.position = movementInfo.destination; // apply
        }

        virtual protected Vector2 getMotionStep()
        {
            return motionInfo.direction * motionInfo.speed * Time.deltaTime;
        }

        protected Vector2 checkStepCollision(Vector2 step)
        {
            Vector2 nextPosition;

            //cannot collide
            if (_collision != null && _collision.isCollidable())
            {
                //Debug.Log("origin");Debug.Log(_pivot.position);

                //returns the center of the collision box
                nextPosition = _collision.checkCollisionRaycasts(step);

                //Debug.Log("next"); Debug.Log(nextPosition);
            }
            else
            {
                nextPosition = (Vector2)_pivot.position + step;
            }

            

            return nextPosition;
        }

        override public void clean()
        {
            base.clean();

            movementInfo.reset();
            motionInfo.reset();
        }

        public bool hasMoved() => movementInfo.delta.sqrMagnitude > 0f;

        public void teleportTo(Vector3 position)
        {
            clean();
            _pivot.position = position;
        }

        public float getHorizontalDirection() => motionInfo.direction.x;

        public string getStamp() => GetType().ToString();

        protected string ct = string.Empty;
        public string stringify()
        {
            ct = GetType().ToString();

            //collision ?
            if (_collision != null) ct += "\n └ " + HalperString.iStringFormatBool("collidable", _collision.isCollidable());
            else ct += "\n └ " + HalperString.iStringFormatBool("collision != null", _collision != null);

            ct += "\n └ " + HalperString.iStringFormatBool("moved", hasMoved());

            return ct;
        }

    }

}