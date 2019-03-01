using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EngineCore
{
    /// <summary>
    /// check状态不会随着点击改变，需要程序设置
    /// </summary>
    public class GameToggleButton : GameCheckBox
    {
        private bool m_cachedIsOn = false;
        private Toggle m_lastActiveToggle = null;

        protected override void OnInit()
        {
            base.OnInit();
            tog.SetValue(false);
            AddClickCallBack(OnToggleClick);
            AddChangeCallBack(onToggleChanged);
        }

        private void OnToggleClick(GameObject toggleButton)
        {
            m_cachedIsOn = tog.isOn;
            if (tog.group != null)
                m_lastActiveToggle = ActivedToggle;
        }

        //AllowSwitchOff的情况下，不允许单点取消
        public void onToggleChanged(bool isOn)
        {
            if (m_cachedIsOn && !ActivedToggle)
            {
                SetValueWithoutOnChangedNotify(m_cachedIsOn);
            }

        }

        private Toggle ActivedToggle => tog.group?.ActiveToggles().SingleOrDefault();
    }
}
