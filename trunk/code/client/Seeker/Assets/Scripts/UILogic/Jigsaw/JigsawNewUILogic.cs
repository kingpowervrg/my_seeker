#define ELAPSE

using DG.Tweening;
using EngineCore;
using GOEngine;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_JIGSAW)]
    public class JigsawUILogic : UILogicBase
    {
        class RewardIcon : GameUIComponent
        {
            private GameImage m_icon;
            private GameLabel m_num_txt;

            public void Refresh(string sprite_, int num_)
            {
                m_icon.Sprite = sprite_;
                m_num_txt.Text = num_.ToString();
            }

            protected override void OnInit()
            {
                base.OnInit();
                m_icon = Make<GameImage>("Image");
                m_num_txt = Make<GameLabel>("Text");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
            }

            public override void OnHide()
            {
                base.OnHide();
            }


        }

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


        private GameUIContainer m_jigsaw_item_container;
        private GameUIComponent m_fly_end_point;

        private GameUIComponent m_time_panel;
        private GameProgressBar m_time_slider;
        private GameImage m_time_slider_img;
        private List<GameUIComponent> m_separator_lines;
        private List<RewardIcon> m_rewards;
        private GameUIComponent m_block_ray_panel;
        private GameLabel m_count_down_time_label;
        private GameButton m_btnPause = null;

        private GameUIEffect m_time_effect;

        private string[,] jigsaw_item_matrix = new string[4, 4];

        public float m_straight_near_dis = 31;// 16.0f;
        private float m_sqrt_straight_near_dis;
        public float m_oblique_near_dis = 24;// 10.0f;
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
        private float m_cur_realtime;
        private bool m_is_update_time;

        private ConfJigsawScene m_scene_data;
        private JigsawGameData m_jigsaw_data;

        private bool m_is_check_over_border;
        private float m_border_right_x;
        private float m_border_left_x;
        private float m_border_up_y;
        private float m_border_down_y;
        private float m_border_width;

        private bool m_isGuid = false;

        private List<float> m_fly_card_idx;

        private Queue<float> m_effect_percents;

        protected override void OnInit()
        {
            base.OnInit();
            NeedUpdateByFrame = true;

            this.m_fly_end_point = this.Make<GameUIComponent>("Panel_animation:EndPoint");
            this.m_jigsaw_item_container = this.Make<GameUIContainer>("Panel_animation:Panel");

            this.m_jigsaw_item_container.EnsureSize<JigsawItem>(1);
            Rect jigsaw_size = this.m_jigsaw_item_container.GetChild<JigsawItem>(0).Widget.rect;
            float r_w = jigsaw_size.width;
            float r_h = jigsaw_size.height;
            m_logic_rect_size = new Vector2(r_w, r_h);

            m_time_panel = Make<GameUIComponent>("Panel_animation:Panel_Time");

            m_rewards = new List<RewardIcon>()
            {
                m_time_panel.Make<RewardIcon>("Image_1"),
                  m_time_panel.Make<RewardIcon>("Image_2"),
                    m_time_panel.Make<RewardIcon>("Image_3"),
                      m_time_panel.Make<RewardIcon>("Image_4"),
            };

            m_separator_lines = new List<GameUIComponent>()
            {
                m_time_panel.Make<GameUIComponent>("Image (1)"),
                m_time_panel.Make<GameUIComponent>("Image (2)"),
                m_time_panel.Make<GameUIComponent>("Image (3)"),
            };

            m_time_slider = m_time_panel.Make<GameProgressBar>("Slider");
            m_time_slider_img = m_time_slider.Make<GameImage>("Fill Area:Fill");
            m_time_effect = m_time_panel.Make<GameUIEffect>("UI_shijiantiao_jianshao");
            m_time_effect.EffectPrefabName = "UI_shijiantiao_jianshao.prefab";

            m_block_ray_panel = this.Make<GameUIComponent>("Panel_animation:Panel_Ray_Block");

            m_count_down_time_label = m_time_panel.Make<GameLabel>("Text");

            m_btnPause = Make<GameButton>("Panel_animation:Button_pause");

            m_sqrt_straight_near_dis = (float)(m_straight_near_dis * m_straight_near_dis);
            m_sqrt_oblique_near_dis = (float)(m_oblique_near_dis * m_oblique_near_dis);
            canvasRectTrans = this.Transform;

            List<float> delta_idx = new List<float>();

            if (0 == m_mx_size % 2)
            {
                for (int i = -m_mx_size / 2; i < 0; ++i)
                {
                    delta_idx.Add(i + 0.5f);
                }

                for (int i = 1; i <= m_mx_size / 2; ++i)
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

            m_effect_percents = new Queue<float>();
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
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
            //if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            //{
            //    m_btnPause.Visible = false;
            //    this.m_isGuid = true;
            //}
            if (this.m_isGuid)
            {
                GameEvents.UI_Guid_Event.OnEnablePause += OnEnablePause;
            }
            this.m_jigsaw_item_container.SetScale(Vector3.one);



            Vector3[] border_bounds = new Vector3[4];
            Vector3[] time_bounds = new Vector3[4];

            TimeModule.Instance.SetTimeout(() =>
            {
                this.m_jigsaw_item_container.Widget.GetWorldCorners(border_bounds);
                this.m_time_slider.Widget.GetWorldCorners(time_bounds);

                m_border_right_x = border_bounds[2].x;
                m_border_left_x = border_bounds[0].x;
                m_border_up_y = time_bounds[0].y;
                m_border_down_y = border_bounds[0].y;
            }, 1.0f);

            m_count_down_time_label.Text = CommonTools.SecondToStringMMSS(this.m_cur_realtime);

            m_time_effect.Visible = false;

            m_time_slider_img.Color = COLOR_100;

            TimeModule.Instance.SetTimeInterval(UpdateTime, 0.5f);

            //TimeModule.Instance.SetTimeInterval(UpdateTimeEffect, 1.0f);



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

            m_time_effect.Visible = false;

            //TimeModule.Instance.RemoveTimeaction(UpdateTimeEffect);
            TimeModule.Instance.RemoveTimeaction(UpdateTime);
        }

        private Color COLOR_100 = new Color(255.0f / 255.0f, 230.0f / 255.0f, 61.0f / 255.0f);
        private Color COLOR_50 = new Color(255.0f / 255.0f, 144.0f / 255.0f, 0);
        private Color COLOR_0 = new Color(255.0f / 255.0f, 0, 0);

        private void UpdateTime()
        {
            if (!this.m_is_update_time)
                return;

            base.Update();

            float time_factor = 1.0f;
            float elapse_time = this.m_total_time - this.m_cur_realtime;

            //Debug.Log("elaspe time = " + elapse_time);

            for (int i = 0; i < m_time_percent_list.Count; ++i)
            {
                if (elapse_time < m_time_percent_list[i])
                {
                    time_factor = m_time_factor_list[i];
                    break;
                }

            }

            //Debug.Log("time factor = " + time_factor);

            this.m_cur_slider_time -= 0.5f * time_factor;
            this.m_cur_realtime -= 0.5f;

            if (this.m_cur_realtime > 0)
            {
                float scaler = this.m_cur_slider_time / this.m_total_time;


                //m_time_slider.Value = scaler;

                TweenSlider.BeginTween(m_time_slider.gameObject, scaler, 0.4f);

                //100：ffe63d   50：ff9000   0：ff0000

                if (scaler > 0.5f)
                {
                    //Debug.Log($"颜色系数 {(scaler - 0.5f) * 2.0f}");
                    m_time_slider_img.Color = Color.Lerp(COLOR_50, COLOR_100, (scaler - 0.5f) * 2.0f);
                }
                else
                {
                    //Debug.Log($"颜色系数 {scaler * 2.0f}");
                    m_time_slider_img.Color = Color.Lerp(COLOR_0, COLOR_50, scaler * 2.0f);
                }
                m_count_down_time_label.Text = CommonTools.SecondToStringMMSS(this.m_cur_realtime);


                UpdateTimeEffect();

            }
            else
            {
                this.m_is_update_time = false;
                this.SendFinish(false);
            }
        }

        //public override void Update()
        //{
        //    if (!this.m_is_update_time)
        //        return;

        //    base.Update();


        //    this.m_cur_time -= Time.deltaTime;

        //    if (this.m_cur_time > 0)
        //    {
        //        float scaler = this.m_cur_time / this.m_total_time;


        //        m_time_slider.Value = scaler;

        //        //100：ffe63d   50：ff9000   0：ff0000

        //        if (scaler > 0.5f)
        //        {
        //            m_time_slider_img.Color = Color.Lerp(COLOR_100, COLOR_50, (scaler - 0.5f) * 2.0f);
        //        }
        //        else
        //        {
        //            m_time_slider_img.Color = Color.Lerp(COLOR_50, COLOR_0, scaler * 2.0f);
        //        }
        //        m_count_down_time_label.Text = CommonTools.SecondToStringMMSS(this.m_cur_time);




        //    }
        //    else
        //    {
        //        this.m_is_update_time = false;
        //        this.SendFinish(false);
        //    }


        //}


        private void UpdateTimeEffect()
        {
            if (!this.m_is_update_time)
                return;

            float scaler = this.m_cur_slider_time / this.m_total_time;

            if (m_effect_percents.Count > 0 && scaler < m_effect_percents.Peek())
            {
                m_effect_percents.Dequeue();
                m_time_effect.Visible = false;
                m_time_effect.Visible = true;
                CommonHelper.EffectProgressbarValueSync(m_time_slider, m_time_effect);
            }
        }

        private void OnEnablePause(bool isPause)
        {
            m_btnPause.Visible = isPause;
        }

        private bool IsBlocked(JigsawItem item_, Vector3 offset_, out Vector3 could_offset_)
        {
            bool ret = false;
            float temp_x;
            float temp_y;
            could_offset_ = offset_;

            var jigsaw = item_;
            Vector3[] icon_bounds = new Vector3[4];

            float precision = 0.01f;

            foreach (var icon in jigsaw.m_child_icon_list)
            {
                icon.Widget.GetWorldCorners(icon_bounds);

                if (offset_.x > 0.0f)
                {
                    float icon_right_x = icon_bounds[2].x + offset_.x;

                    if (icon_right_x >= m_border_right_x)
                    {
                        temp_x = m_border_right_x - icon_bounds[2].x - precision;
                        if (Mathf.Abs(temp_x) < Mathf.Abs(could_offset_.x))
                            could_offset_.x = temp_x;

                        ret = true;
                    }
                }

                if (offset_.x < 0.0f)
                {
                    float icon_left_x = icon_bounds[0].x + offset_.x;

                    if (icon_left_x <= m_border_left_x)
                    {
                        temp_x = m_border_left_x - icon_bounds[0].x + precision;
                        if (Mathf.Abs(temp_x) < Mathf.Abs(could_offset_.x))
                            could_offset_.x = temp_x;

                        ret = true;
                    }
                }


                if (offset_.y > 0.0f)
                {
                    float icon_up_y = icon_bounds[2].y + offset_.y;

                    if (icon_up_y > m_border_up_y)
                    {
                        temp_y = m_border_up_y - icon_bounds[2].y - precision;
                        if (Mathf.Abs(temp_y) < Mathf.Abs(could_offset_.y))
                            could_offset_.y = temp_y;

                        ret = true;
                    }
                }

                if (offset_.y < 0.0f)
                {
                    float icon_down_y = icon_bounds[0].y + offset_.y;

                    if (icon_down_y < m_border_down_y)
                    {
                        temp_y = m_border_down_y - icon_bounds[0].y + precision;
                        if (Mathf.Abs(temp_y) < Mathf.Abs(could_offset_.y))
                            could_offset_.y = temp_y;

                        ret = true;
                    }
                }

            }

            return ret;
        }


        private void AddUILogic()
        {
            m_scene_data = ConfJigsawScene.Get(this.m_cur_scene_id);
            m_jigsaw_data = JigsawDataManager.Instance.GetJigsaw(this.m_cur_jigsaw_id);

            this.InitTime();
            this.RefreshJigsaws();
            ShowJigsaws(false);
            TimeModule.Instance.SetTimeout(() => { ShowJigsaws(true); this.FlyCard(); }, 0.6f);
        }

        private void ShowJigsaws(bool v_)
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    JigsawItem JItem = this.m_jigsaw_item_container.GetChild<JigsawItem>(i * 4 + j);

                    JigsawChipJson chip = m_jigsaw_data.GetChip(i, j);

                    if (null != chip)
                    {
                        JItem.Visible = v_;
                    }
                    else
                    {
                        JItem.Visible = false;
                    }

                }

            }
        }

        private void RemoveUILogic()
        {
            this.StopAllShine();
        }

        private TweenScale m_ts;
        WinFailData result_data;
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

                    GoToCenter();
                    result_data = new WinFailData(ENUM_SEARCH_MODE.E_JIGSAW, rsp);
                    m_ts = TweenScale.BeginTween(this.m_jigsaw_item_container.gameObject, C_WIN_SCALE, 1.5f);
                    m_ts.SetTweenCompletedCallback(() =>
                    {
                        m_btnPause.Visible = true;

                        FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_WIN);
                        param.Param = result_data;


                        EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                    });


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
                        { UBSParamKeyName.TotalTime, m_total_time - m_cur_realtime},
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
                    if (this.m_cur_realtime < 0)
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

        private void GoToCenter()
        {
            //<计算每个图片左上角和右下角的坐标，选出边界，算出整个图片当前的中心位置
            Vector3 left_top = Vector3.zero;
            Vector3 right_bottom = Vector3.zero;
            bool init = false;
            for (int i = 0; i < m_jigsaw_item_container.ChildCount; ++i)
            {
                var item = m_jigsaw_item_container.GetChild<JigsawItem>(i);

                if (item.m_child_icon_list.Count <= 1)
                    continue;

                foreach (var icon in item.m_child_icon_list)
                {

                    Vector3[] my_corners = new Vector3[4];
                    icon.Texture.Widget.GetWorldCorners(my_corners);

                    //Debug.Log($"左上角坐标 {my_corners[1]}");
                    //Debug.Log($"右下角坐标 {my_corners[3]} ---");


                    if (!init)
                    {
                        left_top = my_corners[1];
                        right_bottom = my_corners[3];
                        init = true;
                    }
                    else
                    {
                        if (my_corners[1].x < left_top.x)
                        {
                            left_top.x = my_corners[1].x;
                        }

                        if (my_corners[1].y > left_top.y)
                        {
                            left_top.y = my_corners[1].y;
                        }

                        if (my_corners[3].x > right_bottom.x)
                        {
                            right_bottom.x = my_corners[3].x;
                        }

                        if (my_corners[3].y < right_bottom.y)
                        {
                            right_bottom.y = my_corners[3].y;
                        }
                    }
                }
            }

            //Debug.Log($"最终左上角坐标 {left_top}");
            //Debug.Log($"最终右下角坐标 {right_bottom} ---");

            float pic_center_world_x = (left_top.x + right_bottom.x) / 2;
            float pic_center_world_y = (left_top.y + right_bottom.y) / 2;


            //Debug.Log($"图片中心坐标 {pic_center_world_x} , {pic_center_world_y} ---");

            float x_offset = m_fly_end_point.Widget.gameObject.transform.position.x - pic_center_world_x;
            float y_offset = m_fly_end_point.Widget.gameObject.transform.position.y - pic_center_world_y;
            Vector3 center_offset = new Vector3(x_offset, y_offset, 0.0f);

            for (int i = 0; i < m_jigsaw_item_container.ChildCount; ++i)
            {
                var item = m_jigsaw_item_container.GetChild<JigsawItem>(i);

                if (item.m_child_icon_list.Count <= 1)
                    continue;

                foreach (var icon in item.m_child_icon_list)
                {
                    icon.Texture.gameObject.transform.position += center_offset;
                }
                //item.Widget.gameObject.transform.position += center_offset;
            }
        }





        private void InitTime()
        {
            m_is_update_time = false;


#if ELAPSE
            m_cur_realtime = m_cur_slider_time = m_total_time = m_scene_data.percent4;
#else
            m_cur_slider_time = m_total_time = 300.0f;
#endif


        }


        private void RefreshJigsaws()
        {
            m_time_slider.Value = 1.0f;

            //this.RefreshSeparatorLine();
            this.CalcFactorBetweenRealTimeToSliderTime();
            List<int> reward_count = new List<int>
            {
                this.m_scene_data.num1, //15
            this.m_scene_data.num2, //4
            this.m_scene_data.num3,//3
            this.m_scene_data.num4,//1
            };

            //进度条从右到左，反的
            for (int i = reward_count.Count - 1, j = 0; i >= 0 && j < m_rewards.Count; --i, ++j)
            {
                this.m_rewards[j].Refresh(this.GetRewardIcon(i), reward_count[i]);
            }

            this.m_jigsaw_item_container.Clear();
            this.m_jigsaw_item_container.EnsureSize<JigsawItem>(16);

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
            float delta_t = 0.1f;

            Vector3 endPointWorldPos = this.m_fly_end_point.gameObject.transform.position;
            //Debug.Log($"card end position {endPointWorldPos}");
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
                //Debug.Log($"card position {item.gameObject.transform.position}");
                Vector3 offset = item.m_child_icon_list[0].gameObject.transform.position - item.gameObject.transform.position;
                //Debug.Log($"card  offset  {offset}");
                item.gameObject.transform.SetAsLastSibling();

                int i_x = i % m_mx_size;
                int i_y = i / m_mx_size;

                float idx_x = m_fly_card_idx[i_x];
                float idx_y = -m_fly_card_idx[i_y];

                Vector3 final_aim_pos = endPointWorldPos + new Vector3(idx_x * max_delta_world_dis_x, idx_y * max_delta_world_dis_y, 1.0f);


                TweenPosition tp = TweenPosition.BeginTween(item.gameObject, final_aim_pos - offset, 0.5f + i * delta_t, true);
                tp.SetBuildinEase(Ease.InOutQuad);


                Quaternion random_r = Random.rotationUniform;
                Vector3 v_r = random_r.eulerAngles;
                v_r = new Vector3(0.0f, 0.0f, v_r.z);
                random_r = Quaternion.Euler(v_r);
                item.gameObject.transform.rotation = random_r;
                TweenRotationEuler tr = TweenRotationEuler.BeginTween(item.gameObject, Vector3.zero, 0.5f + i * delta_t);
                tr.SetBuildinEase(Ease.InOutQuad);

                cost_time = 1.0f + i * delta_t;

                ++i;

                temp_list.Remove(item);
            }

            m_is_first_pick = true;
            m_block_ray_panel.SetActive(false);

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

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.jigsaw_click.ToString());
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
                            if (this.IsNear(move_end_logic_rect, cur_logic_rect, neighbours[cur_name], out offset))
                            {
                                if (this.IsIconNear(moved_dp_, judge_dp, neighbours[cur_name]))
                                {
                                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.jigsaw_putdone.ToString());
                                    this.MoveAndCombine(moved_dp_, judge_dp, offset);
                                    return;
                                }


                            }
                        }
                    }

                }
            }

            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.jigsaw_put.ToString());


        }

        private Vector3 IsCanDragg(Vector3 offset_)
        {
            if (null == m_moving_item)
                return Vector3.zero;

            Vector3 could_offset;

            this.IsBlocked(m_moving_item, offset_, out could_offset);

            return could_offset;
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

        }

        private void MoveAndCombine(JigsawItem move_end_dp_, JigsawItem neighbour_, Vector2 offset)
        {
            //Vector2 aim_anchor_pos = move_end_dp_.Widget.anchoredPosition + offset;
            //Vector3 aim_world_pos;
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTrans, aim_anchor_pos, CameraManager.Instance.UICamera, out aim_world_pos);
            //aim_world_pos = new Vector3(aim_world_pos.x, aim_world_pos.y, 0.0f);

            Vector3[] wayPoint = new Vector3[2];
            wayPoint[0] = move_end_dp_.Position;
            move_end_dp_.Widget.anchoredPosition += offset;
            wayPoint[1] = move_end_dp_.Position;
            move_end_dp_.Widget.anchoredPosition -= offset;

            move_end_dp_.Widget.DOPath(wayPoint, 0.1f, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.InQuad).OnComplete(() =>
             {
                 this.Combine(move_end_dp_, neighbour_);
                 this.IsFinish();
             });


            // move_end_dp_.Widget.anchoredPosition += offset;
            //this.Combine(move_end_dp_, neighbour_);
            //this.IsFinish();
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
            combine_.m_child_icon_list.Clear();

            m_drag_able_parents_list.Remove(combine_.gameObject.name);

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

        private List<int> m_time_percent_list;
        private List<float> m_time_factor_list;

        private void CalcFactorBetweenRealTimeToSliderTime()
        {
            ConfJigsawScene data = this.m_scene_data;
            float slider_time = data.percent4 / 4.0f;
            float realtime1 = data.percent1;
            float realtime2 = data.percent2 - data.percent1;
            float realtime3 = data.percent3 - data.percent2;
            float realtime4 = data.percent4 - data.percent3;

            float factor1 = slider_time / realtime1;
            float factor2 = slider_time / realtime2;
            float factor3 = slider_time / realtime3;
            float factor4 = slider_time / realtime4;

            m_time_percent_list = new List<int>()
            {
                 data.percent1,
                 data.percent2,
                 data.percent3,
                 data.percent4,
            };
            m_time_factor_list = new List<float>()
            {
                factor1,
                factor2,
                factor3,
                factor4,
            };

            m_effect_percents.Clear();

            for (int i = 3; i >= 1; --i)
            {
                m_effect_percents.Enqueue(0.25f * i);
            }
        }



        private void RefreshSeparatorLine()
        {
            Vector3[] time_slider_world_corners = new Vector3[4];
            m_time_slider.Widget.GetWorldCorners(time_slider_world_corners);
            float slider_width = time_slider_world_corners[2].x - time_slider_world_corners[0].x;

            ConfJigsawScene data = this.m_scene_data;

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

            percents.Reverse();

            //<进度条是从右向左减少，所以进度需要反着来
            for (int i = 0; i < percents.Count; ++i)
            {
                percents[i] = 100.0f - percents[i];
            }
            percents.Add(100.0f);
            percents.RemoveAt(0);
            //>


            m_effect_percents.Clear();

            for (int i = percents.Count - 2; i >= 0; --i)
            {
                m_effect_percents.Enqueue(percents[i] / 100.0f);
            }
#endif

            for (int i = 0, j = 0; i < this.m_separator_lines.Count && j < percents.Count; ++j, ++i)
            {
                float line_x = time_slider_world_corners[0].x + slider_width * percents[j] / 100.0f;
                m_separator_lines[i].gameObject.transform.position = new Vector3(line_x, m_separator_lines[i].gameObject.transform.position.y, m_separator_lines[i].gameObject.transform.position.z);

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

            for (int i = 0; i < this.m_rewards.Count; ++i)
            {
                float percent = 0f;
                for (int j = 0; j < i; ++j)
                {
                    percent += delta_percents[j];
                }

                float scaler = (percent + delta_percents[i] / 2.0f) / 100.0f;
                float line_x = time_slider_world_corners[0].x + slider_width * scaler;

                m_rewards[i].gameObject.transform.position = new Vector3(line_x, m_rewards[i].gameObject.transform.position.y, m_rewards[i].gameObject.transform.position.z);
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
            GameEvents.UIEvents.UI_Jigsaw_Event.OnJigsawFinish.SafeInvoke(is_win_);
            if (is_win_)
            {
                CSFinishRequest req = new CSFinishRequest();
                req.SceneId = m_cur_scene_id;
                req.AllTime = (int)m_total_time;
                req.RestTime = (int)m_cur_realtime;
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