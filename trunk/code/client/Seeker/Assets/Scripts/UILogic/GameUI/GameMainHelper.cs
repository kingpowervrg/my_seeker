using System;
using System.Collections.Generic;

namespace SeekerGame
{
    public class GameMainHelper : Singleton<GameMainHelper>
    {
        long[] taskID = null;//new long[] { 10001, 10003, 10005, 10009, 10027, 10033, 10063, 10070 };

        long[] fingerTips_TaskID;

        private List<PropUseGuidData> m_propHintDatas = null; //强烈提示

        GameMainArrowTipData[] arrowTipData;

        private long m_currentTaskID = -1;

        public long currentTaskID
        {
            get { return m_currentTaskID;}
            set{ m_currentTaskID = value; }
        }

        public SceneMode m_currentScenMode = SceneMode.NORMALGAME;

        public GameMainHelper()
        {
            LoadInitTaskID();
        }

        public void LoadInitTaskID()
        {
            ConfServiceConfig config = ConfServiceConfig.Get(36);
            if (config == null)
            {
                return;
            }
            taskID = GetTaskIDArrayFromString(config.fieldValue);

            ConfServiceConfig fingerConfig = ConfServiceConfig.Get(38);
            if (fingerConfig == null)
            {
                return;
            }
            fingerTips_TaskID = GetTaskIDArrayFromString(fingerConfig.fieldValue);

            ConfServiceConfig arrowConfig = ConfServiceConfig.Get(40);
            if (arrowConfig == null)
            {
                return;
            }
            arrowTipData = GetArrowTipData(arrowConfig.fieldValue);

            ConfServiceConfig propUseConfig = ConfServiceConfig.Get(44);
            if (propUseConfig == null)
            {
                return;
            }
            m_propHintDatas = new List<PropUseGuidData>();
            //m_propHintDatas = new PropUseGuidDatas();
            //m_propHintDatas.datas = new List<PropUseGuidData>();
            m_propHintDatas.Add(new PropUseGuidData() {
                propIDs = new List<long>() { 3},
                 taskID = 10003
            });
            m_propHintDatas.Add(new PropUseGuidData()
            {
                propIDs = new List<long>() { 2 },
                taskID = 10016
            });
            //m_propHintDatas = Utf8Json.JsonSerializer.Deserialize<PropUseGuidDatas>(propUseConfig.fieldValue);
            //int kk = 0;
        }

        private static long[] GetTaskIDArrayFromString(string str)
        {
            if (string.IsNullOrEmpty(str.Trim()))
            {
                return null;
            }
            long[] taskIDs;
            string taskStr = str.Replace("[", "");
            taskStr = taskStr.Replace("]", "");
            string[] taskSplit = taskStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            taskIDs = new long[taskSplit.Length];
            for (int i = 0; i < taskSplit.Length; i++)
            {
                taskIDs[i] = long.Parse(taskSplit[i]);
            }
            return taskIDs;
        }

        public static GameMainArrowTipData[] GetArrowTipData(string str)
        {
            string[] arrowStrArray = str.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            GameMainArrowTipData[] arrowTipData = new GameMainArrowTipData[arrowStrArray.Length];
            for (int i = 0; i < arrowStrArray.Length; i++)
            {
                string[] arrowSplit = arrowStrArray[i].Split(',');
                if (arrowSplit.Length != 2)
                {
                    UnityEngine.Debug.LogError("error ArrowTipData : " + arrowStrArray[i]);
                    continue;
                }
                GameMainArrowTipData tipData = new GameMainArrowTipData(long.Parse(arrowSplit[0]),long.Parse(arrowSplit[1]));
                arrowTipData[i] = tipData;
            }
            return arrowTipData;
        }

        public static long GetCurrentPropTips()
        {
            List<SceneItemEntity> notFoundEntityList = GameEvents.MainGameEvents.GetSceneItemEntityList(3);
            PlayerPropMsg objectLens = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(1); //寻物镜
            PlayerPropMsg detector = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(2); //探测仪
            PlayerPropMsg bomb = GlobalInfo.MY_PLAYER_INFO.GetBagInfosByID(3); //炸弹
            if (notFoundEntityList.Count <= 2)
            {
                if (objectLens != null && objectLens.Count > 0)
                {
                    return 1;
                }
                else if (bomb != null && bomb.Count > 0)
                {
                    return 3;
                }
                else if (detector != null && detector.Count > 0)
                {
                    return 2;
                }
            }
            else
            {
                if (bomb != null && bomb.Count > 0)
                {
                    return 3;
                }
                else if (detector != null && detector.Count > 0)
                {
                    return 2;
                }
                else if (objectLens != null && objectLens.Count > 0)
                {
                    return 1;
                }
                
            }
            return -1;

        }

        public bool isNeedTips(long sceneId,long taskConfID,bool isTips)
        {
            if (taskID == null)
            {
                return false;
            }
            if (sceneId == 10102007 || sceneId == 10102008 || sceneId == 10102009)
            {
                return isTips;
            }
            if (taskConfID <= 0)
            {
                return false;
            }
            for (int i = 0; i < taskID.Length; i++)
            {
                if (taskID[i] == taskConfID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isNeedFingerTips(long sceneId, long taskConfID, bool isTips)
        {
            if (fingerTips_TaskID == null)
            {
                return false;
            }
            if (sceneId == 10108000)
            {
                return isTips;
            }
            if (taskConfID <= 0)
            {
                return false;
            }
            for (int i = 0; i < fingerTips_TaskID.Length; i++)
            {
                if (fingerTips_TaskID[i] == taskConfID)
                {
                    return true;
                }
            }
            return false;
        }
        //箭头提示
        public GameMainArrowTipData isNeedArrowTips(long taskConfID)
        {
            for (int i = 0; i < arrowTipData.Length; i++)
            {
                if (arrowTipData[i].taskId == taskConfID)
                {
                    return arrowTipData[i];
                }
            }
            return null;
        }

        public bool hasForcePropTips = false;
        public void CheckForcePropTips(long taskConfId)
        {
            for (int i = 0; i < m_propHintDatas.Count; i++)
            {
                if (m_propHintDatas[i].taskID == taskConfId)
                {
                    hasForcePropTips = true;
                    return;
                }
            }
            hasForcePropTips = false;
        }

        /// <summary>
        /// 强烈提示
        /// </summary>
        /// <returns></returns>
        public bool isNeedPropTips(long propId,long taskConfId)
        {
            if (m_propHintDatas == null)
            {
                return false;
            }
            for (int i = 0; i < m_propHintDatas.Count; i++)
            {
                PropUseGuidData propData = m_propHintDatas[i];
                if (propData.taskID == taskConfId)
                {
                    for (int j = 0; j < propData.propIDs.Count; j++)
                    {
                        if (propData.propIDs[j] == propId)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
