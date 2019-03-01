/********************************************************************
	created:  2018-5-30 10:32:9
	filename: EffectEntity.cs
	author:	  songguangze@fotoable.com
	
	purpose:  特效实体
*********************************************************************/
using EngineCore;
using System;
using UnityEngine;

namespace SeekerGame
{
    public class EffectEntity : EntityBase
    {

        public EffectEntity(string entityId) : base(EntityType.Effect, entityId)
        {

        }

        protected override void OnLoadedRes(string assetName, UnityEngine.Object resObject)
        {
            //todo: 实体销毁时机有问题，实体的gameobject已经被销毁，但 entityObject并没有置空，造成第二次使用 this.m_entityobject??  不去实例化 
            //所有的实体系统都有这个问题，实体池与实体释放生命周期有问题，2018-6-26 20:43:36 guangze song
            this.m_entityObject = this.m_entityObject ?? new EffectEntityObject(this, resObject as GameObject);

            this.m_entityObject.SetEntityObjectPositionAndRotation(EntityPosition, EntityEulerRotation);

            OnEntityLoaded?.Invoke();
        }

    }
}