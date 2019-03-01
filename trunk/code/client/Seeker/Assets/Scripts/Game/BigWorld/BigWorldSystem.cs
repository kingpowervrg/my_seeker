//#define DAYNIGHT
using EngineCore;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SeekerGame
{
    public class BigWorldSystem
    {
#if DAYNIGHT
        public const string WORLDNAME = "ChengShiDiTu_01"; //ChengShiDiTu_01
#else
         public const string WORLDNAME = "ChengShiDiTu_02_ShengDanJie_01";
#endif
        public const string SCENENAME = "Scene";
        public const string ROOTNAME = "ChengShiDiTu_03";
        public const long PoliceBuildID = 1; //默认警局ID
        private bool isBuildLoad = false;
        private bool isFirstLoad = true;
        private GameObject m_rootObj;
        private BigWorldGameObject m_gameObject = null;
        private bool m_needShowMainUI = true;
        private string m_openUIName = string.Empty;
        private long m_currentGroupID = 1;

        public BigWorldSystem()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCBuildingListResp, OnRes);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCBuildingUnlockResp, OnRes);
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver += OnLoadingOver;
            GameEvents.BigWorld_Event.OnReflashBigWorld += OnReflashBigWorld;
            m_gameObject = new BigWorldGameObject();
        }

        public void OnDestory()
        {
            UnityEngine.Debug.Log("Destory BigWorld");
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCBuildingListResp, OnRes);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCBuildingUnlockResp, OnRes);
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver -= OnLoadingOver;
            GameEvents.BigWorld_Event.OnReflashBigWorld -= OnReflashBigWorld;
            this.m_gameObject.OnDestory();
            this.isBuildLoad = false;
            this.isFirstLoad = true;
            this.m_gameObject = null;
            this.m_openUIName = string.Empty;
            this.m_needShowMainUI = true;
            this.m_currentGroupID = 1;
        }

        private void OnRequestBigWorldData()
        {
            OnReflashBigWorld();
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(2, true);
        }

        private void OnReflashBigWorld()
        {
            CSBuildingListReq req = new CSBuildingListReq();

#if !NETWORK_SYNC || UNITY_EDITOR
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
        }

        public void OnEnterBigWorld(string openUIName="", bool needshowUI = true,long groupID = 1)
        {
            this.m_needShowMainUI = needshowUI;
            this.m_openUIName = openUIName;
            this.m_currentGroupID = groupID;
            if (!isBuildLoad)
            {
                LoadBigWorld();
            }
            else
            {
                OnRequestBigWorldData();
            }
            
        }

        private void LoadBigWorld()
        {
            Debug.Log("start load build");
            EngineCore.EngineCoreEvents.ResourceEvent.LoadAdditiveScene.SafeInvoke(WORLDNAME, OnLoadedBigWorldCallback);
            
        }

        private void RemoveBigWorld()
        {
            //EngineCore.EngineCoreEvents.ResourceEvent.ReleaseAndRemoveAssetEvent.SafeInvoke(WORLDNAME, null);
            EngineCore.EngineCoreEvents.ResourceEvent.LeaveScene.SafeInvoke();
        }

        private void OnLoadedBigWorldCallback()
        {
            m_gameObject.GetRootObjByScene();
            
            if (m_gameObject.RootObj != null)
            {
                this.isBuildLoad = true;
                m_gameObject.LoadAllSceneMaterial(()=> {
                    OnRequestBigWorldData();
                });
                return;
            }
            Debug.LogError("bigworld error ===");
        }
        

        private void OnRes(object obj)
        {
            if (obj is SCBuildingListResp)
            {
                SCBuildingListResp res = obj as SCBuildingListResp;
                for (int i = 0; i < res.Infos.Count; i++)
                {
                    BuildingInfo buildInfo = res.Infos[i];
                    if (buildInfo.Status > 0)
                    {
                        if (buildInfo.Status == 2)
                        {
                            if (!PlayerPrefTool.GetBuildLockCache(buildInfo.BuildingId))
                            {
                                System.Collections.Generic.Dictionary<UBSParamKeyName, object> _params = new System.Collections.Generic.Dictionary<UBSParamKeyName, object>()
                                {
                                    { UBSParamKeyName.ContentID, buildInfo.BuildingId},
                                    { UBSParamKeyName.ContentType, "canlock"},
                                    { UBSParamKeyName.Description, System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:ms")},
                                };
                                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.build_plan, null, _params);
                            }
                        }
                        if (NewGuid.GuidNewManager.Instance.GetProgressByIndex(7))
                        {
                            m_gameObject.LockBuildStatus(buildInfo.BuildingId, buildInfo.Status);
                        }
                        //if (SeekerGame.NewGuid.GuidNewManager.Instance.GetProgressByIndex(2))
                        //{
                        //    //解锁或可解锁

                        //}

                    }
                }
                
                //if (res.Infos!= null)
                //{
                //    int infoCount = res.Infos.Count;
                    
                //    if (infoCount > 0 && res.Infos[infoCount - 1].BuildingId < ConfBuilding.array.Count - 1 && res.Infos[infoCount - 1].Status == 1)
                //    {
                //        m_gameObject.LockBuildStatus(res.Infos[infoCount - 1].BuildingId + 1, 3);
                //    }
                    
                //}
                
            }
        }

        public bool IsNeedProgress()
        {
            return !isBuildLoad;
        }

        private void OnLoadingOver()
        {
            if (isFirstLoad)
            {
                this.m_gameObject.InitCameraManager(this.m_openUIName,this.m_needShowMainUI,this.m_currentGroupID);
                isFirstLoad = false;
            }
        }
    }
}
