using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using Google.Protobuf;
using GOEngine;

namespace SeekerGame
{
    public abstract class BaseViewComponetLogic : UILogicBase, IViewComponentLogic

    {
        public void OnScRequest(IMessage imsg, params object[] msg_params)
        {
            OnPackageRequest(imsg, msg_params);

            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(imsg);
        }

        public void OnScAsyncRequest(IMessage imsg, params object[] msg_params)
        {
            OnPackageRequest(imsg, msg_params);

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(imsg);
        }

        public void OnScHalfAsyncRequest(IMessage imsg, params object[] msg_params)
        {
            OnPackageRequest(imsg, msg_params);

            GameEvents.NetWorkEvents.SendHalfSyncMsg.SafeInvoke(imsg);
        }

        public virtual void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

        }

        public virtual void OnScResponse(object s)
        {
            DebugUtil.Log("收到消息 " + ((IMessage)s).Descriptor.Name);
        }


    }
}
