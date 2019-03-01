using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using DG.Tweening;
using SeekerGame.NewGuid;
namespace SeekerGame
{
    public class GameSceneCameraSystem
    {
        //private List<GameSceneOtherCamera> m_otherCameraArray = new List<GameSceneOtherCamera>();
        private Dictionary<GameObject, GameSceneOtherCamera> m_otherCameraDic = new Dictionary<GameObject, GameSceneOtherCamera>();
        private Transform m_mainCameraTran = null;
        private GameCameraNew m_mainCamera = null;
        private SceneCameraParams_New m_mainCameraParam = null;
        private Vector3 m_origPosition = Vector3.zero;
        private Vector3 m_origRotate = Vector3.zero;
        private GameButton m_btnBack = null;
        public GameButton m_quitObj = null;
        private GameSceneOtherCamera m_currentOtherCamera = null;
        private bool m_forbiddenTouch = false;
        private string m_currentCamera = string.Empty;
        public GameSceneCameraSystem(GameButton btnback)
        {
            this.m_mainCameraTran = Camera.main.transform;
            this.m_mainCamera = this.m_mainCameraTran.GetComponent<GameCameraNew>();
            this.m_mainCameraParam = this.m_mainCameraTran.GetComponent<SceneCameraParams_New>();
            this.m_origPosition = this.m_mainCameraTran.position;
            this.m_origRotate = this.m_mainCameraTran.eulerAngles;
            this.m_btnBack = btnback;
            GameEvents.SceneEvents.OnClickQuitScene += OnClickQuitScene;
            this.m_mainCamera.backAction = FingerBackScene;
            LoadCameraPoint();
            this.m_currentCamera = "MainCamera";
            GameEvents.SceneEvents.EntityInCurrentCamera = EntityInCurrentCamera;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTouchScreen;
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
            GameEvents.SceneEvents.OnClickQuitScene -= OnClickQuitScene;
            //this.m_btnBack.RemoveClickCallBack(OnBtnBackToMainCamera);
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTouchScreen;
            m_otherCameraDic.Clear();
        }

        private void FingerBackScene()
        {
            OnBtnBackToMainCamera(null);
        }

        private void OnClickQuitScene(int sceneType)
        {
            if (sceneType == 1)
            {
                OnBtnBackToMainCamera(null);
            }
        }

        private void IsCamerPoint(GameObject obj)
        {
            if (obj != null && obj.CompareTag("cameraPoint") && m_otherCameraDic.ContainsKey(obj))
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_in.ToString());
                GameEvents.SceneEvents.SetSceneType.SafeInvoke(1);
                GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(true);
                this.m_forbiddenTouch = true;
                this.m_quitObj.Visible = false;
                GameEvents.MainGameEvents.OnForbidProp.SafeInvoke(-1,true);
                this.m_currentOtherCamera = m_otherCameraDic[obj];
                HideOrShowOtherCamera(false);
                m_otherCameraDic[obj].PlayCameraTween(this.m_mainCameraTran.gameObject,(SceneCameraParams_New cameraParams,bool canZoom,string cameraName) => {
                    this.m_btnBack.Visible = true;
                    this.m_quitObj.Visible = true;
                    this.m_currentCamera = cameraName;
                    this.m_mainCamera.SetSceneCameraParam(cameraParams);
                    this.m_mainCamera.SetCanZoom(canZoom);
                    GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(false);
                    GameEvents.MainGameEvents.OnForbidProp.SafeInvoke(-1, false);
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

        private void OnBtnBackToMainCamera(GameObject obj)
        {
            if (GuidNewNodeManager.Instance.GetNodeStatus(GuidNewNodeManager.ForbidErrorSecond) == NodeStatus.None)
            {
                return;
            }
            GameEvents.SceneEvents.SetSceneType.SafeInvoke(0);
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.zoom_out.ToString());
            OnBtnBackToMainCamera(obj,null);
        }

        private void OnBtnBackToMainCamera(GameObject obj,Action cb)
        {
            this.m_btnBack.Visible = false;
            this.m_quitObj.Visible = false;
            Vector3[] wayPoint = new Vector3[2];
            wayPoint[0] = m_mainCameraTran.position;
            wayPoint[1] = m_origPosition;
            GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(true);
            this.m_forbiddenTouch = true;
            GameEvents.MainGameEvents.OnForbidProp.SafeInvoke(-1, true);
            m_mainCameraTran.DOPath(wayPoint, 2f, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                GameEvents.SceneEvents.OnQuitToMainCamera.SafeInvoke(this.m_currentCamera);
                GameEvents.SceneEvents.SetSceneType.SafeInvoke(0);
                this.m_currentOtherCamera.PlayCameraPointEffect(false);
                HideOrShowOtherCamera(true);
                this.m_mainCamera.SetCanZoom(true);
                this.m_mainCamera.SetSceneCameraParam(m_mainCameraParam);
                GameEvents.MainGameEvents.OnFingerForbidden.SafeInvoke(false);
                this.m_forbiddenTouch = false;
                GameEvents.MainGameEvents.OnForbidProp.SafeInvoke(-1, false);
                this.m_quitObj.Visible = true;
                this.m_currentCamera = "MainCamera";
                if (cb != null)
                {
                    cb();
                }
            });
            m_mainCameraTran.DORotate(m_origRotate, 2f).SetEase(Ease.OutCubic);
        }

        public void SetQuitObj(GameButton quitObj)
        {
            this.m_quitObj = quitObj;
        }

        public void CheckSceneCameraPoint(Action cb,string cameraName)
        {
            if (!this.m_btnBack.Visible || !string.IsNullOrEmpty(cameraName)&& cameraName.Equals(this.m_currentCamera))
            {
                cb();
            }
            else
            {
                OnBtnBackToMainCamera(null,cb);
            }
        }

        private void HideOrShowOtherCamera(bool visible)
        {
            foreach (var kv in m_otherCameraDic)
            {
                if (!kv.Value.Equals(this.m_currentOtherCamera))
                {
                    kv.Value.SetEffectVisible(visible);
                }
            }
        }

        public bool EntityInCurrentCamera(string cameraName)
        {
            return this.m_currentCamera.Equals(cameraName);
        }
    }
}
