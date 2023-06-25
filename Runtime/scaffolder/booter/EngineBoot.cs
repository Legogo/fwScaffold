using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    /// <summary>
    /// element to override to react to booting process
    /// 
    /// create your own version and call boot somehow
    /// </summary>
    abstract public class EngineBoot : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// entry point to trigger engine loading process
        /// </summary>
        virtual public void boot()
        {
            EngineStartup.create();
        }

        /// <summary>
        /// this is called when engine is done loading basic stuff
        /// </summary>
        abstract public void loadingCompleted();

        static public bool isLoading()
        {
            return EngineStartup.instanceExist();
        }
    }

}
