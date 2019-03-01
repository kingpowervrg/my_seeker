using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_GROUP_TOOL_TIPS)]
    public class GroupToolTipsUILogic : BaseViewComponetLogic
    {
        GameUIComponent m_root;
        GroupToolTipsView m_view;

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
                GroupToolTipsDatas data = param as GroupToolTipsDatas;

                m_view.Refresh(data.Datas);

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
            m_view = this.Make<GroupToolTipsView>("Tips_BG");
        }
    }
}
