using System.Collections.Generic;
namespace SeekerGame.NewGuid
{
    public class GuidNewFunctionBase
    {
        //public bool m_status = false;
        public FuncState m_status = FuncState.None;
        protected bool m_isReseting = false;
        protected bool m_isClearing = false;
        protected long m_funcID;
        public long FuncID
        {
            get {
                return m_funcID;
            }
        }

        protected GuidNewBase m_guidBase = null;
        private List<GuidNewFunctionBase> childFunc = new List<GuidNewFunctionBase>();
        public virtual void OnInit(long funcID, string[] param)
        {
            this.m_funcID = funcID;
            
        }

        public virtual void ResetFunc(bool isRetainFunc = true)
        {
            if (isRetainFunc)
            {
                this.m_isReseting = (this.m_status == FuncState.None) && !m_guidBase.IsComplete;
            }
            else
            {
                this.m_isReseting = false;
            }

            this.m_status = FuncState.None;
            
            for (int i = 0; i < childFunc.Count; i++)
            {
                childFunc[i].m_status = FuncState.None;
                childFunc[i].ResetFunc(isRetainFunc);
            }
        }

        public void SetGuidBase(GuidNewBase guid)
        {
            this.m_guidBase = guid;
            CreateChildFunc();
        }

        public virtual void OnLoadRes()
        {

        }

        public virtual void OnRun()
        {
            if (m_status != FuncState.None)
            {
                UnityEngine.Debug.Log(this.m_guidBase.GroupID +  "   hhhhhh  func  " + FuncID);
                return;
            }
            OnExecute();
        }

        public virtual void OnExecute()
        { }

        public virtual void OnDestory(FuncState funcState = FuncState.Complete)
        {
            UnityEngine.Debug.Log("func complete : " + m_funcID);
            this.m_status = funcState;//FuncState.Complete;
            //CreateChildFunc();
            if (this.m_status == FuncState.CompleteError)
            {
                this.m_guidBase.OnNotComplete();
            }
            else
            {
                this.m_guidBase.CheckOnFinish();
            }
            if (funcState == FuncState.Complete)
            {
                ExecuteChildFunc();
            }
            if (this.m_isReseting)
            {
                this.m_status = FuncState.None;
                this.m_isReseting = false;
            }
        }

        private void CreateChildFunc()
        {
            ConfGuidNewFunction confGuidFunc = ConfGuidNewFunction.Get(this.m_funcID);
            if (confGuidFunc == null || confGuidFunc.nextFuncID == null)
            {
                return;
            }
            for (int i = 0; i < confGuidFunc.nextFuncID.Length; i++)
            {
                GuidNewFunctionBase funcBase = GuidNewFunctionFactory.Instance.CreateFunctionById(confGuidFunc.nextFuncID[i]);
                funcBase.SetGuidBase(m_guidBase);
                //funcBase.OnRun();
                childFunc.Add(funcBase);
                //  ConfGuidNewFunction.Get(confGuidFunc.nextFuncID[i]);
            }
        }

        private void ExecuteChildFunc()
        {
            for (int i = 0; i < childFunc.Count; i++)
            {
                childFunc[i].OnRun();
            }
        }


        public FuncState CheckNodeFuncFinish()
        {
            for (int i = 0; i < childFunc.Count; i++)
            {
                //UnityEngine.Debug.Log(childFunc[i].FuncID + "  " + childFunc[i].m_status);
                if (childFunc[i].m_status != FuncState.Complete)
                {
                    return childFunc[i].m_status;
                }
                FuncState funcStatus = childFunc[i].CheckNodeFuncFinish();
                if (funcStatus != FuncState.Complete)
                {
                    return funcStatus;
                }
            }
            return FuncState.Complete;
        }

        public void ForceFuncFinish()
        {
            m_status = FuncState.Complete;
            for (int i = 0; i < childFunc.Count; i++)
            {
                childFunc[i].m_status = FuncState.Complete;
                childFunc[i].ForceFuncFinish();
            }
        }

        public virtual void ForceFuncDestory()
        {
            //m_status = FuncState.Complete;

            for (int i = 0; i < childFunc.Count; i++)
            {
                childFunc[i].ForceFuncDestory();
            }
        }

        public virtual void Tick(float time)
        { }

        public virtual void ClearFunc()
        {
            this.m_isClearing = true;
            for (int i = 0; i < childFunc.Count; i++)
            {
                childFunc[i].ClearFunc();
            }
        }
    }
}
