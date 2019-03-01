using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;

namespace SeekerGame
{
    public interface IViewComponent<T> where T : UILogicBase
    {
        T CurViewLogic();

        //Action Act_Preload { get; }
        //Action<object> Act_Show { get; }
        //Action Act_Hide { get; }


    }


}
