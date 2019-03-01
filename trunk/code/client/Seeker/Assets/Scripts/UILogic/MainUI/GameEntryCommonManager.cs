using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class GameEntryCommonManager : Singleton<GameEntryCommonManager>
    {
        public bool m_needMainUITweener = true;
        public bool m_needTopareaUITweener = true;

        public bool m_mainUITweenerComplete = false;
        public bool m_toPareaTweenerComplete = false;
        public GameEntryCommonManager()
        {
            //GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
        }

        private void OnClearGuid()
        {
            m_needMainUITweener = true;
            m_needTopareaUITweener = true;
            m_mainUITweenerComplete = false;
            m_toPareaTweenerComplete = false;
        }

        public bool TweenerOver()
        {
            return m_mainUITweenerComplete & m_toPareaTweenerComplete;
        }

        public void SetMainUITweener(Action cb)
        {
            if (this.m_needMainUITweener)
            {
                if (cb != null)
                {
                    cb();
                }
                UnityEngine.Debug.Log("MainUI over");
                this.m_needMainUITweener = false;
            }
        }

        public void SetNeedTopareaUITweener(Action cb)
        {
            if (this.m_needTopareaUITweener)
            {
                if (cb != null)
                {
                    cb();
                }
                //UnityEngine.Debug.Log("Toparea over");
                this.m_needTopareaUITweener = false;
            }
        }

    }
}
