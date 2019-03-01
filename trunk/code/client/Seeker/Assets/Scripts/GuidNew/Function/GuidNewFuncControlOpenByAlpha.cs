using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncControlOpenByAlpha : GuidNewFunctionBase
    {
        private string frameName;
        private string[] resName;
        private bool needAlpha = true;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.frameName = param[0];
            this.resName = new string[param.Length - 2];
            for (int i = 1; i < param.Length-1; i++)
            {
                resName[i - 1] = param[i].Replace(":","/");
            }
            this.needAlpha = bool.Parse(param[param.Length - 1]);
        }
        private int count = 0;
        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.frameName);
            if (frame == null)
            {
                OnDestory();
                return;
            }
            if (this.needAlpha)
            {
                int tempCount = 0;
                for (int i = 0; i < resName.Length; i++)
                {
                    Transform tran = frame.FrameRootTransform.Find(resName[i]);
                    if (tran.GetComponent<UnityEngine.UI.Image>() == null)
                    {
                        tran.gameObject.SetActive(true);
                        continue;
                    }
                    TweenAlpha TweenAlpha = tran.gameObject.GetOrAddComponent<TweenAlpha>();
                    TweenAlpha.From = 0f;
                    TweenAlpha.To = 1f;
                    TweenAlpha.Duration = 1f;
                    tempCount++;
                    TweenAlpha.SetTweenCompletedCallback(() =>
                    {
                        count++;
                        if (count == tempCount)
                        {
                            OnDestory();
                        }
                        GameObject.DestroyImmediate(TweenAlpha);
                    });
                    TweenAlpha.PlayForward();
                    if (tran.parent != null)
                    {
                        tran.parent.gameObject.SetActive(true);
                    }
                    tran.gameObject.SetActive(true);
                }
                if (tempCount - count == 0)
                {
                    OnDestory();
                }
            }
            else
            {
                Transform tran = frame.FrameRootTransform.Find(resName[0]);
                tran.gameObject.SetActive(true);
                OnDestory();
            }
            
        }

    }
}
