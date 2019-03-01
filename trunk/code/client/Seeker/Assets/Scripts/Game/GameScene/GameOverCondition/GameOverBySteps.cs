/********************************************************************
	created:  2019-2-18 11:55:27
	filename: GameOverBySteps.cs
	author:	  songguangze@outlook.com
	
	purpose:  步数限制
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class GameOverBySteps : IGameOverCondition
    {
        private int m_limitSteps = 0;

        public GameOverBySteps(int limitSteps)
        {
            this.m_limitSteps = limitSteps;
        }


        public GameSceneBase CurrentScene => SceneModule.Instance.CurrentScene as GameSceneBase;

        public SceneBase.GameResult GameResult
        {
            get
            {
                return IsGameOver() ? SceneBase.GameResult.RUN_OUT_STEPS : SceneBase.GameResult.NONE;
            }
        }

        public bool IsGameOver()
        {
            return m_limitSteps < CurrentScene.TotalTouchCount;
        }


        /// <summary>
        /// 增加步数
        /// </summary>
        /// <param name="addStep"></param>
        public void AddSteps(int addStep)
        {
            this.m_limitSteps += addStep;
        }

        public int RemainStep => m_limitSteps - CurrentScene.TotalTouchCount;
    }
}