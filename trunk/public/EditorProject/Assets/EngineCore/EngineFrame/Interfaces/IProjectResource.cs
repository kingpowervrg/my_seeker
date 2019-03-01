using System;
using GOEngine.Implement;
namespace EngineCore
{
    public interface IProjectResource
    {
        /// <summary>
        /// 切换场景前清理资源
        /// </summary>
        void OnLeaveScene();
        /// <summary>
        /// 确保某Asset的依赖资源已
        /// </summary>
        /// <param name="name">Asset名</param>
        void EnsureDependencies(string name);
        /// <summary>
        /// 从AssetBundle加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        void GetAsset(string name, Action<string, global::UnityEngine.Object> callback, LoadPriority priority = LoadPriority.Default);
        /// <summary>
        /// 预加载动作资源
        /// </summary>
        /// <param name="name">动作资源名</param>
        /// <param name="callBack">回掉</param>
        void PreLoadActAsset(string name, Action<string> callBack = null);

        /// <summary>
        /// 根据资源名得到AssetBundle名
        /// </summary>
        /// <param name="assetname"></param>
        /// <returns></returns>
        string GetBundleName(string assetname);
        /// <summary>
        /// 加载场景资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="priority"></param>
        void GetScene(string name, Action callBack = null, LoadPriority priority = LoadPriority.MostPrior);
        /// <summary>
        ///  add szh
        ///  加载Addivie场景
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="priority"></param>
        void GetAdditiveScene(string name, Action callBack = null, LoadPriority priority = LoadPriority.MostPrior);
        /// <summary>
        /// 从AssetBundle加载字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void GetString(string name, Action<string, string> callback);
        /// <summary>
        /// 加载文本资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        void GetText(string name, Action<string, string> callback, bool fromStream = false);
        /// <summary>
        /// 从本地加载二进制文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="inStream"></param>
        void GetBytes(string name, Action<string, byte[]> callback, bool fromStream = false);
        /// <summary>
        /// 提前加载AssetBundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        void PreloadBundle(string name, Action<string, global::UnityEngine.AssetBundle> callback, LoadPriority priority = LoadPriority.Default, bool dependencyResource = false);
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="name"></param>
        void ReleaseAsset(string name, UnityEngine.Object obj);
        /// <summary>
        /// 加载bundlemap.txt
        /// </summary>
        void LoadBundleMap(Action onLoadBundlemap);
        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="name"></param>
        void RemoveAsset(string name, bool force = true, bool removeInBundleAsset = true);
        void RemoveAllAsset();

        void SetAssetReleasePriority(string name, int _Priority, UnityEngine.Object obj);

        void ReleaseAssetCallback(string name, Action<string, global::UnityEngine.Object> func);
        /// <summary>
        /// 资源最大同时加载数
        /// </summary>
        int MaxLoaderCount { set; }
        /// <summary>
        /// 加载超时秒数
        /// </summary>
        int LoadMaxTime { set; }
        int ScenePoolCount { set; }
        /// <summary>
        /// 添加加载进度条
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <param name="assetNames"></param>
        /// <param name="handler"></param>
        /// <param name="onEnd"></param>
        void SetProgress(string[] bundleNames, string[] assetNames, Action<float> handler, Action onEnd = null);
        void SetProgress(string[] wwwNames, Action<float> handler, Action onEnd = null);
        void RegisterAsset(string name, UnityEngine.Object obj);

        /// <summary>
        /// 取得当前缓存总量
        /// </summary>
        /// <returns></returns>
        int GetCurrentCacheSize();

        /// <summary>
        /// 取得缓存的Bundle数
        /// </summary>
        /// <returns></returns>
        int GetCacheCount();

        /// <summary>
        /// 取得期望缓存量
        /// </summary>
        /// <returns></returns>
        int GetPreferedCacheSize();

        /// <summary>
        /// 设置缓存量
        /// </summary>
        /// <param name="size"></param>
        void SetPreferedCacheSize(int size);

        /// <summary>
        /// 设置是是否以低优先级进行异步加载Asset
        /// </summary>
        /// <param name="value"></param>
        void SetLowAsyncLoadPriority(bool value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.List<string> DumpAssetBundleCacheInfo();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleName"></param>
        void SetAssetBundlePersistent(string bundleName);

        /// <summary>
        /// 设置指定Asset为持久的
        /// </summary>
        /// <param name="assetName"></param>
        void SetAssetPersistent(string bundleName);

        /// <summary>
        /// 
        /// </summary>
        void ForceGCCache();

        /// <summary>
        /// 尝试GC
        /// </summary>
        void TryGCCache();
        /// <summary>
        /// 
        /// </summary>
        void ResetCacheProfiler();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string DumpCacheProfilerInfo();

        int GetPendingBundleCount();

        int GetLoadingBundleIn10Seconds();

        int MaximalInstantiationPerFrame { get; set; }

        /// <summary>
        /// 常规GC量
        /// </summary>
        float RegularGCFactor { get; set; }
        /// <summary>
        /// 切场景GC量
        /// </summary>
        float SceneGCFactor { get; set; }

        /// <summary>
        /// 是否用Assetbundle.LoadFromFileAsync加载bundle，而不是www
        /// </summary>
        bool UseAssetBundleLoadFromFile { get; set; }
        /// <summary>
        /// 客户端同意从远程服务器上加载文件，用于热更新
        /// </summary>
        void GoGetWWWFromServer();
    }
}
