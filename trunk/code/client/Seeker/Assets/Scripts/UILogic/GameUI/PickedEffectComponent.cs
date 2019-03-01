/********************************************************************
	created:  2018-7-24 18:39:51
	filename: PickedEffectComponent.cs
	author:	  songguangze@outlook.com
	
	purpose:  收集到物品效果组件
*********************************************************************/
//#define FLY_0
using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class RandomDropComponent : GameUIComponent
    {
        private List<RandomItemComponent> m_randomItemList = new List<RandomItemComponent>();

        protected override void OnInit()
        {
            base.OnInit();

            for (int i = 1; i <= 4; ++i)
            {
                RandomItemComponent randomItemEffectComponent = Make<RandomItemComponent>("random_item:Image_" + i);
                m_randomItemList.Add(randomItemEffectComponent);
            }
        }

        public void SetRandomDropItem(long[] dropItemList, Vector3 pickedPosition)
        {
            this.Position = pickedPosition;
            Vector2 dropPos = Vector2.zero;
            for (int i = 0; i < dropItemList.Length; ++i)
            {
                switch (i)
                {
                    case 0:
                        dropPos.x = -100;
                        break;
                    case 1:
                        dropPos.x = 100;
                        break;
                    case 2:
                        dropPos.y = 20;
                        break;
                    case 3:
                        dropPos.y = -20;
                        break;
                }

                this.m_randomItemList[i].Widget.anchoredPosition = Vector2.zero;
                this.m_randomItemList[i].Visible = true;
                this.m_randomItemList[i].DoEffect(dropPos, ConfProp.Get(dropItemList[i]).icon);
            }
            TimeModule.Instance.SetTimeout(() => this.Visible = false, 10f);

        }



        public class RandomItemComponent : GameUIComponent
        {
            private GameImage m_randomItemIcon = null;

            protected override void OnInit()
            {
                base.OnInit();
                this.m_randomItemIcon = Make<GameImage>(gameObject);
            }

            public void DoEffect(Vector2 vector2, string dropItemIcon)
            {
                Visible = true;
                this.m_randomItemIcon.Sprite = dropItemIcon;
                Widget.DOJumpAnchorPos(vector2, 30, 1, 0.5f).OnComplete(() =>
                 {
                     TimeModule.Instance.SetTimeout(() =>
                     {
                         Visible = false;
                         Widget.anchoredPosition = Vector2.zero;
                     }, 5f);
                 });
            }

        }

    }

    public class PickedFlyEffectComponent : GameUIComponent
    {
        private GameUIEffect m_effect = null;
        private GameImage m_pickedItemIcon = null;

        private float m_flyDuration = 1f;
        private Vector3 m_startPosition;
        private Vector3 m_endPosition;
        private GameUIEffect m_hintEffect = null;

        private Vector3[] m_wayPoint = new Vector3[3];

        public Action OnFlyEffectPlayFinished;

        protected override void OnInit()
        {
            this.m_effect = Make<GameUIEffect>("flyEffect");
            this.m_pickedItemIcon = Make<GameImage>("Image");
            this.m_hintEffect = Make<GameUIEffect>("hintEffect");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_hintEffect.EffectPrefabName = "UI_zhaodaowupin.prefab";
            this.m_hintEffect.Visible = false;
            this.m_effect.EffectPrefabName = "UI_feizhi_wupinlan02.prefab";
            this.m_effect.Visible = false;
            
        }

        public void PlayPickEffect()
        {
            this.m_hintEffect.Position = this.m_wayPoint[0];
            this.m_hintEffect.Visible = true;
            //TimeModule.Instance
            this.m_effect.Position = this.m_wayPoint[0];
            this.m_effect.Visible = true;
            //this.m_pickedItemIcon.Visible = true;
            this.m_effect.Widget.DOPath(this.m_wayPoint, m_flyDuration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                this.m_effect.Visible = false;
                this.Visible = false;
                OnFlyEffectPlayFinished?.Invoke();
            });
        }


        public void SetPickedItemIcon(string itemIcon)
        {
            this.m_pickedItemIcon.Widget.localScale = Vector3.zero;
            //this.m_pickedItemIcon.Sprite = itemIcon;
        }

        private GameObject m_pickObj = null;
        private Vector3 m_itemPos;
        public void SetPickedObject(GameObject obj,Vector3 itemPos)
        {
            this.m_pickObj = obj;
            this.m_itemPos = itemPos;
        }

        public void SetFlyPath(Vector3 startWorldPosition, Vector3 endWorldPosition, float canvasZ, float flyDuration = 1f)
        {
            this.m_startPosition = startWorldPosition;
            this.Position = this.m_startPosition;
            this.m_endPosition = endWorldPosition;
            this.m_flyDuration = flyDuration;

            UpdatePathWayPoints();
        }


        private void UpdatePathWayPoints()
        {
            float maxX = Mathf.Max(this.m_startPosition.x, this.m_endPosition.x);
            float minX = Mathf.Min(this.m_startPosition.x, this.m_endPosition.x);
            float intermediateX = Mathf.Lerp(minX, maxX, 0.66f);

            float intermediateY = this.m_startPosition.y + 50;

#if FLY_0
            Vector3 centerPos = Vector3.Lerp(m_startPosition,m_endPosition,0.66f);
            centerPos.x -= 30f;
            centerPos.y = this.m_startPosition.y + 30;
            this.m_wayPoint[0] = this.m_startPosition;
            this.m_wayPoint[1] = centerPos;//new Vector3(intermediateX, intermediateY, this.m_endPosition.z);
            this.m_wayPoint[2] = this.m_endPosition;
#else

            this.m_wayPoint[0] = this.m_startPosition;
            this.m_wayPoint[1] = new Vector3(intermediateX, intermediateY, this.m_endPosition.z);
            this.m_wayPoint[2] = this.m_endPosition;
#endif
        }



    }
}