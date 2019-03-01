using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncScenePause : GuidNewFunctionBase
    {
        private bool isPause = true;
        private bool isComplete = false;
        
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.isPause = bool.Parse(param[0]);
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (!isComplete)
            {
                GuidNewModule.Instance.PushFunction(this);
                if (!isPause)
                {
                    OnResume();
                }
                else
                {
                    OnSuspend();
                }
            }
            else
            {
                OnDestory();
            }
            
        }

        private void OnResume()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
            CSSceneResumeRequest resumeReq = new CSSceneResumeRequest();
            resumeReq.PlayerId = GlobalInfo.MY_PLAYER_ID;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(resumeReq);
        }

        private void OnSuspend()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            CSSceneSuspendRequest pauseRequest = new CSSceneSuspendRequest();
            pauseRequest.PlayerId = GlobalInfo.MY_PLAYER_ID;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(pauseRequest);
        }

        private void OnGameOver(SceneBase.GameResult res)
        {
            if (res == SceneBase.GameResult.ALL_ITEM_FOUND)
            {
                this.isComplete = true;
            }
        }

        private void OnResponse(object res)
        {
            if (res is SCSceneResumeResponse && !isPause)
            {
                GuidNewModule.Instance.RemoveFunction(this);
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
                SCSceneResumeResponse resumeMsg = res as SCSceneResumeResponse;
                if (!MsgStatusCodeUtil.OnError(resumeMsg.Result))
                {
                    GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
                }
                OnDestory();
            }
            else if (res is SCSceneSuspendResponse && isPause)
            {
                GuidNewModule.Instance.RemoveFunction(this);
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
                //SCSceneSuspendResponse resumeMsg = res as SCSceneSuspendResponse;
                //if (!MsgStatusCodeUtil.OnError(resumeMsg.Result))
                //{
                //    UnityEngine.Debug.Log("suspend ok ========");
                //}
                OnDestory();
            }

        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            GuidNewModule.Instance.RemoveFunction(this);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
 			MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            GuidNewModule.Instance.RemoveFunction(this);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            GuidNewModule.Instance.RemoveFunction(this);
        }

        float m_tickTime = 0f;
        public override void Tick(float time)
        {
            base.Tick(time);
            this.m_tickTime += UnityEngine.Time.deltaTime;
            if (this.m_tickTime >= 5)
            {
                this.m_tickTime = 0f;
                if (!isComplete)
                {
                    if (!isPause)
                    {
                        OnResume();
                    }
                    else
                    {
                        OnSuspend();
                    }
                }
                else
                {
                    OnDestory();
                }
            }
        }
        //public override void OnDestory()
        //{
        //    base.OnDestory();
        //}
    }
}
