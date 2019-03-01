/********************************************************************
	created:  2018-4-4 13:57:12
	filename: SceneBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景抽象类
*********************************************************************/
using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public abstract class SceneBase
    {
        protected SceneMode m_sceneMode = SceneMode.NORMALSCENE;
        protected Dictionary<EntityType, List<EntityBase>> m_sceneEntityDict = new Dictionary<EntityType, List<EntityBase>>();

        //场景的背景音乐
        public AudioSource m_sceneBGMAudioSource = null;
        private string m_sceneBGMPath = string.Empty;

        //缓存所有场景实体列表
        private List<EntityBase> m_cachedSceneEntityList = new List<EntityBase>();

        //场景内的实体列表是否Dirty
        private bool m_isSceneEntityListDirty = true;

        public SceneBase(SceneMode sceneMode)
        {
            this.m_sceneMode = sceneMode;

            GameEvents.SceneEvents.AddLazyEffectToScene += AddLazyEffect;
            GameEvents.SceneEvents.RemoveEntity += RemoveSceneEntity;
        }

        /// <summary>
        /// 添加场景实体(实体由场景持有)
        /// </summary>
        /// <param name="entityBase"></param>
        protected void AddSceneEntity(EntityBase sceneEntity)
        {
            EntityType entityType = sceneEntity.EntityType;
            List<EntityBase> entityListWithSameType = null;
            if (!this.m_sceneEntityDict.TryGetValue(entityType, out entityListWithSameType))
            {
                entityListWithSameType = new List<EntityBase>();
                this.m_sceneEntityDict[entityType] = entityListWithSameType;
            }

            if (entityListWithSameType.Contains(sceneEntity))
                Debug.LogWarning($"add entity duple entity type:{entityType.ToString()} , entity id:{sceneEntity.EntityId}");
            else
            {
                entityListWithSameType.Add(sceneEntity);
                this.m_isSceneEntityListDirty = true;
            }
        }

        /// <summary>
        /// 移除场景实体 (实体管理器中并不销毁)
        /// </summary>
        /// <param name="entityBase"></param>
        protected void RemoveSceneEntity(EntityBase entityBase)
        {
            EntityType entityType = entityBase.EntityType;
            List<EntityBase> entityListWithSameType = this.m_sceneEntityDict.ContainsKey(entityType) ? this.m_sceneEntityDict[entityType] : null;
            if (entityListWithSameType == null)
                Debug.LogWarning($"remove entity from scene error ,no scene entity type:{entityType.ToString()}");
            else
            {
                if (!entityListWithSameType.Remove(entityBase))
                    Debug.LogWarning($"remove entity from scene error , scene not keep entity {entityBase.EntityId}");
                else
                    this.m_isSceneEntityListDirty = true;
            }

        }


        public virtual void Update()
        {

        }

        /// <summary>
        /// Scene销毁
        /// </summary>
        public virtual void DestroyScene()
        {
            EntityManager.Instance.ReleaseEntityManager();

            GameEvents.SceneEvents.AddLazyEffectToScene -= AddLazyEffect;
            GameEvents.SceneEvents.RemoveEntity -= RemoveSceneEntity;
            //GameEvents.SceneEvents.FreeEntityFromScene -= FreeEntityFromScene;

            ResourceModule.Instance.RemoveAllAsset();
        }

        private void DestoryEntityFromScene(EntityBase entityBase)
        {
            entityBase.Visible = false;
            EntityManager.Instance.DestroyEntity(entityBase.EntityId);
        }


        /// <summary>
        /// 向场景添加特效
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="effectLifeTime"></param>
        /// <returns></returns>
        public EntityBase AddEffect(string effectName, Vector3 position, Vector3 rotation, Vector3 scale, float effectLifeTime)
        {
            EffectEntity effectEntity = EntityManager.Instance.AllocEntity<EffectEntity>(EntityType.Effect, effectName); ;
            effectEntity.EntityPosition = position;
            effectEntity.EntityEulerRotation = rotation;
            effectEntity.EntityScale = scale;
            effectEntity.Load();

            AddSceneEntity(effectEntity);

            return effectEntity;
        }

        /// <summary>
        /// 向场景添加特效实体(LazyLoad)
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public EffectEntity AddLazyEffect(string effectName)
        {
            EffectEntity effectEntity = EntityManager.Instance.AllocEntity<EffectEntity>(EntityType.Effect, effectName);

            AddSceneEntity(effectEntity);

            return effectEntity;
        }

        /// <summary>
        /// 播放场景背景音乐
        /// </summary>
        public void PlaySceneBGM(string sceneBGMPath)
        {
            SceneBGMPath = sceneBGMPath;
            //if (this.m_sceneBGMAudioSource == null)
            //{
            //    PlayAudioInfo bgmAudioInfo = new PlayAudioInfo();
            //    bgmAudioInfo.Name = sceneBGMPath;
            //    bgmAudioInfo.Loop = true;
            //    bgmAudioInfo.RemoveAtEnd = false;
            //    bgmAudioInfo.Type = EngineCore.AudioType.AUDIO_TYPE_BGM;

            //    this.m_sceneBGMAudioSource = EngineCoreEvents.AudioEvents.AddAndPlayMusic.SafeInvoke(bgmAudioInfo);
            //}
            //else
            //{
            //    if (!this.m_sceneBGMAudioSource.isPlaying)
            //        this.m_sceneBGMAudioSource.Play();
            //}

            GlobalInfo.Instance.PlayMusic(sceneBGMPath);
        }

        public void PlaySceneBGM()
        {
            if (!string.IsNullOrEmpty(SceneBGMPath))
                PlaySceneBGM(SceneBGMPath);
        }

        /// <summary>
        /// 停止场景音乐
        /// </summary>
        /// <param name="isFade"></param>
        public void StopAndRemoveSceneBGM(bool isFade)
        {
            if (this.m_sceneBGMAudioSource != null)
                EngineCoreEvents.AudioEvents.StopAudio.SafeInvoke(Audio.AudioType.Music);

            this.m_sceneBGMAudioSource = null;
        }

        public SceneMode SceneMode
        {
            get { return this.m_sceneMode; }
        }

        public List<EntityBase> AllSceneEntities
        {
            get
            {
                if (this.m_isSceneEntityListDirty)
                {
                    foreach (KeyValuePair<EntityType, List<EntityBase>> pair in this.m_sceneEntityDict)
                        this.m_cachedSceneEntityList.AddRange(pair.Value);

                    this.m_isSceneEntityListDirty = false;
                }

                return this.m_cachedSceneEntityList;
            }
        }

        /// <summary>
        /// 场景背景音乐路径(目前一个场景持有一个背景音乐)
        /// </summary>
        public string SceneBGMPath
        {
            get { return this.m_sceneBGMPath; }
            private set { this.m_sceneBGMPath = value; }
        }


        /// <summary>
        /// 游戏状态枚举
        /// </summary>
        public enum GameStatus
        {
            NONE,
            GAMESTART,      //游戏开局
            PAUSE,          //暂停
            GAMING,         //游戏中
            GAMEOVER,       //游戏结束
        }

        /// <summary>
        /// 游戏结果
        /// </summary>
        /// <remarks>
        ///  0~127   失败
        ///  128~255 成功
        /// </remarks>
        public enum GameResult : byte
        {
            NONE,
            TIME_OUT = 1,               //超时
            RUN_OUT_STEPS,              //步数耗尽

            ALL_ITEM_FOUND = 128,       //找到所有物品
        }
    }


}