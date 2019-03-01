using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using Google.Protobuf;

namespace SeekerGame
{
    public interface IViewComponentLogic
    {
        void OnScResponse(object s);

        void OnPackageRequest(IMessage imsg, params object[] msg_params);
    }
}