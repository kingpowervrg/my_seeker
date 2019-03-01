//#define DAYNIGHT
using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeekerGame
{
    public class BigWorldGameObject
    {
        private GameObject m_rootObj;
        public GameObject RootObj
        {
            get
            {
                return m_rootObj;
            }
        }

        private GameObject m_scene_obj;

        //private Dictionary<string, Material> m_cacheMaterial = new Dictionary<string, Material>();

        private List<string> m_loadingMaterial = new List<string>(); //当前正在加载的材质球

        private Dictionary<string, BigWorldBuild> m_builds = new Dictionary<string, BigWorldBuild>();

        private bool m_resLoadCountOver = false;

        private Material m_unLockMaterial = null;
        private Material m_dimainEffectMat = null;
        private Material m_wallEffectMaterial = null;
        private BigWorldDayNightSystem m_dayNight = null;
        private ParticleScale m_unLockEffect = null;

        private string[] m_resMat = new string[] { "Xin_lou_01.mat", "Xin_lou_02.mat", "UnLock.mat" }; //, "WallEffect.mat" 
        private static Dictionary<string, Material> m_CommonMat = new Dictionary<string, Material>();

        private Dictionary<string, GameObject> m_pool_targets_dict;
        private Dictionary<string, GameObject> m_branch_targets_dict;

        public BigWorldGameObject()
        {
            GameEvents.BigWorld_Event.OnClick += OnBuildClick;
            GameEvents.BigWorld_Event.OnUnLock += OnBuildUnLock;
            GameEvents.BigWorld_Event.OnCameraMove += SetTarget;
            GameEvents.BigWorld_Event.OnShowBuildTopUI += OnShowBuildTopUI;
            GameEvents.BigWorld_Event.OnReflashBuildStatus += LockBuildStatus;
            GameEvents.BigWorld_Event.OnCheckBuildStatusByID += OnCheckBuildStatusByID;
            GameEvents.BigWorld_Event.Listen_GetAllPoolAnchors += GetPoolAnchors;
            GameEvents.BigWorld_Event.Listen_GetAllBranchAnchors += GetBranchAnchors;
        }

        public void OnDestory()
        {
            GameEvents.BigWorld_Event.OnClick -= OnBuildClick;
            GameEvents.BigWorld_Event.OnUnLock -= OnBuildUnLock;
            GameEvents.BigWorld_Event.OnShowBuildTopUI -= OnShowBuildTopUI;
            GameEvents.BigWorld_Event.OnCameraMove -= SetTarget;
            GameEvents.BigWorld_Event.OnReflashBuildStatus -= LockBuildStatus;
            GameEvents.BigWorld_Event.OnCheckBuildStatusByID -= OnCheckBuildStatusByID;
            GameEvents.BigWorld_Event.Listen_GetAllPoolAnchors -= GetPoolAnchors;
            GameEvents.BigWorld_Event.Listen_GetAllBranchAnchors -= GetBranchAnchors;
            //this.m_cacheMaterial.Clear();
            this.m_loadingMaterial.Clear();
            this.m_builds.Clear();
            this.m_rootObj = null;
            this.m_resLoadCountOver = false;
            foreach (var kv in m_CommonMat)
            {
                if (kv.Value != null)
                {
                    EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(kv.Key, kv.Value);
                }
            }
            m_CommonMat.Clear();
            //if (m_unLockMaterial != null)
            //{
            //    EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(m_unLockMaterial.name + ".mat", m_unLockMaterial);
            //}
            //if (m_dimainEffectMat != null)
            //{
            //    EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(m_dimainEffectMat.name + ".mat", m_dimainEffectMat);
            //}
            //this.m_cacheMaterial.Clear();
#if DAYNIGHT
            if (this.m_dayNight != null)
            {
                this.m_dayNight.OnDestory();
            }
#else
            ResourceModule.Instance.RemoveAllAsset();
#endif
            //EngineCoreEvents.ResourceEvent.ReleaseAssetEvent
        }

        public void GetRootObjByScene()
        {
            GameObject roots = GameObject.FindGameObjectWithTag("sceneRoot");
            if (roots.name.Equals(BigWorldSystem.SCENENAME))
            {
                m_scene_obj = roots;
                m_rootObj = roots.transform.Find(BigWorldSystem.ROOTNAME).gameObject;
                this.m_unLockEffect = m_rootObj.transform.Find("chengshiditu_jianzhujiesuo_UV").GetComponent<ParticleScale>();
                this.m_unLockEffect.gameObject.SetActive(false);
                InputModule.Instance.Enable = true;
                return;
            }
        }

        public void InitCameraManager(string openUIName, bool needShowMainUI, long buildID = 1)
        {
            Transform target = null;
            ConfBuilding confBuilding = ConfBuilding.Get(buildID);
            if (confBuilding == null)
            {
                target = m_rootObj.transform.Find("Group001");
            }
            else
            {
                target = m_rootObj.transform.Find(confBuilding.lockResource);
            }
            SetTarget(target, openUIName, needShowMainUI);
        }

        public void LoadAllSceneMaterial(Action loadCallback)
        {
            m_CommonMat.Clear();
            int resCount = 0;
            for (int i = 0; i < m_resMat.Length; i++)
            {
                GOGUITools.GetAssetAction.SafeInvoke(m_resMat[i], (prefabName, obj) =>
                {
                    resCount++;
                    Material mat = obj as Material;
                    if (m_CommonMat.ContainsKey(prefabName))
                        m_CommonMat[prefabName] = mat;
                    else
                        m_CommonMat.Add(prefabName, mat);
                    if (m_resMat.Length == resCount)
                    {
                        GetSceneMaterial(loadCallback);
                    }

                }, LoadPriority.Default);
            }

        }

        //加载所有材质球数据
        public void GetSceneMaterial(Action loadCallback)
        {
            //m_cacheMaterial.Clear();
            m_loadingMaterial.Clear();
            m_builds.Clear();

            //float timePercent = CommonTools.GetCurrentTimePercent();
            //MeshRenderer[] render = m_rootObj.GetComponentsInChildren<MeshRenderer>();
            //for (int i = 0; i < render.Length; i++)
            //{
            //    Material mat = render[i].sharedMaterial;
            //    if (mat == null)
            //    {
            //        continue;
            //    }
            //    if (!m_cacheMaterial.ContainsKey(mat.name))
            //    {
            //        //mat.SetFloat("_lerp", timePercent);
            //        m_cacheMaterial.Add(mat.name, mat);
            //    }
            //}

            #region 加载额外材质球
            List<ConfBuilding> confbuilding = ConfBuilding.array;
            //int resLoadCount = 0;
            for (int i = 0; i < confbuilding.Count; i++)
            {
                //<建筑物头标
                Transform target = m_rootObj.transform.Find(confbuilding[i].lockResource);
                if (target == null)
                {
                    Debug.LogError("errror target " + confbuilding[i].lockResource);
                    continue;
                }
                BigWorldBuild build = new BigWorldBuild(confbuilding[i].id, target.gameObject, this.m_unLockEffect);
                if (!m_builds.ContainsKey(confbuilding[i].lockResource))
                {
                    m_builds.Add(confbuilding[i].lockResource, build);
                }

                //                resLoadCount++;
                //                for (int j = 0; j < confbuilding[i].unlockMaterial.Length; j++)
                //                {
                //                    if (resLoadCount == confbuilding.Count && j == confbuilding[i].unlockMaterial.Length - 1)
                //                    {
                //                        m_resLoadCountOver = true;
                //                    }
                //                    string materialName = confbuilding[i].unlockMaterial[j];
                //                    if (!m_cacheMaterial.ContainsKey(materialName) && !m_loadingMaterial.Contains(materialName))
                //                    {
                //                        m_loadingMaterial.Add(materialName);
                //                        GOGUITools.GetAssetAction.SafeInvoke(materialName + "_clone.mat", (prefabName, obj) =>
                //                        {
                //                            Material mat = obj as Material;
                //#if DAYNIGHT
                //                            mat.SetFloat("_lerp", timePercent);
                //#endif
                //                            m_cacheMaterial.Add(materialName, mat);
                //                            m_loadingMaterial.Remove(materialName);
                //                            if (m_loadingMaterial.Count == 0 && m_resLoadCountOver)
                //                            {
                //#if DAYNIGHT
                //                                this.m_dayNight = new BigWorldDayNightSystem(m_cacheMaterial);
                //                                this.m_dayNight.LoadLightTex(()=> {
                //                                    if (loadCallback != null)
                //                                    {
                //                                        loadCallback();
                //                                    }
                //                                    GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(1, true);
                //                                });
                //#else

                //#endif
                //                            }
                //                        }, LoadPriority.Default);
                //                    }
                //                }
            }

            //<支线，池，任务。头标
            m_pool_targets_dict = new Dictionary<string, GameObject>();
            Transform task_root = m_scene_obj.transform.Find("Pool");
            foreach (Transform item in task_root)
            {
                m_pool_targets_dict.Add(item.gameObject.name, item.gameObject);
            }

            m_branch_targets_dict = new Dictionary<string, GameObject>();
            task_root = m_scene_obj.transform.Find("Branch");
            foreach (Transform item in task_root)
            {
                m_branch_targets_dict.Add(item.gameObject.name, item.gameObject);
            }
            //>
            #endregion
            if (loadCallback != null)
            {
                loadCallback();
            }
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(1, true);

        }

        public void SetTarget(long buildID)
        {
            ConfBuilding building = ConfBuilding.Get(buildID);
            if (building == null)
            {
                return;
            }
            Transform target = m_rootObj.transform.Find(building.lockResource);
            if (target == null)
            {
                return;
            }
            Transform tranCenter = target.Find("center");
            if (tranCenter == null)
            {
                return;
            }
            BigWorldCameraManager.Instance.AddCamera();
            BigWorldCameraManager.Instance.SetTargetCamera(tranCenter);
        }

        public static Material GetBuildCommonMatByName(string matName)
        {
            if (m_CommonMat.ContainsKey(matName))
                return m_CommonMat[matName];
            return null;
        }

        void PreSyncUIData()
        {
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(new CSGetPushRequest());

            ShopHelper.RefreshAllShopData();
        }

        public void SetTarget(Transform target, string openUIName, bool needShowMainUI)
        {
            Transform tranCenter = target.Find("center");
            BigWorldCameraManager.Instance.AddCamera();

            PreSyncUIData();

            PreloadBigworldUI();

            BigWorldCameraManager.Instance.SetTarget(tranCenter, () =>
            {

                ResourceModule.Instance.PreloadBundle("UI_xinshouyindao_01.prefab", null);
                //CameraManager.Instance.EnableGameCameraController = true;
                BigWorldCameraManager.Instance.bigWorldCameraEnable = true;
                if (needShowMainUI && (string.IsNullOrEmpty(openUIName) || openUIName.Equals("GiftView")))
                {
                    EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_BANNER);

                    EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_GAMEENTRY);

                    EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_BUILD_TOP);

                    //EngineCoreEvents.UIEvent.ShowUIEvent.SafeInvoke(UIDefine.UI_TASK_ON_BUILD);
                }
                if (!string.IsNullOrEmpty(openUIName) && needShowMainUI)
                {
                    FrameMgr.OpenUIParams data_entry = new FrameMgr.OpenUIParams(UIDefine.UI_GAMEENTRY)
                    {
                        Param = openUIName,
                    };

                    FrameMgr.OpenUIParams data_banner = new FrameMgr.OpenUIParams(UIDefine.UI_BANNER)
                    {
                        Param = openUIName,
                    };

                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(data_entry);
                    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(data_banner);
                    // GameEvents.UIEvents.UI_GameEntry_Event.OnOpenPanel.SafeInvoke(openUIName);
                }
                //ReplaceRimLight(target);
            });
        }

        private void PreloadBigworldUI()
        {
            EngineCoreEvents.UIEvent.PreloadFrame.SafeInvoke(UIDefine.UI_BANNER);

            EngineCoreEvents.UIEvent.PreloadFrame.SafeInvoke(UIDefine.UI_GAMEENTRY);

            EngineCoreEvents.UIEvent.PreloadFrame.SafeInvoke(UIDefine.UI_BUILD_TOP);

        }

        public void LockBuildStatus(long buildID, int status)
        {
            foreach (var kv in m_builds)
            {
                if (kv.Value.BuildID == buildID)
                {
                    kv.Value.LockBuildStatus(status);
                    break;
                }
            }
        }

        private void OnBuildClick(string name)
        {
            if (m_builds.ContainsKey(name))
            {
                m_builds[name].OnBuildClick();
            }
        }

        private void OnBuildUnLock(string name)
        {
            if (m_builds.ContainsKey(name))
            {
                m_builds[name].OnBuildUnLock();
            }
        }

        private void OnShowBuildTopUI(long buildID, string node, bool isGuid)
        {
            foreach (var kv in m_builds)
            {
                if (kv.Value.BuildID == buildID)
                {
                    kv.Value.OnShowBuildTopUI(node, isGuid);
                    break;
                }
            }
        }

        private bool OnCheckBuildStatusByID(long buildID)
        {
            if (buildID <= 0)
            {
                return true;
            }
            foreach (var kv in m_builds)
            {
                if (kv.Value.BuildID == buildID && kv.Value.isBuildUnLock())
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, GameObject> GetPoolAnchors()
        {
            return m_pool_targets_dict;
        }

        private Dictionary<string, GameObject> GetBranchAnchors()
        {
            return m_branch_targets_dict;
        }
    }
}
