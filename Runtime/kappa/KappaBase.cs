using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    /// <summary>
    /// must not be a region object, linked to brain, brain is responsible for everything
    /// </summary>
    abstract public class KappaBase : ScaffObject, iKappaCandidate
    {
        protected BrainBase _brain;

        List<MonoBehaviour> freezeQueue = new List<MonoBehaviour>();

        public KappaBase(BrainBase brain)
        {
            Debug.Assert(brain != null);

            _brain = brain;

            _brain.subKappa(this);
        }
        /// <summary>
        /// called on region change by brain
        /// </summary>
        virtual public void resetKappa()
        {
            //Debug.Log(getStamp() + " RESET");
        }

        /// <summary>
        /// this can be called during build() of a getcapa fetch is called
        /// </summary>
        public void brainReady(BrainBase brain)
        {
            //dans le cas d'un objet généré au runtime y a besoin
            Debug.Assert(brain == _brain, "pas le bon brain");

            //Debug.Log(brain, brain);
            //Debug.Log(_brain, _brain);
        }

        /// <summary>
        /// only called when an actual change is made
        /// multiple subscription of the same object won't trigger it again
        /// </summary>
        /// <param name="freezed"></param>
        virtual protected void onFreeze(bool freezed)
        { }

        public BrainBase getParentBrain() => _brain;

        public bool isFromSameBrain(KappaBase kap)
        {
            return kap.getParentBrain() == getParentBrain();
        }

        /// <summary>
        /// interruption
        /// </summary>
        virtual public void recycle()
        {
            resetKappa();
        }


        public void freeze(MonoBehaviour locker)
        {
            if (!freezeQueue.Contains(locker))
            {
                freezeQueue.Add(locker);
                onFreeze(true);
            }
        }
        public void unfreeze(MonoBehaviour locker)
        {
            if (freezeQueue.Contains(locker))
            {
                freezeQueue.Remove(locker);
                onFreeze(isFreezed());
            }
        }
        public bool isFreezed() => freezeQueue.Count > 0;


        public override string stamp()
        {
            return base.stamp() + $" brain:<b>{_brain.name}</b>";
        }

    }

}
