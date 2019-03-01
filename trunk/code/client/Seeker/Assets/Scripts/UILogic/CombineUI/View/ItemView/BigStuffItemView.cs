using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class BigStuffItemView : GameUIComponent
    {
        readonly Vector2 R_TIPS_OFFSET = new Vector2(0.0f, -50.0f);
        readonly Color R_ICON_GRAY_COLOR = new Color(129.0f / 255, 129.0f / 255, 129.0f / 255, 1.0f);
        readonly Color R_BG_GRAY_COLOR = new Color(129.0f / 255, 129.0f / 255, 129.0f / 255, 0.5f);
        readonly Color R_BG_WHITE_COLOR = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GameImage m_bg;
        GameImage m_icon;

        long m_item_id;
        int m_num;
        protected override void OnInit()
        {
            base.OnInit();
            m_bg = Make<GameImage>("Image_BG");
            m_icon = m_bg.Make<GameImage>("Image");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_bg.AddPressDownCallBack(PressDown);
            m_bg.AddPressUpCallBack(PressUp);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_bg.RemovePressDownCallBack(PressDown);
            m_bg.RemovePressUpCallBack(PressUp);
        }

        public void Refresh(long id_, int num_, string icon_name_, bool exist_)
        {
            m_item_id = id_;
            m_num = num_;

            m_icon.Sprite = icon_name_;

            if (exist_)
            {
                m_bg.Color = R_BG_WHITE_COLOR;
                m_icon.Color = Color.white;
            }
            else
            {
                m_bg.Color = R_BG_GRAY_COLOR;
                m_icon.Color = R_ICON_GRAY_COLOR;
            }
        }

        private void PressDown(GameObject go)
        {
            if (0 == m_item_id)
                return;

            CommonHelper.ShowPropTips(m_item_id, m_num, this.Widget.position, R_TIPS_OFFSET);
        }

        private void PressUp(GameObject go)
        {
            if (0 == m_item_id)
                return;

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TOOL_TIPS);
        }
    }
}
