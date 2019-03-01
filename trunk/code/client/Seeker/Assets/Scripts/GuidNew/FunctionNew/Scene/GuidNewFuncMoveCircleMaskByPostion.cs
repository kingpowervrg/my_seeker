using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncMoveCircleMaskByPostion : GuidNewFunctionBase
    {
        private long m_exhibitID = -1;
        private float m_costTime = 3f;
        private Vector3[] m_points;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_exhibitID = long.Parse(param[0]);
            this.m_costTime = float.Parse(param[1]);
            if (this.m_exhibitID > 0)
            {
                this.m_points = new Vector3[(param.Length - 2) / 2 + 1];
            }
            else
            {
                this.m_points = new Vector3[(param.Length - 2) / 2];
            }

            for (int i = 2; i < param.Length; i += 2)
            {
                this.m_points[(i - 2) / 2] = Vector3.zero;
                this.m_points[(i - 2) / 2].x = float.Parse(param[i]);
                this.m_points[(i - 2) / 2].y = float.Parse(param[i + 1]);
            }
        }

        private Transform emptyTrans = null;
        public override void OnExecute()
        {
            base.OnExecute();
            this.m_centerPos = GameEvents.UI_Guid_Event.OnGetCurrentMaskInfo(0);
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            emptyTrans = frame.FrameRootTransform.Find("tweenEmpty");
            emptyTrans.localPosition = this.m_centerPos;
            if (m_exhibitID > 0)
            {
                SceneItemEntity entity = GameEvents.MainGameEvents.GetSceneItemEntityByID(m_exhibitID);
                if (entity != null)
                {
                    GameObject guidObj = frame.FrameRootTransform.Find("guid/mask").gameObject;
                    Vector3 entityLocalPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(entity.EntityPosition, guidObj);
                    entityLocalPos = guidObj.transform.InverseTransformPoint(entityLocalPos);
                    entityLocalPos.z = 0;
                    this.m_points[this.m_points.Length - 1] = entityLocalPos;
                }
            }
            emptyTrans.DOLocalPath(m_points, this.m_costTime, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InQuad).OnUpdate(OnUpdateTween).OnComplete(() =>
            {
                OnDestory();
            });
        }
        private Vector4 m_centerPos;

        private void OnUpdateTween()
        {
            this.m_centerPos.x = emptyTrans.localPosition.x;
            this.m_centerPos.y = emptyTrans.localPosition.y;
            GameEvents.UI_Guid_Event.OnReflashCircleMask.SafeInvoke(this.m_centerPos, 0);
        }
        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }
    }
}
