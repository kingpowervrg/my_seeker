using EngineCore;
using UnityEngine;
using System.Collections.Generic;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_BUILD_TOP)]
    public class BuildTopUILogic : UILogicBase
    {
        private GameUIContainer m_grid;
        private Dictionary<Transform, List<BuildTopIconComponent>> m_topUI = new Dictionary<UnityEngine.Transform, List<BuildTopIconComponent>>();
        protected override void OnInit()
        {
            base.OnInit();
            this.m_grid = Make<GameUIContainer>("Top");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.BigWorld_Event.OnBuildTopUIByObj += OnBuildTopUIByObj;
            GameEvents.BigWorld_Event.OnReflashScreen += OnReflashScreen;
            GameEvents.BigWorld_Event.OnHideBuidTopUI += OnHideBuidTopUI;
            GameEvents.BigWorld_Event.OnBuildTopActive += OnBuildTopActive;
            GameEvents.BigWorld_Event.OpenBuildTopByHead5NumInSceneID += OpenBuildTopBySceneID;
            GameEvents.BigWorld_Event.OnClickScreen += OnClickScreen;
            GameEvents.BigWorld_Event.OnReflashBuildIconStatus += OnReflashBuildIconStatus;
            BuidTopHelp.Instance.BuildUIShow = true;
            this.m_grid.Visible = (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network);
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.BigWorld_Event.OnBuildTopUIByObj -= OnBuildTopUIByObj;
            GameEvents.BigWorld_Event.OnReflashScreen -= OnReflashScreen;
            GameEvents.BigWorld_Event.OnHideBuidTopUI -= OnHideBuidTopUI;
            GameEvents.BigWorld_Event.OnBuildTopActive -= OnBuildTopActive;
            GameEvents.BigWorld_Event.OpenBuildTopByHead5NumInSceneID -= OpenBuildTopBySceneID;
            GameEvents.BigWorld_Event.OnClickScreen -= OnClickScreen;
            GameEvents.BigWorld_Event.OnReflashBuildIconStatus -= OnReflashBuildIconStatus;
            BuidTopHelp.Instance.BuildUIShow = false;
            m_topUI.Clear();
            this.m_grid.Clear();
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
        }

        private void OnHideBuidTopUI(long buidID,string resName)
        {
            //Transform tran = m_grid.Widget.Find(resName);
            //if (tran == null)
            //{
            //    return;
            //}
            //tran.gameObject.SetActive(false);
            foreach (var kv in m_topUI)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    kv.Value[i].OnHideGuid();
                }
            }
        }

        private void OnBuildTopUIByObj(BuidAchorData archorData, GameObject obj,bool isGuid)
        {
            OnAddBuidUI(archorData,obj,isGuid);
        }

        private void OnAddBuidUI(BuidAchorData archorData, GameObject obj,bool isGuid)
        {
            foreach (var kv in m_topUI)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    if(kv.Value[i].isExist(archorData.m_buidID))
                        return;
                }
            }

            BuildTopIconComponent buildIcon = this.m_grid.AddChild<BuildTopIconComponent>();
            if (isGuid)
            {
                buildIcon.gameObject.name = obj.name;
                buildIcon.SetData(null, true);
            }
            else
            {
                buildIcon.SetData(archorData, isGuid);
            }
            Transform uiObj = buildIcon.gameObject.transform;
            Vector3 localPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(obj.transform.position, uiObj.gameObject);//this.m_grid.gameObject.transform.InverseTransformPoint(obj.transform.position);
            uiObj.transform.position = localPos;
            Transform tran = obj.transform;
            if (m_topUI.ContainsKey(tran))
            {
                m_topUI[tran].Add(buildIcon);
            }
            else
            {
                m_topUI.Add(tran, new List<BuildTopIconComponent>() { buildIcon });
            }
            buildIcon.Visible = true;
        }

        private void OnReflashScreen()
        {
            foreach (var kv in m_topUI)
            {
                Vector3 localPos = EngineCore.Utility.CameraUtility.WorldPointInCanvasRectTransform(kv.Key.transform.position, this.m_grid.gameObject);
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    kv.Value[i].Widget.position = localPos;
                }
            }
        }
        private BuildTopIconComponent m_currentBuildTop = null;

        private void OnBuildTopActive(BuildTopIconComponent currentBuildTop,bool visible)
        {
            if (visible)
            {
                this.m_currentBuildTop = currentBuildTop;
                this.m_currentBuildTop.SetIconVisible(true);
            }
            else
            {
                this.m_currentBuildTop = null;
            }
            foreach (var kv in m_topUI)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    if (!kv.Value[i].Equals(currentBuildTop))
                    {
                        kv.Value[i].SetIconVisible(!visible);
                    }
                    
                }
            }
        }

        private void OpenBuildTopBySceneID(long head_5_num_in_sceneID)
        {
            foreach (var kv in m_topUI)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    if (kv.Value[i].OpenBuildTopIconBySceneID(head_5_num_in_sceneID))
                    {
                        return;
                    }
                }
            }
        }

        private void OnClickScreen()
        {
            if (this.m_currentBuildTop != null)
            {
                this.m_currentBuildTop.OnClick(null);
            }
            this.m_currentBuildTop = null;
        }

        private void OnReflashBuildIconStatus(long buildId,int status)
        {
            foreach (var kv in m_topUI)
            {
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    if (kv.Value[i].isExist(buildId))
                    {
                        kv.Value[i].SetBuildStatus(status);
                    }
                }
            }
        }
    }
}
