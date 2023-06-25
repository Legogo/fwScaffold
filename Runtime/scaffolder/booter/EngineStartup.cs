using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// WON'T AUTO BOOT
/// 
/// create
/// load engine
/// wait for feeders & co
/// destroy
/// 
/// as long as its instance exists == loading ...
/// </summary>

namespace fwp.scaffold
{
    using fwp.scenes;

    public class EngineStartup : MonoBehaviour
    {
        // permet de savoir si le moteur est actif
        //devient true quand on passe le check de compat
        static public bool compatibility = false;

        static protected EngineStartup eStartupInstance = null;

        //[RuntimeInitializeOnLoadMethod]
        static public void create()
        {
            if (eStartupInstance != null)
            {
                Debug.LogWarning($"engineer startup is already running ?");
                return;
            }

            compatibility = isContextEngineCompatible();

            if (!compatibility)
            {
                string filter = getContextEngineCompatibility();
                Debug.LogWarning($"won't load engineer here : scene starts with prefix : <b>" + filter + "</b>");
                return;
            }

            Debug.Log($"engineer is ON ; creating startup object");

            eStartupInstance = new GameObject("[startup]").AddComponent<EngineStartup>();
            eStartupInstance.startupProcess();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void startupProcess()
        {
            Debug.Assert(compatibility, $"{getStamp()} compatibility should be true here");

            //don't load engine on guide scenes (starting with ~)
            if (SceneLoader.doActiveSceneNameContains("~", true))
            {
                Debug.LogWarning("<color=red><b>guide scene</b> not loading engine here</color>");
                return;
            }

            if (!SceneLoader.hasAnyScenesInBuildSettings())
            {
                Debug.Log($"{getStamp()} can't load ?");
            }

            StartCoroutine(processStartup());
        }

        IEnumerator processStartup()
        {
            //Coroutine co = null;

            Debug.Log($"{getStamp()} process startup, frame : " + Time.frameCount);

            //leave a few frame for loading screen to be created and displayed
            //Scene are not flagged as loaded during frame 1
            yield return null;
            yield return null;
            yield return null;

            string engineSceneName = SceneLoader.prefixResource + "engine";

            // then we load engine, to get the feeder script
            var loader = SceneLoader.loadScene(engineSceneName);

            Debug.Log($"{getStamp()} waiting for engine scene ...");

            while (loader != null) yield return null;

            //NEEDED if not present
            //must be created after the (existing ?) engine scene is loaded (doublon)
            //EngineManager.create();

            //safe check for engine scene presence
            Scene engineScene = SceneManager.GetSceneByName(engineSceneName);
            Debug.Assert(engineScene.IsValid());

            while (!engineScene.isLoaded) yield return null;

            Debug.Log($"{getStamp()} triggering feeders ...");

            // les feeders qui sont déjà présents quand on lance le runtime (pas par un load)
            SceneLoaderFeederBase[] feeders = GameObject.FindObjectsOfType<SceneLoaderFeederBase>();
            Debug.Log($"{getStamp()} {feeders.Length} feeders still running");

            for (int i = 0; i < feeders.Length; i++)
            {
                //feeders[i].feed(gameObject.scene);
                if (!feeders[i].isFeeding()) feeders[i].feed();
            }

            //tant qu'on a des loaders qui tournent ...
            while (SceneLoader.areAnyLoadersRunning()) yield return null;

            Debug.Log($"{getStamp()} is done at frame " + Time.frameCount + ", removing gameobject");

            if (engineScene.rootCount <= 0)
            {
                SceneManager.UnloadSceneAsync(engineScene);
            }

            EngineBoot booter = GameObject.FindObjectOfType<EngineBoot>();
            if(booter == null)
            {
                Debug.LogWarning($"{getStamp()} no booter found ?");
            }
            else
            {
                booter?.loadingCompleted();
            }

            yield return null;

            GameObject.Destroy(gameObject);
        }

        static public bool isContextEngineCompatible()
        {
            return getContextEngineCompatibility().Length <= 0;
        }

        static string[] filters = new string[] { "~", "#", "network", "precheck" };

        static string getContextEngineCompatibility()
        {
            for (int i = 0; i < filters.Length; i++)
            {
                if (SceneLoader.doActiveSceneNameContains(filters[i], true))
                {
                    return filters[i];
                }
            }

            return string.Empty;
        }

        public string getStamp()
        {
            return "<color=#081365>" + GetType().ToString() + "</color>";
        }


        static public bool instanceExist()
        {
            //if (eStartupInstance == null) eStartupInstance = GameObject.FindObjectOfType<EngineStartup>();
            return eStartupInstance != null;
        }
    }
}