/********************************************************************
	created:  2018-3-28 10:12:53
	filename: EngineCoreEvents.cs
	author:	  songguangze@outlook.com
	
	purpose:  引擎层事件
*********************************************************************/
using Google.Protobuf;
using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineCore
{
    public class EngineCoreEvents
    {
        /// <summary>
        /// 引擎模块间的事件，用于解耦
        /// </summary>
        public static class ResourceEvent
        {
            /// <summary>
            /// 获取资源
            /// </summary>
            public static SafeAction<string, Action<string, UnityEngine.Object>, LoadPriority> GetAssetEvent;

            /// <summary>
            /// 释放资源
            /// </summary>
            public static SafeAction<string, UnityEngine.Object> ReleaseAssetEvent;

            /// <summary>
            /// 彻底删除资源
            /// </summary>
            public static SafeAction<string, bool, bool> RemoveAssetEvent;

            /// <summary>
            /// 释放并删除资源
            /// </summary>
            public static SafeAction<string, UnityEngine.Object> ReleaseAndRemoveAssetEvent;

            /// <summary>
            /// 删除所有资源（关卡切换时）
            /// </summary>
            public static SafeAction RemoveAllAssetEvent;

            /// <summary>
            /// 尝试GC回收
            /// </summary>
            public static SafeAction TryGCCacheEvent;

            /// <summary>
            /// 加载场景
            /// </summary>
            public static SafeAction<string, Action> LoadAdditiveScene;

            /// <summary>
            /// 加载场景回调
            /// </summary>
            public static SafeAction<Scene, GameObject[]> OnLoadAdditiveScene;

            /// <summary>
            /// 离开场景(用于清资源)
            /// </summary>
            public static SafeAction LeaveScene;

            /// <summary>
            /// 获取文本文件
            /// </summary>
            public static SafeAction<string, Action<string, string>, bool> GetTextAssetEvent;

            /// <summary>
            /// 预加载资源
            /// </summary>
            public static SafeAction<string, Action<bool>> PreloadAssetEvent;

            /// <summary>
            /// 资源服务器连接失败
            /// </summary>
            public static SafeAction<string> AssetsServerError;

            /// <summary>
            /// 检查资源更新
            /// </summary>
            public static SafeAction<Action<int>> CheckUpdate;

            /// <summary>
            /// 提示有更新资源
            /// </summary>
            public static SafeAction<int> OnHaveUpdateAssets;

            /// <summary>
            /// 请求开始更新资源(Action-更新完成回调  Action<AssetUpdateInfo>-更新进度)
            /// </summary>
            public static SafeAction<Action<AssetUpdateInfo>, Action<AssetUpdateInfo>> BeginUpdateAssets;

            /// <summary>
            /// 网络资源下载完成
            /// </summary>
            public static SafeAction<string, byte[]> OnDownloadedAssetFromWebServer;

            /// <summary>
            /// 更新结束事件
            /// </summary>
            public static SafeAction OnFinishDownloadUpdateEvent;

            /// <summary>
            /// 下载资源出错
            /// </summary>
            public static SafeAction<string, string> OnDownloadAssetError;

            /// <summary>
            /// 是否加载Bundle的所有资源
            /// </summary>
            public static Func<string, bool> IsLoadAssetBundleAllAssets;
        }

        /// <summary>
        /// 引擎逻辑层相关UI事件
        /// </summary>
        public static class UIEvent
        {
            /// <summary>
            /// 打开UI
            /// </summary>
            public static SafeAction<string, string> ShowUIByOther;

            /// <summary>
            /// 
            /// </summary>
            public static SafeAction<string> ShowUIEvent;

            /// <summary>
            /// 带参数的打开UI
            /// </summary>
            public static SafeAction<FrameMgr.OpenUIParams> ShowUIEventWithParam;

            /// <summary>
            /// 带参数打开UI并获取窗口GUIFrame对象
            /// </summary>
            public static SafeFunc<FrameMgr.OpenUIParams, GUIFrame> ShowUIAndGetFrameWithParam;

            /// <summary>
            /// 获取UI窗口
            /// </summary>
            public static SafeFunc<string, GUIFrame> GetFrameEvent;

            /// <summary>
            /// 关UI事件带参数
            /// </summary>
            public static SafeAction<FrameMgr.HideUIParams> HideUIWithParamEvent;

            /// <summary>
            /// 关闭UI
            /// </summary>
            public static SafeAction<string> HideUIEvent;

            /// <summary>
            /// 关闭并直接销毁UI
            /// </summary>
            public static SafeAction<string> HideFrameThenDestroyEvent;

            /// <summary>
            /// 虚化UI背景
            /// </summary>
            public static Action<bool> BlurUIBackground;

            /// <summary>
            /// 预加载窗口
            /// </summary>
            public static SafeAction<string> PreloadFrame;
        }


        /// <summary>
        /// 桥接各个模块的Event
        /// </summary>
        public static class BridgeEvent
        {
            /// <summary>
            /// 获取游戏主节点事件(解耦，避免各个模块之间直接引用调用)
            /// </summary>
            public static Func<GameObject> GetGameRootObject;

            /// <summary>
            /// 获取游戏入口对象
            /// </summary>
            public static Func<MonoBehaviour> GetGameRootBehaviour;

            /// <summary>
            /// 获取服务器地址
            /// </summary>
            public static Func<string> GetServerAddress;

            /// <summary>
            /// StartCoroutine
            /// </summary>
            public static Action<IEnumerator> StartCoroutine;
        }


        /// <summary>
        /// 输入相关事件
        /// </summary>
        public static class InputEvent
        {
            /// <summary>
            /// 单手指抬起
            /// </summary>
            public static SafeAction<Gesture> OnOneFingerTouchup;

            /// <summary>
            /// 双指外滑
            /// </summary>
            public static SafeAction<Gesture> OnPinOut;

            /// <summary>
            /// 双指内滑
            /// </summary>
            public static SafeAction<Gesture> OnPinIn;

            /// <summary>
            /// 单指滑动
            /// </summary>
            public static SafeAction<Gesture> OnSwipe;

            /// <summary>
            /// 开始滑动
            /// </summary>
            public static SafeAction<Gesture> OnSwipeBegin;

            /// <summary>
            /// 滑动结束
            /// </summary>
            public static SafeAction<Gesture> OnSwipeEnd;

            /// <summary>
            /// 手指抬起
            /// </summary>
            public static SafeAction<Gesture> OnTouchup;

            /// <summary>
            /// 手指接触屏幕
            /// </summary>
            public static SafeAction<Gesture> OnTouchDown;

            //有操作
            public static SafeAction OnTouchScene;
        }

        /// <summary>
        /// 系统事件
        /// </summary>
        public static class SystemEvents
        {
            //引擎初始化完成
            public static SafeAction OnEngineReady;

            /// <summary>
            /// 发送同步消息中
            /// </summary>
            public static SafeAction<int> OnSendingSyncMsg;

            /// <summary>
            /// 重试同步消息中
            /// </summary>
            public static SafeAction<int, NetworkModule.NetworkStatus> OnRetryingSyncMsg;

            //开启主线程，轮询重试消息
            public static SafeAction<bool> OnEnableRetry;


            public static SafeAction<bool> RetryPendingMsgs;
            /// <summary>
            /// 同步消息接收结果
            /// </summary>
            public static SafeAction<int, NetworkModule.NetworkStatus> OnSendingSyncMsgCallback;

            /// <summary>
            /// 获取玩家身份验证信息（token playerId）
            /// </summary>
            public static Func<string> FetchUserIdentification;

            /// <summary>
            /// 程序后台
            /// </summary>
            public static SafeAction<bool> OnApplicationPause;

            /// <summary>
            /// 网络错误或服务器异常
            /// </summary>
            public static SafeAction<int, string> NetWorkError;


            /// <summary>
            /// 得取此rsp，当时的请求消息
            /// </summary>
            public static SafeFunc<IMessage> GetRspPairReq;

            /// <summary>
            /// 发送耗时：<发送时间string,接受时间strng, 耗时毫秒，地址url，发送结果>
            /// </summary>
            public static SafeAction<string, string, long, string, byte> Listen_SendingCostTimeMS;

            /// <summary>
            /// 下一帧执行
            /// </summary>
            public static SafeAction<Action> SendTaskExecuteNextFrame;
        }

        /// <summary>
        /// 声音模块核心事件
        /// </summary>
        public static class AudioEvents
        {
            /// <summary>
            /// 停止所有音乐
            /// </summary>
            public static SafeAction StopAllAudio;

            /// <summary>
            /// 开始所有音乐
            /// </summary>
            public static SafeAction PlayAllAudio;


            public static SafeAction<Audio.AudioType, string> PlayAudio;

            /// <summary>
            /// PlayAudio And GetAudio 
            /// </summary>
            public static SafeAction<Audio.AudioType, string, Action<Audio>> PlayAndGetAudio;

            /// <summary>
            /// 暂停指定类型的声音
            /// </summary>
            public static SafeAction<Audio.AudioType> PauseAudio;

            /// <summary>
            /// 停止指定类型声音
            /// </summary>
            public static SafeAction<Audio.AudioType> StopAudio;

            /// <summary>
            /// 获取已存在的Audio对象
            /// </summary>
            public static SafeFunc<string, Audio> GetAudio;

            /// <summary>
            /// 通过key获取音频文件路径,用于引擎层一些通用音频的获取
            /// </summary>
            public static Func<string, string> FetchAudioPathByKey;

            /// <summary>
            /// 暂停所有声音
            /// </summary>
            public static SafeAction PauseAllAudio;

            /// <summary>
            /// 暂时指定类型的声音
            /// </summary>
            public static SafeAction<Audio.AudioType> PauseAudioByAudioType;


            public static SafeFunc<string, string> Usr_GetAudioPathBySn;
        }

        /// <summary>
        /// Shader相关事件
        /// </summary>
        public static class ShaderEvent
        {
            //获取Shader事件
            public static Func<string, Shader> GetShaderByName;
        }

        /// <summary>
        /// 摄像机事件
        /// </summary>
        public static class CameraEvents
        {
            /// <summary>
            /// 是否开启PostFx
            /// </summary>
            public static SafeAction<bool> EnablePostFx;

            /// <summary>
            /// 是否开启主场景摄像机
            /// </summary>
            public static Action<bool> EnableMainCamera;
        }

    }
}