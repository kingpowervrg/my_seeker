using UnityEngine;
using System.Collections.Generic;
namespace SeekerGame
{
    public class SceneDecorationEntity : EntityBase
    {
        public SceneDecorationEntity(string entityId) :
            base(EntityType.Scene_Decoration, entityId)
        {

        }


        protected override void OnLoadedRes(string assetName, Object resObject)
        {
            this.m_entityObject = this.m_entityObject ?? new EntityObjectBase(this, resObject as GameObject);

            int randamLocationIndex = GetRandomIndex(EntityData.itemPos);//Random.Range(0, EntityData.itemPos.Count);
            ItemPosInfoJson itemLocationInfo = EntityData.itemPos[randamLocationIndex];

            EntityPosition = new Vector3(itemLocationInfo.pos.x, itemLocationInfo.pos.y, itemLocationInfo.pos.z);
            EntityScale = new Vector3(itemLocationInfo.scale.x, itemLocationInfo.scale.y, itemLocationInfo.scale.z);
            EntityEulerRotation = new Vector3(itemLocationInfo.rotate.x, itemLocationInfo.rotate.y, itemLocationInfo.rotate.z);
            EntityLightInfo = new Vector4(itemLocationInfo.offsetX, itemLocationInfo.offsetY, itemLocationInfo.tilingX, itemLocationInfo.tilingY);
        }

        private int GetRandomIndex(List<ItemPosInfoJson> items)
        {
            float maxNumber = 0;
            for (int i = 0; i < items.Count; i++)
            {
                maxNumber += items[i].percent;
            }
            float randomFactor = Random.Range(0, maxNumber);
            float currentFactor = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                currentFactor += items[i].percent;
                if (currentFactor >= maxNumber)
                    return i;
            }
            return items.Count - 1;
        }

        new public ItemInfoJson EntityData
        {
            get { return m_entityData as ItemInfoJson; }
        }
    }
}