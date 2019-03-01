using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
namespace SeekerGame
{
    class GameUIRadarImage : GameUIComponent
    {
        RadarImage tex;
        GOGUI.LazyImageLoader lazyLoader;
        public RadarImage RadarImage { get { return tex; } }

        public void SetPropList(List<float> list)
        {
            tex.SetPropList(list);
        }


        protected override void OnInit()
        {
            base.OnInit();
            tex = GetComponent<RadarImage>();
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
                if (RadarImage != null && RadarImage.texture != null)
                {
                    RadarImage.SetNativeSize();
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
    }
}
