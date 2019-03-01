/********************************************************************
	created:  2018-3-29 15:22:13
	filename: EngineDelegateCore.cs
	author:	  songguangze@outlook.com
	
	purpose:  引擎底层相关标识配置
*********************************************************************/
using System;
using UnityEngine;

namespace EngineCore
{
    public class EngineDelegateCore
    {
        public static Func<UnityEngine.GameObject, bool> PrefabUnInstantiateRule;
        public static Action<string[]> OnUpdateResource;
        public static bool IsFirstTimeLaunch = false;           //是否是首次启动

        /// <summary>
        /// 是否使用动态资源(资源热更新,动态资源下载)
        /// </summary>
        public static bool DynamicResource = false;


        public static float DefaultMoveSpeed = 8;
        public static float MaxSoundVolume = 1f;
        public static float MaxBGMVolume = 0.8f;
        public static float VolumeDuration = 1;
        public static string GameClientEntrySceneName = string.Empty;           //游戏入口场景名称

        /// <summary>
        /// 是否是编辑器环境下模拟对应平台(Editor下模拟Android,IOS 平台)
        /// </summary>
        public static bool Editor_Simulate_Player = false;

        /// <summary>
        /// 默认背景音乐混音组
        /// </summary>
        public static UnityEngine.Audio.AudioMixerGroup DefaultBGMMixerGroup = null;

        /// <summary>
        /// 默认音效混音组
        /// </summary>
        public static UnityEngine.Audio.AudioMixerGroup DefaultEffectMixerGroup = null;


        /// <summary>
        /// 声音挂载节点
        /// </summary>
        public static GameObject AudioRootGameObject;
    }

    /// <summary>
    /// 通用音频Key
    /// </summary>
    public enum EngineCommonAudioKey
    {
        Button_Click_Common,            //通用点击
        Close_Window,                   //窗口关闭
        table_change,                   //系统内切换页签的音效
        slider_btn,//  friend_consentapply.mp3 通过sliderbutton音效    click
    }
}
