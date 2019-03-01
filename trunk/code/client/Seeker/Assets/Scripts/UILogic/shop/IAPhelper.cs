using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{



    public class IAPhelper
    {
        public static List<ConfCharge> GetGoods()
        {
            List<ConfCharge> all_goods = ConfCharge.array;

            List<ConfCharge> ret = new List<ConfCharge>();

            foreach (var item in all_goods)
            {
                if (1 != item.type)
                    continue;
#if UNITY_ANDROID
                if ((int)(IAP_PLATFROM_TYPE.E_GOOGLE_PLAY) == item.source)
                    ret.Add(item);
#elif UNITY_IOS
                if ((int)(IAP_PLATFROM_TYPE.E_APPLE_STORE) == item.source)
                    ret.Add(item);
#else
                if ((int)(IAP_PLATFROM_TYPE.E_GOOGLE_PLAY) == item.source)
                    ret.Add(item);
#endif

            }

            return ret;
        }
    }
}
