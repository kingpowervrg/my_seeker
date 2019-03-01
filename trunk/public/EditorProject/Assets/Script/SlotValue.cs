using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class SlotValue : MonoBehaviour
    {
        public AnimationCurve m_speedCure;

        public float m_totalTime = 3f;

        public float getSpeed(float time)
        {
            return m_speedCure.Evaluate(time);
        }
    }
}