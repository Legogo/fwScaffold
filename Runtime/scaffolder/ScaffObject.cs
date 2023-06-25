using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    abstract public class ScaffObject : iScaffLog
    {
        //protected bool _freeze;

        public ScaffObject()
        {
            create();
        }

        virtual protected void create()
        { }

        virtual protected bool isVerbose()
        {
            return true;
        }

        //protected void freeze(bool flag) => _freeze = flag;
        //public bool isFreezed() => _freeze;

        protected void logw(string content, Object behav = null) => Debug.LogWarning(stamp() + content, behav);
        protected void log(string content, Object behav = null)
        {
            if (!isVerbose()) return;

            Debug.Log(stamp() + content, behav);
        }

        static public string stampPrefix()
        {
            string prefix = "(ru)";
            if (Application.isEditor) prefix = "(ed)";
            if (!Application.isPlaying) prefix = "!" + prefix;
            prefix += "@" + Time.frameCount;
            return prefix;
        }

        public const string space = " ";

        virtual public string stamp()
        {
            return stampPrefix() + "|" + GetType().ToString() + "| ";
        }

        //[ContextMenu("stringify")]
        virtual public string stringify()
        {
            return GetType().ToString();
        }
    }

}
