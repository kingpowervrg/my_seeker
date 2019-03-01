using System;
using System.Collections.Generic;
using UnityEngine;


namespace EngineCore
{
    public class GameSpriteRenderer : GameUIComponent
    {
        SpriteRenderer sr;
        GOGUI.LazyImageLoader lazyLoader;
        public SpriteRenderer spriteRenderer { get { return sr; } }
        protected override void OnInit()
        {
            base.OnInit();
            sr = GetComponent<SpriteRenderer>();
        }

        public override void SetGray(bool gray)
        {
            if (sr)
            {
                if (gray)
                {
                    sr.material = GrayMaterial;
                }
                else
                {
                    sr.material = null;
                }
            }
        }

        public string TextureName
        {
            set
            {
                if (LazyLoader)
                {
                    LazyLoader.RawImageName = value;
                    LazyLoader.SpriteName = value;
                }
                else
                    return;

                LazyLoader.LoadSpriteRenderer();
            }
            get { return sr.sprite.name; }
        }

        public UnityEngine.Color Color
        {
            get { return sr.color; }
            set { sr.color = value; }
        }

        public GOGUI.LazyImageLoader LazyLoader
        {
            get
            {
                if (!lazyLoader)
                    lazyLoader = GetComponent<GOGUI.LazyImageLoader>();

                return lazyLoader;
            }
        }
    }
}
