/********************************************************************
	created:  2018-3-29 15:22:13
	filename: EngineDelegateCore.cs
	author:	  songguangze@outlook.com
	
	purpose:  引擎底层相关标识配置
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore
{
    public class EngineDelegateCore
    {
        public static Func<UnityEngine.GameObject, bool> PrefabUnInstantiateRule;
        public static Action<string[]> OnUpdateResource;
        public static Action<int> OnShowUpdateAlert;
        public static Action<bool> StratOrEndDownload;
        public static Action<bool> NetWorkError;
        public static bool TestMobile = false;
        public static bool DynamicResource = false;
        public static float DefaultMoveSpeed = 8;
        public static float MaxSoundVolume = 0.8f;
        public static float MaxBGMVolume = 0.8f;
        public static float VolumeDuration = 1;

        /// <summary>
        /// 是否是编辑器运行时
        /// </summary>
        public static bool IsEditorRuntime = false;

        /// <summary>
        /// 默认3D音效最大音量距离
        /// </summary>
        public static float Default3DAudioMinDistance = 2;
        /// <summary>
        /// 默认3D音效最小音量距离
        /// </summary>
        public static float Default3DAudioMaxDistance = 20;
        /// <summary>
        /// 默认音效2D/3D音效混合，0为纯2D，1为纯3D
        /// </summary>
        public static float DefaultEffectSpatialBlend = 0;
        /// <summary>
        /// 默认混响区影响比例
        /// </summary>
        public static float DefaultReverbZoneMix = 1;
        /// <summary>
        /// 默认背景音乐混音组
        /// </summary>
        public static UnityEngine.Audio.AudioMixerGroup DefaultBGMMixerGroup = null;
        /// <summary>
        /// 默认音效混音组
        /// </summary>
        public static UnityEngine.Audio.AudioMixerGroup DefaultEffectMixerGroup = null;
    }
}
