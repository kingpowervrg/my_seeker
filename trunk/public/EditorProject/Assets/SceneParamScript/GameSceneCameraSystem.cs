using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using DG.Tweening;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;
namespace SeekerGame
{
    public class GameSceneCameraSystem
    {
        //private List<GameSceneOtherCamera> m_otherCameraArray = new List<GameSceneOtherCamera>();
        private Dictionary<GameObject, GameSceneOtherCamera> m_otherCameraDic = new Dictionary<GameObject, GameSceneOtherCamera>();
        private Transform m_mainCameraTran = null;
        private GameCamera_New m_mainCamera = null;
        private SceneCameraParams_New m_mainCameraParam = null;
        private Vector3 m_origPosition = Vector3.zero;
        private Vector3 m_origRotate = Vector3.zero;
        private Button m_btnBack = null;
        private GameSceneOtherCamera m_currentOtherCamera = null;
        private bool m_forbiddenTouch = false;
        public GameSceneCameraSystem(Button btnback)
        {
            this.m_mainCameraTran = Camera.main.transform;
            this.m_mainCamera = this.m_mainCameraTran.GetComponent<GameCamera_New>();
            this.m_mainCameraParam = this.m_mainCameraTran.GetComponent<SceneCameraParams_New>();
            this.m_origPosition = this.m_mainCameraTran.position;
            this.m_origRotate = this.m_mainCameraTran.eulerAngles;
            this.m_btnBack = btnback;
            this.m_btnBack.onClick.AddListener(OnBtnBackToMainCamera);
            LoadCameraPoint();
            EasyTouch.On_SimpleTap += OnTouchScreen;
            //EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTouchScreen;
        }

        public void LoadCameraPoint()
        {
            GameObject[] cameraPointObj = GameObject.FindGameObjectsWithTag("cameraPoint");
            for (int i = 0; i < cameraPointObj.Length; i++)
            {
                SceneCameraPoint cameraPoint = cameraPointObj[i].GetComponent<SceneCameraPoint>();
                if (cameraPoint == null || cameraPoint.m_cameraObj == null)
                {
                    continue;
                }
                GameSceneOtherCamera otherCamera = new GameSceneOtherCamera(cameraPoint, cameraPointObj[i]);
                m_otherCameraDic.Add(cameraPointObj[i], otherCamera);
            }
        }

        public void OnDestory()
        {
            EasyTouch.On_SimpleTap -= OnTouchScreen;
            //this.m_btnBack.RemoveClickCallBack(OnBtnBackToMainCamera);
            //EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTouchScreen;
            m_otherCameraDic.Clear();
        }

        private void IsCamerPoint(GameObject obj)
        {
            if (obj != null && obj.CompareTag("cameraPoint") && m_otherCameraDic.ContainsKey(obj))
            {

                this.m_forbiddenTouch = true;
                this.m_currentOtherCamera = m_otherCameraDic[obj];
                m_otherCameraDic[obj].PlayCameraTween(this.m_mainCameraTran.gameObject,(SceneCameraParams_New cameraParams,bool canZoom) => {
                    this.m_btnBack.gameObject.SetActive(true);
                    this.m_mainCamera.SetSceneCameraParam(cameraParams);
                    this.m_forbiddenTouch = false;
                });
            }
        }

        private void OnTouchScreen(HedgehogTeam.EasyTouch.Gesture gesture)
        {
            if (!m_forbiddenTouch && gesture != null && gesture.pickedObject != null)
            {
                IsCamerPoint(gesture.pickedObject);
            }
        }

        private void OnBtnBackToMainCamera()
        {
            OnBtnBackToMainCamera(null,null);
        }

        private void OnBtnBackToMainCamera(GameObject obj,Action cb)
        {
            this.m_btnBack.gameObject.SetActive(false);
            Vector3[] wayPoint = new Vector3[2];
            wayPoint[0] = m_mainCameraTran.position;
            wayPoint[1] = m_origPosition;
            this.m_forbiddenTouch = true;
            m_mainCameraTran.DOPath(wayPoint, 2f, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                this.m_currentOtherCamera.PlayCameraPointEffect(false);
                this.m_mainCamera.SetSceneCameraParam(m_mainCameraParam);
                this.m_forbiddenTouch = false;
                if (cb != null)
                {
                    cb();
                }
            });
            m_mainCameraTran.DORotate(m_origRotate, 2f).SetEase(Ease.OutCubic);
        }


        public void CheckSceneCameraPoint(Action cb)
        {
            if (!this.m_btnBack.IsActive())
            {
                cb();
            }
            else
            {
                OnBtnBackToMainCamera(null,cb);
            }
        }
    }
}
