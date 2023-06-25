using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    public class ScaffMono : ScaffBehaviour
    {
        int debug_lastFrameUpdate = -1;

        virtual public bool canUpdate() => enabled;

        private void Update()
        {
            if (!canUpdate()) return;

            debug_lastFrameUpdate = Time.frameCount;

            update(Time.deltaTime);
        }

        /// <summary>
        /// to update externaly
        /// </summary>
        virtual protected void update(float deltaTime)
        { }

        public override string stringify()
        {
            return base.stringify() + " (" + debug_lastFrameUpdate + ") (update?" + canUpdate() + ")";
        }
    }
}
