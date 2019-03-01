using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using EngineCore.Utility;
namespace SeekerGame
{
    /// <summary>
    /// 提示特效
    /// </summary>
    public class HintEffectComponent : GameUIComponent
    {
        private GameUIEffect m_hintEffect = null;
        private SceneItemEntity m_hintSceneItemEntity = null;

        //循环
        private const float IntervalTime = 4f;

        protected override void OnInit()
        {
            this.m_hintEffect = Make<GameUIEffect>(gameObject);
        }

        public override void OnShow(object param)
        {
            IsForceUpdate = true;
            this.m_hintEffect.EffectPrefabName = "UI_xunzhao02.prefab";
            if (!m_needHint)
            {
                TimeModule.Instance.SetTimeout(EndHint, IntervalTime);
            }
            IsHinting = true;
            this.m_hintEffect.Visible = true;
            //this.m_hintEffect.ReplayEffect();
        }

        private bool m_needHint = false;
        public void SetHintSceneEntity(SceneItemEntity hintEntity,bool needHint)
        {
            this.m_needHint = needHint;
            this.m_hintSceneItemEntity = hintEntity;
            EntityOnCanvasPosition = CameraUtility.WorldPointInCanvasRectTransform(hintEntity.CenterPosition, LogicHandler.Canvas.gameObject);
            this.m_hintEffect.Position = EntityOnCanvasPosition;
            if (needHint)
            {
                this.m_hintSceneItemEntity.EntityObject.ChangeEntityMatToHint();
            }
        }

        public override void ForceUpdate()
        {
            EntityOnCanvasPosition = CameraUtility.WorldPointInCanvasRectTransform(this.m_hintSceneItemEntity.CenterPosition, LogicHandler.Canvas.gameObject);
            this.m_hintEffect.Position = EntityOnCanvasPosition;
        }

        public void EndHint()
        {
            Visible = false;
            IsForceUpdate = false;
            IsHinting = false;
            //if (this.m_hintSceneItemEntity != null && this.m_hintSceneItemEntity.EntityObject != null && m_needHint)
            //{
            //    this.m_hintSceneItemEntity.EntityObject.RecoverEntityMat();
            //}
            TimeModule.Instance.RemoveTimeaction(EndHint);
        }

        public bool ClearHintEntityByCamera(string cameraName)
        {
            if (this.m_hintSceneItemEntity != null && this.m_hintSceneItemEntity.EntityObject != null && this.m_hintSceneItemEntity.CameraName.Equals(cameraName))
            {
                this.m_hintSceneItemEntity.EntityObject.RecoverEntityMat();
                return true;
            }
            return false;
        }

        public void ClearHintEntityByObj(SceneItemEntity entity)
        {
            if (this.m_hintSceneItemEntity != null && this.m_hintSceneItemEntity.EntityObject != null && this.m_hintSceneItemEntity == entity)
            {
                this.m_hintSceneItemEntity.EntityObject.RecoverEntityMat();
            }
        }

        public bool IsHinting { get; set; } = false;

        public Vector3 EntityOnCanvasPosition { get; private set; }

        public SceneItemEntity Entity { get { return this.m_hintSceneItemEntity; } }
    }
}
