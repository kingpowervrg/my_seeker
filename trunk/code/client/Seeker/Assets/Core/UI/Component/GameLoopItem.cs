/********************************************************************
	created:  2018-12-28 11:56:5
	filename: GameLoopItem.cs
	author:	  songguangze@outlook.com
	
	purpose:  循环列表中的条目
*********************************************************************/
using UnityEngine;

namespace EngineCore
{
    public abstract class GameLoopItem : GameUIComponent
    {
        //Item逻辑索引
        private int m_itemIndex = 0;

        //LoopItem 由于事件OnLoopItemAppear DisApear控制显示与隐藏的事件，这里将virtual方法闭死，子类不允许override
        new public void OnShow(object param)
        {
            base.OnShow(param);
        }

        new public void OnHide()
        {
            base.OnHide();
        }

        internal void SetItemGameObject(GameObject loopItemGameObject)
        {
            if (gameObject != loopItemGameObject)
                Init(loopItemGameObject);
        }

        internal void OnLoopItemAppear()
        {
            OnLoopItemBecameVisible();
        }

        internal void OnLoopItemDisappear()
        {
            OnLoopItemBecameInvisible();
        }

        /// <summary>
        /// Loop Item 可见回调，需要在这里注册UI上的事件
        /// </summary>
        protected abstract void OnLoopItemBecameVisible();

        /// <summary>
        /// Loop Item 不可见时回调,需要将事件解除
        /// </summary>
        protected abstract void OnLoopItemBecameInvisible();

        public int ItemIndex
        {
            get { return this.m_itemIndex; }
            internal set { m_itemIndex = value; }
        }
    }
}