using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://docs.unity3d.com/ScriptReference/Rect.html
/// 
/// xmin,ymin _
///          |_|
///             xmax,ymax
/// </summary>

namespace brainer
{
    using fwp.engine.scaffolder;

    public class CapacityCollision2D : BrainerLogicCapacity, iScaffLog
    {
        [HideInInspector] public CapacityCollision2D[] all; // all other objects in scenes
        [HideInInspector] public BoxCollider2D boxCollider;
        [HideInInspector] public Rect recBound = new Rect(); // expressed in world coordinates

        protected Transform _t;

        //step tools
        protected float rayDistance = 5f;
        protected RaycastHit2D hit;
        Vector2 origin = Vector2.zero;
        float min;
        float cornerGap = 0.05f;

        //a l'updateLate on reset donc dans l'inspecteur on voit jamais la valeur
        public CollisionInfo info;
        public LayerMask rayLayer;

        // debug, pour savoir combien de déplacement il y a eu
        private Vector2 frame_h_step;
        private Vector2 frame_v_step;
        private Vector2 frame_last_step;

        protected override void buildCapacity()
        {
            base.buildCapacity();

            _t = transform;
        }

        public override void setupCapacity()
        {
            boxCollider = gameObject.GetComponent<BoxCollider2D>(); // default
            if (boxCollider == null) boxCollider = brain.tr.GetComponentInChildren<BoxCollider2D>(); // first in owner

            if (boxCollider == null)
            {
                Debug.LogError("why use collision if no collider is found on this object ? " + name, transform);
                return;
            }

            if (boxCollider.transform.localScale != Vector3.one)
            {
                Debug.LogError(GetType() + " can't manage scale on collider !", gameObject);
                return;
            }

            recBound = new Rect();
            //destinationBounds = new Rect();

            all = GameObject.FindObjectsOfType<CapacityCollision2D>();
        }

        private void resetCollisionInfo()
        {

            // reset collision
            info.touching_left = info.touching_right = false;
            info.touching_ground = info.touching_ceiling = false;

        }

        /// <summary>
        /// called on moveStep of capacity movement
        /// </summary>
        public Vector2 checkCollisionRaycasts(Vector2 step)
        {
            //debug data
            frame_h_step = frame_v_step = Vector2.zero;
            frame_last_step = step;

            //reeval collision bounds ; bounds are bbox of collider
            //ref because it's static
            solveCollisionBounds(ref recBound, boxCollider);

            //frame collision dat reset
            resetCollisionInfo();

            //kill local collider before raycasting
            boxCollider.enabled = false;

            //Debug.Log("pre");Debug.Log(recBound);

            //those, if hit something, will modify recBound.center position
            if (step.y != 0f)
            {
                if (step.y < 0f) checkRaycastVertical(step, Mathf.Abs(step.y), Vector2.down);
                if (step.y > 0f) checkRaycastVertical(step, Mathf.Abs(step.y), Vector2.up);
            }
            if (step.x != 0f)
            {
                if (step.x < 0f) checkRaycastHorizontal(step, Mathf.Abs(step.x), Vector2.left);
                if (step.x > 0f) checkRaycastHorizontal(step, Mathf.Abs(step.x), Vector2.right);
            }

            boxCollider.enabled = true;

            //Debug.Log("after");Debug.Log(recBound);

            //center of collider
            //return recBound.center;

            //https://docs.unity3d.com/ScriptReference/BoxCollider2D.html

            //box -> transform
            return recBound.center - boxCollider.offset;
        }

        protected void checkRaycastVertical(Vector2 moveStep, float rayDistance, Vector2 rayDir)
        {
            //float absStep = Mathf.Abs(moveStep.y);

            bool touchedSomething = false;

            origin.x = recBound.xMin + (recBound.width * 0.5f); // center
            origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            origin.x = recBound.xMin + cornerGap; // left
            origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            origin.x = recBound.xMax - cornerGap; // right
            origin.y = rayDir.y < 0f ? recBound.yMax : recBound.yMin;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            // si on touche quelque chose on n'applique pas de déplacement, on register juste les infos de collision
            if (touchedSomething)
            {
                //Debug.Log(name + " collision vertical " + rayDir);
                if (rayDir.y < 0) info.touching_ground = true;
                else if (rayDir.y > 0) info.touching_ceiling = true;
            }
            else
            {
                // VERTICAL nothing was touched during step, this cast is meant to actually move the transform
                if (moveStep.y != 0f && (Mathf.Sign(rayDir.y) == Mathf.Sign(moveStep.y)))
                {
                    Debug.DrawLine(recBound.center, recBound.center + rayDir * rayDistance, (moveStep.y > 0f) ? Color.magenta : Color.yellow); // up/down
                    recBound.center += rayDir * rayDistance;
                    frame_v_step += (rayDir * rayDistance); // debug
                }
            }
        }

