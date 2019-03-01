using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ToggleCheckMarkView : GameLoopItem
    {
        long m_id;
        string m_icon_name = null;
        bool m_checked = false;
        GameToggleButton m_toggle;
        GameImage m_icon;

        SafeAction<bool, long> m_on_toggle_checked;

        protected override void OnInit()
        {
            base.OnInit();
            m_toggle = Make<GameToggleButton>("Toggle");
            m_icon = m_toggle.Make<GameImage>("Background:Image");
        }

        public override void Dispose()
        {
            m_toggle.RemoveChangeCallBack(OnToggleChecked);
        }

        public void Refresh(long id_, string icon_, bool checked_, Action<bool, long> OnCheckd)
        {
            m_on_toggle_checked = OnCheckd;
            m_id = id_;
            m_icon_name = icon_;
            //m_checked = checked_;
            if (checked_)
            {
                OnToggleChecked(true);
            }
        }

        private bool isCheck = false;
        void OnToggleChecked(bool check_)
        {
            isCheck = check_;
            m_on_toggle_checked.SafeInvoke(check_, m_id);

        }

        protected override void OnLoopItemBecameVisible()
        {
            if (null == m_icon_name)
                return;
            m_toggle.AddChangeCallBack(OnToggleChecked);
            m_icon.Sprite = m_icon_name;
            m_toggle.SetValueWithoutOnChangedNotify(isCheck);
        }

        protected override void OnLoopItemBecameInvisible()
        {
            m_toggle.SetValueWithoutOnChangedNotify(false);
            m_toggle.RemoveChangeCallBack(OnToggleChecked);
        }
    }
}
