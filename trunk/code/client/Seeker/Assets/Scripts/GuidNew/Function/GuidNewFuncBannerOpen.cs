using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncBannerOpen : GuidNewFunctionBase
    {
        private string frameResName;
        private string rootResName;
        private string fallResName;
        private string maskResName;
        private string effectResName;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.frameResName = param[0];
            this.rootResName = param[1].Replace(":", "/");
            this.fallResName = param[2].Replace(":", "/");
            this.maskResName = param[3].Replace(":", "/");
            if (param.Length > 4)
            {
                this.effectResName = param[4].Replace(":", "/");
            }
            //if (param.Length > 4)
            //{
            //    this.effectResName = new string[param.Length - 4];
            //    for (int i = 4; i < param.Length; i++)
            //    {
            //        this.effectResName[i - 4] = param[i];
            //    }
            //}

        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.frameResName);
            if (frame == null)
            {
                OnDestory();
                return;
            }
            Transform rootTrans = frame.FrameRootTransform.Find(this.rootResName);
            Transform fallTrans = frame.FrameRootTransform.Find(this.fallResName);
            Transform maskTrans = frame.FrameRootTransform.Find(this.maskResName);
            Transform effectTrans = null;
            if (!string.IsNullOrEmpty(this.effectResName))
            {
                effectTrans = frame.FrameRootTransform.Find(this.effectResName);

            }
            if (rootTrans == null || fallTrans == null || maskTrans == null)
            {
                OnDestory();
                return;
            }
            maskTrans.gameObject.SetActive(true);
            if (effectTrans != null)
            {
                effectTrans.gameObject.SetActive(false);
            }
            rootTrans.gameObject.SetActive(true);
            TweenScale fallScale = fallTrans.GetComponent<TweenScale>();
            TweenScale maskScale = maskTrans.GetComponent<TweenScale>();
            if (fallScale == null || maskScale == null)
            {
                OnDestory();
                return;
            }
            fallTrans.gameObject.SetActive(true);
            fallScale.SetTweenCompletedCallback(() =>
            {
                maskScale.SetTweenCompletedCallback(() =>
                {
                    maskTrans.gameObject.SetActive(false);
                    if (effectTrans != null)
                    {
                        effectTrans.gameObject.SetActive(true);
                    }
                    OnDestory();
                });
                maskScale.ResetAndPlay(true);
            });
            fallScale.ResetAndPlay(true);
        }

    }
}
