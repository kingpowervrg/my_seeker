using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameOverByTime : IGameOverCondition
    {
        private int m_limitTime = 0;

        public GameOverByTime(int limitTime)
        {
            this.m_limitTime = limitTime;
        }

        public GameSceneBase CurrentScene => SceneModule.Instance.CurrentScene as GameSceneBase;

        public void AddTime(int addSeconds)
        {
            this.m_limitTime += addSeconds;
        }

        public bool IsGameOver()
        {
            return GameTimer.RemainTime <= 0;
        }

        public GameSceneTime GameTimer => CurrentScene.GameTimer;

        public SceneBase.GameResult GameResult
        {
            get
            {
                return IsGameOver() ? SceneBase.GameResult.TIME_OUT : SceneBase.GameResult.NONE;
            }
        }
    }
}