using EngineCore;
using UnityEngine;
using System.Collections.Generic;

namespace SeekerGame
{
    public class GuidModule : AbstractModule
    {
        private static GuidModule instance;
        private Dictionary<string, GUIFrame> m_OpenUI = new Dictionary<string, GUIFrame>(); //所有已经打开的界面

        private List<GuidUI> m_cacheGuidUI = new List<GuidUI>(); //所有缓存的引导流程
        private List<GuidUI> m_removeGuidUI = new List<GuidUI>();
        public static GuidModule Instance
        {
            get {
                return instance;
            }
        }

        public override void Start()
        {
            base.Start();
            instance = this;
            GameEvents.UI_Guid_Event.OnOpenUI += OpenUI;
            GameEvents.UI_Guid_Event.OnCloseUI += CloseUI;
        }

        protected override void setEnable(bool value)
        {
            base.setEnable(value);
            
        }

        public override void Dispose()
        {
            base.Dispose();
            GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
            GameEvents.UI_Guid_Event.OnCloseUI -= CloseUI;
        }

        private void OpenUI(GUIFrame frame)
        {
            string frameName = frame.ResName;
            if (!m_OpenUI.ContainsKey(frameName))
            {
                m_OpenUI.Add(frameName, frame);
            }
            else
            {
                m_OpenUI[frameName] = frame;
            }
        }

        private void CloseUI(GUIFrame frame)
        {
            string frameName = frame.ResName;
            if (m_OpenUI.ContainsKey(frameName))
            {
                m_OpenUI.Remove(frameName);
                Debug.Log("remove  :" + frameName);
            }
        }

        public void AddGuidUICache(GuidUI guidUI)
        {
            m_cacheGuidUI.Add(guidUI);
        }

        public override void Update()
        {
            base.Update();
            
            for (int i = 0; i < m_cacheGuidUI.Count; i++)
            {
                GuidUI guidUI = m_cacheGuidUI[i];
                string uiName = guidUI.GetUIName();
                if (m_OpenUI.ContainsKey(uiName))
                {
                    ConfGuid confguid = guidUI.GetCurrentConf();
                    if (confguid.hideUINode.Length > 0)
                    {
                        for (int j = 0; j < confguid.hideUINode.Length; j++)
                        {
                            if (m_OpenUI.ContainsKey(confguid.hideUINode[j]))
                            {
                                GUIFrame frame = m_OpenUI[confguid.hideUINode[j]];
                                frame.LogicHandler.OnGuidShow();
                            }
                        }
                    }
                    guidUI.OnOpenUIAction(m_OpenUI[uiName]);
                    m_removeGuidUI.Add(guidUI);
                }
            }
            for (int i = 0; i < m_removeGuidUI.Count; i++)
            {
                m_cacheGuidUI.Remove(m_removeGuidUI[i]);
                
            }
            m_removeGuidUI.Clear();
        }

    }
}
