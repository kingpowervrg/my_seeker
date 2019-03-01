using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using GOEngine;

namespace SeekerGame
{
    public class TalkUIHelper
    {

        private static TalkDialogEnum m_talkEnum = TalkDialogEnum.NormalTalk;
        public static TalkDialogEnum TalkEnum
        {
            get
            {
                return m_talkEnum;
            }
        }

        public static void OnStartTalk(long id, TalkDialogEnum talkEnum = TalkDialogEnum.SceneTalk)
        {
            m_talkEnum = talkEnum;
            if (TalkDialogEnum.SceneTalk == talkEnum)
            {
                CSChatFinishRequest req = new CSChatFinishRequest();
                req.ChatId = id;
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            }

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
            {
                        { UBSParamKeyName.ContentID, id},
            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.dialogue_star, null, _params);
            ConfChat chat = ConfChat.Get(id);
            if (chat == null)
            {
                DebugUtil.LogErrorFormat("talk id not exits : {0}", id);
                return;
            }

            OnStartTalkUI(id, talkEnum);
        }

        private static void OnStartTalkUI(long id, TalkDialogEnum talkEnum = TalkDialogEnum.SceneTalk)
        {
            TalkUIData talkUIData = new TalkUIData();
            talkUIData.m_talk_type = talkEnum;
            talkUIData.talk_id = id;

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SCENETALK);
            param.Param = talkUIData;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }
    }
}

