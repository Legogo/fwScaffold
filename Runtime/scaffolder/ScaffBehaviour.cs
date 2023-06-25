using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    /// <summary>
    /// meant to be the base of mono linked to scaffolding
    /// </summary>
    public class ScaffBehaviour : MonoBehaviour
    {
        bool _debugScene;
        bool _early;
        bool _ready;

        [SerializeField]
        protected bool verbose = false;

        protected void Awake()
        {
            _stampColor = solveStampColor();

            _debugScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene() == gameObject.scene;

            build();
        }

        virtual protected void build()
        {
            if (!isContextLoading())
            {
                setupEarly();
            }
        }

        void Start()
        {
            enabled = false;

            StartCoroutine(processStart());
        }

        protected bool isContextLoading()
        {
            // EngineBoot.isLoading()
            return false;
        }

        IEnumerator processStart()
        {
            Debug.Assert(!_ready, "nop");

            //Debug.Log(name + " is checking for loading ...");

            while (isContextLoading()) yield return null;

            //Debug.Log(name + " is done loading, setuping ...");

            // not twice
            if (!_early) setupEarly();

            yield return null;
            setup();

            yield return null;
            setupLate();

            yield return null;
            if (_debugScene) setupDebug();
        }

        /// <summary>
        /// if generated at runtime, called during build()
        /// use this to setup more stuff right away after loading
        /// </summary>
        virtual protected void setupEarly()
        {
            Debug.Assert(_early == false, "nop");
            _early = true;
        }

        /// <summary>
        /// will be called during Start() frame
        /// </summary>
        virtual protected void setup()
        { }

        virtual protected void setupLate()
        {
            _ready = true;

            enabled = true;
        }

        virtual protected void setupDebug()
        {

        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;
            onDestroy();
        }

        virtual protected void onDestroy()
        { }

        /// <summary>
        /// only the component
        /// </summary>
        [ContextMenu("destroy")]
        public void destroy()
        {
            GameObject.Destroy(this);
        }

        public bool isReady() => _ready;


        virtual public string stamp()
        {
            return ScaffObject.stampPrefix() + $"|<color={_stampColor}>{GetType()}</color>|" + name + ScaffObject.space;
        }

        string _stampColor = string.Empty;
        virtual protected string solveStampColor() => "gray";

        virtual public string stringify()
        {
            return stamp() + " (ready?" + _ready + ")";
        }

        protected void logw(string content, Object behav = null) => Debug.LogWarning(stamp() + content, behav);
        protected void log(string content, Object behav = null)
        {
            if (verbose)
                Debug.Log(stamp() + content, behav);
        }

        [ContextMenu("stringify")]
        void logStringify() => Debug.Log(stringify());

    }

}
