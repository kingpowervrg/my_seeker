/********************************************************************
	created:  2018-4-9 16:8:32
	filename: GameSliderButton.cs
	author:	  songguangze@outlook.com
	
	purpose:  滑动开关UI控件
*********************************************************************/

using GOGUI;
using UnityEngine.Events;

namespace EngineCore
{
    public class GameSliderButton : GameCheckBox
    {
        private UITweenerBase[] UITweeners = null;
        private bool m_isOnWhileInit = false;

        protected override void OnInit()
        {
            base.OnInit();

            UITweeners = gameObject.GetComponentsInChildren<UITweenerBase>(true);

            tog.onValueChanged.AddListener(OnToggleValueChanged);

            this.m_isOnWhileInit = tog.isOn;
        }

        private void OnToggleValueChanged(bool value)
        {
            for (int i = 0; i < UITweeners.Length; ++i)
            {
                if (value)
                {
                    if (!m_isOnWhileInit)
                        UITweeners[i].PlayForward();
                    else
                        UITweeners[i].PlayBackward();
                }
                else
                {
                    if (!m_isOnWhileInit)
                        UITweeners[i].PlayBackward();
                    else
                        UITweeners[i].PlayForward();
                }
            }
        }

        
    }
}