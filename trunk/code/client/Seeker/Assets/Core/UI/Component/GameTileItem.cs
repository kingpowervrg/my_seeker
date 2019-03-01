using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameTileItem : GameTextComponent
    {
        private Transform transform;
        protected override void OnInit()
        {
            base.OnInit();
            transform = gameObject.transform;

        }

        public Transform Transform
        {
            get { return gameObject.transform; }
        }
    }
}
