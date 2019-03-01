//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//#if UNITY_EDITOR
//namespace EngineCore
//{
//    public class EditorResource : ResourceModule
//    {
//        private Dictionary<string, string> mDicFilePath = new Dictionary<string, string>();
//        private Dictionary<string, UnityEngine.Object> mDicFileObject = new Dictionary<string, UnityEngine.Object>();

//        public override void GetAsset(string name, Action<string, UnityEngine.Object> func, LoadPriority priority = LoadPriority.Default)
//        {
//            UnityEngine.Object obj = null;
//            if (mDicFileObject.TryGetValue(name, out obj))
//            {
//                if (func != null)
//                    func(name, GameObject.Instantiate(obj));
//                return;
//            }

//            string fullName = "";
//            if (!mDicFilePath.TryGetValue(name, out fullName))
//            {
//                foreach (string file in UnityEditor.AssetDatabase.GetAllAssetPaths())
//                {
//                    if (file.Contains("/generate/"))
//                        continue;
//                    if (Path.GetFileName(file) == name)
//                    {
//                        fullName = file;
//                        mDicFilePath.Add(name, fullName);
//                        break;
//                    }
//                }
//            }

//            if (null != fullName && fullName != "")
//            {
//                obj = LoadAndRegisterAssetByFullName(fullName);
//                if (obj != null)
//                {
//                    if (func != null)
//                    {
//                        if (name.IndexOf(".exr") >= 0)
//                        {
//                            func(name, obj);
//                        }
//                        else
//                        {
//                            func(name, GameObject.Instantiate(obj));
//                        }
//                    }
//                    return;
//                }
//            }
//        }

//        public UnityEngine.Object LoadAndRegisterAssetByFullName(string fullName)
//        {
//            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fullName, typeof(UnityEngine.Object));
//            string name = Path.GetFileName(fullName);
//            if (obj != null && !mDicFileObject.ContainsKey(name))
//            {
//                mDicFileObject.Add(name, obj);
//            }
//            return obj;
//        }

//        public override void GetScene(string name, Action callback, LoadPriority priority = LoadPriority.Default)
//        {
//            if (ResourceMgr.Instance().GetBundleName(name) == string.Empty)
//            {
//                ResourceMgr.Instance().RegisterBundleIdx(name, name + EngineFileUtil.m_sceneExt);
//            }
//            base.GetScene(name, callback);
//        }

//        internal override void Shutdown()
//        {
//            mDicFileObject.Clear();
//            mDicFilePath.Clear();

//        }

//        public override void GetString(string name, Action<string, string> callback)
//        {
//            if (GOERootCore.IsEditor)
//            {
//                string fullName = "";
//                if (!mDicFilePath.TryGetValue(name, out fullName))
//                {
//                    foreach (string file in UnityEditor.AssetDatabase.GetAllAssetPaths())
//                    {
//                        if (Path.GetFileName(file) == name)
//                        {
//                            fullName = file;
//                            mDicFilePath.Add(name, fullName);
//                            break;
//                        }
//                    }
//                }

//                if (null != fullName && fullName != "")
//                {
//                    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fullName, typeof(UnityEngine.Object));

//                    if (obj != null)
//                    {
//                        if (callback != null)
//                            callback(name, (obj as TextAsset).text);
//                    }
//                }
//            }
//            else
//            {
//                base.GetString(name, callback);
//            }
//        }
//    }
//}

//#endif