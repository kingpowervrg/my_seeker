using EngineCore;
using GOEngine;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{

    [UILogicHandler(UIDefine.UI_EXHIBITION_HALL)]
    public class ExhibitionHallUILogic : UILogicBase
    {

        private GameButton m_btnQuit = null;

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        protected override void OnInit()
        {

            m_btnQuit = Make<GameButton>("Panel_top:Button_quit");

        }

        public override void OnShow(object param)
        {

            this.m_btnQuit.AddClickCallBack(OnBtnQuitClick);


        }

        private void OnBtnQuitClick(GameObject btnQuit)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            GameEvents.SceneEvents.OnLeaveScene.SafeInvoke();
            this.CloseFrame();
            SceneModule.Instance.EnterMainScene();
        }

        public override void OnHide()
        {
            base.OnHide();

            this.m_btnQuit.RemoveClickCallBack(OnBtnQuitClick);
        }

    }
}
