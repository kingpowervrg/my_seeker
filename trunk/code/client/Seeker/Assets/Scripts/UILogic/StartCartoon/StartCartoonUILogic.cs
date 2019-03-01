using EngineCore;
using GOGUI;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_COMICS_1)]
    public class StartCartoonUILogic : UILogicBase
    {
        private StartCartoonCapter m_Capter;
        private string resName = string.Empty;
        private string testName = "Comics_Panel_case1.prefab";
        private GameLabel m_pageLable = null;
        private GameImage m_nextBtn = null;
        private GameVideoComponent m_video = null;
        private GameUIComponent m_comicsCom = null;
        private int m_maxPage = 0;
        private int m_curPage = 1;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_comicsCom = Make<GameUIComponent>("comics");
            this.m_pageLable = this.m_comicsCom.Make<GameLabel>("Text_pagenumber");
            this.m_nextBtn = this.m_comicsCom.Make<GameImage>("Image_bg_2");
            this.m_video = Make<GameVideoComponent>("Video");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNext;
            if (param != null)
            {
                testName = param as string;
            }

            if (m_Capter != null)
            {
                GameObject.DestroyImmediate(m_Capter.gameObject);
            }
            resName = testName;
            if (testName.Contains(".mp4"))
            {
                this.m_video.VideoName = testName;
                this.m_video.Visible = true;
                this.m_comicsCom.Visible = false;
                this.m_video.m_playComplete = delegate ()
                {
                    EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_COMICS_1);
                    CSCartoonRewardRequest rewardReq = new CSCartoonRewardRequest();
                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(rewardReq);
                    //GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.SyncTaskDetailInfo(1);
                };
                MessageHandler.RegisterMessageHandler(MessageDefine.SCCartoonRewardReqsponse, OnRes);

                return;
            }
            else
            {
                this.m_video.Visible = false;
                this.m_comicsCom.Visible = true;
            }
            GOGUITools.GetAssetAction.SafeInvoke(testName, (prefabName, obj) =>
            {
                prefabName = prefabName.Replace(".prefab", "");
                GameObject capterObj = obj as GameObject;
                capterObj.transform.localPosition = Vector3.zero;
                capterObj.transform.localScale = Vector3.one;
                capterObj.transform.eulerAngles = Vector3.one;
                capterObj.transform.SetParent(Transform, false);
                capterObj.transform.name = prefabName;
                this.m_maxPage = capterObj.transform.childCount;
                m_Capter = Make<StartCartoonCapter>(prefabName);
                m_Capter.Visible = true;
                this.m_curPage = 1;
                this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
                this.m_pageLable.Visible = true;
            }, LoadPriority.Default);
            this.m_nextBtn.AddClickCallBack(OnNextBtn);
        }

        public override void OnHide()
        {
            base.OnHide();
            this.m_curPage = 1;
            this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNext;
            this.m_nextBtn.RemoveClickCallBack(OnNextBtn);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCartoonRewardReqsponse, OnRes);
            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
            {
                        { UBSParamKeyName.ContentID, StartCartoonManager.Instance.ContextId},
            };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.carton_finish, null, _params);
            if (m_Capter != null)
            {
                m_Capter.Visible = false;
            }
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void OnNext()
        {
            this.m_curPage++;
            this.m_pageLable.Text = string.Format("{0}/{1}", this.m_curPage, this.m_maxPage);
        }

        private void OnNextBtn(GameObject obj)
        {
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextClick.SafeInvoke();
        }

        private void OnRes(object obj)
        {
            if (obj is SCCartoonRewardReqsponse)
            {

            }
        }
    }
}
