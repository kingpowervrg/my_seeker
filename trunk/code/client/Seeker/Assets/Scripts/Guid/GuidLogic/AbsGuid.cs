using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public abstract class AbsGuid
    {
        protected long m_CurID; //当前节点ID
        protected long m_RootID; //根节点ID
        protected int m_keyID; //

        protected ConfGuid m_CurConf;
        protected ConfGuid m_RootConf;

        protected bool m_isAuto = false;

        private void SetCurrentID(long curID)
        {
            this.m_CurID = curID;
            m_CurConf = ConfGuid.Get(this.m_CurID);
        }

        private void SetRootID(long rootID)
        {
            this.m_RootID = rootID;
            m_RootConf = ConfGuid.Get(this.m_RootID);
        }

        public ConfGuid GetCurrentConf()
        {
            return this.m_CurConf;
        }

        public long GetCurID()
        {
            return this.m_CurID;
        }
        public long GetRootID()
        {
            return this.m_RootID;
        }

        public int GetKeyID()
        {
            return this.m_keyID;
        }

        public void InitGuidInfo(int keyID, long curID,long rootID)
        {
            SetCurrentID(curID);
            SetRootID(rootID);
            this.m_keyID = keyID;
            //GetNextRootID();
            if (m_CurConf.triggerMethod == 0)
            {
                m_isAuto = true;
            }
        }

        public bool IsRoot()
        {
            return m_RootID == m_CurID;
        }
        //即将触发的根节点
        public long GetNextRootID()
        {
            if (IsRoot())
            {
                long nextId = m_CurConf.nextId;
                ConfGuid nextConf = ConfGuid.Get(nextId);
                while (nextConf != null && !nextConf.isMain)
                {
                    nextConf = ConfGuid.Get(nextConf.nextId);
                }
                if (nextConf != null && nextConf.isMain)
                {
                    return nextConf.id;
                }
            }
            return -1;
        }

        private bool GetStatus()
        {
            return GuidManager.Instance.GetProgressByIndex(m_keyID);
        }

        public void StartAuto()
        {
            if (m_isAuto)
            {
                StartGuid();
            }
        }

        protected bool CheckFinish()
        {
            if (m_CurConf != null && m_CurConf.nextId != 0)
            {
                ConfGuid nextConf = ConfGuid.Get(m_CurConf.nextId);
                if (nextConf != null && nextConf.isMain)
                {
                    Debug.Log("start next root : " + m_CurConf.nextId);
                    GuidFactory.Instance.RemoveGuidByID(m_CurConf.id);
                    GuidFactory.Instance.CreateRootGuid(m_CurConf.nextId);
                    return true;
                }
                return false;
            }
            return true;
        }

        protected void MoveToNext()
        {
            Destory();
            if (!CheckFinish())
            {
                long rootID = m_RootID;
                ConfGuid confGuid = ConfGuid.Get(m_CurConf.nextId);
                if (!string.IsNullOrEmpty(confGuid.uiName))
                {
                    rootID = confGuid.id;
                }
                GuidFactory.Instance.CreateGuidByID((GuidEnum)confGuid.type, m_keyID, rootID, m_CurConf.nextId);
            }
            else {
                //整条完成
                Debug.Log(m_RootID + "已完成" + m_CurID);
                GuidFactory.Instance.RemoveGuidByID(m_CurConf.id);
                GuidManager.Instance.SetProgressByIndex(m_keyID);
            }
        }

        public virtual void StartGuid()
        {

        }

        protected virtual void EndGuid()
        {
            MoveToNext();
        }

        protected virtual void Destory()
        {

        }
    }
}
