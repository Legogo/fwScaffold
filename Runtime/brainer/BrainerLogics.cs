using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace brainer
{
    /// <summary>
    /// Le bridge qui gère et update les capacities
    /// Le lien entre un objet du jeu et ses capacités
    /// </summary>
    public class BrainerLogics
    {
        public iBrainCandidate owner;

        public MonoBehaviour mono;
        //public GameObject go;
        public Transform tr; // pivot ?

        //capacities will subscribe to this List on their constructor
        protected List<BrainerLogicCapacity> capacities = new List<BrainerLogicCapacity>();

        public BrainerLogics(iBrainCandidate owner)
        {
            this.owner = owner;

            mono = owner as MonoBehaviour;
            Debug.Assert(mono != null, "nop");

            tr = mono.transform;
        }

        public void subKappa(BrainerLogicCapacity kappa)
        {
            if (capacities.IndexOf(kappa) > -1) return;

            capacities.Add(kappa);

            kappa.setupCapacity();
        }

        /// <summary>
        /// les capacities sont ref quand le brain boot
        /// </summary>
        public T getCapacity<T>() where T : BrainerLogicCapacity
        {
            T tar = (T)capacities.FirstOrDefault(x => x != null && typeof(T).IsAssignableFrom(x.GetType()));
            if (tar != null) return tar;

            //Debug.Assert(tar != null, mono.name+" has no "+ typeof(T));

            //force fetch ref
            if(tar == null)
            {
                tar = tr.GetComponentInChildren<T>();

                //do not sub this capa, it will be subbed when setuping in awake
            }

            return tar;
        }

        virtual public void brainUpdate()
        {
            for (int i = 0; i < capacities.Count; i++) capacities[i].updateCapacity();
        }

    }

    public interface iBrainCandidate
    {
        BrainerLogics getBrain();
    }
}
