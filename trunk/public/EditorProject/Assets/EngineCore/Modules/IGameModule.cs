//模块基类，通常模块是一个逻辑上独立的功能的
using System;
namespace EngineCore
{
    /// <summary>
    /// 游戏Module
    /// </summary>
    public interface IGameModule
    {
        void Start();
        void Update();
        void LateUpdate();
        void Dispose();
        bool Enable { get; }
        byte ModuleType { get; }

        bool AutoStart { get; }
    }
}

