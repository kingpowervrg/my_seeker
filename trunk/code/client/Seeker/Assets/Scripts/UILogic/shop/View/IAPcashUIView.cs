using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using UnityEngine.UI;

namespace SeekerGame
{

    class IAPcashUIView : BaseViewComponet<IAPcashUILogic>
    {
        private GameLabel m_title_txt;
        private GameUIContainer m_grid;

        protected override void OnInit()
        {
            base.OnInit();

            this.SetCloseBtnID("Panel:Button_close");
            m_title_txt = Make<GameLabel>("Panel:Image:Text");
            m_title_txt.Text = LocalizeModule.Instance.GetString("propname_22");
            m_grid = Make<GameUIContainer>("Panel:Panel_prop:grid");


            List<ConfCharge> goods = IAPhelper.GetGoods();
            m_grid.EnsureSize<IAPcashItemUIView>(goods.Count);

            for (int i = 0; i < m_grid.ChildCount && i < goods.Count; ++i)
            {
                m_grid.GetChild<IAPcashItemUIView>(i).Refresh(goods[i],i);
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            for (int i = 0; i < m_grid.ChildCount; ++i)
            {
                m_grid.GetChild<IAPcashItemUIView>(i).Visible = true;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }


    }
}
