using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class GuidManager : Singleton<GuidManager>
    {
        private bool m_isFinish  //是否完成
        {
            get
            {
                return ~m_progress == 0;
            }
        }

        private string m_guidKey
        {
            get {
                return string.Format("{0}_guidProgress", GlobalInfo.MY_PLAYER_ID);
            }
        }

        private int m_progress = 0; //当前进度

        //根据表索引获取当前状态
        public bool GetProgressByIndex(int index)
        {
            return ((m_progress >> index) & 1) == 1;
        }

        public void SetProgressByIndex(int index)
        {
            m_progress = (1 << index) | m_progress;
            PlayerPrefs.SetInt(m_guidKey, m_progress);
        }

        public bool IsFinish()
        {
            return m_isFinish;
        }

        //获取当前进度
        private void GetCurrentProgress()
        {
            if (!PlayerPrefs.HasKey(m_guidKey))
            {
                PlayerPrefs.SetInt(m_guidKey, 0);
                m_progress = 0;
            }
            else
            {
                m_progress = PlayerPrefs.GetInt(m_guidKey);
            }
            
        }

        private Dictionary<long, AbsGuid> m_ExpireGuid = new Dictionary<long, AbsGuid>();

        //初始化加载Guid数据
        public void LoadGuid()
        {

            GuidFactory.Instance.ClearGuid();
            m_ExpireGuid.Clear();
            Debug.Log("Init Guid");
            GetCurrentProgress();

            if (m_isFinish)
            {
                Debug.Log("Guid Finish");
                return;
            }
            
            ModuleMgr.Instance.AddModule<GuidModule>((byte)GameModuleTypes.GUID_MODULE, true);
            List<ConfGuid> guidArray = ConfGuid.array;
            for (int i = 0; i < guidArray.Count; i++)
            {
                ConfGuid confGuid = guidArray[i];
                if (confGuid.isMain)
                {
                    bool progressStatus = GetProgressByIndex(i);
                    if (!progressStatus)
                    {
                        if (!GuidFactory.Instance.RootGuid.ContainsKey(confGuid.id))
                        {
                            AbsGuid guid = GuidFactory.Instance.CreateGuidByID((GuidEnum)confGuid.type, i, confGuid.id, confGuid.id);
                            GuidFactory.Instance.RootGuid.Add(confGuid.id, guid);
                        }
                    }
                    else
                    {
                        AbsGuid guid = GuidFactory.Instance.CreateExpireGuid((GuidEnum)confGuid.type, i, confGuid.id, confGuid.id);
                        m_ExpireGuid.Add(confGuid.id, guid);
                    }
                }
            }
            foreach (var kv in m_ExpireGuid)
            {
                GuidFactory.Instance.StartCacheGuid(kv.Value.GetNextRootID());
            }
            m_ExpireGuid.Clear();
        }

        public void StartGuidByID(long id)
        {
            AbsGuid absGuid = GuidFactory.Instance.GetRootGuidByID(id);
            if (absGuid == null)
            {
                Debug.LogError("guid id is not exist:" + id);
                return;
            }
            absGuid.StartGuid();
        }

    }

}
