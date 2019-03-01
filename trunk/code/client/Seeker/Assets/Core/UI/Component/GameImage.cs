using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameTexture : GameUIComponent
    {
        UnityEngine.UI.RawImage tex;
        GOGUI.LazyImageLoader lazyLoader;
        public UnityEngine.UI.RawImage RawImage { get { return tex; } }
        protected override void OnInit()
        {
            base.OnInit();
            tex = GetComponent<UnityEngine.UI.RawImage>();
            //NeedSetNativeSize = tex is UnityEngine.UI.NativeRawImage;
        }

        public override void SetGray(bool gray)
        {
            if (tex)
            {
                if (gray)
                {
                    tex.material = GrayMaterial;
                }
                else
                {
                    tex.material = null;
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

                }
                else
                    return;

                LazyLoader.LoadRawImage();
            }
            get { return tex.texture.name; }
        }

        public Action<string> OnGetRawImageAction
        {
            set
            {
                if (LazyLoader)
                {
                    lazyLoader.OnGetRawImageAction = value;
                }
            }
        }

        public UnityEngine.Color Color
        {
            get { return tex.color; }
            set { tex.color = value; }
        }

        public bool NeedSetNativeSize
        {
            get
            {
                return LazyLoader.neddSetNativeSize;
            }
            set
            {
                if (RawImage != null && RawImage.texture != null)
                {
                    RawImage.SetNativeSize();
                }
                lazyLoader.neddSetNativeSize = value;
            }
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

        public void SetMaterial(Material customeMaterial)
        {
            this.RawImage.material = customeMaterial;
        }
    }

    public class GameImage : GameUIComponent
    {
        UnityEngine.UI.Image sprite;
        GOGUI.LazyLoadImage lazyLoader;
        private string Old_Sprite = "";


        protected override void OnInit()
        {
            base.OnInit();
            sprite = GetComponent<UnityEngine.UI.Image>();

        }

        public override void SetGray(bool gray)
        {
            if (sprite)
            {
                if (gray)
                {
                    bool isSuccess = UIAtlasManager.GetInstance().SetAtlasMaterial(sprite, LazyLoader.SpriteName, true);
                    if (!isSuccess)
                    {
                        sprite.material = GrayMaterial;
                    }
                }
                else
                {
                    bool isSuccess = UIAtlasManager.GetInstance().SetAtlasMaterial(sprite, LazyLoader.SpriteName);
                    if (!isSuccess)
                    {
                        sprite.material = null;
                    }
                }
            }
        }

        public void SetShadow(bool shadow)
        {
            if (sprite)
            {
                if (shadow)
                {
                    sprite.material = ShadowMaterial;
                }
                else
                {
                    sprite.material = null;
                }
            }
        }

        public void SetMask(string mask)
        {
            //sprite.SetMask(mask);
        }

        public GOGUI.LazyLoadImage LazyLoader
        {
            get
            {
                if (!lazyLoader)
                    lazyLoader = GetComponent<GOGUI.LazyLoadImage>();

                return lazyLoader;
            }
        }

        public Action<string> OnLoaded
        {
            set
            {
                if (LazyLoader)
                {
                    lazyLoader.OnLoaded = value;
                }
            }
        }

        public string Sprite
        {
            set
            {
                if (LazyLoader)
                {
                    if (lazyLoader.SpriteName == value)
                    {
                        lazyLoader.OnLoaded.SafeInvoke(value);
                        return;
                    }

                    //默认png格式
                    string[] spriteNames = value.Split('.');
                    if (spriteNames.Length < 2)
                        value = value + ".png";

                    LazyLoader.SpriteName = value;
                }
                else
                    return;

                LazyLoader.LoadSprite();
                //UIAtlasManager.GetInstance().SetAtlasMaterial(sprite, value);
            }
            get { return sprite.sprite.name; }
        }



        public UnityEngine.Color Color
        {
            get { return sprite.color; }
            set { sprite.color = value; }
        }

        public float FillAmmount
        {
            get { return sprite.fillAmount; }
            set { sprite.fillAmount = value; }
        }

        public bool NeedSetNativeSize
        {
            get
            {
                return LazyLoader.neddSetNativeSize;
            }
            set
            {
                if (sprite != null && sprite.sprite != null)
                {
                    sprite.SetNativeSize();
                }
                LazyLoader.neddSetNativeSize = value;
            }
        }

        public bool Enable
        {
            set
            {
                if (sprite != null)
                {
                    sprite.enabled = value;
                }
            }
            get
            {
                return sprite != null ? sprite.enabled : false;
            }
        }

        public bool EnableClick
        {
            set
            {
                if (sprite != null)
                {
                    sprite.raycastTarget = value;
                }
            }
        }

        //public void SetSpriteMaterial(Material spriteSpecialMaterial)
        //{
        //    sprite.material = spriteSpecialMaterial;
        //}

        public Material SpriteMaterial
        {
            get { return sprite.material; }
            set { sprite.material = value; }
        }

        public UnityEngine.UI.Image GetSprite()
        {
            return sprite;
        }
    }

}
