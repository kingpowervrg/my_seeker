using EngineCore;

namespace SeekerGame
{
    public class PopUpTwoBtnView : GameUIComponent
    {
        private GameButton m_twoFirst_btn;
        private GameButton m_twoSecond_btn;
        private GameLabel m_twoFirst_lab;
        private GameLabel m_twoSecond_lab;


        protected override void OnInit()
        {
            m_twoFirst_btn = this.Make<GameButton>("btn0");
            m_twoSecond_btn = this.Make<GameButton>("btn1");
            m_twoFirst_lab = m_twoFirst_btn.Make<GameLabel>("Text");
            m_twoSecond_lab = m_twoSecond_btn.Make<GameLabel>("Text");
        }

        public override void OnShow(object param)
        {

        }

        public override void OnHide()
        {

        }

        public void Refresh()
        {

        }
    }
}
