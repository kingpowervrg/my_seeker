/********************************************************************
	created:  2018-3-28 10:12:53
	filename: EngineCoreEvents.cs
	author:	  songguangze@outlook.com
	
	purpose:  引擎层事件
*********************************************************************/
using System;


namespace EngineCore
{
    public class EngineCoreEvents
    {
        /// <summary>
        /// 引擎模块间的事件，用于解耦
        /// </summary>
        public static class EngineEvent
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
            /// 删除所有资源（关卡切换时）
            /// </summary>
            public static SafeAction RemoveAllAssetEvent;

            /// <summary>
            /// 尝试GC回收
            /// </summary>
            public static SafeAction TryGCCacheEvent;
        }
#if !RES_PROJECT
        /// <summary>
        /// 引擎逻辑层相关UI事件
        /// </summary>
        public static class UIEvent
        {
            /// <summary>
            /// 打开UI
            /// </summary>
            public static SafeAction<string> ShowUIEvent;

            /// <summary>
            /// 带参数的打开UI
            /// </summary>
            public static SafeAction<FrameMgr.OpenUIParams> ShowUIEventWithParam;

            /// <summary>
            /// 带参数打开UI并获取窗口Frame对象
            /// </summary>
            public static SafeFunc<FrameMgr.OpenUIParams, Frame> ShowUIAndGetFrameWithParam;

            /// <summary>
            /// 获取UI窗口
            /// </summary>
            public static SafeFunc<string, Frame> GetFrameEvent;

            /// <summary>
            /// 关UI事件带参数
            /// </summary>
            public static SafeAction<FrameMgr.HideUIParams> HideUIWithParamEvent;

            /// <summary>
            /// 关闭UI
            /// </summary>
            public static SafeAction<string> HideUIEvent;
        }
#endif
    }

}