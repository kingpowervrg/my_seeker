#if false
#define ELAPSE

using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOEngine;
using GOGUI;
using fastJSON;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_JIGSAW)]
    public class JigsawUILogic : UILogicBase
    {
        private enum ENUM_RECT_DIR
        {
            E_UP,
            E_DOWN,
            E_LEFT,
            E_RIGHT,
            E_LEFT_UP,
            E_RIGHT_UP,
            E_LEFT_DOWN,
            E_RIGHT_DOWN,

        }

        private readonly Vector3 C_WIN_SCALE = new Vector3(1.15f, 1.15f, 1.15f);

        public Dictionary<string, JigsawItem> m_drag_able_parents_list = new Dictionary<string, JigsawItem>();
        public int m_mx_size = 4;
        public float m_straight_near_dis = 16.0f;

        private GameUIContainer m_jigsaw_item_container;
        private GameUIComponent m_fly_end_point;

        private List<GameUIComponent> m_reward_icon_anchors; //奖励锚点，旋转用
        private List<GameUIComponent> m_reward_icon_anchor_pos_list; //奖励锚点上标明图标位置的点
        private List<GameImage> m_reward_icons; //奖励图标
        private List<GameTextComponent> m_texts; //奖励文字
        private List<GameUIComponent> m_separator_lines; //分割
        private List<GameImage> m_time_progress_list; //圆盘计时器
        private GameImage m_hand_of_clock; //钟表指针
        private GameUIComponent m_block_ray_panel;
        private GameLabel m_count_down_time_label;
        private GameButton m_btnPause = null;
        private GameImage m_border_img = null;

        private string[,] jigsaw_item_matrix = new string[4, 4];

        private float m_sqrt_straight_near_dis;
        public float m_oblique_near_dis = 10.0f;
        private float m_sqrt_oblique_near_dis;
        //Dictionary<int, Dictionary<int, string>> m_split_index_mx = new Dictionary<int, Dictionary<int, string>>();
        private const int m_mx_max_size = 4;
        private Vector2 m_logic_rect_size;
        private RectTransform canvasRectTrans;

        private int m_left_chip_count;

        private long m_cur_scene_id;
        private int m_cur_jigsaw_id;
        private float m_total_time;
        private float m_cur_slider_time;
        private bool m_is_update_time;

        private ConfJigsawScene m_scene_data;
        private JigsawGameData m_jigsaw_data;

        private bool m_is_check_over_border;
        private float m_border_min_right_x;
        private float m_border_max_right_x;
        private float m_border_left_x;
        private float m_border_up_y;
        private float m_border_down_y;
        private float m_border_width;

        private bool m_isGuid = false;

        private List<float> m_fly_card_idx;
        protected override void OnInit()
        {
            base.OnInit();
            NeedUpdateByFrame = true;
            this.PreUILogic();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                List<long> vars = param as List<long>;
                this.m_cur_scene_id = vars[0];
                LoadingManager.Instance.SCENE_ID = m_cur_scene_id;

                this.m_cur_jigsaw_id = (int)vars[1];
            }

            this.AddUILogic();

            this.m_btnPause.AddClickCallBack(OnBtnGamePauseClick);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCStartResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFinishResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSuspendResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCQuitResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCResumeResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCJigsawReconnectResponse, OnScResponse);

            GameEvents.UIEvents.UI_Jigsaw_Event.OnPause += OnPause;
            //GameEvents.NetworkWatchEvents.NetPass += NetPass;
            EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause += OnApplicationPauseHandler;
            this.m_isGuid = false;
            if (!NewGuid.GuidNewManager.Instance.GetProgressByIndex(7))
            {
                m_btnPause.Visible = false;
                this.m_isGuid = true;
            }
            if (this.m_isGuid)
            {
                GameEvents.UI_Guid_Event.OnEnablePause += OnEnablePause;
            }
            this.m_jigsaw_item_container.SetScale(Vector3.one);

            m_is_check_over_border = false;

            Vector3[] border_bounds = new Vector3[4];
            this.m_border_img.Widget.GetWorldCorners(border_bounds);
            m_border_min_right_x = border_bounds[1].x;
            m_border_max_right_x = border_bounds[2].x;
            CommonData.S_JIGSAW_RIGHT_LIMIT_WORLD_X = m_border_max_right_x;
            m_border_width = m_border_max_right_x - m_border_min_right_x;
            m_border_img.Visible = false;

            TimeModule.Instance.SetTimeout(() =>
            {
                this.m_jigsaw_item_container.Widget.GetWorldCorners(border_bounds);
                m_border_left_x = border_bounds[0].x;
                m_border_up_y = border_bounds[1].y;
                m_border_down_y = border_bounds[0].y;
            }, 1.0f);

            m_count_down_time_label.Text = CommonTools.SecondToStringMMSS(this.m_cur_slider_time);

        }

        public override void OnHide()
        {
            base.OnHide();

            LoadingManager.Instance.SCENE_ID = 0;

            this.RemoveUILogic();

            this.m_btnPause.RemoveClickCallBack(OnBtnGamePauseClick);

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCStartResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFinishResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSuspendResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCQuitResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCResumeResponse, OnScResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCJigsawReconnectResponse, OnScResponse);

            GameEvents.UIEvents.UI_Jigsaw_Event.OnPause -= OnPause;
            EngineCore.EngineCoreEvents.SystemEvents.OnApplicationPause -= OnApplicationPauseHandler;
            if (this.m_isGuid)
            {
                GameEvents.UI_Guid_Event.OnEnablePause -= OnEnablePause;
            }
        }

        public override void Update()
        {
            if (!this.m_is_update_time)
                return;

            base.Update();


            this.m_cur_slider_time -= Time.deltaTime;

            if (this.m_cur_slider_time > 0)
            {
                float scaler = this.m_cur_slider_time / this.m_total_time;

                foreach (var t in this.m_time_progress_list)
                {
                    t.FillAmmount = scaler;
                }

                float degree = 360.0f * scaler;
                this.m_hand_of_clock.Widget.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, degree));

                m_count_down_time_label.Text = CommonTools.SecondToStringMMSS(this.m_cur_slider_time);

                if (m_is_check_over_border)
                {
                    this.CheckingOverBorder();
                }
            }
            else
            {
                this.m_is_update_time = false;
                this.SendFinish(false);
            }


        }

        private void OnEnablePause(bool isPause)
        {
            m_btnPause.Visible = isPause;
        }

        private bool IsBlocked(JigsawItem item_, Vector3 offset_)
        {
            var jigsaw = item_;
            Vector3[] icon_bounds = new Vector3[4];

            foreach (var icon in jigsaw.m_child_icon_list)
            {
                icon.Widget.GetWorldCorners(icon_bounds);
                float icon_right_x = icon_bounds[2].x + offset_.x;


                if (icon_right_x >= m_border_max_right_x)
                {
                    return true;
                }

                float icon_left_x = icon_bounds[0].x + offset_.x;

                if (icon_left_x <= m_border_left_x)
                {
                    return true;
                }


                float icon_up_y = icon_bounds[2].y + offset_.y;

                if (icon_up_y > m_border_up_y)
                {
                    return true;
                }


                float icon_down_y = icon_bounds[0].y + offset_.y;

                if (icon_down_y < m_border_down_y)
                {
                    return true;
                }

            }

            return false;
        }

        private void CheckingOverBorder()
        {
            Vector3[] icon_bounds = new Vector3[4];

            //foreach (var kvp in m_drag_able_parents_list)
            //{
            //var jigsaw = kvp.Value;
            var jigsaw = m_moving_item;

            foreach (var icon in jigsaw.m_child_icon_list)
            {
                icon.Widget.GetWorldCorners(icon_bounds);
                float icon_right_x = icon_bounds[2].x;

                if (icon_right_x > m_border_min_right_x)
                {
                    float delta = 1.0f - Mathf.Clamp01((m_border_max_right_x - icon_right_x) / m_border_width);
                    this.m_border_img.Color = new Color(this.m_border_img.Color.r, this.m_border_img.Color.g, this.m_border_img.Color.b, delta);

                    m_border_img.Visible = true;
                    return;
                }
            }
            //}

            m_border_img.Visible = false;
        }

        //private void OnScResponse(object s)
        //{
        //    //Debug.Log(s);
        //    if (s is SCloginResponse)
        //    {
        //        var rsp = s as SCloginResponse;

        //        if (1 != rsp.Result)
        //        {
        //            DebugUtil.LogErrorFormat("登陆失败 {0}", rsp.Result);
        //            return;
        //        }

        //        GlobalInfo.MY_PLAYER_ID = rsp.PlayerId;

        //        PlayerInfo info = new PlayerInfo(rsp.PlayerId);

        //        info.SetCash(rsp.Cash).SetCoin(rsp.Coin).SetExp(rsp.Exp).SetExpMultiple(rsp.ExpMultiple)
        //            .SetIcon(rsp.PlayerIcon).SetLaborUnionn(rsp.LaborUnion)
        //            .SetLevel(rsp.Level).SetUpgradeExp(rsp.UpgradeExp).SetVit(rsp.Vit);



        //        PlayerInfoManager.Instance.AddPlayerInfo(rsp.PlayerId, info);

        //        //EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke("UI_sence_login.prefab");

        //        //GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.NORMAL);
        //    }
        //    else if (s is SCOfficerInfosReponse)
        //    {
        //        var rsp = s as SCOfficerInfosReponse;

        //        PlayerInfo p_info = PlayerInfoManager.Instance.GetPlayerInfo(GlobalInfo.MY_PLAYER_ID);
        //        p_info.SetOfficerInfos(rsp.Infos);

        //        //EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke("UI_sence_login.prefab");

        //        //GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.NORMAL);

        //    }
        //    else if (s is SCChapterInfosResponse)
        //    {
        //        var rsp = s as SCChapterInfosResponse;

        //        foreach (var info in rsp.ChapterInfos)
        //        {
        //            ChapterInfoManager.Instance.SetChapterInfo(info.Id, info);
        //        }

        //        EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke("UI_sence_login.prefab");

        //        GameRoot.instance.GameFSM.GotoState((int)ClientFSM.ClientState.NORMAL);

        //    }


        //}

        private void PreUILogic()
        {
            this.m_fly_end_point = this.Make<GameUIComponent>("EndPoint");
            this.m_jigsaw_item_container = this.Make<GameUIContainer>("Panel");

            this.m_jigsaw_item_container.EnsureSize<JigsawItem>(1);
            Rect jigsaw_size = this.m_jigsaw_item_container.GetChild<JigsawItem>(0).Widget.rect;
            float r_w = jigsaw_size.width;
            float r_h = jigsaw_size.height;
            m_logic_rect_size = new Vector2(r_w, r_h);

            m_reward_icon_anchors = new List<GameUIComponent>(); //奖励锚点，旋转用
            GameUIComponent anchor = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel0");
            m_reward_icon_anchors.Add(anchor);
            anchor = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel1");
            m_reward_icon_anchors.Add(anchor);
            anchor = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel2");
            m_reward_icon_anchors.Add(anchor);
            anchor = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel3");
            m_reward_icon_anchors.Add(anchor);

            m_reward_icon_anchor_pos_list = new List<GameUIComponent>();
            GameUIComponent pos = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel0/Anchor");
            m_reward_icon_anchor_pos_list.Add(pos);
            pos = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel1/Anchor");
            m_reward_icon_anchor_pos_list.Add(pos);
            pos = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel2/Anchor");
            m_reward_icon_anchor_pos_list.Add(pos);
            pos = this.Make<GameUIComponent>("Panel_down/Panel_2/Panel3/Anchor");
            m_reward_icon_anchor_pos_list.Add(pos);

            m_reward_icons = new List<GameImage>(); //奖励图标
            GameImage icon = this.Make<GameImage>("Panel_down/Panel_2/Image_0");
            m_reward_icons.Add(icon);
            icon = this.Make<GameImage>("Panel_down/Panel_2/Image_1");
            m_reward_icons.Add(icon);
            icon = this.Make<GameImage>("Panel_down/Panel_2/Image_2");
            m_reward_icons.Add(icon);
            icon = this.Make<GameImage>("Panel_down/Panel_2/Image_3");
            m_reward_icons.Add(icon);

            m_texts = new List<GameTextComponent>(); //奖励文字
            GameTextComponent txt = this.Make<GameTextComponent>("Panel_down/Panel_2/Image_0/Text");
            m_texts.Add(txt);
            txt = this.Make<GameTextComponent>("Panel_down/Panel_2/Image_1/Text");
            m_texts.Add(txt);
            txt = this.Make<GameTextComponent>("Panel_down/Panel_2/Image_2/Text");
            m_texts.Add(txt);
            txt = this.Make<GameTextComponent>("Panel_down/Panel_2/Image_3/Text");
            m_texts.Add(txt);

            m_separator_lines = new List<GameUIComponent>(); //分割
            GameUIComponent line = this.Make<GameUIComponent>("Panel_down/Panel_1/Panel4_1");
            m_separator_lines.Add(line);
            line = this.Make<GameUIComponent>("Panel_down/Panel_1/Panel4_2");
            m_separator_lines.Add(line);
            line = this.Make<GameUIComponent>("Panel_down/Panel_1/Panel4_3");
            m_separator_lines.Add(line);
            line = this.Make<GameUIComponent>("Panel_down/Panel_1/Panel4_4");
            m_separator_lines.Add(line);

            m_time_progress_list = new List<GameImage>(); //圆盘计时器
            GameImage progress = this.Make<GameImage>("Panel_down/Panel_1/Image_1");
            m_time_progress_list.Add(progress);
            progress = this.Make<GameImage>("Panel_down/Panel_1/image_2");
            m_time_progress_list.Add(progress);

            m_hand_of_clock = this.Make<GameImage>("Panel_down/Panel_1/image_6");//钟表指针

            m_block_ray_panel = this.Make<GameUIComponent>("Panel_Ray_Block");

            m_count_down_time_label = this.Make<GameLabel>("Panel_down/Panel_1/Text_time");

            m_btnPause = Make<GameButton>("Button_pause");

            m_border_img = Make<GameImage>("Image");

            m_sqrt_straight_near_dis = (float)(m_straight_near_dis * m_straight_near_dis);
            m_sqrt_oblique_near_dis = (float)(m_oblique_near_dis * m_oblique_near_dis);
            canvasRectTrans = this.Transform;

            List<float> delta_idx = new List<float>();

            if( 0 == m_mx_size % 2)
            {
                for( int i = -m_mx_size / 2; i < 0; ++i)
                {
                    delta_idx.Add(i + 0.5f);
                }

                for( int i = 1; i <= m_mx_size / 2; ++i)
                {
                    delta_idx.Add(i - 0.5f);
                }
            }
            else
            {
                for (int i = -m_mx_size / 2; i < 0; ++i)
                {
                    delta_idx.Add(i);
                }

                for (int i = 0; i <= m_mx_size / 2; ++i)
                {
                    delta_idx.Add(i);
                }
            }

            List<float> temp_lst = new List<float>();

            while (delta_idx.Count > 0)
            {
                if (0 == delta_idx.Count % 2)
                {
                    int first_idx = delta_idx.Count / 2 - 1;
                    int sencond_idx = first_idx + 1;

                    temp_lst.Add(delta_idx[first_idx]);
                    temp_lst.Add(delta_idx[sencond_idx]);

                    delta_idx.RemoveRange(first_idx, 2);
                }
                else
                {
                    int first_idx = delta_idx.Count / 2;
                    temp_lst.Add(delta_idx[first_idx]);
                    delta_idx.RemoveAt(first_idx);
                }
            }

            m_fly_card_idx = temp_lst;
        }

        private void AddUILogic()
        {
            Scene_data = ConfJigsawScene.Get(this.m_cur_scene_id);
            m_jigsaw_data = JigsawDataManager.Instance.GetJigsaw(this.m_cur_jigsaw_id);

            this.InitTime();
            this.RefreshJigsaws();
            TimeModule.Instance.SetTimeout(() => this.FlyCard(), 0.2f);
        }

        private void RemoveUILogic()
        {
            this.StopAllShine();
        }

        private TweenScale m_ts;

        private void OnScResponse(object s)
        {
            if (s is SCStartResponse)
            {
                var rsp = s as SCStartResponse;

                MsgStatusCodeUtil.OnError(rsp.Result);

                //if (1 != rsp.Result)
                //{
                //    DebugUtil.LogError("拼图开始计时失败");
                //}
            }
            else if (s is SCFinishResponse)
            {
                var rsp = s as SCFinishResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Result))
                {
                    m_btnPause.Visible = false;
                    m_ts = TweenScale.Begin(this.m_jigsaw_item_container.gameObject, 1.5f, C_WIN_SCALE, m_ts);
                    m_ts.AddOnFinished(() =>
                   {
                       m_btnPause.Visible = true;
                       WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_JIGSAW, rsp);


                       FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                       param.Param = data;


                       EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                   }
                    );



                    if (0 != rsp.PropId)
                        GlobalInfo.MY_PLAYER_INFO.AddBagInfo(new List<long>() { rsp.PropId }, new List<int> { 1 });

                    int percent = 0;
                    foreach (var item in rsp.Rewards)
                    {
                        percent = item.Percent;
                    }

                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, rsp.Result == 0 ? 1 : 0},
                        { UBSParamKeyName.ContentID, m_cur_jigsaw_id},
                        { UBSParamKeyName.ContentType, rsp.JigsawState},
                        { UBSParamKeyName.TotalTime, m_total_time - m_cur_slider_time},
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_finish, null, _params);
                }
                else
                {
                    WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_JIGSAW, rsp);
                    rsp.Result = 1;

                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                    param.Param = data;


                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);


                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.Success, 0},
                        { UBSParamKeyName.Description, rsp.Result},
                        { UBSParamKeyName.ContentID, m_cur_jigsaw_id},
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_finish, null, _params);
                }

            }
            else if (s is SCSuspendResponse)
            {
                Debug.Log("收到消息 SCSuspendResponse");
                var rsp = s as SCSuspendResponse;

                MsgStatusCodeUtil.OnError(rsp.Result);
            }
            else if (s is SCResumeResponse)
            {
                var rsp = s as SCResumeResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.Result))
                {
                    this.m_is_update_time = true;
                }

            }
            else if (s is SCQuitResponse)
            {
                Debug.Log("收到消息 SCQuitResponse");
                var rsp = s as SCQuitResponse;

                MsgStatusCodeUtil.OnError(rsp.Result);

                this.CloseFrame();

            }
            else if (s is SCJigsawReconnectResponse)
            {
                var rsp = s as SCJigsawReconnectResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                {
                    if (this.m_cur_slider_time < 0)
                        ForceFail();
                    else
                    {
                        if (m_left_chip_count <= 0)
                        {
                            SendFinish(true);
                        }
                        else
                            this.m_is_update_time = true;
                    }
                }
            }
        }

        private void InitTime()
        {
            m_is_update_time = false;


            //计算总耗时
            //List<OfficerInfo> officers = GlobalInfo.MY_PLAYER_INFO.Officer_infos;

            //foreach (int index in PoliceDispatchManager.Instance.GetAllDispath())
            //{

            //}

#if ELAPSE
            m_cur_slider_time = m_total_time = Scene_data.percent4;
#else
            m_cur_slider_time = m_total_time = 300.0f;
#endif


        }


        private void RefreshJigsaws()
        {
            foreach (var t in this.m_time_progress_list)
            {
                t.FillAmmount = 1.0f;
            }

            this.m_hand_of_clock.Widget.localRotation = Quaternion.identity;

            this.RefreshSeparatorLine();



            List<string> reward_count = new List<string>
            {
                this.Scene_data.num1.ToString(),
            this.Scene_data.num2.ToString(),
            this.Scene_data.num3.ToString(),
            this.Scene_data.num4.ToString()
            };

            for (int i = 0; i < this.m_texts.Count && i < reward_count.Count; ++i)
            {
                this.m_texts[i].Text = reward_count[i];
            }

            this.m_jigsaw_item_container.Clear();
            this.m_jigsaw_item_container.EnsureSize<JigsawItem>(16);

            //Rect jigsaw_size = this.m_jigsaw_item_container.GetChild<JigsawItem>(0).Widget.rect;
            //float r_w = jigsaw_size.width;
            //float r_h = jigsaw_size.height;
            //m_logic_rect_size = new Vector2(r_w, r_h);
            m_drag_able_parents_list.Clear();

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    JigsawItem JItem = this.m_jigsaw_item_container.GetChild<JigsawItem>(i * 4 + j);
                    string j_name = string.Format("{0}{1}", i, j);
                    jigsaw_item_matrix[i, j] = j_name;


                    Rect rt = new Rect();

                    JigsawChipJson chip = m_jigsaw_data.GetChip(i, j);

                    if (null != chip)
                    {
                        JItem.gameObject.name = j_name;
                        JItem.RenameRect(j_name);
                        JItem.Visible = true;

                        rt.x = chip.M_tex_size.M_x; rt.y = chip.M_tex_size.M_y; rt.width = chip.M_tex_size.M_w; rt.height = chip.M_tex_size.M_h;
                        JItem.SetTex(chip.M_tex_anme, rt);

                        foreach (DragDropMoveParentTexture d in JItem.m_child_icon_list)
                        {
                            d.Visible = false;
                            d.Visible = true;
                            d.RegisterMoveBegin(PickUpJigsaw);
                            d.RegisterMoveEnd(PutDownJigsaw);
                            d.RegisterMoving(IsCanDragg);
                        }
                        m_drag_able_parents_list.Add(j_name, JItem);
                    }
                    else
                    {
                        JItem.Visible = false;
                    }

                }

            }

            this.m_left_chip_count = this.m_drag_able_parents_list.Values.Count - 1;

            m_block_ray_panel.SetActive(true);
        }

        private void FlyCard()
        {


            float r = Screen.width * 0.8f;
            //float half_h = canvasRectTrans.rect.height * 0.5f;
            float delta_t = 0.1f;

            //Vector3 endPointLocalPos = this.m_fly_end_point.gameObject.transform.localPosition;
            Vector3 endPointWorldPos = this.m_fly_end_point.gameObject.transform.position;
            //if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTrans, this.m_fly_end_point.Widget.anchoredPosition, CameraManager.Instance.UICamera, out endPointWorldPos))
            //    return;
            //endPointWorldPos.z = this.m_fly_end_point.gameObject.transform.position.z;

            Vector3 top_center_world_pos = FrameMgr.Instance.UICamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 1.0f));
            float max_delta_world_dis_y = Mathf.Abs(top_center_world_pos.y) / 4.0f;
            float max_delta_world_dis_x = Mathf.Abs(top_center_world_pos.x) / 5.0f;


            float cost_time = 0.0f;

            int i = 0;

            System.Random ra = new System.Random();
            List<JigsawItem> temp_list = new List<JigsawItem>(m_drag_able_parents_list.Values);

            while (temp_list.Count > 0)
            {
                int chooes = ra.Next(temp_list.Count);

                var item = temp_list[chooes];

                Vector2 r_pos = Random.insideUnitCircle.normalized;

                Vector2 rect_pos = new Vector2(r_pos.x * r, r_pos.y * r);

                item.Widget.anchoredPosition = rect_pos;
                Vector3 offset = item.m_child_icon_list[0].gameObject.transform.position - item.gameObject.transform.position;
                //item.Widget.anchoredPosition = rect_pos - item.m_child_icon_list[0].Widget.anchoredPosition;

                item.gameObject.transform.SetAsLastSibling();
                //TweenScale tp = TweenScale.Begin(item.gameObject, 1.0f + i * delta_t, endPointLocalPos - offset);

                //Vector2 r_aim_pos = Random.insideUnitCircle.normalized * max_delta_world_ids;
                //Vector3 final_aim_pos = endPointWorldPos + new Vector3(r_aim_pos.x, r_aim_pos.y, 1.0f);

                int i_x = i % m_mx_size;
                int i_y = i / m_mx_size;

                float idx_x = m_fly_card_idx[i_x];
                float idx_y = -m_fly_card_idx[i_y];



                //Debug.Log("del idx = " + idx_x + " " + idx_y);

                Vector3 final_aim_pos = endPointWorldPos + new Vector3(idx_x * max_delta_world_dis_x, idx_y * max_delta_world_dis_y, 1.0f);

                //TweenScale tp = TweenScale.Begin(item.gameObject, 1.0f + i * delta_t, endPointWorldPos - offset);
                TweenScale tp = TweenScale.Begin(item.gameObject, 1.0f + i * delta_t, final_aim_pos - offset);
                tp.worldSpace = true;
                tp.easeType = EaseType.easeInOutQuad;


                Quaternion random_r = Random.rotationUniform;
                Vector3 v_r = random_r.eulerAngles;
                v_r = new Vector3(0.0f, 0.0f, v_r.z);
                random_r = Quaternion.Euler(v_r);
                item.gameObject.transform.rotation = random_r;
                TweenRotationEuler tr = TweenRotationEuler.Begin(item.gameObject, 1.0f + i * delta_t, Quaternion.identity);
                tr.easeType = EaseType.easeInOutQuad;

                cost_time = 1.0f + i * delta_t;

                ++i;

                temp_list.Remove(item);
            }

            //foreach (var item in m_drag_able_parents_list.Values)
            //{
            //    Vector2 r_pos = Random.insideUnitCircle.normalized;

            //    Vector2 rect_pos = new Vector2(r_pos.x * r, r_pos.y * r);

            //    item.Widget.anchoredPosition = rect_pos;
            //    Vector3 offset = item.m_child_icon_list[0].gameObject.transform.position - item.gameObject.transform.position;
            //    //item.Widget.anchoredPosition = rect_pos - item.m_child_icon_list[0].Widget.anchoredPosition;

            //    item.gameObject.transform.SetAsLastSibling();
            //    //TweenScale tp = TweenScale.Begin(item.gameObject, 1.0f + i * delta_t, endPointLocalPos - offset);
            //    TweenScale tp = TweenScale.Begin(item.gameObject, 1.0f + i * delta_t, endPointWorldPos - offset);
            //    tp.worldSpace = true;
            //    tp.easeType = EaseType.easeInOutQuad;


            //    Quaternion random_r = Random.rotationUniform;
            //    Vector3 v_r = random_r.eulerAngles;
            //    v_r = new Vector3(0.0f, 0.0f, v_r.z);
            //    random_r = Quaternion.Euler(v_r);
            //    item.gameObject.transform.rotation = random_r;
            //    TweenRotationEuler tr = TweenRotationEuler.Begin(item.gameObject, 1.0f + i * delta_t, Quaternion.identity);
            //    tr.easeType = EaseType.easeInOutQuad;

            //    cost_time = 1.0f + i * delta_t;

            //    ++i;
            //}

            m_is_first_pick = true;
            m_block_ray_panel.SetActive(false);
            //TimeModule.Instance.SetTimeout(() => { this.SendStarTimer(); this.m_is_update_time = true; m_block_ray_panel.SetActive(false); }, cost_time);


        }

        private void StartTimer()
        {
            this.SendStarTimer();
            this.m_is_update_time = true;
            m_block_ray_panel.SetActive(false);
        }

        private bool m_is_first_pick;
        private JigsawItem m_moving_item = null;
        private void PickUpJigsaw(GameObject moved_obj_)
        {
            if (m_is_first_pick)
            {
                m_is_first_pick = false;
                StartTimer();

            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.jigsaw_click.ToString(), null);
            JigsawItem moved_dp_ = this.m_drag_able_parents_list[moved_obj_.name];
            m_moving_item = moved_dp_;
            Debug.Log("捡起的随便是 " + moved_obj_.name);
            Debug.Log("碎片的孩子是 " + m_moving_item.m_child_icon_list);

            moved_dp_.Shadow(true);

            m_is_check_over_border = true;
        }

        private void PutDownJigsaw(GameObject moved_obj_)
        {
            m_moving_item = null;
            m_is_check_over_border = false;
            m_border_img.Visible = false;

            JigsawItem moved_dp_ = this.m_drag_able_parents_list[moved_obj_.name];
            List<GameUIComponent> move_end_logic_rects = moved_dp_.m_child_logic_rect_list;
            List<DragDropMoveParentTexture> move_end_icons = moved_dp_.m_child_icon_list;

            moved_dp_.Shadow(false);

            Vector2 offset = Vector2.zero;


            foreach (GameUIComponent move_end_logic_rect in move_end_logic_rects)
            {
                Dictionary<string, ENUM_RECT_DIR> neighbours = this.GetNeighbours(move_end_logic_rect.gameObject.name);

                if (0 == neighbours.Count)
                {
                    Debug.LogError(string.Format("拼图 {0} 没有邻居", move_end_logic_rect.gameObject.name));
                    continue;
                }

                foreach (JigsawItem judge_dp in m_drag_able_parents_list.Values)
                {
                    if (judge_dp == moved_dp_)
                        continue;

                    foreach (GameUIComponent cur_logic_rect in judge_dp.m_child_logic_rect_list)
                    {
                        string cur_name = cur_logic_rect.gameObject.name;

                        if (neighbours.ContainsKey(cur_name))
                        {
                            //if (this.IsNear(move_end_logic_rect.gameObject, cur_logic_rect.gameObject, neighbours[cur_name], out offset))
                            if (this.IsNear(move_end_logic_rect, cur_logic_rect, neighbours[cur_name], out offset))
                            {
                                if (this.IsIconNear(moved_dp_, judge_dp, neighbours[cur_name]))
                                {
                                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.jigsaw_putdone.ToString(), null);
                                    this.MoveAndCombine(moved_dp_, judge_dp, offset);
                                    return;
                                }


                            }
                        }
                    }

                }
            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound,GameCustomAudioKey.jigsaw_put.ToString(), null);


        }

        private bool IsCanDragg(Vector3 offset_)
        {
            if (null == m_moving_item)
                return false;

            //if (offset_.x < 0)
            //{
            //    return true;
            //}

            return !this.IsBlocked(m_moving_item, offset_);

        }

        private Dictionary<string, ENUM_RECT_DIR> GetNeighbours(string obj_name_)
        {
            Dictionary<string, ENUM_RECT_DIR> ret = new Dictionary<string, ENUM_RECT_DIR>();

            int i_name = int.Parse(obj_name_);

            int row = i_name / 10;
            int col = i_name % 10;

            int row_converted;
            int col_converted;

            //上
            string neighbour_name = null;
            row_converted = row - 1;
            col_converted = col;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_UP;
            }

            //下
            neighbour_name = null;
            row_converted = row + 1;
            col_converted = col;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_DOWN;
            }

            //左
            neighbour_name = null;
            row_converted = row;
            col_converted = col - 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT;
            }

            //右  
            neighbour_name = null;
            row_converted = row;
            col_converted = col + 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT;
            }

            //左上
            neighbour_name = null;
            row_converted = row - 1;
            col_converted = col - 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT_UP;

            }

            //右上
            neighbour_name = null;
            row_converted = row - 1;
            col_converted = col + 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT_UP;
            }

            //左下
            neighbour_name = null;
            row_converted = row + 1;
            col_converted = col - 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT_DOWN;
            }

            //右下
            neighbour_name = null;
            row_converted = row + 1;
            col_converted = col + 1;
            if (this.IsValid(row_converted, col_converted))
            {
                neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
                ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT_DOWN;
            }

            return ret;
        }

        private void IsFinish()
        {
            --m_left_chip_count;

            if (0 == m_left_chip_count)
            {
                this.SendFinish(true);
            }
            //else
            //{
            //    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_JIGSAW);
            //    param.Param = 0;

            //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            //}

        }

        private void MoveAndCombine(JigsawItem move_end_dp_, JigsawItem neighbour_, Vector2 offset)
        {
            move_end_dp_.Widget.anchoredPosition += offset;
            this.Combine(move_end_dp_, neighbour_);
            this.IsFinish();
        }

        private void Combine(JigsawItem combined_, JigsawItem combine_)
        {
            foreach (GameUIComponent combine_logic_rect in combine_.m_child_logic_rect_list)
            {
                combined_.m_child_logic_rect_list.Add(combine_logic_rect);
                combine_logic_rect.gameObject.transform.SetParent(combined_.gameObject.transform, true);
            }

            foreach (DragDropMoveParentTexture combine_icon in combine_.m_child_icon_list)
            {
                combined_.m_child_icon_list.Add(combine_icon);
                combine_icon.gameObject.transform.SetParent(combined_.gameObject.transform, true);
            }

            m_drag_able_parents_list.Remove(combine_.gameObject.name);
            //GameObject.DestroyImmediate(combine_.gameObject);

            this.ShineCombinedJigsaw(combined_);

        }

        private void StopAllShine()
        {
            foreach (var item in m_drag_able_parents_list.Values)
            {
                Shining(item, false);
            }
        }

        private void ShineCombinedJigsaw(JigsawItem item)
        {
            Shining(item, true);
            TimeModule.Instance.SetTimeout(() => { Shining(item, false); }, 1.0f);
        }



        private void Shining(JigsawItem item, bool b_)
        {
            item.Shine(b_);
        }


        private bool IsValid(int row_, int col_)
        {
            if (0 <= row_ && row_ < m_mx_size && 0 <= col_ && col_ < m_mx_size)
                return true;

            return false;
        }

        private Vector2 CalcMyAimCoord(Vector2 neighbour_coord_, ENUM_RECT_DIR dir_)
        {
            Vector2 aim_coord = Vector2.zero;

            if (ENUM_RECT_DIR.E_UP == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x, neighbour_coord_.y - m_logic_rect_size.y);
            }
            else if (ENUM_RECT_DIR.E_DOWN == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x, neighbour_coord_.y + m_logic_rect_size.y);
            }
            else if (ENUM_RECT_DIR.E_LEFT == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y);
            }
            else if (ENUM_RECT_DIR.E_RIGHT == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y);
            }
            if (ENUM_RECT_DIR.E_LEFT_UP == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y - m_logic_rect_size.y);
            }
            else if (ENUM_RECT_DIR.E_RIGHT_UP == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y - m_logic_rect_size.y);
            }
            else if (ENUM_RECT_DIR.E_LEFT_DOWN == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y + m_logic_rect_size.y);
            }
            else if (ENUM_RECT_DIR.E_RIGHT_DOWN == dir_)
            {
                aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y + m_logic_rect_size.y);
            }


            return aim_coord;
        }

        /// <summary>
        /// 邻居在我的哪个方向，是否算靠的很近
        /// </summary>
        /// <param name="moved_"></param>
        /// <param name="neighbour_"></param>
        /// <param name="dir_"></param>
        /// <param name="offset_"></param>
        /// <returns></returns>
        private bool IsNear(GameObject moved_, GameObject neighbour_, ENUM_RECT_DIR dir_, out Vector2 offset_)
        {
            offset_ = Vector3.zero;
            Vector2 moved_coord = this.GetLocalPositionInCanvas(moved_.gameObject);
            Vector2 neighbour_coord = this.GetLocalPositionInCanvas(neighbour_.gameObject);
            Vector2 aim_coord = this.CalcMyAimCoord(neighbour_coord, dir_);

            offset_ = aim_coord - moved_coord;
            float cur_dis = (offset_).sqrMagnitude;


            if (dir_ <= ENUM_RECT_DIR.E_RIGHT)
            {
                if (cur_dis > this.m_sqrt_straight_near_dis)
                    return false;
            }
            else
            {
                if (cur_dis > this.m_sqrt_oblique_near_dis)
                    return false;
            }

            offset_ = aim_coord - moved_coord;
            return true;

        }


        private bool IsNear(GameUIComponent moved_, GameUIComponent neighbour_, ENUM_RECT_DIR dir_, out Vector2 offset_)
        {
            offset_ = Vector3.zero;
            Vector2 moved_coord = this.GetLocalPositionInCanvas(moved_.gameObject);
            Vector2 neighbour_coord = this.GetLocalPositionInCanvas(neighbour_.gameObject);
            Vector2 aim_coord = this.CalcMyAimCoord(neighbour_coord, dir_);

            offset_ = aim_coord - moved_coord;
            float cur_dis = (offset_).sqrMagnitude;


            if (dir_ <= ENUM_RECT_DIR.E_RIGHT)
            {
                if (cur_dis > this.m_sqrt_straight_near_dis)
                    return false;
            }
            else
            {
                if (cur_dis > this.m_sqrt_oblique_near_dis)
                    return false;
            }

            offset_ = aim_coord - moved_coord;
            return true;

        }


        private bool IsIconNear(JigsawItem moved_, JigsawItem neighbour_, ENUM_RECT_DIR dir_)
        {
            Vector2 offset_;

            float judge_dis = 0.0f;

            if (dir_ <= ENUM_RECT_DIR.E_RIGHT)
            {
                judge_dis = this.m_straight_near_dis;
            }
            else
            {
                judge_dis = this.m_oblique_near_dis;
            }

            foreach (DragDropMoveParentTexture tex_moved in moved_.m_child_icon_list)
            {
                float moved_tex_half_width = tex_moved.Texture.Widget.sizeDelta.x / 2;
                float moved_tex_half_height = tex_moved.Texture.Widget.sizeDelta.y / 2;

                Vector2 moved_coord = this.GetLocalPositionInCanvas(tex_moved.gameObject);

                foreach (DragDropMoveParentTexture tex_neighbour in neighbour_.m_child_icon_list)
                {
                    float neighbour_tex_half_width = tex_neighbour.Texture.Widget.sizeDelta.x / 2;
                    float neighbour_tex_half_height = tex_neighbour.Texture.Widget.sizeDelta.y / 2;

                    Vector2 neighbour_coord = this.GetLocalPositionInCanvas(tex_neighbour.gameObject);

                    offset_ = neighbour_coord - moved_coord;

                    float width = moved_tex_half_width + neighbour_tex_half_width;
                    float height = moved_tex_half_height + neighbour_tex_half_height;

                    if (Mathf.Abs(offset_.x) > width + judge_dis || Mathf.Abs(offset_.y) > height + judge_dis)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;

        }

        public Vector2 GetLocalPositionInCanvas(GameObject obj)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, obj.transform.position), CameraManager.Instance.UICamera, out localPoint))
            {
                return localPoint;
            }

            return Vector2.zero;
        }

        private void RefreshSeparatorLine()
        {
            ConfJigsawScene data = this.Scene_data;

            List<float> percents = new List<float>();
            percents.Add(data.percent1);
            percents.Add(data.percent2);
            percents.Add(data.percent3);
            percents.Add(data.percent4);
#if ELAPSE
            for (int i = 0; i < percents.Count; ++i)
            {
                percents[i] = percents[i] / (percents[percents.Count - 1]) * 100.0f;
            }
#endif

            for (int i = 1, j = 0; i < this.m_separator_lines.Count && j < percents.Count; ++j, ++i)
            {
                float ang = percents[j] * 3.6f;
                m_separator_lines[i].Widget.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -ang));
            }

            List<float> delta_percents = new List<float>();

