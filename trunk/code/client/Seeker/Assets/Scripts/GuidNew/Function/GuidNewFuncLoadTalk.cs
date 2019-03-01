using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadTalk : GuidNewFunctionBase
    {
        private float delayTime = 0f;
        private string content;
        private int talkType;
        private int needClickType = 0;  //0表示开启就结束  1表示点击遮罩结束  2表示点击空白结束
        private Vector2 startPos, endPos;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.delayTime = float.Parse(param[0]);
            this.content = param[1];
            this.talkType = int.Parse(param[2]);
            this.startPos = new Vector2(float.Parse(param[3]),float.Parse(param[4]));
            this.endPos = new Vector2(float.Parse(param[5]), float.Parse(param[6]));
            if (param.Length == 8)
            {
                needClickType = int.Parse(param[7]);
            }
        }

        private void OnMaskClick(Vector2 worldPos, bool inner)
        {
            if (needClickType == 1 && inner)
            {
                
                OnDestory();
            }
        }

        private void TimeOut()
        {
            GameEvents.UI_Guid_Event.OnTalkEvent.SafeInvoke(content, talkType, startPos, endPos, needClickType);
        }

        public override void OnExecute()
        {
            base.OnExecute();

            if (delayTime > 0)
            {
                TimeModule.Instance.SetTimeout(TimeOut, delayTime);
            }
            else
            {
                TimeOut();
            }
            if (needClickType == 1)
            {
                //GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
                GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
                //GameEvents.UI_Guid_Event.OnShowMask.SafeInvoke(null, null, MaskAnimType.None, "");
            }
            else if (needClickType == 2)
            {
                GameEvents.UI_Guid_Event.OnEnableClick += OnEnableClick;
                TimeModule.Instance.SetTimeout(()=> {
                    if (this.m_isClearing)
                    {
                        return;
                    }
                    GameEvents.UI_Guid_Event.OnTalkClick += OnTalkClick;
                },this.delayTime + 1f);
            }
            else if (needClickType == 3)
            {
                GameEvents.UI_Guid_Event.OnMaskTalkVisible += OnMaskTalkVisible;
            }
            else
            {
                OnDestory();
            }

        }

        private void OnMaskTalkVisible(bool visible)
        {
            OnDestory();
        }

        private void OnEnableClick(bool enableClick)
        {
            if (needClickType == 2)
            {
                OnTalkClick();
            }
        }

        private bool m_talkOver = false;
        private void OnTalkClick()
        {
            GameEvents.UI_Guid_Event.OnMaskTalkVisible.SafeInvoke(false);
            OnDestory();
            //if (this.m_talkOver)
            //{
            //    return;
            //}
            //this.m_talkOver = true;
            //TimeModule.Instance.SetTimeout(()=> {
               
            //},delayTime + 1f);
            
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            this.m_talkOver = false;
            if (needClickType == 1)
            {
                TimeModule.Instance.RemoveTimeaction(TimeOut);
                GameEvents.UI_Guid_Event.OnMaskTalkVisible.SafeInvoke(false);
                GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
            }
            else if (needClickType == 2)
            {
                GameEvents.UI_Guid_Event.OnEnableClick -= OnEnableClick;
                GameEvents.UI_Guid_Event.OnTalkClick -= OnTalkClick;
            }
            else if (needClickType == 3)
            {
                GameEvents.UI_Guid_Event.OnMaskTalkVisible -= OnMaskTalkVisible;
            }
            base.OnDestory(funcState);
            
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
            GameEvents.UI_Guid_Event.OnEnableClick -= OnEnableClick;
            GameEvents.UI_Guid_Event.OnTalkClick -= OnTalkClick;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible -= OnMaskTalkVisible;
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
            GameEvents.UI_Guid_Event.OnEnableClick -= OnEnableClick;
            GameEvents.UI_Guid_Event.OnTalkClick -= OnTalkClick;
            GameEvents.UI_Guid_Event.OnMaskTalkVisible -= OnMaskTalkVisible;
        }

    }
}
