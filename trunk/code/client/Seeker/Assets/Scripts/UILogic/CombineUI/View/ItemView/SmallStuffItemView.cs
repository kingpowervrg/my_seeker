using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class SmallStuffItemView : GameUIComponent
    {
        readonly Vector2 R_TIPS_OFFSET = new Vector2(0.0f, -30.0f);
        readonly Color R_ICON_GRAY_COLOR = new Color(129.0f / 255, 129.0f / 255, 129.0f / 255, 1.0f);
        GameImage m_icon;
        GameLabel m_num;

        long m_item_id;
        int m_item_num;

        protected override void OnInit()
        {
            base.OnInit();
            m_icon = Make<GameImage>("Image");
            m_num = Make<GameLabel>("Text");


        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_icon.AddPressDownCallBack(PressDown);
            m_icon.AddPressUpCallBack(PressUp);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_icon.RemovePressDownCallBack(PressDown);
            m_icon.RemovePressUpCallBack(PressUp);
        }


        public void Refresh(long id_, string icon_name_, int cur_num_, int need_num_)
        {
            m_item_id = id_;
            m_item_num = cur_num_;

            m_icon.Sprite = icon_name_;
            m_num.Text = $"{cur_num_}/{need_num_}";
            if (cur_num_ >= need_num_)
            {
                m_icon.Color = Color.white;

            }
            else
            {
                m_icon.Color = R_ICON_GRAY_COLOR;
            }
        }

        private void PressDown(GameObject go)
        {
            if (0 == m_item_id)
                return;

            CommonHelper.ShowPropTips(m_item_id, m_item_num, this.Widget.position, R_TIPS_OFFSET);
        }

        private void PressUp(GameObject go)
        {
            if (0 == m_item_id)
                return;

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TOOL_TIPS);
        }
    }
}
