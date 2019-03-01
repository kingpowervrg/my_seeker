/********************************************************************
	created:  2018-4-13 11:7:48
	filename: SceneItemEntity.cs
	author:	  songguangze@outlook.com
	
	purpose:  场景目标实体
*********************************************************************/
using UnityEngine;

namespace SeekerGame
{
    public class SceneItemEntity : EntityBase
    {
        public SceneItemEntity(string entityId) : base(EntityType.Scene_Object, entityId)
        {

        }

        protected override void OnLoadedRes(string assetName, Object resObject)
        {
            this.m_entityObject = this.m_entityObject ?? new SceneItemEntityObject(this, resObject as GameObject);

            if (EntityData.itemPos.Count == 0)
                Debug.LogError($"SceneItem no position data, ItemName:{EntityData.itemName} , ItemID:{EntityData.itemID}");

            //客户端自己随机放一个位置，mmp
            int randamLocationIndex = Random.Range(0, EntityData.itemPos.Count);
            ItemPosInfoJson itemLocationInfo = EntityData.itemPos[randamLocationIndex];
            EntityData.itemPos.RemoveAt(randamLocationIndex);

            Confexhibit sceneItemConfig = Confexhibit.Get(EntityData.itemID);
            if (sceneItemConfig == null)
                Debug.LogError($"no item :{EntityData.itemID}in exhibit ");

            EntityPosition = new Vector3(itemLocationInfo.pos.x, itemLocationInfo.pos.y, itemLocationInfo.pos.z);
            EntityScale = new Vector3(itemLocationInfo.scale.x, itemLocationInfo.scale.y, itemLocationInfo.scale.z);
            EntityEulerRotation = new Vector3(itemLocationInfo.rotate.x, itemLocationInfo.rotate.y, itemLocationInfo.rotate.z);
            EntityLightInfo = new Vector4(itemLocationInfo.offsetX, itemLocationInfo.offsetY, itemLocationInfo.tilingX, itemLocationInfo.tilingY);

            EntityObject.SetColliderScale(sceneItemConfig.colliderScale);
            Vector3 finalPos = EntityObject.EntityTransform.rotation * EntityObject.entityCollider.center;
            centerPostion = EntityPosition + new Vector3(finalPos.x * EntityScale.x, finalPos.y * EntityScale.y, finalPos.z * EntityScale.z);
            CameraName = itemLocationInfo.cameraNode;
        }

        new public SceneItemEntityObject EntityObject
        {
            get { return m_entityObject as SceneItemEntityObject; }
        }
        private Vector3 centerPostion = Vector3.zero;
        public Vector3 CenterPosition
        {
            get
            {
                return centerPostion;
            }
        }

        public string CameraName;

        new public ItemInfoJson EntityData
        {
            get { return m_entityData as ItemInfoJson; }
        }
    }
}