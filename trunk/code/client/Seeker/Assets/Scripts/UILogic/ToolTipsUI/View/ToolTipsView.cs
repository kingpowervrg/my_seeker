﻿using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    class ToolTipsView : BaseViewComponet<LevelUpUILogic>
    {
        public const float C_WIDTH = 280.0f;
        private const float C_BG_EXCEPT_GRID_H = 145.0f;
        private const float C_ADDRESS_ITEM_H = 25.0f;

        private GameImage m_bg;
        private GameLabel m_tool_name;
        private GameImage m_icon;
        private GameLabel m_num;
        private GameLabel m_descs;
        private GameUIContainer m_address_grid;

        protected override void OnInit()
        {
            base.OnInit();
            m_bg = Make<GameImage>("BG");
            m_tool_name = Make<GameLabel>("Text_name");
            m_icon = Make<GameImage>("Image_icon:Icon");
            m_num = Make<GameLabel>("Text_number");
            m_descs = Make<GameLabel>("Text_detail");
            m_address_grid = Make<GameUIContainer>("grid");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh(long item_id_, int cur_count_, int max_count_)
        {
            ConfProp tool = ConfProp.Get(item_id_);

            m_tool_name.Text = LocalizeModule.Instance.GetString(tool.name);
            m_icon.Sprite = tool.icon;

            if (0 != max_count_)
                m_num.Text = string.Format("{0} / {1}", cur_count_, max_count_);
            else if (0 != cur_count_)
                m_num.Text = cur_count_.ToString();
            else
                m_num.Text = "";


            m_num.color = cur_count_ < max_count_ ? Color.red : Color.white;
            m_descs.Text = LocalizeModule.Instance.GetString(tool.description);

            List<string> adr_list = new List<string>();
            string[] adr_array = tool.address;

            foreach (var adr in adr_array)
            {
                if (string.IsNullOrEmpty(adr))
                    continue;

                adr_list.Add(adr);
            }

            m_address_grid.EnsureSize<AddressItemView>(adr_list.Count);

            for (int i = 0; i < adr_list.Count; ++i)
            {
                m_address_grid.GetChild<AddressItemView>(i).Refresh(LocalizeModule.Instance.GetString(adr_list[i]));
            }

            m_bg.Widget.sizeDelta = new Vector2(m_bg.Widget.sizeDelta.x, C_BG_EXCEPT_GRID_H + adr_list.Count * C_ADDRESS_ITEM_H);
        }
    }

}