/********************************************************************
	created:  2018-4-8 12:21:43
	filename: CameraController.cs
	author:	  songguangze@outlook.com
	
	purpose:  摄像机管理器
*********************************************************************/
using EngineCore;
using SeekerGame.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeekerGame
{
    public class CameraManager : Singleton<CameraManager>
    {
        private Camera m_gameMainCamera = null;

        private SceneBlur m_sceneBlur = new SceneBlur();
        private GameCameraNew m_gameCameraController = null;
        private PostFX.PostEffectBehaviour m_postEffectBehaviour = null;

        private float m_dragDelta = 1f;
        private float m_pinInDelta = 1f;
        private float m_ZoomKick = 1f;

        public float DragDelta
        {
            get
            {
                if (m_isAutoPlay)
                {
                    return 1f;
                }
                return m_dragDelta;
            }
        }

        public float PinInDelta
        {
            get
            {
                if (m_isAutoPlay)
                {
                    return 1f;
                }
                return m_pinInDelta;
            }
        }

        public float ZoomKick
        {
            get { return m_ZoomKick; }
        }

        private bool m_isAutoPlay = false;
        public bool IsAutoPlay
        {
            get { return m_isAutoPlay; }
            set { m_isAutoPlay = value; }
        }

        public void Init()
        {
            //this.TempCamera = GameObject.Find("TempCamera").GetComponent<Camera>();
            EngineCoreEvents.ResourceEvent.OnLoadAdditiveScene += OnLoadAdditiveScene;
            EngineCoreEvents.CameraEvents.EnableMainCamera = EnableGameSceneCamera;
        }

        public void ResetMainCamera()
        {
            MainCamera.cullingMask = LayerMask.GetMask("Default", "SceneTarget");
        }

        /// <summary>
        /// 是否激活场景摄像机控制器
        /// </summary>
        public bool EnableGameCameraController
        {
            get
            {
                return this.m_gameCameraController != null && this.m_gameCameraController.enabled;
            }
            set
            {
                if (this.m_gameCameraController != null)
                {
                    this.m_gameCameraController.enabled = value;
                }
            }
        }

        public SceneBlur ScreenBlurComponent
        {
            get { return this.m_sceneBlur; }
        }

        public Camera MainCamera
        {
            get
            {
                return this.m_gameMainCamera;
            }
            private set
            {
                this.m_gameMainCamera = value;
            }
        }

        public Camera UICamera
        {
            get { return FrameMgr.Instance.UICamera; }
        }

        public PostFX.PostEffectBehaviour PostEffectBehaviour
        {
            get
            {
                if (this.m_postEffectBehaviour == null)
                    this.m_postEffectBehaviour = MainCamera.gameObject.GetOrAddComponent<PostFX.PostEffectBehaviour>();

                return this.m_postEffectBehaviour;
            }
        }

        public Camera ScreenPaintBoardCamera
        {
            get
            {
                return ScreenDrawer.instance.ScreenDrawingBoardCamera;
            }
        }

        public T GetPostFX<T>(PostFX.EffectType postFXType) where T : PostFX.PostEffectBase
        {
            T postEffect = PostEffectBehaviour.GetPostEffectsList().Find(postEffectBase => postEffectBase.et == postFXType) as T;
            return postEffect;
        }

        private void OnLoadAdditiveScene(Scene loadedAdditiveScene, GameObject[] sceneRootObjects)
        {
            for (int i = 0; i < sceneRootObjects.Length; ++i)
            {
                if (sceneRootObjects[i].name == "Main Camera")
                {
                    MainCamera = sceneRootObjects[i].GetComponent<Camera>();
                    break;
                }
            }
        }

        private void InitGameMainCamera()
        {
            if (this.m_gameMainCamera == null)
            {
                GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCameraObject != null)
                    this.m_gameMainCamera = mainCameraObject.GetComponent<Camera>();
            }

        }

        private void EnableGameSceneCamera(bool isEnable)
        {
            InitGameMainCamera();

            if (this.m_gameMainCamera != null)
                this.m_gameMainCamera.enabled = isEnable;
        }

        public void InitCameraController<T>(bool isEnable = true) where T : GameCameraNew
        {
            if (this.m_gameCameraController != null)
            {
                if (!(this.m_gameCameraController is T))
                {
                    GameObject.Destroy(this.m_gameCameraController);
                    this.m_gameCameraController = MainCamera.gameObject.AddComponent<T>();
                }
            }
            else
                this.m_gameCameraController = MainCamera.gameObject.AddComponent<T>();

            this.m_gameCameraController.enabled = isEnable;

        }

        public void SetCameraParam(long sceneId)
        {
            this.m_dragDelta = 1f;
            this.m_pinInDelta = 1f;
            ConfScene confscene = ConfScene.Get(sceneId);
            string cameraParam = confscene.cameraParam.Trim();
            if (confscene != null && !string.IsNullOrEmpty(cameraParam))
            {
                string[] paramSplit = cameraParam.Split(',');
                if (paramSplit.Length >= 1)
                {
                    m_dragDelta = float.Parse(paramSplit[0]);
                }
                if (paramSplit.Length >= 2)
                {
                    m_pinInDelta = float.Parse(paramSplit[1]);
                }
                if (paramSplit.Length >= 3)
                {
                    m_ZoomKick = float.Parse(paramSplit[2]);
                }
            }
        }
    }
}