using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using DG.Tweening;
using System.Linq;
using EngineCore.Utility;
namespace SeekerGame
{
    /// <summary>
    /// 扫描仪特效组件
    /// </summary>
    public class ScanerEffectComponent : GameUIComponent
    {
        private GameUIEffect m_scanerEffect = null;
        private float m_canvasMaxX = 0f;
        private List<HintEffectComponent> m_HintEffectComponentList = null;
        private List<Vector2> m_hintEntityCanvasPosition = null;
        private int nextDetectEntityIndex = 0;
        private List<SceneItemEntity> m_foundEntityList = null;

        protected override void OnInit()
        {
            this.m_scanerEffect = Make<GameUIEffect>(gameObject);
            this.m_scanerEffect.EffectPrefabName = "UI_tancebo.prefab";
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_canvasMaxX = LogicHandler.Transform.rect.xMax;
            //GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
        }

        public override void OnHide()
        {
            base.OnHide();
            //GameEvents.MainGameEvents.OnGameStatusChange -= OnGameStatusChange;
        }

        //private void OnGameStatusChange(SceneBase.GameStatus status)
        //{
        //    this.m_scanerEffect.Visible = !(status == SceneBase.GameStatus.PAUSE);
        //}

        public void DoScan(float scanTime)
        {
            m_foundEntityList = GameEvents.MainGameEvents.GetFoundTaskItemEntityList();

            //Visible = true;
            m_canvasMaxX = LogicHandler.Transform.rect.xMax;
            this.m_scanerEffect.Visible = true;
            Widget.DOLocalMoveX(m_canvasMaxX, scanTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                this.m_scanerEffect.Visible = false;
                //Visible = false;
                Widget.anchoredPosition = Vector2.zero;
            }).OnUpdate(() =>
            {
                if (nextDetectEntityIndex < this.m_HintEffectComponentList.Count)
                {
                    if (gameObject.transform.localPosition.x > this.m_hintEntityCanvasPosition[nextDetectEntityIndex].x)
                    {
                        bool isEntityFound = GameEvents.MainGameEvents.IsSceneItemFound(this.m_HintEffectComponentList[nextDetectEntityIndex].Entity.EntityId);
                        if (!isEntityFound)
                            m_HintEffectComponentList[nextDetectEntityIndex].Visible = true;

                        nextDetectEntityIndex++;

                    }
                }
            });
        }

        public void SetHintEntityComponentList(List<HintEffectComponent> effectComponents)
        {
            //排序
            this.m_HintEffectComponentList = effectComponents.OrderBy(component => component.EntityOnCanvasPosition.x).ToList();
            if (this.m_HintEffectComponentList.Count > 0)
            {
                this.m_hintEntityCanvasPosition = new List<Vector2>(this.m_HintEffectComponentList.Count);

                for (int i = 0; i < this.m_HintEffectComponentList.Count; ++i)
                {
                    //world pos to canvas pos
                    Vector2 hintEntityInCanvasSpace = CameraUtility.WorldPointToCanvasLocalPosition(this.m_HintEffectComponentList[i].Entity.EntityPosition, LogicHandler.Canvas);
                    this.m_hintEntityCanvasPosition.Add(hintEntityInCanvasSpace);
                }

            }
            this.nextDetectEntityIndex = 0;

        }
    }
}
