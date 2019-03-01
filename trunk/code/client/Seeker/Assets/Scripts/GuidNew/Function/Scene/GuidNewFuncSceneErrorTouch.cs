using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using HedgehogTeam.EasyTouch;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneErrorTouch : GuidNewFunctionBase
    {
        private float m_checkTime = 3f; //检测时间
        private float m_currentTime = 0f;
        private bool m_isRun = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_checkTime = float.Parse(param[0]);
            
        }

        public override void OnExecute()
        {
            base.OnExecute();
            EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnOneFingerTouchup;
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
            //GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
            GuidNewModule.Instance.PushFunction(this);
            this.m_currentTime = Time.time;
            this.m_isRun = true;
        }

        private void OnOneFingerTouchup(Gesture gesture)
        {
            this.m_currentTime = Time.time;
            Debug.Log("sceneTime reclocking");
        }

        private void OnResponse(object obj)
        {
            Debug.Log("sceneTime reclocking");
            if (obj is SCSceneSuspendResponse)
            {
                this.m_currentTime = Time.time;
                this.m_isRun = false;
            }
            else if (obj is SCSceneResumeResponse)
            {
                this.m_currentTime = Time.time;
                this.m_isRun = true;
                Debug.Log("start sceneTime ---");
            }
        }

        //private void OnGameStatusChange(SceneBase.GameStatus status)
        //{
        //    Debug.Log("sceneTime reclocking");
        //    this.m_currentTime = Time.time;
        //    if (status == SceneBase.GameStatus.GAMING)
        //    {
        //        this.m_isRun = true;
        //        Debug.Log("start sceneTime ---");
        //    }
        //    else
        //    {
        //        this.m_isRun = false;
        //    }
        //}

        //private void 

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnOneFingerTouchup;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
            //GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChange;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnOneFingerTouchup;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnOneFingerTouchup;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneResumeResponse, OnResponse);
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            //OnDestory();
            this.m_isRun = false;
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            if (m_isRun)
            {
                if (Time.time - this.m_currentTime >= m_checkTime)
                {
                    OnDestory();
                }
            }
        }
    }
}
