using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_Loading)]
    public class LoadingUILogic : UILogicBase
    {
        private GameProgressBar m_Slider;
        private GameUIEffect m_Effect;
        private LoadingSystem m_LoadingSystem;
        private float width = 600f;
        private GameTexture m_loadingTexture = null;
        private string[] tipKeys = null;
        public const int MAXTIPS = 27;
        private int m_tipIndex = -1;
        private GameLabel m_tipLab = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.AutoDestroy = false;
            this.m_Slider = Make<GameProgressBar>("Animation:Slider");
            this.m_Effect = Make<GameUIEffect>("Animation:Slider:UI_jindutiao");
            this.m_loadingTexture = Make<GameTexture>("Animation:RawImage");
            this.NeedUpdateByFrame = true;
            this.m_tipLab = Make<GameLabel>("Animation:Text");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_loadingTexture.TextureName = $"image_loading_{ Random.Range(1, 6)}.png";
            tipKeys = new string[MAXTIPS];
            for (int i = 0; i < MAXTIPS; i++)
            {
                if (i < 9)
                {
                    tipKeys[i] = string.Format("tips_00{0}", (i + 1));
                }
                else
                {
                    tipKeys[i] = string.Format("tips_0{0}", (i + 1));
                }

            }
            if (param != null)
            {
                LoadingData loadingData = (LoadingData)param;
                Reset(loadingData);
                if (loadingData.isBigWorld)
                {
                    GameEvents.UIEvents.UI_Loading_Event.OnStartLoading.SafeInvoke();
                }
            }
            this.m_tipLab.Text = GetRandomTip();
            this.width = this.m_Slider.Widget.sizeDelta.x;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_LoadingSystem.OnHide();
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver.SafeInvoke();
            NewGuid.GuidNewManager.Instance.OnReflashGuidStatus();
            this.m_Effect.Visible = false;
        }

        private void Reset(LoadingData loadingData)
        {
            m_LoadingSystem = new LoadingSystem(loadingData);
            this.m_Slider.Value = 0f;
            this.m_Effect.Widget.anchoredPosition = Vector2.zero; ;
            this.m_Effect.EffectPrefabName = "UI_jindutiao.prefab";
            this.m_Effect.Visible = true;
            this.m_tipIndex = -1;
            this.timesection = 0f;
        }

        public override void Update()
        {
            base.Update();
            timesection += Time.deltaTime;
            if (timesection >= 20f)
            {
                timesection = 0f;
                this.m_tipLab.Text = GetRandomTip();
            }
            m_LoadingSystem.UpdateLoading();
            this.m_Slider.Value = m_LoadingSystem.TimeSection;
            this.m_Effect.Widget.anchoredPosition = Vector2.right * width * m_LoadingSystem.TimeSection;
        }

        private float timesection = 0f;
        private string GetRandomTip()
        {
            int index = Random.Range(0, MAXTIPS);
            if (m_tipIndex == index)
            {
                m_tipIndex = (index + 1) % tipKeys.Length;
            }
            else
            {
                m_tipIndex = index;
            }
            return LocalizeModule.Instance.GetString(tipKeys[m_tipIndex]);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
    }
}
