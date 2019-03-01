using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    public class GameHyperlinkButton : GameButton
    {
        private Text m_hyperlinkUrlText = null;
        private string m_realHyperlinkUrl = string.Empty;
        protected override void OnInit()
        {
            base.OnInit();

            if (button != null)
            {
                this.m_hyperlinkUrlText = button.GetComponentInChildren<Text>(true);
                if (this.m_hyperlinkUrlText == null)
                    Debug.LogError("no url address");

                if (!StringUtils.IsUrlValid(this.m_hyperlinkUrlText.text, out m_realHyperlinkUrl))
                    Debug.LogError($"{this.m_hyperlinkUrlText.text} is not valid url");
            }
        }

        public override void OnShow(object param)
        {
            AddClickCallBack(OnHyperlinkButtonClick);
        }


        private void OnHyperlinkButtonClick(GameObject hyperlinkButton)
        {
            Application.OpenURL(m_realHyperlinkUrl);
        }

        public override void OnHide()
        {
            base.OnHide();
            RemoveClickCallBack(OnHyperlinkButtonClick);
        }


    }
}