#if ELAPSE
            delta_percents.Add(percents[0]);
            delta_percents.Add(percents[1] - percents[0]);
            delta_percents.Add(percents[2] - percents[1]);
            delta_percents.Add(percents[3] - percents[2]);
#else
            delta_percents.Add(data.percent1);
            delta_percents.Add(data.percent2 - data.percent1);
            delta_percents.Add(data.percent3 - data.percent2);
            delta_percents.Add(data.percent4 - data.percent3);
#endif

            for (int i = 0; i < this.m_reward_icon_anchors.Count; ++i)
            {
                float percent = 0f;
                for (int j = 0; j < i; ++j)
                {
                    percent += delta_percents[j];
                }

                float ang = percent * 3.6f + delta_percents[i] * 1.8f;

                this.m_reward_icon_anchors[i].Widget.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -ang));
            }

            for (int i = 0; i < this.m_reward_icon_anchor_pos_list.Count && i < this.m_reward_icons.Count; ++i)
            {
                this.m_reward_icons[i].gameObject.transform.position = this.m_reward_icon_anchor_pos_list[i].gameObject.transform.position;
                this.m_reward_icons[i].Sprite = this.GetRewardIcon(i);
            }

        }

        private string GetRewardIcon(int idx)
        {
            ConfJigsawScene data = ConfJigsawScene.Get(m_cur_scene_id);

            EUNM_BASE_REWARD type = EUNM_BASE_REWARD.E_INVALID;

            if (0 == idx)
            {
                type = (EUNM_BASE_REWARD)(data.type1);
            }
            else if (1 == idx)
            {
                type = (EUNM_BASE_REWARD)(data.type2);
            }
            else if (2 == idx)
            {
                type = (EUNM_BASE_REWARD)(data.type3);
            }
            else if (3 == idx)
            {
                type = (EUNM_BASE_REWARD)(data.type4);
            }

            switch (type)
            {
                case EUNM_BASE_REWARD.E_CASH:
                    return "icon_mainpanel_cash_2.png";

                case EUNM_BASE_REWARD.E_COIN:
                    return "icon_mainpanel_coin_2.png";

                case EUNM_BASE_REWARD.E_EXP:
                    return "icon_mainpanel_exp_2.png";

                case EUNM_BASE_REWARD.E_VIT:
                    return "icon_mainpanel_energy_2.png";
            }

            DebugUtil.LogError(string.Format("jigsaw reward {0} type {1} is error", idx, type));
            return "label_mail_unread_1.png";

        }

        private void SendStarTimer()

        {
            CSStartRequest req = new CSStartRequest();
            req.PlayerId = GlobalInfo.MY_PLAYER_ID;


#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
        }

        private void SendFinish(bool is_win_)
        {
            this.m_is_update_time = false;

            if (is_win_)
            {
                CSFinishRequest req = new CSFinishRequest();
                req.SceneId = m_cur_scene_id;
                req.Result = is_win_ ? 1 : 0;
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            }
            else
            {
                CSFinishRequest req = new CSFinishRequest();
                req.SceneId = m_cur_scene_id;
                req.Result = is_win_ ? 1 : 0;
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

                ForceFail();
            }

        }

        private void ForceFail()
        {
            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, m_cur_jigsaw_id},
                        { UBSParamKeyName.ContentType, 4},
                        { UBSParamKeyName.Success, 0},
                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pintu_finish, null, _params);

            SCFinishResponse msg = new SCFinishResponse();
            msg.Result = 1;
            WinFailData data = new WinFailData(ENUM_SEARCH_MODE.E_JIGSAW, msg);


            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_WIN)
            {
                Param = data

            });
        }


        /// <summary>
        /// 游戏暂停按钮
        /// </summary>
        /// <param name="btnGamePause"></param>
        private void OnBtnGamePauseClick(GameObject btnGamePause)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            this.m_is_update_time = false;
            CSSuspendRequest pauseRequest = new CSSuspendRequest();
            pauseRequest.PlayerId = GlobalInfo.MY_PLAYER_ID;

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(pauseRequest);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(pauseRequest);
#endif


            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(
                new FrameMgr.OpenUIParams(UIDefine.UI_GAME_MAIN_SETTING)
                {
                    Param = new PauseData()
                    {
                        m_mode = ENUM_SEARCH_MODE.E_JIGSAW,
                        m_id = m_cur_scene_id,
                    }
                });
        }


        private void OnPause(bool val_)
        {
            if (val_)
            {
                this.m_is_update_time = false;
            }
            else
            {
                this.m_is_update_time = true;
            }
        }


        private void OnApplicationPauseHandler(bool isPause)
        {

            if (isPause && this.m_is_update_time)
            {

                OnBtnGamePauseClick(null);
            }
        }

        //private void NetPass()
        //{
        //    if (!m_is_update_time)
        //    {
        //        ForceFail();
        //    }
        //}
    }
}
#endif