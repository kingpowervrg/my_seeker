using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    public class CombineGiftItemView : GameUIComponent
    {
        GameImage m_icon;
        GameLabel m_num;
        protected override void OnInit()
        {
            base.OnInit();

            m_icon = Make<GameImage>("Image");
            m_num = Make<GameLabel>("Text_Desc");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh( string icon_name_, int num_)
        {
            m_icon.Sprite = icon_name_;
            m_num.Text = num_.ToString();
        }
    }
}

