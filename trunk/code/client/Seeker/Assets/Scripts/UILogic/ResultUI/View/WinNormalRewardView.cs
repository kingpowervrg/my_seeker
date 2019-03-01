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
    public class WinNormalRewardView : GameUIComponent
    {

        private GameUIComponent m_output_scroll;
        private GameUIContainer m_output_grid;
        private GameScrollView m_drop_scroll;
        private GameUIContainer m_drop_grid;
        private UnityEngine.UI.GridLayoutGroup m_drop_layout;

        private Dictionary<EUNM_BASE_REWARD, DropItemIconShake> m_output_item_dict;

        float m_drop_item_size_x, m_drop_item_space_x;
        float m_drop_grid_ori_local_x;
        int m_move_index, m_move_count;
        float m_from_x, m_cur_x;


        protected override void OnInit()
        {
            base.OnInit();

            m_output_scroll = Make<GameUIComponent>("Scroll View (1)");
            m_output_grid = m_output_scroll.Make<GameUIContainer>("Viewport");
            m_drop_scroll = Make<GameScrollView>("Scroll View");
            m_drop_grid = m_drop_scroll.Make<GameUIContainer>("Viewport");
            m_drop_grid_ori_local_x = m_drop_grid.gameObject.transform.localPosition.x;
            m_drop_layout = m_drop_grid.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            m_drop_item_size_x = m_drop_layout.cellSize.x;
            m_drop_item_space_x = m_drop_layout.spacing.x;
        }


        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_drop_grid.Widget.DOKill();
            TimeModule.Instance.RemoveTimeaction(DropItemMove);
        }

        public void Refresh(WinFailData data_)
        {
            RefreshOutPut(data_);
            RefreshDrop(data_);
        }



        public void RefreshOutPut(WinFailData data_)
        {
            int total_exp = 0, total_vit = 0, total_coin = 0, total_cash = 0;

            if (ENUM_SEARCH_MODE.E_JIGSAW == data_.m_mode)
            {
                var msg = data_.m_msg as SCFinishResponse;
                int cash = 0, coin = 0, exp = 0, vit = 0;

                foreach (var item in msg.Rewards)
                {
                    switch (item.Type)
                    {
                        case (int)EUNM_BASE_REWARD.E_CASH:
                            cash = item.Num;
                            break;
                        case (int)EUNM_BASE_REWARD.E_COIN:
                            coin = item.Num;
                            break;
                        case (int)EUNM_BASE_REWARD.E_EXP:
                            exp = item.Num;
                            break;
                        case (int)EUNM_BASE_REWARD.E_VIT:
                            vit = item.Num;
                            break;
                    }
                }
                total_exp = exp; total_vit = vit; total_coin = coin; total_cash = cash;

            }
            else if (ENUM_SEARCH_MODE.E_CARTOON == data_.m_mode)
            {
                var msg = data_.m_msg as SCCartoonRewardReqsponse;

                total_exp = msg.Exp; total_vit = msg.Vit; total_coin = msg.Coin; total_cash = msg.Cash;
            }
            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == data_.m_mode)
            {
                SCSceneRewardResponse msg = data_.m_msg as SCSceneRewardResponse;

                total_exp = msg.OutputExp; total_vit = msg.OutputVit; total_coin = msg.OutputCoin; total_cash = msg.OutputCash;
            }




            List<EUNM_BASE_REWARD> types = new List<EUNM_BASE_REWARD>();
            List<int> counts = new List<int>();
            m_output_item_dict = new Dictionary<EUNM_BASE_REWARD, DropItemIconShake>();
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

            m_output_grid.EnsureSize<DropItemIconShake>(types.Count);

            for (int i = 0; i < m_output_grid.ChildCount; ++i)
            {
                var item = m_output_grid.GetChild<DropItemIconShake>(i);

                item.InitSprite(CommonHelper.GetOutputIconName(types[i]), counts[i]);
                item.Visible = false;
                item.Visible = true;

                m_output_item_dict.Add(types[i], item);
            }

            TweenOutputNum(total_exp, total_vit, total_coin, total_cash);
        }



        public void RefreshDrop(WinFailData data_)
        {
            List<OutPutData> datas = new List<OutPutData>();

            if (ENUM_SEARCH_MODE.E_JIGSAW == data_.m_mode)
            {
                var msg = data_.m_msg as SCFinishResponse;

                ConfProp prop = ConfProp.Get(msg.PropId);




                if (null != prop)
                {
                    OutPutData data = new OutPutData()
                    {
                        Icon = ConfProp.Get(msg.PropId).icon,
                        Count = 0,
                        ItemID = msg.PropId,
                    };


                    datas.Add(data);
                }


            }

            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == data_.m_mode)
            {

                SCSceneRewardResponse msg = data_.m_msg as SCSceneRewardResponse;

                foreach (var item in msg.GiftItems)
                {
                    OutPutData data = new OutPutData()
                    {
                        Icon = ConfProp.Get(item.ItemId).icon,
                        Count = item.Num,
                        ItemID = item.ItemId,
                    };

                    datas.Add(data);
                }

                //for (int i = 1; i < 4; ++i)
                //{
                //    OutPutData data = new OutPutData()
                //    {
                //        Icon = ConfProp.Get((long)i).icon,
                //        Count = 1,
                //        ItemID = (long)i,
                //    };
                //    datas.Add(data);
                //}
            }



            m_drop_grid.EnsureSize<DropItemIcon>(datas.Count);

            for (int i = 0; i < m_drop_grid.ChildCount; ++i)
            {
                var item = m_drop_grid.GetChild<DropItemIcon>(i);

                item.InitSprite(datas[i].Icon, datas[i].Count, datas[i].ItemID);
                item.gameObject.transform.localScale = Vector3.zero;
                item.Visible = true;
            }

            if (datas.Count > 0)

            {
                m_move_index = 0;
                m_move_count = datas.Count - 1;
                m_drop_scroll.scrollView.horizontal = false;

                m_cur_x = m_from_x = m_drop_grid_ori_local_x - (m_drop_item_size_x + m_drop_item_space_x) * m_move_count;
                m_drop_grid.gameObject.transform.localPosition = new Vector3(m_from_x, m_drop_grid.gameObject.transform.localPosition.y, m_drop_grid.gameObject.transform.localPosition.z);
                DropItemMoveFinished();
                //DropItemMove();
            }

        }

        void DropItemMove()
        {
            if (m_move_index >= m_move_count)
            {
                m_drop_scroll.scrollView.horizontal = true;
                return;
            }

            ++m_move_index;
            float to_x = m_from_x + (m_drop_item_size_x + m_drop_item_space_x) * m_move_index;
            var tween = m_drop_grid.Widget.DOLocalMoveX(to_x, 2.0f).OnComplete(DropItemMoveFinished);

            // DOTween.To(() => { return m_cur_x; }, (val) => { m_cur_x = val; m_drop_grid.Widget.offsetMin = new Vector2(m_cur_x, m_drop_grid.Widget.offsetMin.y); }, to_x, 1.0f).OnComplete(DropItemMoveFinished);

        }

        void DropItemMoveFinished()
        {
            int cur_drop_item_idx = m_move_count - m_move_index;
            ShowDropItem(cur_drop_item_idx);
            //TimeModule.Instance.SetTimeout(DropItemMove, 0.7f);
        }

        void ShowDropItem(int idx_)
        {
            if (0 <= idx_ && idx_ < m_drop_grid.ChildCount)
            {
                var child = m_drop_grid.GetChild<DropItemIcon>(idx_);
                child.gameObject.transform.localScale = Vector3.zero;
                child.Widget.DOScale(Vector3.one, 1f).OnComplete(DropItemMove);
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