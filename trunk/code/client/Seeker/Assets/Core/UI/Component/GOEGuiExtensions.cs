using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public static class GOEGuiExtensions
    {
        public static GameObject GetUGUIComponentByID(this GameObject root, string id)
        {
            string[] arr = id.Split(':');
            string path = string.Empty;
            if (arr[0] == root.name)
            {
                if (arr.Length == 1)
                    return root;
                path = string.Join("/", arr, 1, arr.Length - 1);
                Transform tt = root.transform.Find(path);
                if (tt != null)
                {
                    return tt.gameObject;
                }
                return null;
            }
            else
            {
                path = string.Join("/", arr);
            }
            Transform t = getChildByPath(root.transform, path);
            if (t != null)
                return t.gameObject;
            return null;
        }

        private static Transform getChildByPath(Transform t, string path)
        {
            Transform tt = t.Find(path);
            if (tt != null)
            {
                return tt;
            }
            int len = t.childCount;
            for (int i = 0; i < len; i++)
            {
                Transform ct = t.GetChild(i);
                tt = ct.Find(path);
                if (tt != null)
                {
                    return tt;
                }
                tt = getChildByPath(ct, path);
                if (tt != null)
                    return tt;
            }
            return null;
        }

        private static bool checkPathMatch(Transform t, string path)
        {
            List<string> idList = new List<string>(path.Split(':'));
            idList.Reverse();
            int len = idList.Count;
            Transform tmpTran = t;
            for (int i = 0; i < len; i++)
            {
                if (tmpTran.gameObject.name != idList[i])
                    return false;
                tmpTran = tmpTran.parent;
            }
            return true;
        }

        public static T ToGameUIComponent<T>(this GameObject go)
            where T : GameUIComponent
        {
            if (go)
            {
                GOGUI.EventTriggerListener uel = go.GetComponent<GOGUI.EventTriggerListener>();
                if (uel)
                {
                    return uel.parameter as T;
                }
                else
                    return null;
            }
            else
                return null;
        }




        public static GameUIComponent ToGameUIComponent(this GameObject go)
        {
            if (go)
            {
                GOGUI.EventTriggerListener uel = go.GetComponent<GOGUI.EventTriggerListener>();
                if (uel)
                {
                    return uel.parameter as GameUIComponent;
                }
                else
                    return null;
            }
            else
                return null;
        }

        #region 针对UIContainer 优化
        public static T ToGameUIComponentEx<T>(this GameObject go)
         where T : GameUIComponent
        {
            if (go)
            {
                GOGUI.ComParameter uel = go.GetComponent<GOGUI.ComParameter>();
                if (uel)
                {
                    return uel.parameter as T;
                }
                else
                    return null;
            }
            else
                return null;
        }
        public static GameUIComponent ToGameUIComponentEx(this GameObject go)
        {
            if (go)
            {
                GOGUI.ComParameter uel = go.GetComponent<GOGUI.ComParameter>();
                if (uel)
                {
                    return uel.parameter as GameUIComponent;
                }
                else
                    return null;
            }
            else
                return null;
        }
        #endregion  
    }
}