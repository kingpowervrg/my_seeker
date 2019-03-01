using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class GroupToolTipsView : BaseViewComponet<GroupToolTipsUILogic>
    {
        public const float C_WIDTH = 280.0f;
        private const float C_GRID_ADD_H = 30.0f;
        private const float C_GRID_ITEM_H = 65.5f;

        GameImage m_bg;
        private GameUIContainer m_item_grid;

        protected override void OnInit()
        {
            base.OnInit();
            m_bg = Make<GameImage>("Tips_BG:BG");
            m_item_grid = Make<GameUIContainer>("Tips_BG:grid");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh(List<GroupToolTipsData> datas)
        {
            m_item_grid.EnsureSize<IconNameNumItemView>(datas.Count);

            for (int i = 0; i < datas.Count; ++i)
            {
                m_item_grid.GetChild<IconNameNumItemView>(i).Refresh(datas[i].ItemID, datas[i].CurCount);
                m_item_grid.GetChild<IconNameNumItemView>(i).Visible = true;
            }

            m_bg.Widget.sizeDelta = new Vector2(m_bg.Widget.sizeDelta.x, C_GRID_ADD_H + datas.Count * C_GRID_ITEM_H);
        }
    }

}
