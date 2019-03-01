/********************************************************************
	created:  2018-4-26 11:38:26
	filename: GameUIEffect.cs
	author:	  songguangze@outlook.com
	purpose:  UI特效组件
    remark：所有特效必须由场景承载
    update :
        2018-6-27 13:32:5 - guangze song:
        UI特效组件的销毁不受EntityManager管理，由于UI特效在主界面，跟随界面的生命周期，
        可能遇到频繁切换场景和主UI之前的情况所以UI特效生命周期由UI控制，
*********************************************************************/
using EngineCore;
using System;
using UnityEngine;

namespace SeekerGame
{
    public class GameUIEffect : GameUIComponent
    {
        protected EffectEntity m_effectEntity;
        protected string m_effectPrefabName;
        protected float m_effectScale = 1.0f;
        protected Vector2 m_realScale = Vector2.one;
        bool maskable;
        private bool m_fitUIScale = false;

        protected int m_renderQueue = -1;
        private UIEffectScale m_effectScaleComponent;

        private string m_sortingLayerName = "Default";

        public Action OnPlayEffect;

        protected override void OnInit()
        {
            UIEffectCanvas uiEffectRootCanvas = gameObject.GetComponent<UIEffectCanvas>();
            if (uiEffectRootCanvas)
            {
                this.m_renderQueue = uiEffectRootCanvas.CustomSortOrder;
                //GameObject.DestroyImmediate(uiEffectRootCanvas);
            }

            Canvas uiEffectCanvas = gameObject.GetComponent<Canvas>();
            if (uiEffectCanvas != null)
            {
                this.m_renderQueue = uiEffectCanvas.sortingOrder;
                GameObject.DestroyImmediate(uiEffectCanvas);
            }
        }

        public void SetRenderQueue(int queue)
        {
            this.m_renderQueue = queue;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (this.m_effectEntity == null || IsEffectEntityAvaliable(EffectPrefabName))
            {
                if (!string.IsNullOrEmpty(EffectPrefabName))
                    PlayEffect();
            }
            else
            {
                OnPlayEffect?.Invoke();
                this.m_effectEntity.Visible = true;
            }
        }

        public void SetScale(float scale = 0.5f, int renderqueue = -1)
        {
            this.m_effectScale = scale;

            if (this.m_effectScaleComponent != null && this.m_effectEntity != null && this.m_effectEntity.EntityObject != null)
            {
                this.m_effectScaleComponent.Scale = scale;
                this.m_effectScaleComponent.SetSortingOrder(renderqueue != -1 ? renderqueue : this.m_renderQueue);
            }
        }

        public void SetSortingLayer(string sortingLayerName)
        {
            this.m_sortingLayerName = sortingLayerName;
            if (this.m_effectScaleComponent != null && this.m_effectEntity != null && this.m_effectEntity.EntityObject != null)
                this.m_effectScaleComponent.SetSortingLayerName(sortingLayerName);

        }

        public void SetRealScale(Vector3 scale)
        {
            this.m_realScale = scale;
            if (this.m_effectScaleComponent != null && this.m_effectEntity != null && this.m_effectEntity.EntityObject != null)
            {
                this.m_effectScaleComponent.RealScale = scale;
            }
        }

        public void SetRotation(Vector3 rotation)
        {
            this.m_effectEntity.EntityEulerRotation = rotation;
        }

        public void Refresh()
        {
            if (m_effectScaleComponent != null)
                m_effectScaleComponent.Dirty = true;
        }

        /// <summary>
        /// 重新播放特效
        /// </summary>
        public void ReplayEffect()
        {
            if (this.m_effectEntity != null)
            {
                if (Visible)
                    Visible = false;

                Visible = true;
            }
        }

        public void SetEffectHideTime(float time)
        {
            TimeModule.Instance.SetTimeout(SetEffectHide, time);
        }

        private void SetEffectHide()
        {
            Visible = false;
        }

        /// <summary>
        /// 播放UI特效
        /// </summary>
        private void PlayEffect()
        {
            if (this.m_effectEntity != null)
            {
                if (this.m_effectEntity.AssetLoadStatus == ResStatus.WAIT && this.m_effectEntity.AssetName != EffectPrefabName)
                {
                    this.m_effectEntity.InterruptLoadEntity();
                    this.m_effectEntity = null;

                    LoadEffectEntityInternal();
                }
            }
            else
            {
                LoadEffectEntityInternal();
            }
        }

        private void LoadEffectEntityInternal()
        {
            this.m_effectEntity = EntityManager.Instance.AllocEntity<EffectEntity>(EntityType.Effect, EffectPrefabName);
            this.m_effectEntity.OnEntityLoaded = OnEffectEntityLoaded;
            this.m_effectEntity.LifeCycle = EntityLifecycle.MANUAL;

            this.m_effectEntity.Load();
        }



