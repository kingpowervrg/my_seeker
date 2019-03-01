/********************************************************************
	created:  2018-4-4 14:5:35
	filename: EntityBase.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏实体基类
*********************************************************************/

using EngineCore;
using System;
using UnityEngine;

namespace SeekerGame
{
    public abstract class EntityBase
    {
        private string m_entityID = string.Empty;                   //实体GUID
        private EntityType m_entityType = EntityType.None;          //实体类型

        protected object m_entityData = null;                       //实体数据名
        protected string m_assetName = string.Empty;                //实体资源
        protected EntityObjectBase m_entityObject = null;           //实体渲染对象
        protected ResStatus m_assetLoadStatus = ResStatus.NONE;     //资源加载状态
        protected ResStatus m_assetPreloadStatus = ResStatus.NONE;  //资源预加载状态

        public Action OnEntityLoaded;           //实体加载成功

        //实体中断加载回调(实体在加载过程中,中断操作)
        protected Action<EntityBase> OnInterruptLoadEntityCallback;


        //实体位置，旋转，缩放信息
        protected Vector3 m_entityPosition = Vector3.zero;
        protected Vector3 m_entityRotation = Vector3.zero;
        protected Vector3 m_entityScale = Vector3.one;

        /// <summary>
        /// lightmap
        /// </summary>
        protected Texture m_entityLightMap = null;

        public EntityBase(EntityType entityType, string entityId)
        {
            this.m_entityID = entityId;
            this.m_entityType = entityType;
        }

        /// <summary>
        /// 设置资源名称
        /// </summary>
        /// <param name="assetName"></param>
        public void SetAssetName(string assetName)
        {
            this.m_assetName = assetName;
        }

        public void SetLightTexture(Texture tex)
        {
            this.m_entityLightMap = tex;
        }

        /// <summary>
        /// 加载资源对象
        /// </summary>
        /// <param name="resName"></param>
        private void Load(string resName)
        {
            this.m_assetLoadStatus = ResStatus.WAIT;
            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(resName, (assetName, assetObj) =>
            {
                if (assetObj == null)
                {
                    this.m_assetLoadStatus = ResStatus.ERROR;
                    Debug.Log("asset :" + resName + " not found");
                }
                else
                {
                    this.m_assetLoadStatus = ResStatus.OK;
                    OnEntityLoadedResInternel(assetName, assetObj);
                }
            }, LoadPriority.Default);
        }


