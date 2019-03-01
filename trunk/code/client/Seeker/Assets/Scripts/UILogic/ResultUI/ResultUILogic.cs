//#define TRY_AGAIN
using EngineCore;
using GOEngine;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_WIN)]
    public class ResultUILogic : BaseViewComponetLogic
    {
        private WinView m_win;
        private LoseView m_fail;

        private List<string> m_exclude_ui = new List<string>();

        private ENUM_SEARCH_MODE m_mode = ENUM_SEARCH_MODE.E_INVALID;
        private WinFailData m_data;

        int m_add_exp;

        private bool m_is_win = false;

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.FULLSCREEN;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();


            m_exclude_ui.Add(UIDefine.UI_WIN);
            m_win = Make<WinView>("Panel_animation");
            m_fail = Make<LoseView>("Panel_animation_failed");
            NeedUpdateByFrame = true;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);


            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(true);

            GameEvents.UI_Guid_Event.OnSceneShowResult += OnSceneShowResult;

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);

            GameEvents.TaskEvents.OnBlockSyncTask.SafeInvoke(true);

            m_is_win = false;

            if (null != param)
            {
                var ret = param as WinFailData;

                this.m_mode = ret.m_mode;
                m_data = ret;
                m_add_exp = 0;

                if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
                {
                    var msg = ret.m_msg as SCFinishResponse;

                    if (0 == msg.Result)
                    {
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.jigsaw_done.ToString());
                        this.ShowWin(ret);

                        if (0 != msg.PropId)
                        {
                            GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(msg.PropId, 1);

                            GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
                        }
                    }
                    else
                    {
                        this.ShowFail(ret);
                    }

                }
                else if (ENUM_SEARCH_MODE.E_CARTOON == this.m_mode)
                {
                    var msg = ret.m_msg as SCCartoonRewardReqsponse;
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.sucess.ToString());
                    this.ShowWin(ret);
                }
                else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
                {
                    InputModule.Instance.Enable = false;
                    var msg = ret.m_msg as SCSceneRewardResponse;
                    GameEvents.UI_Guid_Event.OnFindSceneResult.SafeInvoke(msg.SceneId > 0);
                    if (msg.SceneId > 0)
                    {
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.sucess.ToString());
                        this.ShowWin(ret);

                        foreach (var item in msg.GiftItems)
                        {
                            GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(item.ItemId, item.Num);
                        }

                        GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
                    }
                    else
                    {
                        this.ShowFail(ret);
                    }


                }
            }

        }

        private void OnSceneShowResult()
        {
            m_win.Visible = true;
        }


        public override void OnGuidShow(int type = 0)
        {
            base.OnGuidShow(type);


        }

        public override void OnHide()
        {
            base.OnHide();

            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);

            //GameEvents.UI_Guid_Event.OnSeekOpenClose.SafeInvoke(false);
            GameEvents.UI_Guid_Event.OnSceneShowResult -= OnSceneShowResult;

            GameEvents.TaskEvents.OnBlockSyncTask.SafeInvoke(false);


            if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
            {

                EngineCoreEvents.UIEvent.HideUIWithParamEvent.SafeInvoke(new FrameMgr.HideUIParams(UIDefine.UI_JIGSAW) { DestroyFrameImmediately = true, DestoryFrameDelayTime = 0 });
#if TRY_AGAIN
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENGER_GAME_UI)
                {
                    Param = this.m_rsp_scene_id
                });
#endif
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch.SafeInvoke(1.0f);

                //结算也变为了防止，任务结算提前弹出，在前面组织到任务结算的刷新，所以在这里要补充刷新一次
                if (m_is_win)
                    GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
            }
            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
            {
                //FrameMgr.Instance.HideAllFrames(m_exclude_ui);
                //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_ENGER_GAME_UI);
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch.SafeInvoke(3.0f);
            }
            else if (ENUM_SEARCH_MODE.E_EVENTGAME == this.m_mode)
            {
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_EVENT_INGAME_SCORE);
                GameEvents.UIEvents.UI_GameEntry_Event.OnBlockTaskTouch.SafeInvoke(1.0f);

                //结算也变为了防止，任务结算提前弹出，在前面组织到任务结算的刷新，所以在这里要补充刷新一次
                if (m_is_win)
                    GameEvents.TaskEvents.OnSyncedTaskList.SafeInvoke(1);
            }
            if (GameEvents.PlayerEvents.RequestLatestPlayerInfo != null)
            {
                GameEvents.PlayerEvents.RequestLatestPlayerInfo();
            }

#if MORE_MSG
            GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.SyncTaskDetailInfo(0);
#endif

            //同步背包
            //GlobalInfo.MY_PLAYER_INFO.SyncPlayerBag();
            QuitScene();
        }

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);


        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);
        }

        public override void Update()
        {
            base.Update();

        }

        string m_open_ui_name;
        void QuitScene()
        {
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
                {
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        GameEvents.SceneEvents.OnLeaveScene.SafeInvoke();
                        GameEvents.SceneEvents.OnLeaveSceneComplete.SafeInvoke();
                    }, 1f);

                }
                return;
            }

            if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
            {
                Action act = () =>
                {
                    GameEvents.SceneEvents.OnLeaveScene.SafeInvoke();
                    if (NewGuid.GuidNewNodeManager.Instance.GetNodeStatus(NewGuid.GuidNewNodeManager.ChangeStateToLogin) == NewGuid.NodeStatus.Complete)
                    {
                        SceneModule.Instance.EnterMainScene(m_open_ui_name);
                    }
                    GameEvents.SceneEvents.OnLeaveSceneComplete.SafeInvoke();
                };

                if (!GameEvents.PlayerEvents.OnExpChanged.IsNull && GameEvents.PlayerEvents.OnExpChanged.SafeInvoke(act, m_add_exp))
                {

                }
                else
                {
                    act();
                }
            }
            else
            {
                GameEvents.PlayerEvents.OnExpChanged.SafeInvoke(null, m_add_exp);
            }
        }


        public void Quit(string open_ui_name_ = null)
        {
            this.CloseFrame();
            m_open_ui_name = open_ui_name_;

        }


        private void ShowWin(WinFailData data_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.sucess.ToString());
            m_is_win = true;
            m_win.Refresh(data_);
            m_win.Visible = true;
            m_fail.SetVisible(false);

        }

        private void ShowFail(WinFailData data_)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.fail.ToString());
            m_win.SetVisible(false);
            m_fail.Refresh(data_);
            m_fail.Visible = true;
        }

        public void AddExp(int exp_) { m_add_exp += exp_; }
    }

}