        protected void checkRaycastHorizontal(Vector2 moveStep, float rayDistance, Vector2 rayDir)
        {
            //float absStep = Mathf.Abs(moveStep.x); // to be sure

            bool touchedSomething = false;

            //Debug.Log(moveStep + " , " + rayDir);
            //Debug.Log(transform.position);

            origin.y = recBound.yMax + (recBound.yMin - recBound.yMax) * 0.5f; // center
            origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            origin.y = recBound.yMin - cornerGap; // top
            origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            origin.y = recBound.yMax + cornerGap; // bottom
            origin.x = rayDir.x < 0f ? recBound.xMin : recBound.xMax;
            if (raycastCheck(origin, rayDir, rayDistance)) touchedSomething = true;

            //Debug.Log(transform.position);

            if (touchedSomething)
            {
                //Debug.Log("collision horizontal");
                if (rayDir.x < 0) info.touching_left = true;
                else if (rayDir.x > 0) info.touching_right = true;
            }
            else
            {
                // HORIZONTAL nothing was touched during step, moving on
                if (moveStep.x != 0f && (Mathf.Sign(rayDir.x) == Mathf.Sign(moveStep.x)))
                {
                    Debug.DrawLine(recBound.center, recBound.center + rayDir * rayDistance, Color.yellow); // left/right
                    recBound.center += rayDir * rayDistance;
                    frame_h_step += (rayDir * rayDistance); // debug
                }
            }

        }

        /// <summary>
        /// layer pour caster (utilisé par les horiz/vertic)
        /// la layer est déter dans l'inspecteur de la kappa
        /// </summary>
        protected bool raycastCheck(Vector2 origin, Vector2 dir, float distance)
        {
            distance = Mathf.Abs(distance);

            //si a la base l'avatar ne se déplace pas du tout sur cet axe
            //if (distance == 0f) distance = 0.1f;

            Vector2 movement = Vector2.zero;
            Vector2 tmpOrigin = origin;
            Vector2 step = dir * distance;

            Debug.DrawLine(tmpOrigin, tmpOrigin + dir * distance, Color.green); // to show ray

            bool touch = false;
            bool noMove = distance == 0f; // cas specific
            if (dir.sqrMagnitude == 0f) Debug.LogError("dir ??");

            hit = Physics2D.Raycast(origin, dir, distance, rayLayer);

            int safe = 600;

            //l'origin est DANS un obstacle, faut reculer jusqu'à trouvé un endroit safe
            while (hit.collider != null && hit.distance == 0f && safe > 0)
            {
                movement -= (noMove ? dir : step) * 0.9f;
                tmpOrigin = origin + movement;

                touch = true;

                hit = Physics2D.Raycast(tmpOrigin, dir, distance, rayLayer);

                Debug.DrawLine(recBound.center, hit.point, Color.white); // to show where is the hit point

                safe--;
            }

            if (safe <= 0) Debug.LogWarning(name + " safe! " + tmpOrigin + " , " + movement + " , " + dir + " , " + step);

            //si j'ai qq chose en face je déplace
            if (hit.collider != null)
            {
                movement += dir * hit.distance;
                recBound.center += movement;
            }

            return touch;
        }

        static public void solveCollisionBounds(ref Rect bound, BoxCollider2D collider)
        {
            Vector3 ext = collider.bounds.extents;

            float x = collider.bounds.center.x;
            //float x = collider.bounds.center.x + collider.offset.x; // real center
            //float x = transform.position.x;
            bound.xMin = x - ext.x;
            bound.xMax = x + ext.x;

            float y = collider.bounds.center.y;
            //float y = collider.bounds.center.y + collider.offset.y;
            //float y = transform.position.y;
            bound.yMin = y + ext.y;
            bound.yMax = y - ext.y;
        }

