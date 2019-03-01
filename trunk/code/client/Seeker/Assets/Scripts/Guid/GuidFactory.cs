using System.Collections.Generic;
using System;
using GOEngine;

namespace SeekerGame
{
    public class GuidFactory : Singleton<GuidFactory>
    {

        private Dictionary<GuidEnum, Type> guidTypes = new Dictionary<GuidEnum, Type>();

        private Dictionary<long, AbsGuid> m_RootGuid = new Dictionary<long, AbsGuid>(); //当前根节点 如果已经完成则不添加 key->起始ID  value->当前ID对象

        public Dictionary<long, AbsGuid> RootGuid
        {
            get
            {
                return m_RootGuid;
            }
        }

        public void ClearGuid()
        {
            m_RootGuid.Clear();
        }

        public AbsGuid GetRootGuidByID(long rootID)
        {
            if (RootGuid.ContainsKey(rootID))
            {
                return RootGuid[rootID];
            }
            return null;
        }

        public void StartCacheGuid(long guidID)
        {
            if (m_RootGuid.ContainsKey(guidID))
            {
                m_RootGuid[guidID].StartGuid();
            }
        }

        public GuidFactory()
        {
            guidTypes.Add(GuidEnum.Guid_Talk,typeof(GuidTalk));
            guidTypes.Add(GuidEnum.Guid_Click,typeof(GuidUIMask));
            guidTypes.Add(GuidEnum.Guid_Drag,typeof(GuidUIMask));
            guidTypes.Add(GuidEnum.Guid_DragScene, typeof(GuidUIMask));
            guidTypes.Add(GuidEnum.Guid_Cartoon, typeof(StartGuidCartoon));
        }

        private AbsGuid CreateGuid(GuidEnum guidEnum)
        {
            if (guidTypes.ContainsKey(guidEnum))
            {
                Type guidType = guidTypes[guidEnum];
                AbsGuid baseGuid = Activator.CreateInstance(guidType) as AbsGuid;
                return baseGuid;
            }
            return null;
        }

        public AbsGuid CreateGuidByID(GuidEnum guidEnum, int keyID, long rootID,long curID)
        {
            AbsGuid absGuid = CreateGuid(guidEnum);
            absGuid.InitGuidInfo(keyID,curID, rootID);
            absGuid.StartAuto();
            return absGuid;
        }

        public AbsGuid CreateExpireGuid(GuidEnum guidEnum, int keyID, long rootID, long curID)
        {
            AbsGuid absGuid = CreateGuid(guidEnum);
            absGuid.InitGuidInfo(keyID, curID, rootID);
            absGuid.GetNextRootID();
            return absGuid;
        }

        public AbsGuid CreateRootGuid(long id)
        {
            if (!m_RootGuid.ContainsKey(id))
            {
                DebugUtil.LogError("guid root not exist !!! " + id);
                return null;
            }
            m_RootGuid[id].StartGuid();
            return m_RootGuid[id];
        }

        public void RemoveGuidByID(long id)
        {
            if (m_RootGuid.ContainsKey(id))
            {
                m_RootGuid.Remove(id);
            }
        }
        //public AbsGuid CreateGuidByKey(GuidEnum guidEnum,long keyID ,long rootID, long curID)
        //{
        //    //Dictionary<long,AbsGuid> Progress = GuidManager.Instance.Progress;
        //    AbsGuid absGuid = CreateGuidByID(guidEnum, keyID,rootID, curID);
        //    //if (Progress.ContainsKey(keyID))
        //    //{
        //    //    Progress[keyID] = absGuid;
        //    //}
        //    //else
        //    //{
        //    //    Progress.Add(keyID, absGuid);
        //    //}
        //    return absGuid;
        //}
    }
}
