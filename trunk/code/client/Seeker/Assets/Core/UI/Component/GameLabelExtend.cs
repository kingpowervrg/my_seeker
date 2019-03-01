using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace EngineCore
{
    public class GameLabelExtend : GameLabel
    {
        private bool m_isRoll = false;
        private int m_startNumber;
        private int m_endNumber;
        private float m_deltaTime = 1f;
        private float m_curTime = 0f;
        private float m_totalTime = 0f;
        private float m_speed;
        private bool m_isAdd = true;
        private int m_once = 1;
        protected override void OnInit()
        {
            base.OnInit();
            //need
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (m_isRoll)
            {
                m_curTime += Time.deltaTime;
                m_totalTime += Time.deltaTime;
                if (m_curTime >= m_speed)
                {
                    m_startNumber+= m_once;
                    Text = m_startNumber.ToString();
                    m_curTime = 0f;
                }
                if (m_totalTime >= m_deltaTime)
                {
                    m_isRoll = false;
                    Text = m_endNumber.ToString();
                }
            }
        }

        public void SetChangeTextRoll(int startNumber,int endNumber)
        {
            m_isRoll = true;
            m_startNumber = startNumber;
            m_endNumber = endNumber;
            m_isAdd = true;
            m_once = 1;
            if (m_startNumber > m_endNumber)
            {
                m_isAdd = false;
                m_once = -1;
            }
            Text = m_startNumber.ToString();
            m_speed = m_deltaTime / (float)Math.Abs(m_endNumber - m_startNumber);
            m_curTime = 0f;
            m_totalTime = 0f;
        }
    }

}
