using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameTile : GameUIContainer
    {
        private UnityEngine.UI.LayoutGroup _uiTile;
        Transform cachedTransform;
        protected override void OnInit()
        {
            _uiTile = GetComponent<UnityEngine.UI.LayoutGroup>();
            cachedTransform = _uiTile.transform;
            base.OnInit();
        }

        protected override Transform ContainerTransform
        {
            get
            {
                return cachedTransform;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
            if (ClearChildWhenShow)
                Clear();
        }
        public override void OnHide()
        {
            base.OnHide();
        }
        public bool ClearChildWhenShow { get; set; }
        public override void Clear()
        {
            base.Clear();
        }
    }
}