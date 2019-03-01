/********************************************************************
	created:  2018-4-4 14:5:35
	filename: EntityFactory.cs
	author:	  songguangze@outlook.com
	
	purpose:  实体工厂
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeekerGame
{
    public class EntityFactory : Singleton<EntityFactory>
    {
        private Dictionary<EntityType, Type> m_entityDict = new Dictionary<EntityType, Type>();

        public EntityFactory()
        {
            m_entityDict.Add(EntityType.Scene_Object, typeof(SceneItemEntity));
            m_entityDict.Add(EntityType.Scene_Decoration, typeof(SceneDecorationEntity));
            m_entityDict.Add(EntityType.Effect, typeof(EffectEntity));
            m_entityDict.Add(EntityType.Scene_Exhibit, typeof(SceneExhibitEntity));
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public EntityBase CreateEntity(EntityType entityType)
        {
            if (this.m_entityDict.ContainsKey(entityType))
            {
                Type entityInstanceType = m_entityDict[entityType];
                string entityId = AssignEntityId(entityType);

                EntityBase entityInstance = Activator.CreateInstance(entityInstanceType, entityId) as EntityBase;

                return entityInstance;
            }
            else
                throw new Exception("no entity " + entityType);

        }

        /// <summary>
        /// 创建新的Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateEntity<T>() where T : EntityBase
        {
            KeyValuePair<EntityType, Type>? entityTypePair = this.m_entityDict.SingleOrDefault(pair => pair.Value == typeof(T));
            if (!entityTypePair.HasValue)
                throw new Exception("no entity :" + typeof(T));

            EntityType entityType = entityTypePair.Value.Key;
            T newEntity = CreateEntity(entityType) as T;

            return newEntity;
        }


        /// <summary>
        /// 分配EntityId
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private string AssignEntityId(EntityType entityType)
        {
            return entityType.ToString() + "_" + Guid.NewGuid().ToString();
        }
    }
}