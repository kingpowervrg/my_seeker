/********************************************************************
	created:  2018-4-8 10:18:5
	filename: GameSceneTime.cs
	author:	  songguangze@fotoable.com
	
	purpose:  游戏场景内时间的逻辑
*********************************************************************/
using EngineCore;
using SeekerGame.NewGuid;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameSceneTime
    {
        private readonly float m_gameTotalTime;
        private float m_gameRemainTime = 0f;

        public float ElaspeTime
        {
            get { return m_gameTotalTime - m_gameRemainTime; }
        }

        public float RemainTime => m_gameRemainTime;


        private int m_errorTouchCounter = 0;
        private SceneBase.GameStatus m_currentGameStatus = SceneBase.GameStatus.NONE;
        private float m_totalAddTime = 0;

        private int m_punishTimes = 0;

        private float m_lastErrorTimeStamp = 0;

        private List<GameStatusChangedRecord> m_gameTimeRecordList = new List<GameStatusChangedRecord>();

        public GameSceneTime(float sceneTotalTime)
        {
            this.m_gameTotalTime = sceneTotalTime;
            this.m_gameRemainTime = sceneTotalTime;
            this.m_rimTime = 0f;
            this.m_propTipsTime = 0f;
            this.m_arrowTipsTime = 0f;
            m_forcePropHintTime = 0f;
            GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChanged;

            GameEvents.MainGameEvents.OnErrorTouch += OnErrorTouch;
            GameEvents.MainGameEvents.AddGameTime += AddGameTime;
            GameEvents.MainGameEvents.OnResetFingerTips += OnResetFingerTips;
            GameEvents.MainGameEvents.OnResetArrowTipsTime += OnResetArrowTipsTime;
            //GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneObject;
            GameEvents.MainGameEvents.OnSceneClick += OnSceneClick;
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnGamePauseResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCSceneReconnectResponse, OnReconnectResponse);
        }

        /// <summary>
        /// 游戏状态改变
        /// </summary>
        /// <param name="gameStatus"></param>
        private void OnGameStatusChanged(SceneBase.GameStatus gameStatus)
        {
            this.m_currentGameStatus = gameStatus;
            this.m_gameTimeRecordList.Add(new GameStatusChangedRecord()
            {
                GameStatus = this.m_currentGameStatus,
                GameStatusChangedTime = TimeModule.GameRealTime.RealTime
            });
        }

        /// <summary>
        /// 游戏暂停回调
        /// </summary>
        /// <param name="messageResponse"></param>
        private void OnGamePauseResponse(object messageResponse)
        {
            SCSceneSuspendResponse msg = messageResponse as SCSceneSuspendResponse;
            if (msg.Result == 0)
            {
                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.PAUSE);
            }
        }

        private void OnReconnectResponse(object messageResponse)
        {
            SCSceneReconnectResponse msg = messageResponse as SCSceneReconnectResponse;
            if (!MsgStatusCodeUtil.OnError(msg.ResponseStatus))
            {
                GameEvents.MainGameEvents.OnGameStatusChange.SafeInvoke(SceneBase.GameStatus.GAMING);
            }
        }


        /// <summary>
        /// 错误操作
        /// </summary>
        /// <param name="gameRealTime"></param>
        private void OnErrorTouch(float gameRealTime, Vector2 touchScreenPoint)
        {
            if (this.m_currentGameStatus == SceneBase.GameStatus.GAMING)
            {
                if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.ForbidErrorSecond) == NodeStatus.None)
                {
                    return;
                }
                if (this.m_errorTouchCounter <= 5)
                {
                    if (gameRealTime - m_lastErrorTimeStamp <= GameConst.PUNISH_THRESHOLD)
                        this.m_errorTouchCounter++;
                    else
                        this.m_errorTouchCounter = 0;
                }
                else
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.error.ToString());
                    this.m_errorTouchCounter = 0;
                    this.m_punishTimes++;
                    this.m_gameRemainTime -= GameConst.PUNISH_TIME;
                    GameEvents.MainGameEvents.OnPunish.SafeInvoke(GameConst.PUNISH_TIME, touchScreenPoint, this.m_gameRemainTime);
                }

                m_lastErrorTimeStamp = gameRealTime;
            }
        }

        private void OnResetFingerTips()
        {
            this.m_rimTime = 0f;
        }

        private void OnSceneClick(Vector2 postion)
        {
            this.m_propTipsTime = 0f;
            //Debug.Log("click scene =====");
        }

        private float m_rimTime = 0f;
        private float m_propTipsTime = 0f;
        private float m_arrowTipsTime = 0f;
        private float m_forcePropHintTime = 0f;
        //private int m_rimNum = 0;

        public void Tick(float deltaTime)
        {
            if (this.m_currentGameStatus == SceneBase.GameStatus.GAMING)
            {
                this.m_gameRemainTime -= deltaTime;
                this.m_rimTime += deltaTime;
                if (!GameMainHelper.Instance.hasForcePropTips)
                {
                    this.m_propTipsTime += deltaTime;
                    this.m_arrowTipsTime += deltaTime;
                }
                m_forcePropHintTime += deltaTime;
                if (this.m_gameRemainTime <= 0)
                {
                    this.m_gameRemainTime = 0f;
                    this.m_propTipsTime = 0f;
                    this.m_rimTime = 0f;
                    this.m_arrowTipsTime = 0f;
                    this.m_forcePropHintTime = 0f;
                    GameEvents.MainGameEvents.OnGameTimeTick.SafeInvoke(0);
                    // GameTimeOut();
                }
                else
                {
                    if (this.m_rimTime >= 3f)  //手指提示
                    {
                        this.m_rimTime = 0f;
                        List<SceneItemEntity> sceneItem = GameEvents.MainGameEvents.GetSceneItemEntityList.SafeInvoke(1);
                        if (sceneItem.Count > 0 && SeekerGame.NewGuid.GuidNewNodeManager.Instance.GetNodeStatus(SeekerGame.NewGuid.GuidNewNodeManager.SceneTips) == NewGuid.NodeStatus.Complete)
                        {
                            GameEvents.SceneEvents.OnSceneExhibitTips.SafeInvoke(sceneItem[0], true);
                        }
                    }
                    if (this.m_propTipsTime >= 8f) //道具刷流光
                    {
                        //Debug.Log("prop use tips =====");
                        this.m_propTipsTime = 0f;
                        GameEvents.MainGameEvents.OnPropUseTips.SafeInvoke(GameMainHelper.GetCurrentPropTips(), 0, 0);
                    }
                    if (this.m_forcePropHintTime >= 10f)
                    {
                        this.m_forcePropHintTime = 0f;
                        GameEvents.MainGameEvents.OnPropUseTips.SafeInvoke(0, 2, 0);
                    }
                    if (this.m_arrowTipsTime >= 10f) //箭头提示
                    {
                        this.m_arrowTipsTime = 0f;
                        GameEvents.MainGameEvents.OnPropUseTips.SafeInvoke(GameMainHelper.GetCurrentPropTips(), 1, 7);
                    }
                    //todo 临时修改

                    GameEvents.MainGameEvents.OnGameTimeTick.SafeInvoke(Mathf.FloorToInt(this.m_gameRemainTime));
                }

            }
        }

        private void AddGameTime(bool isTime, float time)
        {
            if (isTime)
            {
                m_totalAddTime += time;
                this.m_gameRemainTime += time;
            }
            else
            {
                this.m_gameRemainTime += this.m_gameRemainTime * (time / 100f);
            }
        }

        /// <summary>
        /// 时间到 游戏结束
        /// </summary>
        private void GameTimeOut()
        {
            OnGameStatusChanged(SceneBase.GameStatus.GAMEOVER);
            GameEvents.MainGameEvents.OnGameOver.SafeInvoke(SceneBase.GameResult.TIME_OUT);
        }

        public void Dispose()
        {
            GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChanged;
            GameEvents.MainGameEvents.AddGameTime -= AddGameTime;
            GameEvents.MainGameEvents.OnErrorTouch -= OnErrorTouch;
            //GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneObject;
            GameEvents.MainGameEvents.OnSceneClick -= OnSceneClick;
            GameEvents.MainGameEvents.OnResetFingerTips -= OnResetFingerTips;
            GameEvents.MainGameEvents.OnResetArrowTipsTime -= OnResetArrowTipsTime;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneSuspendResponse, OnGamePauseResponse);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSceneReconnectResponse, OnReconnectResponse);
        }

        private void OnResetArrowTipsTime()
        {
            this.m_arrowTipsTime = 0f;
        }

        /// <summary>
        /// 游戏状态改变记录
        /// </summary>
        private class GameStatusChangedRecord
        {
            public float GameStatusChangedTime;
            public SceneBase.GameStatus GameStatus;
        }


        public int PunishTimes => this.m_punishTimes;

        public float GameCostTime => this.m_gameTotalTime + m_totalAddTime - this.m_gameRemainTime;

    }
}