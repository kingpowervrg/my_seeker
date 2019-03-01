/********************************************************************
	created:  2018-4-19 12:8:7
	filename: SceneFactory.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景工厂
              id: 00001  普通玩法     
                  10001  迷雾玩法
                  20001  黑天玩法
                   1 普通
                   2 反词
                   3 剪影

                例: 11001  迷雾普通
                    12001  迷雾反词
                    13001  迷雾剪影
*********************************************************************/
using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class SceneFactory : Singleton<SceneFactory>
    {
        private Dictionary<SceneMode, Type> m_sceneDict = new Dictionary<SceneMode, Type>();

        public SceneFactory()
        {
            m_sceneDict.Add(SceneMode.NORMALGAME, typeof(NormalGame));
            m_sceneDict.Add(SceneMode.FOGGY, typeof(FoggyGame));
            m_sceneDict.Add(SceneMode.DARKER, typeof(DarkerGame));
            m_sceneDict.Add(SceneMode.NORMALSCENE, typeof(NormalScene));
            m_sceneDict.Add(SceneMode.NORMALTALK, typeof(NormalTalkScene));
            m_sceneDict.Add(SceneMode.EXHITITIONHALL, typeof(ExhititionHallScene));
        }

        /// <summary>
        /// 创建指定类型的Scene
        /// </summary>
        /// <param name="sceneMode"></param>
        public SceneBase CreateScene(SceneMode sceneMode, int sceneId = -1)
        {
            Type targetSceneType = null;
            if (this.m_sceneDict.TryGetValue(sceneMode, out targetSceneType))
            {
                SceneBase scene = null;
                if (sceneId != -1)
                    scene = Activator.CreateInstance(targetSceneType, sceneId) as SceneBase;
                else
                    scene = Activator.CreateInstance(targetSceneType) as SceneBase;

                return scene;
            }

            return null;
        }
    }

    /// <summary>
    /// 场景类型
    /// </summary>
    public enum SceneMode
    {
        NORMALGAME = 0,     //标准模式
        FOGGY = 1,      //迷雾模式
        DARKER = 2,      //黑天
        NORMALTALK = 3,     //对话场景
        EXHITITIONHALL = 4, //展厅
        NORMALSCENE = 10,     //标准场景
        
    }

    /// <summary>
    /// 寻物模式类型
    /// </summary>
    public enum SceneItemFindingMode
    {
        Once,
        OneItemMultipleTime
    }

    /// <summary>
    /// 游戏结束条件限制
    /// </summary>
    public enum GameOverCondition
    {
        TIME_LIMIT,
        STEPS_LIMIT,
    }
}