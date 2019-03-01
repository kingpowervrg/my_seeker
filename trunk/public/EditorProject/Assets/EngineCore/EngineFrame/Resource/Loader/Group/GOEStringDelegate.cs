using System;
using System.Collections.Generic;

namespace EngineCore
{
    internal class GOEStringDelegate : ResComponent
    {
        private Dictionary<string, string> mStringDic = new Dictionary<string, string>();
        private Dictionary<string, Action<string, string>> mTxtHandlerDic = new Dictionary<string, Action<string, string>>();
        internal string GetString(string name, Action<string, string> callback = null)
        {
            //已经缓存
            if (mStringDic.ContainsKey(name))
            {
                if (callback != null)
                    callback(name, mStringDic[name]);
            }
            else
            {
                if (mTxtHandlerDic.ContainsKey(name))
                    mTxtHandlerDic[name] += callback;
                else
                    mTxtHandlerDic.Add(name, callback);
                ResourceModule.Instance.GetAsset(name, OnGetAsset, LoadPriority.MostPrior);
            }
            return null;
        }

        internal override void OnLeaveScene()
        {
            base.OnLeaveScene();
            mStringDic.Clear();
        }

        private void OnGetAsset(string name, UnityEngine.Object obj)
        {
            string textStr = (obj as UnityEngine.TextAsset).text;
            if (mStringDic.ContainsKey(name))
                mStringDic[name] = textStr;
            else
                mStringDic.Add(name, textStr);

            if (mTxtHandlerDic.ContainsKey(name))
            {
                mTxtHandlerDic[name](name, textStr);
                mTxtHandlerDic.Remove(name);
            }

            //释放TextAsset对象
            ResourceModule.Instance.ReleaseAsset(name, obj);

        }
    }
}
