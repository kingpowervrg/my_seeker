using EngineCore;
using Google.Protobuf;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_TOOL_TIPS)]
    public class ToolTipsUILogic : BaseViewComponetLogic
    {
        GameUIComponent m_root;
        ToolTipsView m_view;

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);
        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.sencond_pop.ToString());

            if (null != param)
            {
                ToolTipsData data = param as ToolTipsData;

                m_view.Refresh(data.ItemID, data.CurCount, data.MaxCount);

                Vector2 tip_pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_root.Widget, data.ScreenPos, CameraManager.Instance.UICamera, out tip_pos);

                m_view.Widget.anchoredPosition = tip_pos;
                m_view.Visible = true;
            }
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            m_root = this.Make<GameUIComponent>(root.name);
            m_view = this.Make<ToolTipsView>("Tips_BG");
        }
    }
}
