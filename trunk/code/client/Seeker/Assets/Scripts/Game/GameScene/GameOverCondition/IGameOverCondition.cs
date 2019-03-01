/********************************************************************
	created:  2019-2-18 11:55:27
	filename: IGameOverCondition.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏结束条件
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public interface IGameOverCondition
    {
        /// <summary>
        /// 是否游戏结束
        /// </summary>
        /// <returns></returns>
        bool IsGameOver();

        /// <summary>
        /// 当前游戏场景
        /// </summary>
        GameSceneBase CurrentScene { get; }

        /// <summary>
        /// 游戏结果
        /// </summary>
        SceneBase.GameResult GameResult { get; }
    }
}
