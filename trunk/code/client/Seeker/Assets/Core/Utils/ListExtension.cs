using System;
using System.Collections.Generic;

namespace EngineCore
{
    static class ListExtension
    {
        public static int SafeCount<T>(this List<T> evt)
        {
            if (evt != null)
                return evt.Count;
            return 0;
        }
    }

}
