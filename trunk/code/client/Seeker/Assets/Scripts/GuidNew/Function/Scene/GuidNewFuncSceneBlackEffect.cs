using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using EngineCore.Utility;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneBlackEffect : GuidNewFunctionBase
    {
        private string entityID;
        private GameUIEffect m_effect = null;
        Vector2 endLocalPos = Vector2.zero;
        Vector2 startLocalPos = Vector2.zero;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            //GameEvents.UI_Guid_Event.OnFindSceneResult += OnGameOver;
            Vector2 center = Vector2.right * Screen.width / 2f + Vector2.up * Screen.height / 2f;
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            Transform artTran = frame.FrameRootTransform.Find("guid/art");
            startLocalPos = CameraUtility.ScreenPointInLocalRectTransform(center, artTran.gameObject);
            List<SceneItemEntity> sceneItem = GameEvents.MainGameEvents.GetSceneItemEntityList.SafeInvoke(1);
            if (sceneItem.Count > 0)
            {
                entityID = sceneItem[0].EntityId;
                //Debug.Log(" item name  ===  " + sceneItem[0].EntityData.itemName + "  " + sceneItem[0].EntityPosition);
                //endLocalPos = CameraUtility.WorldPointInCanvasRectTransform(sceneItem[0].EntityPosition, artTran.gameObject);
                //endLocalPos = GuidNewTools.WordToLocalPos(artTran,sceneItem[0].EntityPosition);
                Vector2 screenPos = Camera.main.WorldToScreenPoint(sceneItem[0].EntityPosition);
                endLocalPos = CameraUtility.ScreenPointInLocalRectTransform(screenPos, artTran.gameObject);
                GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(20010, "UI_xinshouyindao_huadong02.prefab", startLocalPos, Vector2.one, 0f);
                GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(20011, "UI_xinshouyindao_huadong02_quan.prefab", endLocalPos, Vector2.one, 0f);

                m_effect = GameEvents.UI_Guid_Event.OnGetMaskEffect(20010);
                if (m_effect != null)
                {
                    GuidNewModule.Instance.PushFunction(this);
                }
                GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }


        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
        }
        private float timesection = 0f;
        public override void Tick(float time)
        {
            base.Tick(time);
            if (m_effect == null)
            {
                GuidNewModule.Instance.RemoveFunction(this);
                return;
            }
            timesection += Time.deltaTime;
            if (Vector2.Distance(m_effect.Widget.anchoredPosition, endLocalPos) <= 1f)
            {
                timesection = 0f;
            }
            m_effect.Widget.anchoredPosition = Vector2.Lerp(startLocalPos,endLocalPos, timesection);
        }

        private void OnPickedSceneItem(SceneItemEntity entity)
        {
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(20010, true);
            GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(20011, true);
            GuidNewModule.Instance.RemoveFunction(this);
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
            OnDestory();
            //if (entityID.Equals(entity.EntityId))
            //{

            //}
        }

    }
}
