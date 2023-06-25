using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fwp.scaffold
{
    public class EngineTools
    {

        const string OUTPUT_STAMP_NULL = "[null]";

        static public string getStamp(Type typ)
        {
            return $"({Time.frameCount}){getStampColored(typ)}";
        }
        static public string getStamp(MonoBehaviour mono)
        {
            return getStamp(mono);
        }

        //https://docs.unity3d.com/Manual/StyledText.html
        static public string getStamp(MonoBehaviour obj, string color = "#e128df")
        {
            if (obj == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("stamping ; object is null ?");
#endif
                return Time.frameCount + " ? " + OUTPUT_STAMP_NULL;
            }

            return $"({Time.frameCount}){getStampColored(obj.GetType(), obj.name)}";
        }

        static public string getStampColored(Type typ, string color = "#ccc", string name = "")
        {
            return $"<color={color}>{typ.ToString()}</color>@{name}";
        }

        static public void log(string data, MonoBehaviour logTarget = null)
        {
            Debug.Log(logTarget.GetType() + " | " + data, logTarget);
        }


        static public Transform extractChildOfName(string name, Transform context)
        {
            Transform child = context.Find(name);
            if (child != null) return child;

            foreach (Transform tr in context)
            {
                child = extractChildOfName(name, tr);
            }

            //if (child == null) Debug.LogWarning("couldn't find child of name " + name + " in hierarchy of " + context.name);

            return child;
        }

        static public Transform getCarrierByContext(string name, Transform context = null)
        {
            return extractChildOfName(name, context);
        }

        static public Transform getCarrier(string name)
        {
            GameObject output = GameObject.Find(name);

            if (output != null)
            {
                return output.transform;
            }

            Debug.LogWarning("couldn't find carry of name " + name);
            return null;
        }

        static public T getComponentInSceneByContext<T>(string carryName, Transform context)
        {
            return getCarrierByContext(carryName, context).GetComponent<T>();
        }

        static public T getComponentInScene<T>(string carryName)
        {
            Transform obj = getCarrier(carryName);
            if (obj != null) return obj.GetComponent<T>();
            return default(T);
        }

        // Increase n towards target by speed
        static public float IncrementTowards(float n, float target, float a)
        {
            if (n == target)
            {
                return n;
            }
            else
            {
                float dir = Mathf.Sign(target - n);
                n += a * Time.deltaTime * dir;
                return (dir == Mathf.Sign(target - n)) ? n : target;
            }
        }

    }

    [Serializable]
    public class ObjectFetch
    {
        public string carry = "";
        protected Component comp;

        public T getComponentOnce<T>() where T : Component
        {
            if (comp == null) comp = getComponent<T>(carry);
            return (T)comp;
        }

        public T getComponent<T>(string carry)
        {
            Transform obj = EngineTools.getCarrier(carry);
            if (obj == null)
            {
                Debug.LogWarning("no object found with name : " + carry);
                return default(T);
            }

            return obj.GetComponent<T>();
        }
    }
}