using DG.Tweening;
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


    public class CombineView : BaseViewComponet<CombineUILogic>
    {
        Combine3DView m_3D_view;
        GameTexture m_tex_view;
        GameImage m_icon_view;

        ToggleWithArrowTween[] m_left_toggles = new ToggleWithArrowTween[3];
        GameLoopUIContainer<ToggleCheckMarkView> m_top_toggle_grid;
        BigStuffItemView[] m_big_stuffs = new BigStuffItemView[6];
        SmallStuffItemView[] m_small_stuffs = new SmallStuffItemView[4];
        GameButton m_gift_btn;
        GameButton m_combine_btn;
        GameImage m_stored_img;

        GameLabel m_name_txt;
        GameLabel m_No_txt;
        GameLabel m_progress_txt;

        long m_cur_combine_id;
        ENUM_COMBINE_TYPE m_combine_type;
        int m_cur_mix_num;
        long m_fixed_prop_id;
        long m_fixed_combine_id;

        private UnityEngine.UI.GridLayoutGroup m_drop_layout;
        float m_drop_grid_ori_local_x;
        float m_drop_item_size_x, m_drop_item_space_x;
        protected override void OnInit()
        {
            base.OnInit();

            m_3D_view = Make<Combine3DView>("Panel:3D");
            m_tex_view = Make<GameTexture>("Panel:RawImage_Tex");
            m_icon_view = Make<GameImage>("Panel:Image_Icon");

            m_left_toggles[0] = Make<ToggleWithArrowTween>("leftBtn:btnTotal");
            m_left_toggles[0].Refresh((int)ENUM_COMBINE_TYPE.POLICE, ENUM_COMBINE_TYPE.POLICE.ToString(), false, LeftToggleChecked);
            m_left_toggles[1] = Make<ToggleWithArrowTween>("leftBtn:btnRecently");
            m_left_toggles[1].Refresh((int)ENUM_COMBINE_TYPE.COLLECTION, ENUM_COMBINE_TYPE.COLLECTION.ToString(), false, LeftToggleChecked);
            m_left_toggles[2] = Make<ToggleWithArrowTween>("leftBtn:btnEnergy");
            m_left_toggles[2].Refresh((int)ENUM_COMBINE_TYPE.OTHER, ENUM_COMBINE_TYPE.OTHER.ToString(), false, LeftToggleChecked);
            m_top_toggle_grid = Make<GameLoopUIContainer<ToggleCheckMarkView>>("Panel:ScrollView:Viewport");
            m_drop_grid_ori_local_x = m_top_toggle_grid.gameObject.transform.localPosition.x;
            m_drop_layout = m_top_toggle_grid.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            m_drop_item_size_x = m_drop_layout.cellSize.x;
            m_drop_item_space_x = m_drop_layout.spacing.x;
            m_big_stuffs[0] = Make<BigStuffItemView>("Panel:Panel:Big0");
            m_big_stuffs[1] = Make<BigStuffItemView>("Panel:Panel:Big1");
            m_big_stuffs[2] = Make<BigStuffItemView>("Panel:Panel:Big2");
            m_big_stuffs[3] = Make<BigStuffItemView>("Panel:Panel:Big3");
            m_big_stuffs[4] = Make<BigStuffItemView>("Panel:Panel:Big4");
            m_big_stuffs[5] = Make<BigStuffItemView>("Panel:Panel:Big5");

            m_small_stuffs[0] = Make<SmallStuffItemView>("Panel:Panel:Image (6)");
            m_small_stuffs[1] = Make<SmallStuffItemView>("Panel:Panel:Image (7)");
            m_small_stuffs[2] = Make<SmallStuffItemView>("Panel:Panel:Image (8)");
            m_small_stuffs[3] = Make<SmallStuffItemView>("Panel:Panel:Image (9)");

            m_gift_btn = Make<GameButton>("Panel:Image:Button");
            m_combine_btn = Make<GameButton>("Panel:Panel:Button");
            m_stored_img = Make<GameImage>("Panel:Panel:Image_colected");
            m_name_txt = Make<GameLabel>("Panel:Text");
            m_No_txt = Make<GameLabel>("Panel:Text (1)");
            m_progress_txt = Make<GameLabel>("Panel:Text (2)");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                //合成任务指定的合成
                m_fixed_prop_id = (long)param;

            }
            else
                m_fixed_prop_id = CurViewLogic().Fixed_prop_id;

            m_gift_btn.AddClickCallBack(OnGiftClicked);
            m_combine_btn.AddClickCallBack(OnCombineClicked);

            SwitchOutputView(ENUM_COMBINE_TYPE.ALL);

            if (0 != m_fixed_prop_id)
            {
                RefreshLeftToggleByFixedID();
            }
            else
            {
                if (m_left_toggles[0].Checked)
                {
                    m_left_toggles[0].Checked = false;
                }

                m_left_toggles[0].Checked = true;
            }
        }

        public override void OnHide()
        {
            base.OnHide();

            m_gift_btn.RemoveClickCallBack(OnGiftClicked);
            m_combine_btn.RemoveClickCallBack(OnCombineClicked);

            m_left_toggles[0].Checked = false;
        }

        public void RefreshCombinedPropCount(long combine_id_)
        {
            if (m_cur_combine_id == combine_id_)
            {
                var combine = ConfCombineFormula.Get(combine_id_);
                RefreshCombineProp(combine);
            }
        }

        void RefreshLeftToggleByFixedID()
        {
            ConfProp prop = ConfProp.Get(m_fixed_prop_id);

            if ((int)PROP_TYPE.E_OFFICER == prop.type)
            {
                if (m_left_toggles[(int)ENUM_COMBINE_TYPE.POLICE].Checked)
                {
                    m_left_toggles[(int)ENUM_COMBINE_TYPE.POLICE].Checked = false;
                }

                m_left_toggles[(int)ENUM_COMBINE_TYPE.POLICE].Checked = true;
            }
            else if ((int)PROP_TYPE.E_EXHABIT == prop.type)
            {
                if (m_left_toggles[(int)ENUM_COMBINE_TYPE.COLLECTION].Checked)
                {
                    m_left_toggles[(int)ENUM_COMBINE_TYPE.COLLECTION].Checked = false;
                }

                m_left_toggles[(int)ENUM_COMBINE_TYPE.COLLECTION].Checked = true;
            }

        }

        void RefreshTopToggleByFixedID()
        {
            List<ConfCombineFormula> combine_datas = ConfCombineFormula.array.Where((e) => e.outputId == m_fixed_prop_id).ToList();
            m_fixed_combine_id = 0;
            if (combine_datas.Count > 0)
            {
                m_fixed_combine_id = combine_datas.First().id;
            }
        }

        void OnGiftClicked(GameObject obj)
        {
            //ConfCombineFormula.Get(m_cur_combine_id);
            CurViewLogic().ShowGift(m_cur_combine_id);
        }

        void OnCombineClicked(GameObject obj)
        {
            CurViewLogic().ReqCombine(m_cur_combine_id);
        }

        void LeftToggleChecked(bool v_, int i_)
        {
            if (v_)
            {
                m_combine_type = (ENUM_COMBINE_TYPE)i_;

                SwitchOutputView(m_combine_type);
                List<ConfCombineFormula> combine_datas = ConfCombineFormula.array.Where((e) => e.type == i_).OrderBy(e => e.id).ToList();

                m_top_toggle_grid.EnsureSize<ToggleCheckMarkView>(combine_datas.Count());

                if (0 == combine_datas.Count())
                {
                    NoneItems();
                    return;
                }

                if (0 != m_fixed_prop_id)
                {
                    RefreshTopToggleByFixedID();
                    //m_fixed_prop_id = 0;
                }

                if (0 != m_fixed_combine_id)
                {
                    int move_count = 0;

                    for (int i = 0; i < m_top_toggle_grid.ChildCount; ++i)
                    {
                        ConfCombineFormula data = combine_datas[i];
                        ToggleCheckMarkView element = m_top_toggle_grid.GetChild<ToggleCheckMarkView>(i);
                        element.Visible = false;
                        element.Visible = true;
                        string icon_name = GetOutputIcon(data.id);

                        if (data.id == m_fixed_combine_id)
                            move_count = i;

                        element.Refresh(data.id, null != icon_name ? icon_name : "BaoLuoGongZuoShi_01_bangqiushoutao_01.png", data.id == m_fixed_combine_id, TopToggleChecked);
                    }


                    //m_drop_scroll.scrollView.horizontal = false;

                    float from_x = m_drop_grid_ori_local_x - (m_drop_item_size_x + m_drop_item_space_x) * move_count;
                    m_top_toggle_grid.gameObject.transform.localPosition = new Vector3(from_x, m_top_toggle_grid.gameObject.transform.localPosition.y, m_top_toggle_grid.gameObject.transform.localPosition.z);

                    //m_fixed_combine_id = 0;
                }
                else
                {
                    for (int i = 0; i < m_top_toggle_grid.ChildCount; ++i)
                    {
                        ConfCombineFormula data = combine_datas[i];
                        ToggleCheckMarkView element = m_top_toggle_grid.GetChild<ToggleCheckMarkView>(i);
                        element.Visible = false;
                        element.Visible = true;
                        string icon_name = GetOutputIcon(data.id);

                        element.Refresh(data.id, null != icon_name ? icon_name : "BaoLuoGongZuoShi_01_bangqiushoutao_01.png", i == 0, TopToggleChecked);
                    }
                }
            }
        }

        void TopToggleChecked(bool v_, long i_)
        {
            if (v_)
            {

                m_cur_combine_id = i_;
                ShowOutputContent(m_cur_combine_id);
                ConfCombineFormula combine = ConfCombineFormula.Get(m_cur_combine_id);
                m_name_txt.Text = LocalizeModule.Instance.GetString(combine.name);
                m_No_txt.Text = $"NO.{combine.id}";

                RefreshCombineProp(combine);


            }
        }

        void NoneItems()
        {
            m_3D_view.Visible = false;
            m_tex_view.Visible = false;
            m_icon_view.Visible = false;

            for (int i = 0; i < m_big_stuffs.Length; ++i)
            {
                m_big_stuffs[i].Visible = false;
            }

            for (int i = 0; i < m_small_stuffs.Length; ++i)
            {
                m_small_stuffs[i].Visible = false;
            }

            m_gift_btn.Visible = false;
            m_combine_btn.Visible = false;
            m_stored_img.Visible = false;
        }

        string GetOutputIcon(long combine_id_)
        {
            ConfCombineFormula data = ConfCombineFormula.Get(combine_id_);
            long out_put_id = data.outputId;

            if (0 == out_put_id)
                return null;

            switch ((ENUM_COMBINE_TYPE)data.type)
            {
                case ENUM_COMBINE_TYPE.POLICE:
                    {
                        return ConfOfficer.Get(ConfProp.Get(out_put_id).officerId).icon;
                    }

                case ENUM_COMBINE_TYPE.COLLECTION:
                case ENUM_COMBINE_TYPE.OTHER:
                    {
                        return ConfProp.Get(out_put_id).icon;
                    }

                default:
                    break;
            }

            return null;
        }

        ConfCombineFormula GetCombineData(int idx_)
        {
            List<ConfCombineFormula> combine_datas = ConfCombineFormula.array.Where((e) => e.type == (int)m_combine_type).ToList();

            if (0 <= idx_ && idx_ < combine_datas.Count)
            {
                return combine_datas[idx_];
            }

            return null;
        }

        void RefreshCombineProp(ConfCombineFormula combine)
        {
            m_cur_mix_num = CombineDataManager.Instance.GetCurCount(m_cur_combine_id);
            m_progress_txt.Text = 0 == combine.mixLimit ? "" : $"{m_cur_mix_num}/{combine.mixLimit}";

            int sum = 0;

            sum += RefreshBigStuffs(m_cur_combine_id) ? 1 : 0;
            sum += RefreshSmallStuffs(m_cur_combine_id) ? 1 : 0;

            m_stored_img.Visible = false;
            m_combine_btn.Visible = true;
            m_gift_btn.Visible = true;
            if (sum >= 2 && (0 == combine.mixLimit || m_cur_mix_num < combine.mixLimit))
            {
                m_combine_btn.Enable = true;
                m_combine_btn.SetGray(false);
            }
            else
            {
                if ((int)ENUM_COMBINE_TYPE.COLLECTION == combine.type)
                {
                    if (sum < 2)
                    {
                        m_combine_btn.Enable = false;
                        m_combine_btn.SetGray(true);
                    }
                    else
                    {
                        m_stored_img.Visible = true;
                        m_combine_btn.Visible = false;
                    }
                }
                else
                {
                    m_combine_btn.Enable = false;
                    m_combine_btn.SetGray(true);
                }
            }
        }

        bool RefreshBigStuffs(long id_)
        {
            ConfCombineFormula data = ConfCombineFormula.Get(id_);
            long[] big_props = new long[] { data.propId1, data.propId2, data.propId3, data.propId4, data.propId5, data.propId6 };

            int sum = 0;
            PlayerPropMsg prop_info;
            bool have;
            int num;
            for (int i = 0; i < big_props.Length; ++i)
            {
                if (0 == big_props[i])
                {
                    m_big_stuffs[i].Visible = false;
                    sum += 1;
                    continue;
                }

                prop_info = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(big_props[i]);
                have = null != prop_info ? true : false;
                sum += have ? 1 : 0;
                num = have ? prop_info.Count : 0;

                m_big_stuffs[i].Refresh(big_props[i], num, ConfProp.Get(big_props[i]).icon, have);
                m_big_stuffs[i].Visible = true;
            }

            return sum >= 6;
        }

        bool RefreshSmallStuffs(long id_)
        {

            ConfCombineFormula data = ConfCombineFormula.Get(id_);
            long[] special_props = new long[] { data.specialPropId1, data.specialPropId2, data.specialPropId3, data.specialPropId4 };
            int[] special_needs = new int[] { data.special1Count, data.special2Count, data.special3Count, data.special4Count };

            int sum = 0;
            int? cur_num;
            int need_num;

            for (int i = 0; i < special_props.Length; ++i)
            {
                if (0 == special_props[i])
                {
                    m_small_stuffs[i].Visible = false;
                    sum += 1;
                    continue;
                }

                cur_num = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(special_props[i])?.Count;
                need_num = special_needs[i];
                sum += cur_num.GetValueOrDefault() >= need_num ? 1 : 0;
                m_small_stuffs[i].Refresh(special_props[i], ConfProp.Get(special_props[i]).icon, cur_num.GetValueOrDefault(), need_num);
                m_small_stuffs[i].Visible = true;
            }
            return sum >= 4;
        }

        void SwitchOutputView(ENUM_COMBINE_TYPE combine_type_)
        {
            m_3D_view.Visible = false;
            m_tex_view.Visible = false;
            m_icon_view.Visible = false;

            switch (combine_type_)
            {
                case ENUM_COMBINE_TYPE.POLICE:
                    m_tex_view.Visible = true;
                    break;
                case ENUM_COMBINE_TYPE.COLLECTION:
                    m_3D_view.Visible = true;
                    break;
                case ENUM_COMBINE_TYPE.OTHER:
                    m_icon_view.Visible = true;
                    break;
                default:
                    break;
            }


        }

        void ShowOutputContent(long combine_id_)
        {
            long out_put_id = ConfCombineFormula.Get(combine_id_).outputId;

            if (0 == out_put_id)
                return;

            switch (m_combine_type)
            {
                case ENUM_COMBINE_TYPE.POLICE:
                    {
                        m_tex_view.TextureName = ConfOfficer.Get(ConfProp.Get(out_put_id).officerId).hollowPortrait;
                    }
                    break;
                case ENUM_COMBINE_TYPE.COLLECTION:
                    {
                        m_3D_view.Refresh(ConfProp.Get(out_put_id).exhibit);
                        //m_3D_view.Refresh(Confexhibit.Get(out_put_id).assetName);

                        //int ret = UnityEngine.Random.Range(1, 3);
                        //m_3D_view.Refresh(0 == ret % 2 ? "FaYuanWai_01_MoTuoChe_01.prefab" : "G_BaoLuoGongZuoShi_01_gangtiexia_01.prefab");
                    }
                    break;
                case ENUM_COMBINE_TYPE.OTHER:
                    {
                        m_icon_view.Sprite = ConfProp.Get(out_put_id).icon;
                    }
                    break;
                default:
                    break;
            }
        }




    }
}
