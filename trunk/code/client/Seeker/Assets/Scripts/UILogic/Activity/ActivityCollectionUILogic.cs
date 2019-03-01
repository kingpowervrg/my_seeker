/********************************************************************
	created:  2018-7-25 14:58:51
	filename: ActivityCollectionUILogic.cs
	author:	  songguangze@outlook.com
	
	purpose:  收集性活动UI
*********************************************************************/
using EngineCore;
using SeekerGame;
using System.Collections.Generic;
using UnityEngine;

namespace Seeker
{
    [UILogicHandler(UIDefine.UI_SCENE_RANDOM_COLLECTION_ACTIVITY)]
    public class ActivityCollectionUILogic : UILogicBase
    {
        private GameTexture m_imgActivity = null;
        private GameLabel m_lbActivityName = null;
        private GameLabel m_lbActivityTime = null;
        private ActivityDetailComponent m_activityDetailComponent = null;
        private ActivityRewardComponent m_activityReardComponent = null;
        private ActivityBaseInfo m_activityInfo = null;

        protected override void OnInit()
        {
            this.m_imgActivity = Make<GameTexture>("RawImage");
            this.m_lbActivityName = Make<GameLabel>("Text_title");
            this.m_lbActivityTime = Make<GameLabel>("Text_time");
            this.m_activityDetailComponent = Make<ActivityDetailComponent>("Panel_1:Panel_left");
            this.m_activityReardComponent = Make<ActivityRewardComponent>("Panel_1:Panel_right");
        }

        public override void OnShow(object param)
        {
            if (param == null)
                Debug.LogError("no activity data");

            this.m_activityInfo = param as ActivityBaseInfo;

            MessageHandler.RegisterMessageHandler(MessageDefine.SCActivityDropResponse, OnSyncDropActivityResponse);

            CSActivityRequest requestDropActivityDetail = new CSActivityRequest();
            requestDropActivityDetail.Id = this.m_activityInfo.Id;
#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(requestDropActivityDetail);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(requestDropActivityDetail);
#endif

            this.m_lbActivityTime.Text = $"{CommonTools.TimeStampToDateTime(this.m_activityInfo.StartTime).ToString("yyyy-MM-dd HH:mm:ss")} ~ {CommonTools.TimeStampToDateTime(this.m_activityInfo.EndTime).ToString("yyyy-MM-dd HH:mm:ss")}";

            SetCloseBtnID("Button_close");
        }

        private void OnSyncDropActivityResponse(object message)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCActivityDropResponse, OnSyncDropActivityResponse);
            SCActivityDropResponse msg = message as SCActivityDropResponse;

            //this.m_imgActivity.TextureName = msg.BackgroundSource;

            bool hasCollectAll = false;
            if (msg.HasReward)
                hasCollectAll = true;
            else
            {
                for (int i = 0; i < msg.Items.Count; ++i)
                {
                    PropItem propItem = msg.Items[i];
                    hasCollectAll = propItem.HasNum >= propItem.Num;
                    if (!hasCollectAll)
                        break;
                }
            }

            this.m_lbActivityName.Text = LocalizeModule.Instance.GetString(msg.Name);


