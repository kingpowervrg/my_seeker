using System.Collections.Generic;
namespace SeekerGame
{
    interface IUBSBridge
    {
        void StartBridge();
        void DisposeBridge();

        void LogEvent(UBSEventKeyName key_);
        void LogEvent(UBSEventKeyName key_, float? value4sum_);
        void LogEvent(UBSEventKeyName key_, float? value4sum_, Dictionary<UBSParamKeyName, object> params_);

        /// <summary>
        /// 是否初始化成功
        /// </summary>
        bool IsInitialized { get; set; }

        /// <summary>
        /// 附加基本信息
        /// </summary>
        /// <returns></returns>
        Dictionary<UBSParamKeyName, object> AppendBaseData();
    }
}
