using UnityEngine;
using System.Collections.Generic;
namespace SeekerGame
{
    public class SceneExhibitEntity : EntityBase
    {
        public SceneExhibitEntity(string entityId) :
            base(EntityType.Scene_Exhibit, entityId)
        {

        }


        protected override void OnLoadedRes(string assetName, Object resObject)
        {
            this.m_entityObject = this.m_entityObject ?? new SceneExhibitEntityObject(this, resObject as GameObject);

            //long prop_id = EntityData;

            //int randamLocationIndex = GetRandomIndex(EntityData.itemPos);//Random.Range(0, EntityData.itemPos.Count);
            //ItemPosInfoJson itemLocationInfo = EntityData.itemPos[randamLocationIndex];

            //EntityPosition = new Vector3(itemLocationInfo.pos.x, itemLocationInfo.pos.y, itemLocationInfo.pos.z);
            //EntityScale = new Vector3(itemLocationInfo.scale.x, itemLocationInfo.scale.y, itemLocationInfo.scale.z);
            //EntityEulerRotation = new Vector3(itemLocationInfo.rotate.x, itemLocationInfo.rotate.y, itemLocationInfo.rotate.z);
            //EntityLightInfo = new Vector4(itemLocationInfo.offsetX, itemLocationInfo.offsetY, itemLocationInfo.tilingX, itemLocationInfo.tilingY);
        }

       

        new public long EntityData
        {
            get { return (long)m_entityData; }
        }
    }
}