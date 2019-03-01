/********************************************************************
	created:  2018-4-10 12:36:25
	filename: EntityManager.cs
	author:	  songguangze@outlook.com
	
	purpose:  实体管理器
    remark:
        update 2018-6-27 10:45:30：
        实体管理器全局持有,切场景时对实体管理器进行清理
*********************************************************************/
using EngineCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeekerGame
{
    public class EntityManager : Singleton<EntityManager>
    {
        //对象池大小
        private const int ENTITY_POOL_SIZE = 100;

        private Dictionary<EntityType, List<EntityBase>> m_entityDict = new Dictionary<EntityType, List<EntityBase>>();

        private Dictionary<string, EntityBase> m_allEntityDict = new Dictionary<string, EntityBase>();

        //实体内存池
        public Dictionary<string, MemoryPool<EntityBase>> m_entityPool = new Dictionary<string, MemoryPool<EntityBase>>();

        private Dictionary<string, string> m_entityShader = new Dictionary<string, string>();

        private Dictionary<string, string> m_hintShader = new Dictionary<string, string>();
        public EntityManager()
        {
            InitEntityShader();
            InitHintShader();
        }

        private void InitEntityShader()
        {
            m_entityShader.Add("Mobile/Diffuse", "Seeker/Exhibit/Diffuse");
            m_entityShader.Add("Legacy Shaders/Transparent/Diffuse", "Seeker/Exhibit/Alpha");
            m_entityShader.Add("Legacy Shaders/Diffuse", "Seeker/Exhibit/Diffuse");
            m_entityShader.Add("Legacy Shaders/Transparent/Cutout/Diffuse", "Seeker/Exhibit/Cutoff");
        }

        private void InitHintShader()
        {
            m_hintShader.Add("Seeker/Exhibit/Diffuse", "Seeker/Exhibit/OutLineEdge");
            m_hintShader.Add("Seeker/Exhibit/Alpha", "Seeker/Exhibit/OutLineEdge");
            m_hintShader.Add("Seeker/Exhibit/Cutoff", "Seeker/Exhibit/OutLineEdge");
        }

        public string GetEntityShader(string srcShader)
        {
            if (m_entityShader.ContainsKey(srcShader))
            {
                return m_entityShader[srcShader];
            }
            return string.Empty;
        }

        public string GetHintShader(string srcShader)
        {
            if (m_hintShader.ContainsKey(srcShader))
            {
                return m_hintShader[srcShader];
            }
            return string.Empty;
        }

        public void InitEntityManager()
        {
            this.EntityManagerRoot = new GameObject("EntityManager");
        }

        /// <summary>
        /// 获取实体类型获取实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public T GetEntityByEntityId<T>(string entityId) where T : EntityBase
        {
            if (this.m_allEntityDict.ContainsKey(entityId))
            {
                EntityBase entity = m_allEntityDict[entityId];
                return entity as T;
            }

            return null;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(EntityBase entity)
        {
            if (!entity.IsFromPool)
            {
                if (!this.m_entityDict.ContainsKey(entity.EntityType))
                    m_entityDict.Add(entity.EntityType, new List<EntityBase>());

                this.m_entityDict[entity.EntityType].Add(entity);
                this.m_allEntityDict.Add(entity.EntityId, entity);
            }
        }

        /// <summary>
        /// 一出实体
        /// </summary>
        /// <param name="entityId"></param>
        public void RemoveEntity(string entityId)
        {
            if (m_allEntityDict.ContainsKey(entityId))
            {
                EntityBase entity = m_allEntityDict[entityId];
                m_allEntityDict.Remove(entityId);

                List<EntityBase> entityListWithEntityType = m_entityDict[entity.EntityType];
                entityListWithEntityType.Remove(entity);

                entity = null;
            }
        }

        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="isRemove"></param>
        public void DestroyEntity(string entityId, bool isRemove = true)
        {
            EntityBase destroyEntity = GetEntityByEntityId<EntityBase>(entityId);
            destroyEntity?.Destory();

            if (isRemove)
                RemoveEntity(entityId);
        }

        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="destroyEntity"></param>
        public void DestroyEntity(EntityBase destroyEntity, bool isRemove = true)
        {
            destroyEntity?.Destory();
            if (isRemove)
                RemoveEntity(destroyEntity.EntityId);
        }


        /// <summary>
        /// Tick
        /// </summary>
        public void Update()
        {
            foreach (KeyValuePair<string, EntityBase> pair in m_allEntityDict)
                if (pair.Value.EntityMemoryStatus == EntityMemoryStatus.IN_USING)
                    pair.Value.Update();
        }

        /// <summary>
        /// 销毁实体管理器
        /// </summary>
        public void DestroyEntityManager()
        {
            string[] entityIds = m_allEntityDict.Keys.ToArray();
            for (int i = 0; i < entityIds.Length; ++i)
            {
                DestroyEntity(entityIds[i], true);
            }

            this.m_allEntityDict.Clear();
            this.m_entityDict.Clear();
            this.m_entityPool.Clear();

            //GameObject.Destroy(EntityManagerRoot);
        }

        /// <summary>
        /// 释放实体管理器(释放自动管理的实体)
        /// </summary>
        public void ReleaseEntityManager()
        {
            string[] entityIds = m_allEntityDict.Keys.ToArray();
            for (int i = 0; i < entityIds.Length; ++i)
            {
                EntityBase releaseEntity = m_allEntityDict[entityIds[i]];
                if (releaseEntity.LifeCycle == EntityLifecycle.AUTO)
                {
                    DestroyEntity(releaseEntity, true);

                    if (this.m_entityPool.ContainsKey(releaseEntity.AssetName))
                        this.m_entityPool.Remove(releaseEntity.AssetName);
                }
            }

            //实体管理释放尝试GC
            EngineCoreEvents.ResourceEvent.TryGCCacheEvent.SafeInvoke();

            //this.m_allEntityDict.Clear();
            //this.m_entityDict.Clear();
            //this.m_entityPool.Clear();
        }


        /// <summary>
        /// 获取指定种类实体列表
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public List<EntityBase> GetEntityListByEntityType(EntityType entityType)
        {
            if (this.m_entityDict.ContainsKey(entityType))
                return m_entityDict[entityType];

            return null;
        }

        /// <summary>
        /// 获取所有实体对象
        /// </summary>
        /// <returns></returns>
        public List<EntityBase> GetAllEntityList()
        {
            return this.m_allEntityDict.Values.ToList();
        }

        /// <summary>
        /// 回收实体
        /// </summary>
        /// <param name="freeEntity"></param>
        public void FreeEntity(EntityBase freeEntity)
        {
            if (freeEntity.EntityObject == null)
                DestroyEntity(freeEntity.EntityId, true);
            else
            {
                freeEntity.Visible = false;
                freeEntity.EntityMemoryStatus = EntityMemoryStatus.WAITING_REUSE;

                string freeEntityAssetName = freeEntity.AssetName;

                MemoryPool<EntityBase> entityMemoryPool = null;
                if (!this.m_entityPool.TryGetValue(freeEntityAssetName, out entityMemoryPool))
                {
                    entityMemoryPool = new MemoryPool<EntityBase>(ENTITY_POOL_SIZE);
                    this.m_entityPool[freeEntityAssetName] = entityMemoryPool;
                }

                if (entityMemoryPool.Free(freeEntity))
                {
                    //已经被释放
                    if (EntityManagerRoot != null)
                        freeEntity.EntityObject.SetEntityParent(EntityManagerRoot.transform);
                }
                else
                    DestroyEntity(freeEntity.EntityId, true);
            }
        }

        /// <summary>
        /// 分配实体
        /// </summary>
        /// <param name="assetName"></param>
        public EntityBase AllocEntity(EntityType entityType, string assetName = "")
        {
            EntityBase entity = null;
            if (!string.IsNullOrEmpty(assetName))
            {
                MemoryPool<EntityBase> memoryPool = null;
                if (this.m_entityPool.TryGetValue(assetName, out memoryPool))
                {
                    entity = memoryPool.Alloc();
                    if (entity == null)
                        entity = CreateEntity(entityType, assetName);
                    else
                        entity.IsFromPool = true;
                }
                else
                {
                    memoryPool = new MemoryPool<EntityBase>(ENTITY_POOL_SIZE);
                    this.m_entityPool.Add(assetName, memoryPool);
                    entity = CreateEntity(entityType, assetName);
                }
            }
            else
                entity = CreateEntity(entityType, assetName);

            AddEntity(entity);

            entity.EntityMemoryStatus = EntityMemoryStatus.IN_USING;

            return entity;
        }

        /// <summary>
        /// 分配实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityType"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T AllocEntity<T>(EntityType entityType, string assetName = "") where T : EntityBase
        {
            T entity = AllocEntity(entityType, assetName) as T;

            return entity;
        }


        /// <summary>
        /// 创建新的实体
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private EntityBase CreateEntity(EntityType entityType, string assetName = "")
        {
            EntityBase entity = EntityFactory.Instance.CreateEntity(entityType);
            entity.EntityMemoryStatus = EntityMemoryStatus.IN_USING;
            if (!string.IsNullOrEmpty(assetName))
                entity.SetAssetName(assetName);

            return entity;
        }

        /// <summary>
        /// 实体管理器根节点
        /// </summary>
        public GameObject EntityManagerRoot { get; private set; }
    }
}