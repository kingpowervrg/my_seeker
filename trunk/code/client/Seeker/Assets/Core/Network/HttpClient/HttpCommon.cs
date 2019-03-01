using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore
{
    /// <summary>
    /// 发消息状态回调参数
    /// </summary>
    public class SendMessageCallback
    {
        public int MessageId;
        public NetworkModule.NetworkStatus Status;
    }
}