        private void OnEffectEntityLoaded()
        {
            if (Widget == null)
            {
                EntityManager.Instance.FreeEntity(this.m_effectEntity);
                this.m_effectEntity = null;
                //GameEvents.SceneEvents.FreeEntityFromScene.SafeInvoke(this.m_effectEntity);
            }
            else
            {
                this.m_effectEntity.EntityObject.EntityTransform.SetParent(Widget, false);
                this.m_effectEntity.EntityObject.EntityTransform.localPosition = Vector3.zero;
                this.m_effectEntity.EntityScale = Vector3.one;
                this.m_effectEntity.EntityObject.Layer = LayerDefine.UIShowLayer;
                if (ClearRotation)
                {
                    this.m_effectEntity.EntityObject.EntityTransform.localEulerAngles = Vector3.zero;
                }
                m_effectScaleComponent = this.m_effectEntity.EntityObject.EntityGameObject.GetOrAddComponent<UIEffectScale>();
                m_effectScaleComponent.Scale = m_effectScale;
                m_effectScaleComponent.RealScale = m_realScale;
                m_effectScaleComponent.SetSortingOrder(m_renderQueue);
                m_effectScaleComponent.SetSortingLayerName(m_sortingLayerName);
                FitUIScale = true;

                OnPlayEffect?.Invoke();

                LogicHandler.UIFrame.SetFrameDirty();

                this.m_effectEntity.Visible = Visible;
            }
        }

        public bool ClearRotation = false;

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }

            set
            {
                base.Visible = value;
                if (this.m_effectEntity != null)
                    this.m_effectEntity.Visible = value;
            }
        }


        public override void OnHide()
        {
            base.OnHide();
            TimeModule.Instance.RemoveTimeaction(SetEffectHide);
            if (this.m_effectEntity != null)
                this.m_effectEntity.Visible = false;

            //this.m_effectEntity = null;
            //DisposeEffectEntity();
        }


        public override void Dispose()
        {
            TimeModule.Instance.RemoveTimeaction(SetEffectHide);
            //base.Dispose();
            if (m_effectScaleComponent != null)
                m_effectScaleComponent.Scale = 1;

            //this.m_effectEntity = null;
            DisposeEffectEntity();
        }

        private void DisposeEffectEntity()
        {
            if (m_effectEntity != null)
            {
                //GameEvents.SceneEvents.FreeEntityFromScene.SafeInvoke(this.m_effectEntity);
                //EntityManager.Instance.FreeEntity(this.m_effectEntity);
                //手动销毁
                this.m_effectEntity.InterruptLoadEntity();
                EntityManager.Instance.DestroyEntity(this.m_effectEntity.EntityId);
            }

            this.m_effectEntity = null;
        }

        public string EffectPrefabName
        {
            set
            {
                if (!IsEffectEntityAvaliable(value))
                {
                    this.m_effectPrefabName = value;
                    if (Visible)
                        PlayEffect();
                }
                else
                    OnPlayEffect?.Invoke();
            }
            get
            {
                return this.m_effectPrefabName;
            }
        }


        public EffectEntity Entity
        {
            get { return this.m_effectEntity; }
        }


        /// <summary>
        /// 是否适应UI缩放
        /// </summary>
        public bool FitUIScale
        {
            get { return this.m_fitUIScale; }
            set
            {
                this.m_fitUIScale = value;
                if (value)
                {
                    float designRatio = (float)GameConst.REFERENCE_RESOLUTION_X / GameConst.REFERENCE_RESOLUTION_Y;
                    float currentRatio = FrameMgr.Instance.ScreenAspectRatio;
                    if (currentRatio < designRatio)
                        SetScale(currentRatio / designRatio);
                }
            }
        }

        private bool IsEffectEntityAvaliable(string effectName)
        {
            if (this.m_effectEntity != null)
            {


                if (this.m_effectEntity.EntityObject != null || this.m_effectEntity.AssetLoadStatus <= ResStatus.OK)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }


        /*
                public bool Maskable
                {
                    get
                    {
                        return maskable;
                    }
                    set
                    {
                        maskable = value;
                        if (actor != null && actor.GameObject)
                        {
                            effScale.Maskable = value;
                        }
                    }
                }

                private float yaw, pitch, roll;
                public void SetRotation(float raw, float pitch, float roll)
                {
                    this.roll = roll;
                    this.pitch = pitch;

                    if (actor != null && actor.GameObject)
                        SetRotationSub(raw, pitch, roll);
                }

                void SetRotationSub(float yaw, float pitch, float roll)
                {
                    //Vector3 one = new Vector3(1, 0, 1);
                    Quaternion degree = Quaternion.Euler(yaw, 0, roll) * Quaternion.Euler(yaw, 0, pitch);
                    //degree = Quaternion.Inverse(Quaternion.LookRotation(one)) * degree;
                    //one = degree * one;
                    actor.GameObject.transform.localRotation = degree;
                }*/
    }

}