            this.m_activityDetailComponent.SetActivityData(msg.Description, msg.Items, msg.SceneId, msg.CollectDes, msg.SceneDes,hasCollectAll);
            this.m_activityReardComponent.SetRewardInfo(this.m_activityInfo.Id, hasCollectAll, msg.Tips, msg.RewardSource);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }



        private class ActivityDetailComponent : GameUIComponent
        {
            private GameLabel m_lbActivityIntro = null;
            private GameLabel m_lbCollectItemTitle = null;
            private GameLabel m_lbSceneTitle = null;

            private GameUIContainer m_collectItemContainer = null;
            private GameUIContainer m_activeSceneContainer = null;

            protected override void OnInit()
            {
                this.m_collectItemContainer = Make<GameUIContainer>("Scroll View_1:Viewport");
                this.m_lbSceneTitle = Make<GameLabel>("Text_title (3)");
                this.m_lbCollectItemTitle = Make<GameLabel>("Text_title (2)");
                this.m_lbActivityIntro = Make<GameLabel>("Text_detail");
                this.m_activeSceneContainer = Make<GameUIContainer>("Scroll View_2:Viewport");
            }

            public void SetActivityData(string activityDesc, IList<PropItem> dropItemList, IList<long> sceneID, string collectItemDesc, string acvititySceneDesc,bool hasReward)
            {
                this.m_lbActivityIntro.Text = LocalizeModule.Instance.GetString(activityDesc);

                this.m_collectItemContainer.EnsureSize<CollectItemComponent>(dropItemList.Count);
                for (int i = 0; i < dropItemList.Count; ++i)
                {
                    CollectItemComponent collectItemInfo = this.m_collectItemContainer.GetChild<CollectItemComponent>(i);
                    if (hasReward)
                        dropItemList[i].HasNum = dropItemList[i].Num;

                    collectItemInfo.SetCollectItemData(dropItemList[i]);
                    collectItemInfo.Visible = true;
                }

                this.m_activeSceneContainer.EnsureSize<ActivitySceneComponent>(sceneID.Count);
                for (int i = 0; i < sceneID.Count; ++i)
                {
                    ActivitySceneComponent sceneInfo = this.m_activeSceneContainer.GetChild<ActivitySceneComponent>(i);
                    sceneInfo.SetSceneID(sceneID[i]);
                    sceneInfo.Visible = true;
                }

                this.m_lbSceneTitle.Text = LocalizeModule.Instance.GetString(acvititySceneDesc);
                this.m_lbCollectItemTitle.Text = LocalizeModule.Instance.GetString(collectItemDesc);
            }


            private class CollectItemComponent : GameUIComponent
            {
                private GameImage m_imgCollectItemIcon = null;
                private GameUIComponent m_isArchieveFlag = null;
                private GameLabel m_lbCurrentCollectNum = null;

                protected override void OnInit()
                {
                    this.m_imgCollectItemIcon = Make<GameImage>("Background:Image");
                    this.m_isArchieveFlag = Make<GameUIComponent>("Image_received");
                    this.m_lbCurrentCollectNum = Make<GameLabel>("Text");
                }

                public void SetCollectItemData(PropItem propItem)
                {
                    this.m_imgCollectItemIcon.Sprite = ConfProp.Get(propItem.Id).icon;
                    this.m_isArchieveFlag.Visible = propItem.HasNum >= propItem.Num;
                    this.m_lbCurrentCollectNum.Text = $"{propItem.HasNum}/{propItem.Num}";
                }


            }

            private class ActivitySceneComponent : GameUIComponent
            {
                private GameTexture m_imgSceneThumb;
                private GameLabel m_lbSceneName;
                private long m_sceneID;

                protected override void OnInit()
                {
                    this.m_imgSceneThumb = Make<GameTexture>("RawImage");
                    this.m_lbSceneName = Make<GameLabel>("Text");
                }

                public override void OnShow(object param)
                {
                    this.m_imgSceneThumb.AddClickCallBack(OnEnterSceneClick);

                }

                public void SetSceneID(long sceneID)
                {
                    this.m_sceneID = sceneID;
                    m_imgSceneThumb.TextureName = ConfScene.Get(sceneID).thumbnail;
                    m_lbSceneName.Text = LocalizeModule.Instance.GetString(ConfScene.Get(sceneID).name);
                }


                private void OnEnterSceneClick(GameObject imgScene)
                {
                    //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENGER_GAME_UI)
                    //{
                    //    Param = this.m_sceneID
                    //});

                    CommonHelper.OpenEnterGameSceneUI(this.m_sceneID);
                    LogicHandler.CloseFrame();

                }

                public override void OnHide()
                {
                    this.m_imgSceneThumb.RemoveClickCallBack(OnEnterSceneClick);
                }


            }
        }

        private class ActivityRewardComponent : GameUIComponent
        {
            private GameButton m_btnReward = null;
            private GameTexture m_imgActivityBg = null;
            private long m_activityID = 0;
            private GameButton m_btnActivityTips = null;
            private string m_activityTips = string.Empty;


            protected override void OnInit()
            {
                base.OnInit();
                this.m_btnReward = Make<GameButton>("btn_receive");
                this.m_imgActivityBg = Make<GameTexture>("RawImage_gift");
                this.m_btnActivityTips = Make<GameButton>("Button_detail");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_btnReward.AddClickCallBack(OnBtnCollectRewardClick);
                this.m_btnActivityTips.AddClickCallBack(OnShowActivityTipClick);
            }

            public void SetRewardInfo(long activityID, bool hasArchieve, string acvitityTips, string activityIcon)
            {
                this.m_btnReward.Enable = hasArchieve;
                this.m_activityID = activityID;
                m_activityTips = LocalizeModule.Instance.GetString(acvitityTips);
                this.m_imgActivityBg.TextureName = activityIcon;
            }

            private void OnBtnCollectRewardClick(GameObject btnCollectReward)
            {
                CSActivityRewardRequest activityReward = new CSActivityRewardRequest();
                activityReward.Id = this.m_activityID;

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(activityReward);
#else
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(activityReward);
#endif



                LogicHandler.CloseFrame();
            }

            private void OnShowActivityTipClick(GameObject btnShowTips)
            {
                PopUpManager.OpenNormalOnePop(this.m_activityTips);
            }

            public override void OnHide()
            {
                base.OnHide();
                this.m_btnReward.RemoveClickCallBack(OnBtnCollectRewardClick);
                this.m_btnActivityTips.RemoveClickCallBack(OnShowActivityTipClick);
            }

        }
    }
}