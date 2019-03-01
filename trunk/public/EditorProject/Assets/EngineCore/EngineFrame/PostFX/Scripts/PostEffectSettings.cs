using System;
using System.Collections.Generic;
using UnityEngine;
namespace PostFX
{
    public enum EffectType
    {
        Bloom,
        ScreenGlow,
        FrameFadeout,
        OutlineEffect,
        Blend2Camera_Color,
        Blend2Camera_ColorDodge,
        Blend2Camera_Saturation,
        Colors_Adjust_ColorRGB,
        FX_EarthQuake,
        TV_WideScreenCircle,
        TV_WideScreenHorizontal,
        TV_WideScreenVertical,
        LensDistortionBlur,
        ChromaticAberration,
        BrightnessSaturationAndContrast,
        FastVignette,
        ScreenSpeedLine,
        ScreenFlip,
        AmplifyColor,
        BlurOptimized,
        EdgeDetection,
        SpiritPressure,
        BlurWithMask,
        DarkSceneWithMask,
        Glitch,
        OldMoive,
    }

    [Serializable]
    public class PostEffectSettings
    {

        [Tooltip("This is bloom")]
        [SerializeField]
        private XBloom bloom = new XBloom();

        [Tooltip("This is ScreenGlow")]
        [SerializeField]
        private ScreenGlow screenGlow = new ScreenGlow();

        [Tooltip("This is FrameFadeout")]
        [SerializeField]
        private FrameFadeout frameFadeout = new FrameFadeout();

        [Tooltip("This is OutlineEffect")]
        [SerializeField]
        private OutlineEffect outlineEffect = new OutlineEffect();

        [Tooltip("This is CameraFilterPack_Blend2Camera_Color")]
        [SerializeField]
        private CameraFilterPack_Blend2Camera_Color Blend2Camera_Color = new CameraFilterPack_Blend2Camera_Color();


        [Tooltip("This is CameraFilterPack_Blend2Camera_ColorDodge")]
        [SerializeField]
        private CameraFilterPack_Blend2Camera_ColorDodge Blend2Camera_ColorDodge = new CameraFilterPack_Blend2Camera_ColorDodge();

        [Tooltip("This is CameraFilterPack_Blend2Camera_Saturation")]
        [SerializeField]
        private CameraFilterPack_Blend2Camera_Saturation Blend2Camera_Saturation = new CameraFilterPack_Blend2Camera_Saturation();

        [Tooltip("This is CameraFilterPack_Colors_Adjust_ColorRGB")]
        [SerializeField]
        private CameraFilterPack_Colors_Adjust_ColorRGB Colors_Adjust_ColorRGB = new CameraFilterPack_Colors_Adjust_ColorRGB();

        [Tooltip("This is CameraFilterPack_FX_EarthQuake")]
        [SerializeField]
        private CameraFilterPack_FX_EarthQuake FX_EarthQuake = new CameraFilterPack_FX_EarthQuake();

        [Tooltip("This is CameraFilterPack_TV_WideScreenCircle")]
        [SerializeField]
        private CameraFilterPack_TV_WideScreenCircle TV_WideScreenCircle = new CameraFilterPack_TV_WideScreenCircle();

        [Tooltip("This is CameraFilterPack_TV_WideScreenHorizontal")]
        [SerializeField]
        private CameraFilterPack_TV_WideScreenHorizontal TV_WideScreenHorizontal = new CameraFilterPack_TV_WideScreenHorizontal();

        [Tooltip("This is CameraFilterPack_TV_WideScreenVertical")]
        [SerializeField]
        private CameraFilterPack_TV_WideScreenVertical TV_WideScreenVertical = new CameraFilterPack_TV_WideScreenVertical();


        [Tooltip("This is LensDistortionBlur")]
        [SerializeField]
        private LensDistortionBlur lensDistortionBlur = new LensDistortionBlur();

        [Tooltip("This is ChromaticAberration")]
        [SerializeField]
        private ChromaticAberration chromaticAberration = new ChromaticAberration();

        [Tooltip("This is BrightnessSaturationAndContrast")]
        [SerializeField]
        private BrightnessSaturationAndContrast brightnessSaturationAndContrast = new BrightnessSaturationAndContrast();


        [Tooltip("This is FastVignette")]
        [SerializeField]
        private FastVignette fastVignette = new FastVignette();

        [Tooltip("This is Screen Speed Line")]
        [SerializeField]
        private SpeedLineEffect speedLine = new SpeedLineEffect();


        [Tooltip("This is Screen Flip")]
        [SerializeField]
        private ScreenFlip screenFlip = new ScreenFlip();

        [Tooltip("This is AmplifyColor")]
        [SerializeField]
        private AmplifyColor amplifyColor = new AmplifyColor();


        [Tooltip("This is BlurOptimized")]
        [SerializeField]
        private BlurOptimized blurOptimized = new BlurOptimized();



        [Tooltip("This is EdgeDetection")]
        [SerializeField]
        private EdgeDetection edgeDetection = new EdgeDetection();

        [Tooltip("This is SpiritPressure")]
        [SerializeField]
        private SpiritPressure spiritPressure = new SpiritPressure();

        [Tooltip("This is BlurWithMask")]
        [SerializeField]
        private BlurWithMask blurWithMask = new BlurWithMask();

        [Tooltip("This is DarkSceneWithMask")]
        [SerializeField]
        private DarkSceneWithMask darkSceneWithMask = new DarkSceneWithMask();

        [Tooltip("This is Glitch")]
        [SerializeField]
        private Glitch m_glith = new Glitch();

        [Tooltip("This is OldMoive")]
        [SerializeField]
        private OldMoiveEffect m_oldmoive = new OldMoiveEffect();

        public List<PostEffectBase> Initialization()
        {
            List<PostEffectBase> list = new List<PostEffectBase>();
            list.Add(bloom);
            list.Add(screenGlow);
            list.Add(frameFadeout);
            list.Add(outlineEffect);
            list.Add(Blend2Camera_Color);
            list.Add(Blend2Camera_ColorDodge);
            list.Add(Blend2Camera_Saturation);
            list.Add(Colors_Adjust_ColorRGB);
            list.Add(FX_EarthQuake);
            list.Add(TV_WideScreenCircle);
            list.Add(TV_WideScreenHorizontal);
            list.Add(TV_WideScreenVertical);
            list.Add(lensDistortionBlur);
            list.Add(chromaticAberration);
            list.Add(brightnessSaturationAndContrast);
            list.Add(fastVignette);
            list.Add(speedLine);
            list.Add(screenFlip);
            list.Add(amplifyColor);
            list.Add(blurOptimized);
            list.Add(edgeDetection);
            list.Add(spiritPressure);
            list.Add(blurWithMask);
            list.Add(darkSceneWithMask);
            list.Add(m_glith);
            list.Add(m_oldmoive);
            return list;
        }

    }
}