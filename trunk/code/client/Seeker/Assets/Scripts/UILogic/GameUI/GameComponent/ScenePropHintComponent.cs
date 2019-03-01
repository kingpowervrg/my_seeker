using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    public class ScenePropHintComponent : GameUIComponent
    {
        private GameLabel m_lab;
        private GameImage m_img;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_img = Make<GameImage>("Image");
            this.m_lab = Make<GameLabel>("Image:Text");
        }

        public void SetContent(string content)
        {
            this.m_lab.Text = content;
            TimeModule.Instance.SetTimeout(CalutionImage, 0.2f);
        }

        public override void OnHide()
        {
            base.OnHide();
            TimeModule.Instance.RemoveTimeaction(CalutionImage);
        }

        public void CalutionImage()
        {
            if (this.m_img != null)
            {
                this.m_img.Widget.sizeDelta = new Vector2(this.m_lab.Widget.sizeDelta.x + 10, this.m_img.Widget.sizeDelta.y);
            }
        }

    }
}
