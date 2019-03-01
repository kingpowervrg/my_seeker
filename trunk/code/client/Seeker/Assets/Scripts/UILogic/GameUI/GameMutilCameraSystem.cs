using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    public class GameMutilCameraSystem
    {
        private GameButton m_btnBack = null;
        private GameObject[] m_doors = null;
        private Camera m_currentCamera = null;
        private GameObject m_currentObj = null;
        private Camera m_mainCamera = null;
        private bool m_isForbid = false;
        public GameMutilCameraSystem(GameButton btnBack)
        {
            this.m_btnBack = btnBack;
        }

        public void OnShow()
        {
            //this.m_btnBack.AddClickCallBack(BtnBck);
            this.m_btnBack.Visible = false;
            this.m_doors = GameObject.FindGameObjectsWithTag("sceneDoor");
            if (this.m_doors != null)
            {
                EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTouchScreen;
            }
            this.m_mainCamera = Camera.main;
            this.m_currentCamera = this.m_mainCamera;
            GameEvents.SceneEvents.OnClickQuitScene += OnClickQuitScene;
            GameEvents.MainGameEvents.OnFingerForbidden += OnFingerForbidden;
        }

        public void OnHide()
        {
            //m_btnBack.RemoveClickCallBack(BtnBck);
            if (this.m_doors != null)
            {
                EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTouchScreen;
            }
            this.m_currentCamera = this.m_mainCamera;
            GameEvents.SceneEvents.OnClickQuitScene -= OnClickQuitScene;
            GameEvents.MainGameEvents.OnFingerForbidden -= OnFingerForbidden;
        }

        private void OnClickQuitScene(int sceneType)
        {
            if (sceneType == 2)
            {
                BtnBck(null);
            }
        }

        private void BtnBck(GameObject obj)
        {
            GameEvents.SceneEvents.SetSceneType.SafeInvoke(0);
            if (this.m_mainCamera.gameObject.activeSelf)
            {
                return;
            }
            this.m_btnBack.Visible = false;
            this.m_currentCamera.gameObject.SetActive(false);
            this.m_mainCamera.gameObject.SetActive(true);
            this.m_currentCamera.gameObject.tag = "Untagged";
            this.m_mainCamera.gameObject.tag = "MainCamera";
            this.m_currentCamera = this.m_mainCamera;
            if (this.m_currentObj != null)
            {
                this.m_currentObj.SetActive(true);
            }
            this.m_currentObj = null;
            HedgehogTeam.EasyTouch.EasyTouch.instance.touchCameras[0].camera = this.m_currentCamera;
        }

        private void OnTouchScreen(HedgehogTeam.EasyTouch.Gesture gesture)
        {
            if (gesture.pickedObject != null && !m_isForbid)
            {
                OnSelectGameObject(gesture.pickedObject);
            }
        }

        public void OnSelectGameObject(GameObject obj)
        {
            for (int i = 0; i < m_doors.Length; i++)
            {
                if (m_doors[i] == obj)
                {
                    GameEvents.SceneEvents.SetSceneType.SafeInvoke(2);
                    SceneDoorCamera door = m_doors[i].GetComponent<SceneDoorCamera>();
                    door.m_camera.gameObject.SetActive(true);
                    this.m_currentCamera.gameObject.SetActive(false);
                    this.m_currentObj = obj;
                    this.m_currentCamera = door.m_camera;
                    this.m_currentCamera.gameObject.tag = "MainCamera";
                    this.m_mainCamera.gameObject.tag = "Untagged";
                    this.m_currentObj.SetActive(false);
                    HedgehogTeam.EasyTouch.EasyTouch.instance.touchCameras[0].camera = this.m_currentCamera;
                    this.m_btnBack.Visible = true;
                    return;
                }
            }
        }

        private void OnFingerForbidden(bool flag)
        {
            m_isForbid = flag;
        }
    }
}
