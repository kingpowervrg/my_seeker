using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{
    class AddressItemView : GameUIComponent
    {
        private GameLabel m_address_txt;
        protected override void OnInit()
        {
            base.OnInit();
            m_address_txt = Make<GameLabel>("Text");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh(string address_)
        {
            m_address_txt.Text = address_;
        }
    
    
    }
}
