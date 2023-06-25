using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace brainer
{
    /// <summary>
    /// Some specific capacity that an LogicItem can have
    /// It's updated by it's owner
    /// capacities are mono to get param in inspector
    /// </summary>
    abstract public class BrainerLogicCapacity : MonoBehaviour
    {
        protected iBrainCandidate owner; // to fetch other capac
        protected BrainerLogics brain;

        List<MonoBehaviour> lockers = new List<MonoBehaviour>();

        private void Awake()
        {
            owner = GetComponentInParent<iBrainCandidate>();
            Debug.Assert(owner != null);

            brain = owner.getBrain();
            Debug.Assert(brain != null);

            buildCapacity();

        }

        private void Start()
        {
            //this will trigged setup (after build)
            brain.subKappa(this);
        }

        /// <summary>
        /// at this point brain is present and this kappa is NOT subbed yet
        /// </summary>
        virtual protected void buildCapacity()
        { }

        virtual public void setupCapacity()
        { }

        /// <summary>
        /// reboot all params
        /// </summary>
        virtual public void restartCapacity() { }

        virtual public void updateCapacity() { }

        public bool isLocked() => lockers.Count > 0;
        public void lockCapacity(MonoBehaviour locker)
        {
            if (lockers.IndexOf(locker) < 0)
            {
                lockers.Add(locker);
            }
        }
        public void unlockCapacity(MonoBehaviour locker)
        {
            if (lockers.IndexOf(locker) > -1)
            {
                lockers.Remove(locker);
            }
        }

        virtual public void clean()
        { }

        private void OnDestroy()
        { }

        public bool hasSameBrain(BrainerLogics other) => brain == other;
        public BrainerLogics getBrain() => brain;

    }

}
