using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.engine.scaffolder;

/// <summary>
/// objet qui a des capacités
/// il connait la liste
/// il peut fournir des capacités a ceux qui demandent
/// </summary>

namespace fwp.scaffold
{
    abstract public class BrainBase : ScaffMono
    {
        const string STATE_PREFIX = "s_";

        protected List<iKappaCandidate> kappas = new List<iKappaCandidate>();

        bool _existOnStartup = false;

        protected override void build()
        {
            base.build();

            _existOnStartup = Time.frameCount < 5;
        }

        protected override void setup()
        {
            base.setup();

            //Debug.Log($"#{getStamp()} setuping x{kappas.Count} kappas");

            for (int i = 0; i < kappas.Count; i++)
            {
                kappas[i].brainReady(this);
            }

            if (_existOnStartup) setupExistOnStartup();
        }

        protected override void setupLate()
        {
            base.setupLate();

            resetKappas();
        }

        /// <summary>
        /// contextual call to reset all kappas
        /// </summary>
        virtual public void reset()
        {
            resetKappas();
        }

        virtual protected void resetKappas()
        {
            for (int i = 0; i < kappas.Count; i++)
            {
                kappas[i].resetKappa();
            }
        }

        /// <summary>
        /// pour pouvoir faire un truc spé quand l'objet est déjà présent dans la scène
        /// quand on démarre le projet
        /// </summary>
        virtual protected void setupExistOnStartup()
        { }

        /// <summary>
        /// show all rig
        /// </summary>
        public void doMaterialize()
        {
            //Debug.Log("mat");
            gameObject.SetActive(true);
        }

        /// <summary>
        /// hide visual
        /// </summary>
        public void doDematerialize()
        {
            //Debug.Log("demat");
            gameObject.SetActive(false);
        }

        protected sealed override void update(float dt)
        {
            base.update(dt);

            brainUpdate();

            for (int i = 0; i < kappas.Count; i++)
            {
                var inst = kappas[i] as iKappaUpdatable;
                if(inst != null)
                {
                    if (!inst.canUpdate()) continue;
                    inst.updateKappa(dt);
                }
                
            }
        }

        /// <summary>
        /// before kappas
        /// </summary>
        virtual protected void brainUpdate()
        {
        }

        public void subKappa(KappaBase capa)
        {
            if (kappas.IndexOf(capa) > 0) return;
            kappas.Add(capa);

            //Debug.Log(getStamp() + " subbed capa : " + capa, capa);
        }

        public void unsubKappa(KappaBase capa)
        {
            if (kappas.IndexOf(capa) < 0) return;
            kappas.Remove(capa);

            //Debug.Log(getStamp() + " un-subbed capa : " + capa, capa);
        }

        /// <summary>
        /// called by any child kappa that need to bubble an event
        /// </summary>
        virtual public void kappaEvent(KappaBase kappa)
        { }

        /// <summary>
        /// this will only check for exiting/fetched kappas (present in buff array)
        /// during setup phase it's best to use "safe" to avoid racing conditions
        /// </summary>
        public T getCapacity<T>() where T : KappaBase
        {
            for (int i = 0; i < kappas.Count; i++)
            {
                var inst = kappas[i] as T;
                if (inst != null) return inst;
            }
            return null;
        }

        /// <summary>
        /// fetch + assert
        /// </summary>
        public T getCapacitySafe<T>() where T : KappaBase
        {
            T tar = getCapacityFetch<T>();

            //safe
            if (tar == null)
            {
                Debug.LogError(name + " has no capac " + typeof(T) + " (dont ask for capa in build())", transform);
            }

            return tar;
        }

        /// <summary>
        /// search for it
        /// if none, fetch for it in hierarchy
        /// force refresh kappas array if not found
        /// less opti, fine for boot
        /// </summary>
        public T getCapacityFetch<T>() where T : KappaBase
        {
            T tar = getCapacity<T>();

            //force subscribe
            if (tar == null)
            {
                tar = GetComponentInChildren<T>();
                if (tar != null) tar.brainReady(this);
            }

            return tar;
        }

        public List<KappaBase> getAllKappas()
        {
            List<KappaBase> kaps = new List<KappaBase>();
            kaps.AddRange(transform.GetComponentsInChildren<KappaBase>());
            return kaps;
        }

        public virtual void onSelected()
        {
            //TinyQueries.camera.setFollowTarget(transform);
        }

        public virtual void onUnselected()
        {
            //throw new System.NotImplementedException();
        }

        virtual public bool isSelectable() => gameObject.activeSelf;

        virtual public bool isSelected() => false;

        public T fetchInHierarchy<T>(string gameObjectName) where T : Component
        {
            T[] cmps = transform.GetComponentsInChildren<T>();
            if (cmps.Length <= 0)
            {
                Debug.LogError("searching for " + typeof(T) + " on " + gameObjectName + ", but context " + name + " has no component of that type");
                return null;
            }

            //Debug.Log("?" + cmps.Length);

            for (int i = 0; i < cmps.Length; i++)
            {
                //Debug.Log(cmps[i].name + " ? " + gameObjectName);

                if (cmps[i].name == gameObjectName)
                {
                    return cmps[i];
                }
            }
            return null;
        }


        static public Transform toggleChangeling(MonoBehaviour parent, string changelingName)
        {
            //toggle transforms
            // search by name (ToLower)
            Transform curPivot = parent.transform.Find($"{STATE_PREFIX}{changelingName.ToLower()}");

            //Debug.Assert(curPivot != null);

            if (curPivot == null)
            {
                //Debug.LogWarning($"changeling for {parent.name}, state <b>{changelingName}</b> doesn't have matching child");
                return null;
            }

            Debug.Log("changeling !");

            foreach (Transform child in parent.transform)
            {
                // only states with prefix
                if (!child.name.StartsWith(STATE_PREFIX)) continue;

                if (child == curPivot)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }

            return curPivot;
        }

        public void attach(Transform tarParent, bool align = true)
        {
            transform.SetParent(tarParent);

            //reset to pivot
            if (tarParent != null && align)
            {
                transform.localPosition = Vector3.zero;
            }
        }

        public Vector2 Position
        {
            get { return transform.position; }
        }

        public override string stringify()
        {
            string output = base.stringify();

            output += "\n[KAPPAS x" + kappas.Count + "]";

            for (int i = 0; i < kappas.Count; i++)
            {
                output += "\n " + kappas[i].stringify();
            }

            return output;
        }

    }

}
