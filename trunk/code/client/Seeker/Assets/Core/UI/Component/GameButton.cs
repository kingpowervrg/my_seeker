using GOGUI;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    public class GameButton : GameTextComponent
    {
        protected UnityEngine.UI.Button button;
        GameUIComponent lbDisable, lbEnable;
        UnityEngine.UI.Image image;
        private bool showEnable;
        Color defaultColor, hoverColor, pressColor;

        private UIButtonEffect m_buttonEffectComponent = null;

        protected override void OnInit()
        {
            base.OnInit();
            button = GetComponent<UnityEngine.UI.Button>();
            image = GetComponent<UnityEngine.UI.Image>();
            m_buttonEffectComponent = GetComponent<UIButtonEffect>();
            if (button != null)
            {
                showEnable = button.interactable;
                defaultColor = button.colors.normalColor;
                hoverColor = button.colors.highlightedColor;
                pressColor = button.colors.pressedColor;
                /*
                lbEnable = Make<GameUIComponent>("Text");
                if (lbEnable == null)
                    lbEnable = Make<GameUIComponent>("Text1");
                lbDisable = Make<GameUIComponent>("Text2");*/
            }
            else
                showEnable = true;
        }

        public bool Enable
        {
            get { return button.enabled; }
            set
            {
                if (button.enabled != value)
                {
                    button.enabled = value;

                    if (m_buttonEffectComponent)
                        m_buttonEffectComponent.enabled = value;

                    if (image != null)
                    {
                        image.raycastTarget = value;

                        if( value)
                        {
                            GraphicRegistry.RegisterGraphicForCanvas(LogicHandler.UIFrame.UIRootCanvas, image);
                        }
                        else
                        {
                            GraphicRegistry.UnregisterGraphicForCanvas(LogicHandler.UIFrame.UIRootCanvas, image);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 显示禁用态，逻辑上按钮依然可以点击
        /// </summary>
        public bool ShowEnable
        {
            get { return showEnable; }

            set
            {
                showEnable = value;
                if (value)
                {
                    var colors = button.colors;

                    colors.normalColor = defaultColor;
                    colors.highlightedColor = hoverColor;
                    colors.pressedColor = pressColor;
                    button.colors = colors;

                    //button.SetState(UIButtonColor.State.Normal, true);
                    if (lbEnable != null)
                        lbEnable.Visible = true;
                    if (lbDisable != null)
                        lbDisable.Visible = false;
                }
                else
                {
                    var colors = button.colors;
                    colors.normalColor = colors.disabledColor;
                    colors.highlightedColor = hoverColor;
                    colors.pressedColor = pressColor;
                    button.colors = colors;
                    //button.SetState(UIButtonColor.State.Disabled, true);
                    if (lbDisable != null)
                        lbDisable.Visible = true;
                    if (lbEnable != null)
                        lbEnable.Visible = false;
                }
            }
        }

        public override void SetGray(bool gray)
        {
            if (image)
            {
                if (gray)
                {
                    if (image != null && image.sprite != null)
                    {
                        bool isSuccess = UIAtlasManager.GetInstance().SetAtlasMaterial(image, image.sprite.name + ".png", true);
                        if (!isSuccess)
                        {
                            image.material = GrayMaterial;
                        }
                    }
                }
                else
                {
                    if (image != null && image.sprite != null)
                    {
                        bool isSuccess = UIAtlasManager.GetInstance().SetAtlasMaterial(image, image.sprite.name + ".png");
                        if (!isSuccess)
                        {
                            image.material = null;
                        }
                    }
                }
            }
        }
        GameImage m_imgRedPoint = null;
        public virtual void SetRedPoint(bool show, string redName = "ImgWarn")
        {
            if (m_imgRedPoint == null) m_imgRedPoint = Make<GameImage>(redName);
            if (m_imgRedPoint == null) return;
            m_imgRedPoint.Visible = show;
        }

        public void SetGrayAndUnClick(bool flag)
        {
            ShowEnable = flag;
            button.interactable = flag;
        }
    }

}
