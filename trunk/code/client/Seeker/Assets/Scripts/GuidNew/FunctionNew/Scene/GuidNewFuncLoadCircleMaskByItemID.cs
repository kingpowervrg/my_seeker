using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadCircleMaskByItemID : GuidNewFuncLoadCircleMaskByPosition
    {
        private long m_itemID = -1;
        private string itemID = string.Empty;

        private int type = 0; //0表示为物件ID  1表示为指示图标
        public override void OnInit(long funcID, string[] param)
        {
            this.m_needbaseInit = false;
            base.OnInit(funcID, param);
            if (param.Length > 1)
            {
                long.TryParse(param[1], out m_itemID);
                if (m_itemID <= 0)
                {
                    itemID = param[1];
                    this.type = 1;
                }

                this.m_radius = long.Parse(param[2]);
            }

            this.m_centerPos = Vector4.zero;
        }

        public override void OnExecute()
        {
            if (type == 0)
            {
                SceneItemEntity entity = GameEvents.MainGameEvents.GetSceneItemEntityByID(m_itemID);
                if (entity != null)
                {
                    GetMaskLocalPosition(entity.EntityPosition);
                }
            }
            else if (type == 1)
            {
                GameObject[] cubeObjs = GameObject.FindGameObjectsWithTag("cameraPoint");
                for (int i = 0; i < cubeObjs.Length; i++)
                {
                    if (cubeObjs[i].name.Equals(itemID))
                    {
                        GetMaskLocalPosition(cubeObjs[i].transform.position);
                        break;
                    }
                }
            }

            base.OnExecute();
        }

        private void GetMaskLocalPosition(Vector3 worldPos)
        {
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            GameObject guidObj = frame.FrameRootTransform.Find("guid/mask").gameObject;
            Vector3 entityLocalPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(worldPos, guidObj);
            entityLocalPos = guidObj.transform.InverseTransformPoint(entityLocalPos);
            entityLocalPos.z = 0;
            m_centerPos.x = entityLocalPos.x;
            m_centerPos.y = entityLocalPos.y;
        }

    }
}
