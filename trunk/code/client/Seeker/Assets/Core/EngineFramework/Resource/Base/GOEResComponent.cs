using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore
{
    //#if UNITY_EDITOR
    //    public
    //#else
    //    internal
    //#endif
    public class ResComponent : GOEBaseComponent
    {
        internal ResourceMgr ResourceMgr
        {
            get { return Owner as ResourceMgr; }
        }

        internal virtual void OnLeaveScene()
        {

        }
    }
}
