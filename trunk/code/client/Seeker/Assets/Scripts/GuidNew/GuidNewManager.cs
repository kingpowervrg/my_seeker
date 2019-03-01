using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewManager : Singleton<GuidNewManager>
    {
        public bool m_skipGuid
        {
            get {
                return GameRoot.instance.skipGuid;
            }
        }

        private bool m_isFinish  //是否完成
        {
            get
            {
                //byte tempProgress = (byte)~m_progress;
                return m_progress >= 8388607;
            }
        }


        private bool m_guidInit = false;

        private long m_progress = 0; //当前进度

        //根据表索引获取当前状态
        public bool GetProgressByIndex(int index)
        {
            //int currentFactor = 1 << index;
            //return IsFinish() || (m_progress & currentFactor) == currentFactor;
            return IsFinish() || ((m_progress >> index) & 1) == 1;
        }

        public void SetProgressByIndex(int index)
        {

            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {

                        { UBSParamKeyName.ContentID, index},

                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.guide_in, null, _params);

            m_progress = (1L << index) | m_progress;
            SaveCurrentProgress(m_progress);

            //todo 要求新增guide_in字段
            //Dictionary<UBSParamKeyName, object> enterSceneFromChapterMapDict = new Dictionary<UBSParamKeyName, object>();
            //enterSceneFromChapterMapDict.Add(UBSParamKeyName.ContentID, m_progress);
            //UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.guide, null, enterSceneFromChapterMapDict);
        }

        public void TestProgressIndex(int index)
        {
            m_progress = (~(1L << index)) & m_progress;
            SaveCurrentProgress(m_progress);
        }

        public void ForceSaveProgress(int index)
        {
            Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {

                        { UBSParamKeyName.ContentID, index},

                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.guide_in, null, _params);

            long progress = (1L << index) | m_progress;
            SaveCurrentProgress(progress);
        }

        public void SetTotalProgress(int progress)
        {
            this.m_progress = progress;
            SaveCurrentProgress(m_progress);
        }

        public bool IsFinish()
        {
            return m_isFinish || m_skipGuid;
        }

        public void SyncProgress(long progress)
        {
            Debug.Log("sync progress " + progress);
            this.m_progress = progress;
        }

        private void SaveCurrentProgress(long progress)
        {
            //if (progress == 0)
            //{
            //    return;
            //}
            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                Debug.Log("save progress : " + progress);
                CSPlayerGuildUpdateRequest req = new CSPlayerGuildUpdateRequest();
                req.Guild = progress.ToString();
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
            }
            else
            {
                SetCurrentProgressValueToLocal();
            }
            //if (progress == m_localMaxProgressValue)
            //{
            //    GlobalInfo.GAME_NETMODE = GameNetworkMode.Network;
            //}
        }

        private Dictionary<long, GuidNewBase> m_Guid = new Dictionary<long, GuidNewBase>();
        public Dictionary<long, GuidNewBase> Guid
        {
            get
            {
                return m_Guid;
            }
        }

        public bool GetGuidNewStatus(long ID)
        {
            if (!m_Guid.ContainsKey(ID))
            {
                return true;
            }
            return m_Guid[ID].IsComplete;
        }

        public void RemoveGuid(long Id)
        {
            if (m_Guid.ContainsKey(Id))
            {
                m_Guid.Remove(Id);
            }
            else
            {
                Debug.LogError("guid ID error : " + Id);
            }
        }

        private bool m_moduleStart = false;
        private void OnLoadGuid()
        {
            if (!m_guidInit && !m_moduleStart)
            {
                ModuleMgr.Instance.AddModule<GuidNewModule>((byte)GameModuleTypes.GUID_MODULE, true);
                this.m_guidInit = false;
                this.m_moduleStart = true;
            }

            if (m_guidInit)
            {
                Debug.Log("Guid has init ====");
                return;
            }
            if (this.IsFinish())
            {
                Debug.Log("Guid Complete ====");
                return;
            }
            EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GUID);

            for (int i = 0; i < ConfGuidNew.array.Count; i++)
            {
                ConfGuidNew guid = ConfGuidNew.array[i];
                if (GetProgressByIndex(guid.groupId))
                {
                    continue;
                }
                GuidNewBase newBase = GuidNewFunctionFactory.Instance.CreateGuidById(guid.id);
                newBase.OnInitStart();
                m_Guid.Add(guid.id, newBase);
            }
            this.m_guidInit = true;
        }

        public void OnLoad()
        {
            OnLoadGuid();
            //GetCurrentProgress();
        }

        public void OnReflashGuidStatus()
        {
            if (this.IsFinish() || !this.m_guidInit)
            {
                return;
            }
            GuidNewModule.Instance.OnTryStart();
        }

        //重置引导
        public void OnResetGuidByGroupID(int groupID)
        {
            foreach (var kv in m_Guid)
            {
                if (groupID == kv.Value.GroupID)
                {
                    kv.Value.OnReset();
                }
            }
        }

        public bool OnCheckGroupComplete(int groupID)
        {
            foreach (var kv in m_Guid)
            {
                if (groupID == kv.Value.GroupID)
                {
                    if (!kv.Value.IsComplete)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void OnGroupComplete(int groupID)
        {
            List<GuidNewBase> complete = new List<GuidNewBase>();
            foreach (var kv in m_Guid)
            {
                if (groupID == kv.Value.GroupID)
                {
                    complete.Add(kv.Value);
                    //kv.Value.OnEnd();
                }
            }
            for (int i = 0; i < complete.Count; i++)
            {
                
                complete[i].OnEnd();
            }
            complete.Clear();
        }


        public void ReLoadGuid(long guidID)
        {
            if (IsFinish())
            {
                return;
            }
            if (m_Guid.ContainsKey(guidID))
            {
                m_Guid[guidID].OnReset(false);
            }
        }

        public void OnClearGuid()
        {
            foreach (var kv in m_Guid)
            {
                kv.Value.OnClear();
            }
            m_Guid.Clear();
        }

        public void OnClearLocalProgress()
        {
            m_progress = 0;
            m_guidInit = false;
            PlayerPrefs.SetString(m_guidKey, m_progress.ToString());
        }

        #region 1122新增本地存储
        public const string m_guidKey = "guidProgress";

        private int m_localMaxProgressValue = 31; //本地最大存储进度  4

        public void LoadCurrentProgressValueForLocal()
        {
            if (PlayerPrefs.HasKey(m_guidKey))
            {
                string progress = PlayerPrefs.GetString(m_guidKey);
                if (string.IsNullOrEmpty(progress.Trim()))
                {
                    m_progress = 0;
                }
                else
                {
                    m_progress = long.Parse(progress);
                }
            }
            else
            {
                m_progress = 0;
            }
        }

        private void SetCurrentProgressValueToLocal()
        {
            PlayerPrefs.SetString(m_guidKey, m_progress.ToString());
        }

        private void ClearLocalProgressValue()
        {
            PlayerPrefs.SetInt(m_guidKey,0);
        }

        public bool CheckNeedSkipLocalGuid()
        {
            return m_progress >= m_localMaxProgressValue || IsFinish();
        }

        #endregion
    }
}
