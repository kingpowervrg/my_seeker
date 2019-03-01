#if OFFICER_SYS
using EngineCore;
using GOEngine;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    public class PoliceUpgradeUI : GameUIComponent
    {
        private enum ENUM_BTN_TYPE
        {
            E_ALL_DISABLE,
            E_EMPLOY_DISABLE,
            E_EMPLOY_ENABLE,
            E_UPGRADE_DISABLE,
            E_UPGRADE_ENABLE,
        }
        private struct ToolData
        {
            public long m_tool_id;
            public int m_tool_count;
            public int m_tool_max_count;
        }

        private long m_officer_id;
        private long m_officer_server_id;

        private SafeAction m_onBack;

        private GameButton m_back_btn;

        private GameTexture m_body_tex;
        private GameImage m_level_icon;
        private GameLabel m_name_title;
        private List<IconGrayTransparent> m_tool_icons;

        private ToolTip m_tool_tip;

        private List<GameUIComponent> m_item_roots;
        private List<GameImage> m_items;
        private List<GameImage> m_item_progress;
        private List<GameLabel> m_items_nums;
        private List<GameLabel> m_items_max_nums;

        private GameButton m_upgrade_btn;
        private GameLabel m_upgrade_btn_label;

        private GameButton m_employ_btn;
        private GameLabel m_employ_btn_label;

        private List<GameUIEffect> m_upgrade_effects;



#region 
        List<ToolData> m_tool_datas;
        List<ToolData> m_item_datas;
#endregion


        public void RegisterBack(System.Action back_)
        {
            this.m_onBack = back_;
        }

        protected override void OnInit()
        {
            m_back_btn = this.Make<GameButton>("btnBack");

            m_body_tex = this.Make<GameTexture>("BG_Frame:Portrait");
            m_level_icon = this.Make<GameImage>("Image_Level:icon");
            m_name_title = this.Make<GameLabel>("Image_Level:title");

            m_tool_icons = new List<IconGrayTransparent>();
            m_tool_icons.Add(this.Make<IconGrayTransparent>("Panel_Tools:Image0"));
            m_tool_icons.Add(this.Make<IconGrayTransparent>("Panel_Tools:Image1"));
            m_tool_icons.Add(this.Make<IconGrayTransparent>("Panel_Tools:Image2"));
            m_tool_icons.Add(this.Make<IconGrayTransparent>("Panel_Tools:Image3"));
            m_tool_icons.Add(this.Make<IconGrayTransparent>("Panel_Tools:Image4"));

            for (int i = 0; i < m_tool_icons.Count; ++i)
            {
                m_tool_icons[i].gameObject.name = i.ToString();
                m_tool_icons[i].Disable_alpha = 0.9f;
            }

            m_tool_tip = this.Make<ToolTip>("Panel_Tools:Tips_BG");

            m_item_roots = new List<GameUIComponent>
            {
                this.Make<GameUIComponent>("Panel_Items:Image0"),
                this.Make<GameUIComponent>("Panel_Items:Image1:"),
                this.Make<GameUIComponent>("Panel_Items:Image2"),
                this.Make<GameUIComponent>("Panel_Items:Image3"),
            };

            m_items = new List<GameImage>
            {
                this.Make<GameImage>("Panel_Items:Image0:Icon"),
                this.Make<GameImage>("Panel_Items:Image1:Icon"),
                this.Make<GameImage>("Panel_Items:Image2:Icon"),
                this.Make<GameImage>("Panel_Items:Image3:Icon"),
            };

            m_items_nums = new List<GameLabel>
            {
                this.Make<GameLabel>("Panel_Items:Image0:Text_Num"),
                this.Make<GameLabel>("Panel_Items:Image1:Text_Num"),
                this.Make<GameLabel>("Panel_Items:Image2:Text_Num"),
                this.Make<GameLabel>("Panel_Items:Image3:Text_Num"),
            };

            m_items_max_nums = new List<GameLabel>
            {
                this.Make<GameLabel>("Panel_Items:Image0:Text_Max"),
                this.Make<GameLabel>("Panel_Items:Image1:Text_Max"),
                this.Make<GameLabel>("Panel_Items:Image2:Text_Max"),
                this.Make<GameLabel>("Panel_Items:Image3:Text_Max"),
            };


            m_item_progress = new List<GameImage>
            {
                this.Make<GameImage>("Panel_Items:Image0:Icon_progress"),
                this.Make<GameImage>("Panel_Items:Image1:Icon_progress"),
                this.Make<GameImage>("Panel_Items:Image2:Icon_progress"),
                this.Make<GameImage>("Panel_Items:Image3:Icon_progress"),
            };

            m_employ_btn = this.Make<GameButton>("btnEmploy");
            m_employ_btn_label = this.Make<GameLabel>("btnEmploy:Text");
            m_employ_btn_label.Text = LocalizeModule.Instance.GetString("UI_Police.employ");

            m_upgrade_btn = this.Make<GameButton>("btnUpgrade");
            m_upgrade_btn_label = this.Make<GameLabel>("btnUpgrade:Text");
            m_upgrade_btn_label.Text = LocalizeModule.Instance.GetString("UI_Police.upgrade");

            m_upgrade_effects = new List<GameUIEffect>()
            {
                this.Make<GameUIEffect>("Effect_Root:UI_jingyuanpaiqian_xunzhang01"),
                this.Make<GameUIEffect>("Effect_Root:UI_jingyuanpaiqian_xunzhang02"),
                this.Make<GameUIEffect>("Effect_Root:UI_jingyuanpaiqian_xunzhang03"),
                this.Make<GameUIEffect>("Effect_Root:UI_jingyuanpaiqian_xunzhang04"),
            };

            int effect_i = 0;
            m_upgrade_effects.ForEach((item) => { item.EffectPrefabName = string.Format("UI_jingyuanpaiqian_xunzhang0{0}.prefab", ++effect_i); });

        }

        public override void OnShow(object param)
        {
            EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause += ShowTips;

            this.m_back_btn.AddClickCallBack(OnBackClicked);

            for (int i = 0; i < m_tool_icons.Count; ++i)
            {
                m_tool_icons[i].AddPressDownCallBack(OnToolPressDown);
                m_tool_icons[i].AddPressUpCallBack(OnToolPressUp);
            }

            for (int i = 0; i < m_items.Count; ++i)
            {
                m_items[i].AddPressDownCallBack(OnItemPressDown);
                m_items[i].AddPressUpCallBack(OnItemPressUp);
            }

            this.m_employ_btn.AddClickCallBack(OnEmployClicked);
            this.m_upgrade_btn.AddClickCallBack(OnUpgradeClicked);

            m_tool_tip.Visible = false;

            this.HideUpgradeEffect();
        }

        public override void OnHide()
        {
            EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= ShowTips;

            this.m_back_btn.RemoveClickCallBack(OnBackClicked);

            for (int i = 0; i < m_tool_icons.Count; ++i)
            {
                m_tool_icons[i].RemovePressDownCallBack(OnToolPressDown);
                m_tool_icons[i].RemovePressUpCallBack(OnToolPressUp);
            }

            for (int i = 0; i < m_items.Count; ++i)
            {
                m_items[i].RemovePressDownCallBack(OnItemPressDown);
                m_items[i].RemovePressUpCallBack(OnItemPressUp);
            }

            this.m_employ_btn.RemoveClickCallBack(OnEmployClicked);
            this.m_upgrade_btn.RemoveClickCallBack(OnUpgradeClicked);

            this.HideUpgradeEffect();

        }

        public void Refresh(ConfOfficer ori_info_, OfficerInfo server_info_ = null, bool with_effect_ = false)
        {
            m_body_tex.TextureName = ori_info_.portrait;
            m_body_tex.SetGray(false);
            this.m_officer_id = ori_info_.id;
            this.m_officer_server_id = null == server_info_ ? -1 : server_info_.PlayerOfficerId;

            int lvl = null == server_info_ ? 0 : server_info_.Level;

            m_level_icon.Sprite = PoliceUILogicAssist.GetPoliceRankIcon(lvl);
            m_name_title.Text = LocalizeModule.Instance.GetString(ori_info_.name);

            ConfCombineFormula next_level_combine_info = PoliceUILogicAssist.GetCombineInfo(ori_info_, lvl + 1);

            if (null != next_level_combine_info)
            {
                //tool
                List<string> tool_icon_names = new List<string>();
                tool_icon_names.Add(ConfProp.Get(next_level_combine_info.propId1).icon);
                tool_icon_names.Add(ConfProp.Get(next_level_combine_info.propId2).icon);
                tool_icon_names.Add(ConfProp.Get(next_level_combine_info.propId3).icon);
                tool_icon_names.Add(ConfProp.Get(next_level_combine_info.propId4).icon);
                tool_icon_names.Add(ConfProp.Get(next_level_combine_info.propId5).icon);

                List<long> tool_ids = new List<long>();
                tool_ids.Add(next_level_combine_info.propId1);
                tool_ids.Add(next_level_combine_info.propId2);
                tool_ids.Add(next_level_combine_info.propId3);
                tool_ids.Add(next_level_combine_info.propId4);
                tool_ids.Add(next_level_combine_info.propId5);

                bool lock_officer = false;

                for (int i = 0; i < m_tool_icons.Count; ++i)
                {
                    bool disable = !GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(tool_ids[i]);
                    if (disable)
                    {
                        lock_officer = true;
                    }
                    m_tool_icons[i].Refresh(tool_icon_names[i], disable);
                }

                ToolData t_d1 = new ToolData
                {
                    m_tool_id = next_level_combine_info.propId1,
                    m_tool_count = GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(next_level_combine_info.propId1) ? GlobalInfo.MY_PLAYER_INFO.Bag_infos[next_level_combine_info.propId1].Count : 0,
                    m_tool_max_count = 1
                };

                ToolData t_d2 = new ToolData
                {
                    m_tool_id = next_level_combine_info.propId2,
                    m_tool_count = GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(next_level_combine_info.propId2) ? GlobalInfo.MY_PLAYER_INFO.Bag_infos[next_level_combine_info.propId2].Count : 0,
                    m_tool_max_count = 1
                };
                ToolData t_d3 = new ToolData
                {
                    m_tool_id = next_level_combine_info.propId3,
                    m_tool_count = GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(next_level_combine_info.propId3) ? GlobalInfo.MY_PLAYER_INFO.Bag_infos[next_level_combine_info.propId3].Count : 0,
                    m_tool_max_count = 1
                };
                ToolData t_d4 = new ToolData
                {
                    m_tool_id = next_level_combine_info.propId4,
                    m_tool_count = GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(next_level_combine_info.propId4) ? GlobalInfo.MY_PLAYER_INFO.Bag_infos[next_level_combine_info.propId4].Count : 0,
                    m_tool_max_count = 1
                };
                ToolData t_d5 = new ToolData
                {
                    m_tool_id = next_level_combine_info.propId5,
                    m_tool_count = GlobalInfo.MY_PLAYER_INFO.Bag_infos.ContainsKey(next_level_combine_info.propId5) ? GlobalInfo.MY_PLAYER_INFO.Bag_infos[next_level_combine_info.propId5].Count : 0,
                    m_tool_max_count = 1
                };
                m_tool_datas = new List<ToolData>()
                {
                    t_d1,
                    t_d2,
                    t_d3,
                    t_d4,
                    t_d5
                };



                //item
                List<string> item_icon_names = new List<string>();
                item_icon_names.Add(ConfProp.Get(next_level_combine_info.specialPropId1).icon);
                item_icon_names.Add(ConfProp.Get(next_level_combine_info.specialPropId2).icon);
                item_icon_names.Add(ConfProp.Get(next_level_combine_info.specialPropId3).icon);
                item_icon_names.Add(ConfProp.Get(next_level_combine_info.specialPropId4).icon);

                List<long> item_ids = new List<long>();
                item_ids.Add(next_level_combine_info.specialPropId1);
                item_ids.Add(next_level_combine_info.specialPropId2);
                item_ids.Add(next_level_combine_info.specialPropId3);
                item_ids.Add(next_level_combine_info.specialPropId4);

                List<int> item_nums = new List<int>();
                item_nums.Add(next_level_combine_info.special1Count);
                item_nums.Add(next_level_combine_info.special2Count);
                item_nums.Add(next_level_combine_info.special3Count);
                item_nums.Add(next_level_combine_info.special4Count);

                m_item_datas = new List<ToolData>();

                for (int i = 0; i < item_ids.Count; ++i)
                {
                    m_items[i].Sprite = item_icon_names[i];
                    m_items[i].gameObject.name = i.ToString();

                    PlayerPropMsg cur_item_in_bag;
                    int cur_num = 0;
                    if (GlobalInfo.MY_PLAYER_INFO.Bag_infos.TryGetValue(item_ids[i], out cur_item_in_bag))
                    {
                        cur_num = cur_item_in_bag.Count;
                    }
                    m_items_nums[i].Text = cur_num.ToString();
                    m_items_max_nums[i].Text = item_nums[i].ToString();
                    m_item_progress[i].FillAmmount = (float)(((float)cur_num) / item_nums[i]);

                    if (cur_num < item_nums[i])
                        lock_officer = true;

                    ToolData t_d = new ToolData
                    {
                        m_tool_id = item_ids[i],
                        m_tool_count = cur_num,
                        m_tool_max_count = item_nums[i]
                    };

                    m_item_datas.Add(t_d);

                }

                if (0 == lvl)
                {
                    if (!lock_officer)
                    //   this.m_employ_btn.Visible = true;
                    {
                        SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_EMPLOY_ENABLE);
                    }
                    else
                    //this.m_employ_btn.Visible = false;
                    {
                        //TimeModule.Instance.SetTimeout(() => SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_EMPLOY_DISABLE), 0.5f);
                        SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_EMPLOY_DISABLE);
                    }

                    this.m_upgrade_btn.Visible = false;
                    m_body_tex.SetGray(true);
                }
                else
                {

                    if (!lock_officer)
                        //this.m_upgrade_btn.Visible = true;
                        SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_UPGRADE_ENABLE);
                    else
                        //this.m_upgrade_btn.Visible = false;
                        //TimeModule.Instance.SetTimeout(() => SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_UPGRADE_DISABLE), 0.5f);
                        SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_UPGRADE_DISABLE);
                }

                m_tool_icons.ForEach((tool) => { tool.Visible = true; });
                m_item_roots.ForEach((tool) => { tool.Visible = true; });
            }
            else
            {
                m_tool_icons.ForEach((tool) => { tool.Visible = false; });
                m_item_roots.ForEach((tool) => { tool.Visible = false; });


                this.SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE.E_ALL_DISABLE);
            }


            if (with_effect_)
            {
                ShowUpgradeEffect(lvl);
            }


        }

        private void HideUpgradeEffect()
        {
            m_upgrade_effects.ForEach((item) => { item.Visible = false; });
        }

        private void ShowUpgradeEffect(int lvl_)
        {
            int i = 0;

            switch (lvl_)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    i = 0;
                    break;
                case 5:
                case 6:
                    i = 1;
                    break;
                case 7:
                case 8:
                    i = 2;
                    break;
                case 9:
                case 10:
                    i = 3;
                    break;
                default:
                    DebugUtil.LogError("没有的警员等级，没有对应的升级特效 " + lvl_);
                    break;
            }


            this.m_upgrade_effects[i].Visible = false;
            this.m_upgrade_effects[i].Visible = true;
        }


        private void OnBackClicked(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.Visible = false;

            if (!m_onBack.IsNull)
            {
                m_onBack.SafeInvoke();
            }
        }

        private void OnToolPressDown(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());

            int index = int.Parse(obj_.name);

            ToolData t_d = m_tool_datas[index];

            m_tool_tip.Refresh(t_d.m_tool_id, t_d.m_tool_count, t_d.m_tool_max_count);

            RectTransform rect = obj_.GetComponent<RectTransform>();

            m_tool_tip.Widget.anchoredPosition = rect.anchoredPosition - new Vector2(m_tool_tip.Widget.sizeDelta.x * 0.5f, -20.0f);
            m_tool_tip.Visible = true;
        }
        private void OnToolPressUp(GameObject obj_)
        {
            m_tool_tip.Visible = false;
        }


        private void OnItemPressDown(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            int index = int.Parse(obj_.name);

            ToolData t_d = m_item_datas[index];

            m_tool_tip.Refresh(t_d.m_tool_id, t_d.m_tool_count, t_d.m_tool_max_count);

            m_tool_tip.gameObject.transform.position = obj_.transform.position;
            m_tool_tip.Widget.anchoredPosition = m_tool_tip.Widget.anchoredPosition - new Vector2(m_tool_tip.Widget.sizeDelta.x * 0.5f + 30.0f, index * -30.0f);
            //RectTransform rect = obj_.GetComponent<RectTransform>();
            //m_tool_tip.Widget.anchoredPosition = rect.anchoredPosition - new Vector2(m_tool_tip.Widget.sizeDelta.x * 0.5f, -20.0f);
            m_tool_tip.Visible = true;
        }
        private void OnItemPressUp(GameObject obj_)
        {
            m_tool_tip.Visible = false;
        }

        private void OnEmployClicked(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.policeman_hire.ToString());
            CSOfficerCombineRequest req = new CSOfficerCombineRequest();
            req.PlayerId = GlobalInfo.MY_PLAYER_ID;
            req.OfficerId = this.m_officer_id;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif

        }

        private void OnUpgradeClicked(GameObject obj_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.policeman_promote.ToString());
            CSOfficerUpdateRequest req = new CSOfficerUpdateRequest();
            req.PlayerId = GlobalInfo.MY_PLAYER_ID;
            req.PlayerOfficerId = this.m_officer_server_id;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif


        }

        private void SwitchUpgradAndEmployBtn(ENUM_BTN_TYPE t_)
        {
            switch (t_)
            {
                case ENUM_BTN_TYPE.E_ALL_DISABLE:
                    {
                        this.m_employ_btn.Visible = false;
                        this.m_upgrade_btn.Visible = false;
                    }
                    break;
                case ENUM_BTN_TYPE.E_EMPLOY_DISABLE:
                    {
                        this.m_employ_btn.Visible = true;
                        this.m_employ_btn.SetGray(true);
                        this.m_employ_btn.Enable = false;
                        this.m_upgrade_btn.Visible = false;
                    }
                    break;
                case ENUM_BTN_TYPE.E_EMPLOY_ENABLE:
                    {
                        this.m_employ_btn.Visible = true;
                        this.m_employ_btn.SetGray(false);
                        this.m_employ_btn.Enable = true;
                        this.m_upgrade_btn.Visible = false;
                    }
                    break;
                case ENUM_BTN_TYPE.E_UPGRADE_DISABLE:
                    {
                        this.m_employ_btn.Visible = false;
                        this.m_upgrade_btn.Visible = true;
                        this.m_upgrade_btn.SetGray(true);
                        this.m_upgrade_btn.Enable = false;

                    }
                    break;
                case ENUM_BTN_TYPE.E_UPGRADE_ENABLE:
                    {
                        this.m_employ_btn.Visible = false;
                        this.m_upgrade_btn.Visible = true;
                        this.m_upgrade_btn.SetGray(false);
                        this.m_upgrade_btn.Enable = true;
                    }
                    break;
            }
        }

        private void ShowTips(bool v_)
        {
            if (false == v_)
                this.m_tool_tip.Visible = v_;
        }
    }
}

#endif