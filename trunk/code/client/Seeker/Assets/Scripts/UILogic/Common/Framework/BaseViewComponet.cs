using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;

namespace SeekerGame
{
    public abstract class BaseViewComponet<T> : GameUIComponent, IViewComponent<T> where T : UILogicBase

    {
        public T CurViewLogic()
        {
            return LogicHandler as T;
        }


    }
}
