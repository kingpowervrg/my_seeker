/********************************************************************
	created:  2018-4-10 15:42:28
	filename: SceneItemEntityObject.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景待寻找物件渲染对象
*********************************************************************/
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class SceneExhibitEntityObject : EntityObjectBase
    {
        public BoxCollider entityCollider
        {
            get { return m_entityCollider; }
        }
        private BoxCollider m_entityCollider = null;

        public SceneExhibitEntityObject(SceneExhibitEntity owner, GameObject entityGameObject) :
            base(owner, entityGameObject)
        {
            GameObjectUtil.SetLayer(entityGameObject, LayerDefine.SceneTargetObjectLayer, true);
            Renderer sceneItemRenderer = entityGameObject.GetComponentInChildren<Renderer>(true);


            if (sceneItemRenderer != null)
            {
                m_entityCollider = sceneItemRenderer.gameObject.AddComponent<BoxCollider>();
                IsEnable = false;
            }
        }


        public void SetColliderScale(float scaleFactor = 1)
        {
            this.m_entityCollider.size *= scaleFactor;
        }

        public override bool IsEnable
        {
            get
            {
                return base.IsEnable;
            }

            set
            {
                base.IsEnable = value;
                this.m_entityCollider.enabled = value;
            }
        }
    }
}