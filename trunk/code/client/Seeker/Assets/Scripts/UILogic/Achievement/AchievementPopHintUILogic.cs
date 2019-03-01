using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ACHIEVEMENTHINT)]
    public class AchievementPopHintUILogic : UILogicBase
    {
        private GameLabel m_contentLab = null;
        private GameUIComponent m_component = null;
        //private TweenScale m_tweenPos = null;
        // private GameImage m_bgImg = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_component = Make<GameUIComponent>("bg");
            //this.m_bgImg = Make<>
            this.m_contentLab = Make<GameLabel>("bg:Text");
            //this.m_tweenPos = this.m_component.GetComponent<TweenScale>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_Achievement_Event.AchievementStateChange += AchievementStateChange;
            if (param != null)
            {
                long id = (long)param;
                AchievementStateChange(id, true);
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_Achievement_Event.AchievementStateChange -= AchievementStateChange;
        }

        private void AchievementStateChange(long id, bool state)
        {
            ConfAchievement achievement = ConfAchievement.Get(id);
            this.m_contentLab.Text = LocalizeModule.Instance.GetString("Achievement_progress", LocalizeModule.Instance.GetString(achievement.name));

            TimeModule.Instance.SetTimeout(() =>
            {
                this.m_component.Widget.sizeDelta = new Vector2(this.m_contentLab.Widget.sizeDelta.x + 10f, this.m_component.Widget.sizeDelta.y);
                m_component.Visible = state;
                //this.m_tweenPos.ResetAndPlay();
                TimeModule.Instance.SetTimeout(HideAchievement, 3f);
            }, 0.2f);

        }

        private void HideAchievement()
        {
            //this.m_tweenPos.PlayBackward();
            TimeModule.Instance.SetTimeout(() =>
            {
                m_component.Visible = false;
            }, 0.5f);
        }

        public static void Show(long id)
        {
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_ACHIEVEMENTHINT);
            param.Param = id;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            GameEvents.UIEvents.UI_Achievement_Event.AchievementStateChange.SafeInvoke(id, true);
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.WINDOWED;


    }
}
