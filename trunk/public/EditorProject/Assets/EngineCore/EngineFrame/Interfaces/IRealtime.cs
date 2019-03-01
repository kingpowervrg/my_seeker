namespace EngineCore
{
    /// <summary>
    /// 游戏真实时间
    /// </summary>
    public interface IRealtime
    {
        /// <summary>
        /// 每帧的间隔
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// 当前的时间 秒为单位
        /// </summary>
        float RealTime
        {
            get;
            set;
        }

        /// <summary>
        /// 当前的时间 毫秒为单位
        /// </summary>
        long RealTimeMill { get; }

        /// <summary>
        /// 时间开始
        /// </summary>
        void Start();

        /// <summary>
        /// 时间的Tick
        /// </summary>
        void Update();
    }
}
