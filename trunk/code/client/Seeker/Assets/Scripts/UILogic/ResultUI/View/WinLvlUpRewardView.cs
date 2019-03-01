//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class WinLvlUpRewardView : GameUIComponent
    {
        private GameLabel m_scene_name;
        private GameLabel m_scene_desc;
        private GameUIComponent m_output_root;
        private GameUIContainer m_output_grid;

        private Dictionary<EUNM_BASE_REWARD, DropItemIcon> m_output_item_dict;


        protected override void OnInit()
        {
            base.OnInit();

            m_scene_name = Make<GameLabel>("Text");
            m_scene_desc = Make<GameLabel>("Text (1)");
            m_output_root = Make<GameUIComponent>("Scroll View (1)");
            m_output_grid = m_output_root.Make<GameUIContainer>("Viewport");

        }


        public override void OnShow(object param)
        {
            base.OnShow(param);

        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void Refresh(WinFailData data_)
        {
            RefreshOutPut(data_);
        }



        public void RefreshOutPut(WinFailData data_)
        {
            int total_exp = 0, total_vit = 0, total_coin = 0, total_cash = 0;

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == data_.m_mode)
            {
                SCSceneRewardResponse msg = data_.m_msg as SCSceneRewardResponse;
                //20   体力 
                //21   金币
                //22  钞票 
                //23  经验

                foreach (var item in msg.UpLevelRewards)
                {
                    if (20 == item.ItemId)
                    {
                        total_vit += item.Num;
                    }
                    else if (21 == item.ItemId)
                    {
                        total_coin += item.Num;
                    }
                    else if (22 == item.ItemId)
                    {
                        total_cash += item.Num;
                    }
                    else if (23 == item.ItemId)
                    {
                        total_exp += item.Num;
                    }
                }


                List<EUNM_BASE_REWARD> types = new List<EUNM_BASE_REWARD>();
                List<int> counts = new List<int>();
                m_output_item_dict = new Dictionary<EUNM_BASE_REWARD, DropItemIcon>();
                if (total_exp > 0)
                {
                    types.Add(EUNM_BASE_REWARD.E_EXP);
                    counts.Add(total_exp);

                }

                if (total_vit > 0)
                {
                    types.Add(EUNM_BASE_REWARD.E_VIT);
                    counts.Add(total_vit);

                }

                if (total_coin > 0)
                {
                    types.Add(EUNM_BASE_REWARD.E_COIN);
                    counts.Add(total_coin);

                }

                if (total_cash > 0)
                {
                    types.Add(EUNM_BASE_REWARD.E_CASH);
                    counts.Add(total_cash);

                }

                m_output_grid.EnsureSize<DropItemIcon>(types.Count);

                for (int i = 0; i < m_output_grid.ChildCount; ++i)
                {
                    var item = m_output_grid.GetChild<DropItemIcon>(i);

                    item.InitSprite(CommonHelper.GetOutputIconName(types[i]), counts[i]);
                    item.Visible = false;
                    item.Visible = true;

                    m_output_item_dict.Add(types[i], item);
                }

                TweenOutputNum(total_exp, total_vit, total_coin, total_cash);
            }
        }

        private void TweenOutputNum(int bonus_exp, int bonus_vit, int bonus_coin, int bonus_cash)
        {
            foreach (var kvp in m_output_item_dict)
            {
                int aim_num = 0;
                if (EUNM_BASE_REWARD.E_EXP == kvp.Key)
                {
                    aim_num = bonus_exp;
                }
                else if (EUNM_BASE_REWARD.E_VIT == kvp.Key)
                {
                    aim_num = bonus_vit;
                }
                else if (EUNM_BASE_REWARD.E_COIN == kvp.Key)
                {
                    aim_num = bonus_coin;
                }
                else if (EUNM_BASE_REWARD.E_CASH == kvp.Key)
                {
                    aim_num = bonus_cash;
                }

                kvp.Value.SetNum(aim_num, 2.0f);
            }
        }

    }





}