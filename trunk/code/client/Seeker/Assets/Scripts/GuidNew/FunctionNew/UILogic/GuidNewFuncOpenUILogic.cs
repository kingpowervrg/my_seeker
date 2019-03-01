using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 打开界面UI
    /// </summary>
    public class GuidNewFuncOpenUILogic : GuidNewFunctionBase
    {
        private int m_type = -1; //目前0表示对话  1表示播放视频  2表示打开拼图和寻物  3事件
        private string m_param = string.Empty;
        private float m_delayTime = 0f;
        private bool m_isDelaying;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                this.m_type = int.Parse(param[0]);
            }
            if (param.Length >= 2)
            {
                this.m_param = param[1];
            }
            if (param.Length >= 3)
            {
                this.m_delayTime = float.Parse(param[2]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (m_type < 0)
            {
                OnDestory();
                return;
            }
            if (m_delayTime > 0)
            {
                this.m_isDelaying = true;
                TimeModule.Instance.SetTimeout(OnDelayExecute, m_delayTime);
            }
            else
            {
                OnDelayExecute();
            }
        }

        private void OnDelayExecute()
        {
            this.m_isDelaying = false;
            if (0 == m_type)
            {
                GameEvents.UI_Guid_Event.OnUIEnableClick.SafeInvoke(true);
                long talkId = long.Parse(this.m_param);
                GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish += OnTalkFinish;
                TalkUIHelper.OnStartTalk(talkId);
            }
            else if (1 == m_type)
            {
                ProloguePlayVideoManager.PlayVideo(m_param);
                OnDestory();
            }
            else if (2 == m_type)
            {
                CommonHelper.OpenEnterGameSceneUI(long.Parse(m_param));
                OnDestory();
            }
            //else if (3 == m_type)
            //{
            //    EventGameUIAssist.BeginEventGame(long.Parse(m_param));
            //    OnDestory();
            //}
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            if (this.m_isDelaying)
            {
                TimeModule.Instance.RemoveTimeaction(OnDelayExecute);
                this.m_isDelaying = false;
            }
        }

        private void OnTalkFinish(long talkID)
        {
            GameEvents.UIEvents.UI_Talk_Event.OnTalkFinish -= OnTalkFinish;
            OnDestory();
        }


    }
}
