using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOGUI;
using UnityEngine;

class CustomTweenRotation /*: TweenRotation*/
{

  /*  static new public CustomTweenRotation Begin(GameObject go, float duration, Quaternion rot)
    {
        CustomTweenRotation comp = UITweener.Begin<CustomTweenRotation>(go, duration);
        comp.from = comp.value.eulerAngles;
        comp.to = rot.eulerAngles;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = Quaternion.Slerp(Quaternion.Euler(from.x, from.y, from.z), Quaternion.Euler(to.x, to.y, to.z), factor);

    }*/

}

