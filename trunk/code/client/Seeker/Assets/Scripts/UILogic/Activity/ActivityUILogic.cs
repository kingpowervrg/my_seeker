using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public enum ActivityUIType
    {
        Start,
        Quit,
        Choose,
        None
    }

    [UILogicHandler(UIDefine.UI_ACTIVITY)]
    public class ActivityUILogic : UILogicBase
    {
        private int m_img_Hei = 410;
        private float m_img_Width = 240;
        private float m_startPosX = 0;

        private GameButton m_close_btn;
        private GameUIComponent m_scroll_view;
        private GameUIContainer m_grid_con;
        private GridValue m_gridValue;
        private GameImage m_bg_img;
        private Transform m_leftPoint_tran;
        private GameUIEffect m_GameEffect;

        private List<ActivityComponent> coms = new List<ActivityComponent>();

        private Vector3 m_startPos = Vector3.zero;
        private Vector3 m_endPos = Vector3.zero;
        private ActivityUIType m_uiType = ActivityUIType.None;

        private float m_delayTime = 0.2f;
        private float m_chooseQuitTime = 0f;
        private float m_normalQuitTime = 0f;

        private float m_screenWidth;

        private int m_startIndex = 0;
        private int m_endIndex = -1;

        private List<ActivityBaseInfo> m_activity_data = new List<ActivityBaseInfo>();

        private bool m_startComplete = false;
        private void InitController()
        {
            m_leftPoint_tran = Transform.Find("Panel_animation/leftPoint");
            m_close_btn = Make<GameButton>("Button_close");
            m_scroll_view = Make<GameUIComponent>("Panel_animation:Panel");
            m_grid_con = Make<GameUIContainer>("Panel_animation:Panel:grid");
            m_bg_img = Make<GameImage>("RawImage");
            m_GameEffect = Make<GameUIEffect>("UI_huodong_rukou");
            m_gridValue = m_grid_con.GetComponent<GridValue>();
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_startComplete = false;
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.activity_in, 1.0f, null);

            GameEvents.UIEvents.UI_Activity_Event.OnTweenFinish += OnTweenFinish;
            GameEvents.UIEvents.UI_Activity_Event.OnClick += OnClick;
            GameEvents.UIEvents.UI_Activity_Event.OnChooseFinish += OnChooseFinish;

            DragEventTriggerListener.Get(m_scroll_view.gameObject).onDragStart += OnDragStart;


            MessageHandler.RegisterMessageHandler(MessageDefine.SCActivityBaseListResponse, OnResponse);
            m_screenWidth = m_leftPoint_tran.localPosition.x * 2;
            m_startPosX = m_leftPoint_tran.localPosition.x;
            m_close_btn.AddClickCallBack(OnClose);
            m_delayTime = m_gridValue.m_delayTime;
            m_chooseQuitTime = m_gridValue.m_chooseQuitTime;
            m_normalQuitTime = m_gridValue.m_normalQuitTime;
            m_GameEffect.EffectPrefabName = "UI_huodong_rukou.prefab";
            CSActivityBaseListRequest req = new CSActivityBaseListRequest();
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }
        public override void OnHide()
        {
            base.OnHide();
            this.m_startComplete = false;
            GameEvents.UIEvents.UI_Activity_Event.OnTweenFinish -= OnTweenFinish;
            GameEvents.UIEvents.UI_Activity_Event.OnChooseFinish -= OnChooseFinish;
            GameEvents.UIEvents.UI_Activity_Event.OnClick -= OnClick;
            DragEventTriggerListener.Get(m_scroll_view.gameObject).onDragStart -= OnDragStart;

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCActivityBaseListResponse, OnResponse);
            m_close_btn.RemoveClickCallBack(OnClose);
            m_uiType = ActivityUIType.None;
            m_startIndex = 0;
            m_endIndex = 0;
            m_activity_data.Clear();

        }


        private void OnInitPanel()
        {
            coms.Clear();
            //List<ConfActivityBase> activitys = m_activity_data;
            int count = m_activity_data.Count;
            float gridWidth = count * m_img_Width + (count - 1) * m_gridValue.m_spacing + m_gridValue.m_paddingLeft + m_gridValue.m_paddingRight;
            if (count == 0)
            {
                gridWidth = 0;
            }
            m_grid_con.Widget.sizeDelta = new Vector2(gridWidth, m_grid_con.Widget.sizeDelta.y);
            m_grid_con.X = 0;
            m_grid_con.Clear();
            m_startPosX = m_startPosX - gridWidth / 2f;
            if (m_startPosX < 0)
            {
                m_startPosX = 0;
            }
            m_grid_con.EnsureSize<ActivityComponent>(count);
            RectTransform templateObj = m_grid_con.ContainerTemplate.GetComponent<RectTransform>();
            //RectTransform templateRect = temp
            m_startPos = new Vector3(m_screenWidth + 10, templateObj.anchoredPosition.y, 0);
            m_endPos = new Vector3(-(m_img_Width + 20), templateObj.anchoredPosition.y, 0);
            for (int i = 0; i < count; i++)
            {
                ActivityComponent com = m_grid_con.GetChild<ActivityComponent>(i);
                Vector3 comPos = com.Widget.anchoredPosition;
                com.Widget.anchoredPosition = m_startPos + m_img_Width * Vector3.right;
                //com.gameObject.transform.localPosition = m_startPos + m_img_Width * Vector3.right;
                Vector3 targetPos = new Vector3((m_img_Width + m_gridValue.m_spacing) * i + m_gridValue.m_paddingLeft + m_startPosX + m_img_Width / 2f, comPos.y, comPos.z);
                if (i > m_endIndex)
                {
                    com.Widget.anchoredPosition = targetPos;
                    //com.gameObject.transform.localPosition = targetPos;
                }
                com.SetData(m_activity_data[i], targetPos, i, m_delayTime);
                com.Visible = true;
                coms.Add(com);
            }
        }

        private void OnTweenFinish(int index)
        {
            if (m_uiType == ActivityUIType.Start || m_uiType == ActivityUIType.Quit)
            {
                index++;
                if (index <= m_endIndex && index < coms.Count)
                {
                    //Debug.Log("index = " + index);
                    coms[index].PlayTween();
                }
                else
                {
                    if (m_uiType == ActivityUIType.Quit)
                    {
                        EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ACTIVITY);
                    }
                    else
                    {
                        TimeModule.Instance.SetTimeout(() =>
                        {
                            this.m_startComplete = true;
                            Debug.Log("move over ===");
                        }, 0.2f);

                    }
                }
            }
            else if (m_uiType == ActivityUIType.Choose)
            {
                int nextIndex = coms[index].GetNextIndex();
                if (nextIndex >= 0)
                {
                    coms[nextIndex].PlayTween();
                }
                else
                {
                    if (coms[index].GetChooseState())
                    {
                        coms[index].PlayChooseEffect();
                    }
                }

            }

        }

        private void OnClick(int index)
        {
            if (!this.m_startComplete)
            {
                return;
            }
            Vector3 startPos = m_startPos - Vector3.right * (m_grid_con.X - m_img_Width / 2f);
            Vector3 endPos = m_endPos - Vector3.right * (m_grid_con.X + m_img_Width / 2f);
            CheckGameView();
            SetChooseEndIndex(m_startIndex, index, endPos);
            if (index < m_activity_data.Count - 1)
            {
                SetChooseEndIndex(index + 1, m_endIndex + 1, startPos);
            }

            Vector3 centerPos = startPos;//(startPos + endPos) / 2f + Vector3.right * m_img_Width / 2f;
            centerPos.x = m_screenWidth / 2f;// - m_img_Width / 2f;
            coms[index].SetEndPos(centerPos, -1);
            coms[index].SetChooseState(true);
            m_uiType = ActivityUIType.Choose;
            if (m_startIndex != index)
            {
                coms[m_startIndex].PlayTween();
            }
            if (index + 1 < coms.Count)
            {
                coms[index + 1].PlayTween();
            }
            coms[index].SetDuration(m_chooseQuitTime);
            coms[index].PlayTween();
            //coms[index].PlayTween();
        }

        private void SetChooseEndIndex(int startIndex, int endIndex, Vector3 pos)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i != endIndex - 1)
                {
                    coms[i].SetEndPos(pos, i + 1);
                }
                else
                {
                    coms[i].SetEndPos(pos, -1);
                }
                coms[i].SetDuration(m_normalQuitTime);
            }
        }

        private void OnClose(GameObject obj)
        {
            if (!this.m_startComplete)
            {
                return;
            }
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.activity_open.ToString());

            m_uiType = ActivityUIType.Quit;
            if (coms.Count == 0)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ACTIVITY);
            }
            else
            {
                CheckGameView();
                Vector3 endPos = new Vector3(m_endPos.x - m_grid_con.X, m_endPos.y, m_endPos.z);
                for (int i = 0; i < coms.Count; i++)
                {
                    coms[i].SetEndPos(endPos);
                }
                if (coms.Count > 0)
                {
                    coms[m_startIndex].PlayTween();
                }
            }

        }

        private bool IsInGameView(ActivityComponent com)
        {
            float posX = com.X + m_grid_con.X;
            if (posX + m_img_Width > 0 && posX < m_screenWidth)
            {
                return true;
            }
            return false;

        }

        private void CheckGameView()
        {
            bool isFirst = false;
            for (int i = 0; i < coms.Count; i++)
            {
                if (IsInGameView(coms[i]))
                {
                    if (!isFirst)
                    {
                        m_startIndex = i;
                        isFirst = true;
                    }
                }
                else
                {
                    if (isFirst)
                    {
                        m_endIndex = i;
                        break;
                    }
                }

            }
        }

        private void GetInitLastIndex()
        {
            float contentWidth = m_screenWidth - m_gridValue.m_paddingLeft;
            int totalNum = Mathf.FloorToInt((contentWidth + m_gridValue.m_spacing) / (m_img_Width + m_gridValue.m_spacing));
            m_endIndex = Mathf.Min(totalNum, m_activity_data.Count - 1);

        }

        private void OnChooseFinish(int index)
        {
            if (index > m_activity_data.Count - 1)
            {
                Debug.Log("activity error");
                return;
            }
            ActivityBaseInfo activityBase = m_activity_data[index];
            //ConfActivityBase activityBase = ConfActivityBase.Get(index + 1);
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_ACTIVITY);
            if (activityBase != null)
            {
                if (0 == activityBase.TargetType)
                {
                    string[] targetEntryPrefab = activityBase.TargetPrefab.Split(',');
                    string targetPrefab = targetEntryPrefab[0];
                    if (activityBase.Stage == 1)
                        targetPrefab = targetEntryPrefab[1];

                    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(targetPrefab);
                    param.Param = activityBase;
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                }
            }

        }

        private void OnDragStart(GameObject go, Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > 75.0f)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.activity_change.ToString());
            }
        }

        private void OnResponse(object obj)
        {
            if (obj is SCActivityBaseListResponse)
            {

                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.activity_open.ToString());

                SCActivityBaseListResponse res = obj as SCActivityBaseListResponse;
                for (int i = 0; i < res.BaseInfos.Count; i++)
                {
                    m_activity_data.Add(res.BaseInfos[i]);
                }
                GetInitLastIndex();
                OnInitPanel();
                m_uiType = ActivityUIType.Start;
                if (coms.Count > 0)
                {
                    coms[0].PlayTween();
                }
            }
        }

    }


}
