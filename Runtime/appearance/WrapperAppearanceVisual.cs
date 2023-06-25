using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    /// <summary>
    /// used by objects that uses a Visual pivot
    /// </summary>
    public class WrapperAppearanceVisual : WrapperAppearance
    {
        /// <summary>
        /// all sub transform starting with a prefix
        /// </summary>
        Transform[] subs;

        const string visualSubPrefix = "sub_";
        const string visualPivotName = "visual";

        public WrapperAppearanceVisual(MonoBehaviour owner) : base(owner)
        {

            Debug.Assert(pivotSymbol != null, "no 'Visual' hierarchy of " + owner, owner);

            var pivot = getPivot();
            if (pivot != null)
            {
                List<Transform> list = new List<Transform>();
                foreach (Transform tr in pivot)
                {
                    if (tr.name.StartsWith(visualSubPrefix))
                    {
                        list.Add(tr);
                    }
                }
                subs = list.ToArray();

                if (subs.Length > 0)
                {
                    Debug.Log(owner.name + " has x" + subs.Length + " subs", owner);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void toggleSub(string subName)
        {
            if (subs.Length <= 0)
            {
                Debug.LogWarning("no subs to toggle ?");
                return;
            }

            for (int i = 0; i < subs.Length; i++)
            {
                bool candid = subs[i].name.EndsWith(subName);

                //Debug.Log(subs[i].name + " ? " + candid);

                subs[i].gameObject.SetActive(candid);
            }
        }

        protected override Transform getPivot()
        {
            return owner.transform.Find(visualPivotName);
        }

        public override string stringify()
        {
            string content = base.stringify();
            content += "\n subs x" + subs.Length;

            foreach (Transform tr in owner.transform)
            {
                content += "\n  " + tr.name;
            }

            return content;
        }
    }

}
