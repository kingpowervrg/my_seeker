using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GOGUI
{
    public class TweenGroup
    {
        List<UITweenerBase> tweens = new List<UITweenerBase>();
        public List<UITweenerBase> Tweens { get { return tweens; } }

        public void ResetAndPlay()
        {
            foreach (var i in tweens)
            {
                
            }
        }

        public void Stop()
        {
            foreach (var i in tweens)
            {
                i.enabled = false;
            }
        }
    }
    public static class GOGUITools
    {
        public static SafeAction<string, System.Action<string, Object>, LoadPriority> GetAssetAction;
        public static SafeAction<string, Object> ReleaseAssetAction;
        public static Camera UICamera;

        public static Dictionary<int, TweenGroup> GetTweenGroup(this GameObject go, System.Action<int, UITweenerBase> onProcessTweener = null)
        {
            Dictionary<int, TweenGroup> res = new Dictionary<int, TweenGroup>();
            UITweenerBase[] t = go.GetComponents<UITweenerBase>();
            /*foreach (var i in t)
            {
                int group = i.tweenGroup;
                TweenGroup grp;
                if (!res.TryGetValue(group, out grp))
                {
                    grp = new TweenGroup();
                    res[group] = grp;
                }
                if (onProcessTweener != null)
                    onProcessTweener(group, i);
                grp.Tweens.Add(i);
            }*/

            return res;
        }

        /// <summary>
        /// 添加模板子节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Transform AddChild(Transform parent, Transform templateTransform)
        {
            Transform childTransform = Object.Instantiate(templateTransform, parent, false);
            return childTransform;
        }


        /// <summary>
        ///  Class + Function 转换为 Class.Function.
        /// </summary>
        static public string GetFuncName(object obj, string method)
        {
            if (obj == null) return "<null>";
            string type = obj.GetType().ToString();
            int period = type.LastIndexOf('/');
            if (period > 0) type = type.Substring(period + 1);
            return string.IsNullOrEmpty(method) ? type : type + "/" + method;
        }

        /// <summary>
        /// 添加缺少的组件
        /// </summary>
        static public T AddMissingComponent<T>(this GameObject go) where T : Component
        {
#if UNITY_FLASH
        object comp = go.GetComponent<T>();
#else
            T comp = go.GetComponent<T>();
#endif
            if (comp == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    RegisterUndo(go, "Add " + typeof(T));
#endif
                comp = go.AddComponent<T>();
            }
#if UNITY_FLASH
        return (T)comp;
#else
            return comp;
#endif
        }

        static public void RegisterUndo(UnityEngine.Object obj, string name)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(obj, name);
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        static public void SetDirty(Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

    }
}