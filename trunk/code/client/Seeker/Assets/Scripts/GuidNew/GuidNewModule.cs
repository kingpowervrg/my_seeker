using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewModule : AbstractModule
    {
        private static GuidNewModule instance;
        public static GuidNewModule Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<string, GUIFrame> m_OpenUI = new Dictionary<string, GUIFrame>(); //所有已经打开的界面
        private List<GuidNewFunctionBase> m_cacheFunction = new List<GuidNewFunctionBase>(); //所有缓存的函数
        private List<GuidNewFunctionBase> m_removeFunction = new List<GuidNewFunctionBase>();

        public GUIFrame m_guidFrame = null;
        public GUIFrame guidFrame
        {
            get {
                if (m_guidFrame == null)
                {
                    m_guidFrame = GetFrameByResName(UIDefine.UI_GUID);
                }
                return m_guidFrame;
            }
        }
        public override void Start()
        {
            base.Start();
            instance = this;
            GameEvents.UI_Guid_Event.OnOpenUI += OpenUI;
            GameEvents.UI_Guid_Event.OnCloseUI += CloseUI;
            GameEvents.UI_Guid_Event.OnGuidNewEnd += OnGuidEnd;
            GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
        }

        public override void Dispose()
        {
            base.Dispose();
            GameEvents.UI_Guid_Event.OnOpenUI -= OpenUI;
            GameEvents.UI_Guid_Event.OnCloseUI -= CloseUI;
            GameEvents.UI_Guid_Event.OnGuidNewEnd -= OnGuidEnd;
            GameEvents.UI_Guid_Event.OnClearGuid -= OnClearGuid;
        }

        public override void Update()
        {
            base.Update();
            this.Tick(Time.deltaTime);
        }

        private void OpenUI(GUIFrame frame)
        {
            string frameName = frame.ResName;
            if (frameName.Equals(UIDefine.UI_SYNC_LOADING))
            {
                return;
            }

            if (frameName.Equals(UIDefine.UI_GUEST_LOGIN) || frameName.Equals(UIDefine.UI_GUIDLOGIN))
            {
                ProloguePlayVideoManager.UnRegisterGuest();
            }

            if (!m_OpenUI.ContainsKey(frameName))
            {
                m_OpenUI.Add(frameName, frame);
                OnTryStart();
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
                //Debug.Log("remove  :" + frameName);
            }
        }

        public GUIFrame GetFrameByResName(string resName)
        {
            if (m_OpenUI.ContainsKey(resName))
            {
                return m_OpenUI[resName];
            }
            //Debug.LogError("error  " + resName);
            return null;
        }
        //当前界面是否打开
        public bool IsFrameOpen(string resName)
        {
            if (m_OpenUI.ContainsKey(resName))
            {
                return true;
            }
            return false;
        }

        public void PushFunction(GuidNewFunctionBase func)
        {
            m_cacheFunction.Add(func);
        }

        public void RemoveFunction(GuidNewFunctionBase func)
        {
            if (m_cacheFunction.Contains(func))
            {
                m_cacheFunction.Remove(func);
            }
        }

        public void Tick(float time)
        {
            for (int i = 0; i < m_cacheFunction.Count; i++)
            {
                m_cacheFunction[i].Tick(time);
            }
        }

        public void OnGuidEnd(long guidID)
        {
            if (GuidNewManager.Instance.Guid.ContainsKey(guidID))
            {
                GuidNewManager.Instance.Guid[guidID].OnEnd();
            }
            OnTryStart();
        }

        public void OnTryStart()
        {
            if (Instance == null)
            {
                return;
            }
            foreach (var kv in GuidNewManager.Instance.Guid)
            {
                if (kv.Value == null)
                {
                    continue;
                }
                kv.Value.OnTryStart();
            }
        }

        private void OnClearGuid()
        {
            this.m_cacheFunction.Clear();
        }

    }

    
}
