using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewBase
    {
        protected long m_currentID;

        protected ConfGuidNew m_currentConf;

        protected GuidNewFunctionBase[] m_func = null; //执行条件
        protected GuidNewPreFunctionBase[] m_preFunc = null; //运行前置条件
        protected bool m_isRunning = false;
        protected bool m_isComplete = false;

        public bool IsComplete
        {
            get
            {
                return m_isComplete;
            }
        }

        public int GroupID
        {
            get {
                return m_currentConf.groupId;
            }
        }

        public void SetGuidID(long currentID)
        {
            this.m_currentID = currentID;
            m_currentConf = ConfGuidNew.Get(m_currentID);
            int preFuncCount = 0;
            if (m_currentConf.preFuncId != null)
            {
                preFuncCount = m_currentConf.preFuncId.Length;
            }
            if (m_currentConf.initFuncIds != null)
            {
                int initFuncCount = m_currentConf.initFuncIds.Length;
                for (int i = 0; i < initFuncCount; i++)
                {
                    GuidNewFunctionBase initFunc = GuidNewFunctionFactory.Instance.CreateFunctionById(m_currentConf.initFuncIds[i]);
                    initFunc.SetGuidBase(this);
                    initFunc.OnRun();
                }
            }

            m_preFunc = new GuidNewPreFunctionBase[preFuncCount];
            for (int i = 0; i < preFuncCount; i++)
            {
                m_preFunc[i] = GuidNewFunctionFactory.Instance.CreatePreFunctionById(m_currentConf.preFuncId[i]);
            }

            int executeFuncCount = m_currentConf.funcIds.Length;
            m_func = new GuidNewFunctionBase[executeFuncCount];
            for (int i = 0; i < executeFuncCount; i++)
            {
                m_func[i] = GuidNewFunctionFactory.Instance.CreateFunctionById(m_currentConf.funcIds[i]);
                m_func[i].SetGuidBase(this);
            }

        }

        public virtual void OnInitStart()
        {
            if (!GuidNewManager.Instance.GetProgressByIndex(m_currentConf.groupId))
            {
                OnTryStart();
            }
        }

        private void CheckStatus()
        {
            if (this.m_isRunning || this.IsComplete)
            {
                return;
            }
            if (m_preFunc.Length == 0)
            {
                OnStart();
                return;
            }
            int temp = 0;
            for (int i = 0; i < m_preFunc.Length; i++)
            {
                m_preFunc[i].OnCheck(()=> {
                    temp++;
                    if (temp == m_preFunc.Length)
                    {
                        OnStart();
                    }
                });
            }
        }

        public virtual void OnTryStart()
        {
            CheckStatus();

        }

        public virtual void OnStart()
        {
            if (this.m_isRunning || this.IsComplete)
            {
                return;
            }
            this.m_isRunning = true;
            for (int i = 0; i < m_func.Length; i++)
            {
                m_func[i].OnRun();
            }
           
        }

        public void CheckOnFinish()
        {
            if (!m_currentConf.funcFinsh || this.m_isComplete || m_func == null)
            {
                return;
            }
            for (int i = 0; i < m_func.Length; i++)
            {
                //UnityEngine.Debug.Log(m_func[i].FuncID + "  " + m_func[i].m_status);
                if (!CheckFuncStatus(m_func[i].m_status))
                {
                    return;
                }
                FuncState funcState = m_func[i].CheckNodeFuncFinish();
                if (!CheckFuncStatus(funcState))
                {
                    return;
                }
            }
            CheckGroupComplete();
        }

        public void CheckGroupComplete()
        {
            this.m_isComplete = true;
            if (!GuidNewManager.Instance.OnCheckGroupComplete(GroupID))
            {
                OnComplete();
            }
            else
            {
                GuidNewManager.Instance.OnGroupComplete(GroupID);
                //OnEnd();
            }
        }

        //强制函数完成
        public void ForceFuncFinish()
        {
            for (int i = 0; i < m_func.Length; i++)
            {
                m_func[i].ForceFuncFinish();

            }
        }

        private bool CheckFuncStatus(FuncState funcState)
        {
            if (funcState == FuncState.Complete)
            {
                return true;
            }
            else if(funcState == FuncState.CompleteError)
            {
                OnNotComplete();
                return false;
            }
            return false;
        }
        //失败
        public virtual void OnNotComplete()
        {
            GuidNewManager.Instance.OnResetGuidByGroupID(GroupID);
        }
        //重置
        public virtual void OnReset(bool retainFunc = true)
        {
            GameEvents.UI_Guid_Event.OnClearMaskEffect.SafeInvoke(false); //true
            UnityEngine.Debug.Log("reset group :" + GroupID);
            
            for (int i = 0; i < m_func.Length; i++)
            {
                m_func[i].ResetFunc(retainFunc);
            }
            this.m_isRunning = false;
            this.m_isComplete = false;
        }


        //当前完成
        public virtual void OnComplete()
        {
            this.m_isRunning = false;
            this.m_isComplete = true;
            //GameEvents.UI_Guid_Event.OnGuidNewNext.SafeInvoke(m_currentConf.nextId);
            GameEvents.UI_Guid_Event.OnClearMaskEffect.SafeInvoke(false); //true
            GameEvents.UI_Guid_Event.OnGuidNewComplete.SafeInvoke(m_currentID);
            GuidNewManager.Instance.OnReflashGuidStatus();
            
            UnityEngine.Debug.Log("end guid :" + m_currentID);
        }

        //整个结束
        public virtual void OnEnd()
        {
            this.m_isComplete = true;
            GuidNewManager.Instance.SetProgressByIndex(m_currentConf.groupId);
            GameEvents.UI_Guid_Event.OnGuidNewNext.SafeInvoke(m_currentConf.nextId);
            GameEvents.UI_Guid_Event.OnClearMaskEffect.SafeInvoke(false); //true
            this.m_isRunning = false;
            OnDestoryFunc();
            GuidNewManager.Instance.RemoveGuid(m_currentID);
            UnityEngine.Debug.Log("remove guid :" + m_currentID);
            GameEvents.UI_Guid_Event.OnGuidNewComplete.SafeInvoke(m_currentID);
            GuidNewManager.Instance.OnReflashGuidStatus();
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnBlock.SafeInvoke(false);
        }

        public void OnSaveCurrentProgress()
        {
            GuidNewManager.Instance.ForceSaveProgress(m_currentConf.groupId);
        }

        public virtual void OnDestoryFunc()
        {
            for (int i = 0; i < m_func.Length;i++)
            {
                m_func[i].ForceFuncDestory();
            }
        }

        public virtual void OnClear()
        {
            UnityEngine.Debug.Log("Guid Clear -------------");
            for (int i = 0; i < m_func.Length; i++)
            {
                m_func[i].ClearFunc();
            }
            m_func = null;
            m_preFunc = null;
        }

    }
}
