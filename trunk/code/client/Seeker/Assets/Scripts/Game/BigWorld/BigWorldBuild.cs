using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class BigWorldBuild
    {
        private long m_BuildID;

        private GameObject m_buildObj;

        private GameObject m_buildWall = null;

        private ConfBuilding m_confBuild;

        private int status = 0; //0表示不能解锁 1 表示已经解锁  2表示可以解锁  3表示加锁

        private const string LockShader = "SeekerGame/RimLight";
        private const string NormalShader = "Mobile/Diffuse";
        private Material m_unlockMaterial = null;
        private ParticleScale m_unLockEffect = null;
        private Material m_dimianEffectMat = null;

        private Dictionary<string, Material> m_cacheMaterial = new Dictionary<string, Material>();

        //private long[] m_completeTaskIDs = null;
        //private long[] m_sceneIDs = null;
        private BuidAchorData m_achorData = null;

        public BigWorldBuild(long buildId, GameObject buildObj, ParticleScale wallEffectMaterial)
        {
            this.m_BuildID = buildId;
            this.m_buildObj = buildObj;
            this.m_unlockMaterial = BigWorldGameObject.GetBuildCommonMatByName("UnLock.mat");
            //this.m_cacheMaterial = cacheMaterial;
            this.m_confBuild = ConfBuilding.Get(buildId);
            long[] m_sceneIDs = CommonTools.StringToLongArray(m_confBuild.sceneIds);
            long[] m_completeTaskIDs = CommonTools.StringToLongArray(m_confBuild.taskIds);
            m_achorData = new BuidAchorData(buildId, m_sceneIDs, m_completeTaskIDs);
            this.status = 0;
            this.m_srcPos = buildObj.transform.position;
            //this.m_unlockMaterial = unLockMaterial;
            this.m_unLockEffect = wallEffectMaterial;
            //this.m_dimianEffectMat = dimainEffectMat;
            //Transform wallTran = buildObj.transform.Find("wall");

            //if (wallTran != null)
            //{
            //    this.m_buildWall = wallTran.gameObject;
            //    this.m_buildWall.SetActive(false);
            //}
        }

        public long BuildID
        {
            get
            {
                return m_BuildID;
            }
        }

        private void ChangeBuildMaterial()
        {
            MeshRenderer[] renderer = m_buildObj.transform.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderer.Length; i++)
            {
                Material commonMat = BigWorldGameObject.GetBuildCommonMatByName(renderer[i].sharedMaterial.name + ".mat");
                renderer[i].sharedMaterial = commonMat;
            }
            //if (m_confBuild == null)
            //{
            //    return;
            //}
            ////Transform target = m_rootObj.transform.Find(confBuild.lockResource);
            //if (m_confBuild.unlockMaterial.Length != m_confBuild.unlockResource.Length)
            //{
            //    Debug.LogError("buid resource error" + m_BuildID + "  unlockMaterial:" + m_confBuild.unlockMaterial.Length + "  unlockResource:" + m_confBuild.unlockResource.Length);
            //    return;
            //}
            //for (int i = 0; i < m_confBuild.unlockMaterial.Length; i++)
            //{
            //    Transform nodeTran = m_buildObj.transform.Find(m_confBuild.unlockResource[i]);
            //    if (nodeTran == null)
            //    {
            //        Debug.LogError("unlockResource error  " + m_confBuild.unlockResource[i]);
            //        continue;
            //    }
            //    MeshRenderer render = nodeTran.GetComponent<MeshRenderer>();
            //    if (render == null)
            //    {
            //        continue;
            //    }
            //    if (!m_cacheMaterial.ContainsKey(m_confBuild.unlockMaterial[i]))
            //    {
            //        Debug.LogError("material is not exist " + m_confBuild.unlockMaterial[i]);
            //        continue;
            //    }
            //    if (cb != null)
            //    {
            //        cb(render, m_cacheMaterial[m_confBuild.unlockMaterial[i]]);
            //    }
            //    //render.material = m_cacheMaterial[m_confBuild.unlockMaterial[i]];
            //}
        }

        //解锁指定ID建筑物
        public void UnLockBuildEffect()
        {
            ChangeBuildMaterial();
        }

        //可以解锁的建筑物
        public void CanLockBuildEffect()
        {
            if (this.m_buildWall == null)
            {
                return;
            }
            //BuildWallActive(true);
        }

        //播放解锁
        private void PlayUnLockEffect(Action callback = null)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.unlock_success.ToString());
            BigWorldGroupData groupData = this.m_buildObj.GetComponent<BigWorldGroupData>();
            if (groupData == null)
            {
                Debug.LogError("group data error");
                return;
            }
            List<Material> mats = new List<Material>();
            Dictionary<MeshRenderer, Material> m_srcMats = new Dictionary<MeshRenderer, Material>();
            //List<Material> m_srcMats = new List<Material>();
            //Material m_dimianMat = null;
            //BigWorldDimianData dimianData = null;
            //Material dimainMat = null;
            MeshRenderer[] renderer = m_buildObj.transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderer.Length; i++)
            {
                Material mat = renderer[i].sharedMaterial;
                BigWorldModelData modelData = renderer[i].gameObject.GetComponent<BigWorldModelData>();
                if (modelData != null)
                {
                    renderer[i].material = m_unlockMaterial;
                    renderer[i].material.SetFloat("_MaxHei", modelData.maxHei);
                    renderer[i].material.SetTexture("_MainTex", mat.GetTexture("_MainTex"));
                    renderer[i].material.SetTexture("_LightMapTex", mat.GetTexture("_NightTex"));
                    //render.material.SetFloat("_LightLerp", mat.GetFloat("_lerp"));
                    mats.Add(renderer[i].material);
                }
                m_srcMats.Add(renderer[i], BigWorldGameObject.GetBuildCommonMatByName(mat.name + ".mat"));
            }


            groupData.PlayEffect(mats, () =>
            {

                TimeModule.Instance.SetTimeout(() =>
                {
                    foreach (var kv in m_srcMats)
                    {
                        kv.Key.sharedMaterial = kv.Value;
                    }
                    this.m_unLockEffect.transform.SetParent(this.m_buildObj.transform);
                    this.m_unLockEffect.transform.localPosition = Vector3.zero;
                    //this.m_unLockEffect.transform.position = m_buildObj.transform.position; //+ Vector3.up * 0.1f

                    this.m_unLockEffect.transform.localPosition += groupData.m_FuncPos;
                    this.m_unLockEffect.scale = groupData.m_EffectScale;
                    this.m_unLockEffect.gameObject.SetActive(true);
                    TimeModule.Instance.SetTimeout(() => { this.m_unLockEffect.gameObject.SetActive(false); }, 2f);
                    Dictionary<UBSParamKeyName, object> _params = new Dictionary<UBSParamKeyName, object>()
                    {
                        { UBSParamKeyName.ContentID, BuildID},
                        { UBSParamKeyName.ContentType, "unLock"},
                        { UBSParamKeyName.Description, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:ms")},
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.build_plan, null, _params);

                    GameObject.DestroyImmediate(groupData);

                    GameEvents.BigWorld_Event.OnBuildUnLockComplete.SafeInvoke(BuildID);
                    if (callback != null)
                    {
                        callback();
                    }
                }, 1f);

            });
        }

        //private void BuildWallActive(bool flag)
        //{
        //    if (flag)
        //    {
        //        Debug.Log("lock ======= " + m_buildObj.name);
        //    }
        //    if (this.m_buildWall == null)
        //    {
        //        return;
        //    }
        //    this.m_buildWall.SetActive(flag);
        //}

        public void LockBuildStatus(int status)
        {
            this.status = status;
            this.m_achorData.BuildStatus = status;
            if (status == 2)
            {
                //CanLockBuildEffect();
                GameEvents.BigWorld_Event.OnReflashBuildIconStatus.SafeInvoke(m_BuildID, status);
            }
            else if (status == 1)
            {
                UnLockBuildEffect();
                BigWorldGroupData groupData = this.m_buildObj.GetComponent<BigWorldGroupData>();
                if (groupData != null)
                {
                    GameObject.DestroyImmediate(groupData);
                }
            }
            OnShowBuildTopUI(m_confBuild.anchors, false);

        }

        public void SetStatus(int status)
        {
            this.status = status;
        }

        public void OnBuildClick()
        {
            //if (!SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(4))
            //{
            //    return;
            //}
            //todo//////////////////////////临时注释
            if (m_BuildID >= 4)
            {
                PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("group_unlock_future_tips"));
                return;
            }

            if (status == 2)
            {
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_UNLOCK);
                param.Param = m_BuildID;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
            else if (status == 0)
            {
                if (m_confBuild.unlockTask > 0)
                {
                    GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.IsCompleteTaskByConfigID(m_confBuild.unlockTask, (bool taskComplete) =>
                    {
                        if (!taskComplete)
                        {
                            string taskName = string.Empty;
                            ConfTask confTask = ConfTask.Get(m_confBuild.unlockTask);
                            if (confTask != null)
                            {
                                taskName = LocalizeModule.Instance.GetString(confTask.name);
                            }
                            if (GlobalInfo.MY_PLAYER_INFO.Level < m_confBuild.unlockLevel)
                            {
                                PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("map_clock_level_mission", m_confBuild.unlockLevel, taskName));
                            }
                            else
                            {
                                PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("group_unlock_tips", taskName));
                            }
                            return;
                        }

                    });
                    // bool taskComplete = true;//GlobalInfo.MY_PLAYER_INFO.PlayerTaskSystem.IsCompleteTaskByConfigID(m_confBuild.unlockTask);

                }
                else
                    PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("map_clock_level", m_confBuild.unlockLevel));
            }
        }

        private Vector3 m_srcPos = Vector3.zero;

        public void OnBuildUnLock()
        {
            status = 1;
            //BuildWallActive(false);
            GameEvents.BigWorld_Event.OnReflashBuildIconStatus.SafeInvoke(m_BuildID, status);
            PlayUnLockEffect(() =>
            {

                //long nextBuildId = m_BuildID + 1;
                //if (nextBuildId < ConfBuilding.array.Count)
                //{
                //    ConfBuilding nextBuild = ConfBuilding.Get(nextBuildId);
                //    if (nextBuild != null)
                //    {
                //        GameEvents.BigWorld_Event.OnReflashBuildStatus.SafeInvoke(nextBuildId,3);
                //       //OnShowBuildTopUI(nextBuild.anchors, false);
                //    }
                //}

            });

        }
        private bool m_isGuid = false;
        private Transform nodeTran = null;
        public void OnShowBuildTopUI(string node, bool isGuid)
        {
            this.m_isGuid = isGuid;
            nodeTran = m_buildObj.transform.Find(node);
            if (nodeTran == null)
            {
                Debug.LogError("build node error " + node);
                return;
            }
            if (BuidTopHelp.Instance.BuildUIShow)
            {

                GameEvents.BigWorld_Event.OnBuildTopUIByObj.SafeInvoke(m_achorData, nodeTran.gameObject, isGuid);
            }
            else
            {
                TimeModule.Instance.SetTimeout(OnTickBuildTop, 0.1f, true);
            }
        }

        private void OnTickBuildTop()
        {
            if (m_achorData == null || nodeTran == null)
            {
                TimeModule.Instance.RemoveTimeaction(OnTickBuildTop);
                return;
            }
            if (BuidTopHelp.Instance.BuildUIShow)
            {
                TimeModule.Instance.RemoveTimeaction(OnTickBuildTop);
                GameEvents.BigWorld_Event.OnBuildTopUIByObj.SafeInvoke(m_achorData, nodeTran.gameObject, this.m_isGuid);
            }
        }

        public bool isBuildUnLock()
        {
            return this.status == 1;
        }
        //private void On
    }

    public class BuidAchorData
    {
        public long m_buidID;
        public int m_buildStatus;
        public List<BuidEnterData> m_enterData = new List<BuidEnterData>();

        public BuidAchorData(long buildID, long[] sceneIds, long[] taskIds)
        {
            this.m_buidID = buildID;
            if (sceneIds == null || taskIds == null)
            {
                return;
            }
            for (int i = 0; i < sceneIds.Length; i++)
            {
                BuidEnterData enterdata = new BuidEnterData(sceneIds[i], taskIds[i]);
                m_enterData.Add(enterdata);
            }
        }

        public int BuildStatus
        {
            get { return m_buildStatus; }
            set { m_buildStatus = value; }
        }
    }

    public class BuidEnterData
    {
        public long m_taskID;
        public long m_sceneID;
        public BuidEnterData(long sceneId, long taskID)
        {
            this.m_taskID = taskID;
            this.m_sceneID = sceneId;
        }
    }
}