        public int GetCollisionDirection
        {
            get { return info.touching_left ? -1 : info.touching_right ? 1 : 0; }
        }

        public bool isCollidable()
        {
            if (!enabled)
            {
                //Debug.Log("  not enabled ? "+enabled);
                return false;
            }

            if (boxCollider == null)
            {
                //Debug.Log("  no box collider ?");
                return false;
            }

            if (!boxCollider.enabled)
            {
                //Debug.Log("  box collider not enabled");
                return false;
            }
            return true;
        }

        public bool isGrounded()
        {
            return info.touching_ground;
        }

        public bool isTouchingSide()
        {
            return info.touching_left || info.touching_right;
        }

        public bool isRoofed()
        {
            return info.touching_ceiling;
        }

        public bool isTouchingSomething(bool butGround = false)
        {
            if (butGround) return isTouchingSide() || isRoofed();
            return isGrounded() || isTouchingSide() || isRoofed();
        }

        protected void drawBox(Rect rect, Color col)
        {
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMax), col);
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMin), col);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            //actual bounds
            //solveBounds();
            drawBox(recBound, Color.yellow); // yellow is destination

            // COLLISION
            // drawing vertical / horizontal lines along player collider to show what side was touched

            if (info.touching_left) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMin, recBound.yMax), Color.red);
            if (info.touching_right) Debug.DrawLine(new Vector2(recBound.xMax, recBound.yMin), new Vector2(recBound.xMax, recBound.yMax), Color.red);

            if (info.touching_ceiling) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMin), new Vector2(recBound.xMax, recBound.yMin), Color.red);
            if (info.touching_ground) Debug.DrawLine(new Vector2(recBound.xMin, recBound.yMax), new Vector2(recBound.xMax, recBound.yMax), Color.red);

            //Gizmos.DrawSphere(recBound.center, 0.05f);
        }

        string iStringFormatBool(string label, bool val)
        {
            return label + " ? " + val;
        }

        public string getStamp() => GetType().ToString();

        public string stringify()
        {
            string ct = string.Empty;

            ct += "\n~touching~";
            ct += "\n " + iStringFormatBool("touching_ceiling", info.touching_ceiling);
            ct += "\n " + iStringFormatBool("touching_left", info.touching_left);
            ct += "\n " + iStringFormatBool("touching_right", info.touching_right);
            ct += "\n " + iStringFormatBool("touching_ground", info.touching_ground);

            if (!isCollidable())
            {
                ct += "\n  ~notCollidable~";
                ct += "\n  " + iStringFormatBool("enabled", enabled);
                ct += "\n  " + iStringFormatBool("boxCollider", boxCollider != null);
                if (boxCollider != null) ct += "\n  " + iStringFormatBool("boxCollider.enabled", boxCollider.enabled);
            }

            ct += "\n~data frame~";
            ct += "\n  frame_step h : " + frame_h_step.x + " x " + frame_h_step.y;
            ct += "\n  frame_step v : " + frame_v_step.x + " x " + frame_v_step.y;
            ct += "\n  step : " + frame_last_step.x + " x " + frame_last_step.y;

            ct += "\n  ~box~";
            ct += "\n" + boxBoundsToString();

            return ct;
        }

        protected string boxBoundsToString()
        {
            if (boxCollider == null) return "no box collider";

            string ct = "";
            ct = recBound.xMin + " x " + recBound.yMin + " ┌ " + boxCollider.bounds.extents.x + " ┐ " + recBound.xMax + " - " + recBound.yMin;
            ct += "\n" + boxCollider.bounds.extents.y + " |     | ";
            ct += "\n" + recBound.xMin + " x " + recBound.yMax + " └ " + boxCollider.bounds.extents.x + " ┘ " + recBound.xMax + " - " + recBound.yMax;

            return ct;
        }

    }

    public struct CollisionInfo
    {
        public bool touching_left;
        public bool touching_right;
        public bool touching_ground;
        public bool touching_ceiling;
    }

}