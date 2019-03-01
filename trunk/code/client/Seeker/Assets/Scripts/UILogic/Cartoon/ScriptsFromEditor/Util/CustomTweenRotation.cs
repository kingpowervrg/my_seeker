using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CustomTweenRotation : TweenRotationEuler
{

    static public CustomTweenRotation Begin(GameObject go, float duration, Quaternion rot)
    {
        //todo://
        CustomTweenRotation comp = go.GetOrAddComponent<CustomTweenRotation>();
        return comp;


        /*     UITweenerBase.Begin<CustomTweenRotation>(go, duration);
             comp.From = comp.value.eulerAngles;
             comp.to = rot.eulerAngles;

             if (duration <= 0f)
             {
                 comp.Sample(1f, true);
                 comp.enabled = false;
             }
             return comp;* /*/
    }

    /* protected override void OnUpdate(float factor, bool isFinished)
     {
         value = Quaternion.Slerp(Quaternion.Euler(From.x, From.y, From.z), Quaternion.Euler(to.x, to.y, to.z), factor);

     }*/

}

