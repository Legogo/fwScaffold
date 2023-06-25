using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace brainer.capacity
{
    public class CapacityTopDownMovement : CapacityMovement2D
    {
        public TopDownMovementData destinationOverride;

        Vector2 _motion;

        public void injectInputs(bool left, bool right, bool up, bool down, float speed = 1f)
        {
            if (left) _motion.x = -1f;
            else if (right) _motion.x = 1f;
            else _motion.x = 0f;

            if (up) _motion.y = 1f;
            else if (down) _motion.y = -1f;
            else _motion.y = 0f;

            _motion.Normalize();

            _motion *= speed;

            //Debug.Log("motion is : " + _motion);
            //Debug.Log(left + "," + right);
        }

        protected override Vector2 solveMotion()
        {
            if (destinationOverride != null)
            {
                return destinationOverride.getDestination() - (Vector2)_pivot.position;
            }

            return _motion;
        }
    }


    /// <summary>
    /// un ordre de déplacement
    /// AutoMove toward
    /// </summary>
    public class TopDownMovementData
    {
        int uid;
        public Transform destinationTr;
        public Vector3 destinationPt;

        public TopDownMovementData()
        {
            uid = Random.Range(0, 999999);
        }

        /// <summary>
        /// has a specific destination (not just a direction)
        /// </summary>
        /// <returns></returns>
        public bool hasDestination()
        {
            if (destinationTr != null) return true;
            if (destinationPt.sqrMagnitude != 0f) return true;
            return false;
        }

        public Vector2 getDestination()
        {
            if (destinationTr != null) return destinationTr.position;
            return destinationPt;
        }

        public int getUid() { return uid; }
    }

}
