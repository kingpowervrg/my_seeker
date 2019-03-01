using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_FB_Loading)]
    public class FBLoadingUILogic : UILogicBase
    {
        private GameProgressBar m_Slider;
        private GameUIEffect m_Effect;
        private float max_time = 5.0f;
        private float cur_time = 0.0f;
        private bool is_show = false;
        float width;
        protected override void OnInit()
        {
            base.OnInit();
            this.AutoDestroy = false;
            this.m_Slider = Make<GameProgressBar>("Slider");
            this.m_Effect = Make<GameUIEffect>("UI_jindutiao");
            this.NeedUpdateByFrame = true;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            if (param != null)
            {
                LoadingData loadingData = (LoadingData)param;
                Reset(loadingData);
                GameEvents.UIEvents.UI_Loading_Event.OnStartLoading.SafeInvoke();
            }

            cur_time = 0.0f;
            is_show = true;
            this.width = this.m_Slider.Widget.sizeDelta.x;

        }

        public override void OnHide()
        {
            base.OnHide();

            this.m_Effect.Visible = false;
        }

        private void Reset(LoadingData loadingData)
        {
            this.m_Slider.Value = 0f;
            this.m_Effect.Widget.anchoredPosition = Vector2.zero; ;
            this.m_Effect.EffectPrefabName = "UI_jindutiao.prefab";
            this.m_Effect.Visible = true;
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void Update()
        {
            base.Update();

            if (false == is_show)
                return;

            cur_time += Time.deltaTime;

            if (cur_time > max_time)
            {
                is_show = false;
                this.CloseFrame();

                return;
            }

            float f = cur_time / max_time;

            this.m_Slider.Value = f;
            this.m_Effect.Widget.anchoredPosition = Vector2.right * width * f;
        }
    }
}
