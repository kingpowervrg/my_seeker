using System.Collections.Generic;

namespace SeekerGame
{
    public class GuidTalk : AbsGuid
    {

        public override void StartGuid()
        {
            base.StartGuid();
            GameEvents.UIEvents.UI_Talk_Event.OnTalkRewardFinish += TalkOver;
            long talkID = long.Parse(m_CurConf.typeValue);
            TalkUIHelper.OnStartTalk(talkID);
        }

        protected override void EndGuid()
        {
            base.EndGuid();
        }

        protected override void Destory()
        {
            base.Destory();
            GameEvents.UIEvents.UI_Talk_Event.OnTalkRewardFinish -= TalkOver;
        }

        private void TalkOver()
        {
            EndGuid();
        }
    }
}