        /// <summary>
        /// 加载资源对象
        /// </summary>
        public void Load()
        {
            if (this.EntityObject != null)
            {
                OnEntityLoadedResInternel(AssetName, this.m_entityObject.EntityGameObject as UnityEngine.Object);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.m_assetName))
                    Load(this.m_assetName);
            }
        }

        /// <summary>
        /// 预加载实体资源
        /// </summary>
        public void PreloadAsset()
        {
            if (!string.IsNullOrEmpty(this.m_assetName))
            {
                this.m_assetPreloadStatus = ResStatus.WAIT;
                EngineCoreEvents.ResourceEvent.PreloadAssetEvent.SafeInvoke(this.m_assetName, preloadStatus =>
                {
                    if (preloadStatus)
                        this.m_assetPreloadStatus = ResStatus.OK;
                    else
                        this.m_assetPreloadStatus = ResStatus.ERROR;
                });
            }
        }


        /// <summary>
        /// 实体加载回调
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="resObject"></param>
        private void OnEntityLoadedResInternel(string assetName, UnityEngine.Object resObject)
        {
            EntityRawAssetObject = resObject;

            // 中断处理
            if (OnInterruptLoadEntityCallback != null)
            {
                OnInterruptLoadEntityCallback(this);
                OnInterruptLoadEntityCallback = null;
            }
            else
                OnLoadedRes(assetName, resObject);
        }


        /// <summary>
        /// Entity资源加载回调
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="resObject"></param>
        protected abstract void OnLoadedRes(string assetName, UnityEngine.Object resObject);

        public void SetEntityData(object entityData)
        {
            this.m_entityData = entityData;
        }


        public virtual void Update()
        {
            if (m_entityObject != null)
                m_entityObject.Update();
        }

        public virtual void Destory()
        {
            if (m_entityObject != null)
            {
                m_entityObject.Destroy();
                //Debug.LogError("destroy entity :" + this.m_assetName);
                m_entityObject = null;
            }
        }

        /// <summary>
        /// 中断加载实体
        /// </summary>
        /// <param name="OnInterruptCallback"></param>
        public void InterruptLoadEntity(Action<EntityBase> OnInterruptCallback = null)
        {
            if (AssetLoadStatus == ResStatus.WAIT)
            {
                if (OnInterruptLoadEntityCallback == null)
                {
                    OnInterruptLoadEntityCallback = (entity) =>
                    {
                        EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(AssetName, EntityRawAssetObject);
                        OnInterruptLoadEntityCallback = null;
                    };
                }
                else
                    OnInterruptLoadEntityCallback = OnInterruptCallback;
            }
        }

        #region Properties
        public string EntityId
        {
            get { return this.m_entityID; }
        }

        public EntityType EntityType
        {
            get { return this.m_entityType; }
        }

        public EntityObjectBase EntityObject
        {
            get { return this.m_entityObject; }
        }

        public object EntityData
        {
            get { return this.m_entityData; }
        }

        public string AssetName
        {
            get { return this.m_assetName; }
        }

        public Vector3 EntityPosition
        {
            get
            {
                //更新实体位置信息
                if (EntityObject != null)
                    this.m_entityPosition = EntityObject.EntityTransform.position;

                return this.m_entityPosition;
            }
            set
            {
                this.m_entityPosition = value;
                if (EntityObject != null)
                    EntityObject.EntityTransform.position = this.m_entityPosition;
            }
        }

        public Vector3 EntityEulerRotation
        {
            get
            {
                if (EntityObject != null)
                    this.m_entityRotation = EntityObject.EntityTransform.eulerAngles;

                return this.m_entityRotation;
            }
            set
            {
                this.m_entityRotation = value;
                if (EntityObject != null)
                    EntityObject.EntityTransform.eulerAngles = this.m_entityRotation;
            }
        }

        public Vector3 EntityScale
        {
            get
            {
                if (EntityObject != null)
                    this.m_entityScale = EntityObject.EntityTransform.localScale;

                return this.m_entityScale;
            }
            set
            {
                this.m_entityScale = value;
                if (EntityObject != null)
                    EntityObject.EntityTransform.localScale = this.m_entityScale;
            }
        }

        public Vector4 EntityLightInfo
        {
            set
            {
                if (EntityObject != null)
                {
                    EntityObject.SetLightMap(value, this.m_entityLightMap);
                }
            }
        }

        /// <summary>
        /// 实体的可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                return EntityObject != null && EntityObject.Visible;
            }
            set
            {
                if (EntityObject != null && EntityObject.EntityGameObject != null)
                    EntityObject.Visible = value;
            }
        }

        /// <summary>
        /// 资源预加载状态
        /// </summary>
        public ResStatus PreloadStatus => this.m_assetPreloadStatus;

        /// <summary>
        /// 资源加载状态 
        /// </summary>
        public ResStatus AssetLoadStatus => this.m_assetLoadStatus;

        /// <summary>
        /// 实体内存状态
        /// </summary>
        public EntityMemoryStatus EntityMemoryStatus { get; set; }

        /// <summary>
        /// 是否来源于资源池
        /// </summary>
        public bool IsFromPool { get; set; } = false;

        /// <summary>
        /// 实体原始资源文件
        /// </summary>
        public UnityEngine.Object EntityRawAssetObject { get; private set; }

        public EntityLifecycle LifeCycle { get; set; } = EntityLifecycle.AUTO;
        #endregion

    }

    /// <summary>
    /// 实体内存状态
    /// </summary>
    public enum EntityMemoryStatus
    {
        IN_USING,             //使用中
        WAITING_REUSE,        //等待重用
    }

    /// <summary>
    /// 实体生命周期
    /// </summary>
    public enum EntityLifecycle
    {
        AUTO,
        MANUAL
    }
}