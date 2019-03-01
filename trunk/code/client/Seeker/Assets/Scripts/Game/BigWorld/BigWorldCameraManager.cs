using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class BigWorldCameraManager : Singleton<BigWorldCameraManager>
    {
        private bool m_needTweener = true;
        public bool NeedTweener
        {
            get {
                return m_needTweener;
            }
        }

        public Camera m_camera
        {
            get
            {
                return CameraManager.Instance.MainCamera;
            }
        }

        private BigWorldCameraTrans m_bigWorldCameraTrans = null;

        public void AddCamera()
        {
            if (m_camera != null && m_bigWorldCameraTrans == null)
            {
                m_bigWorldCameraTrans = m_camera.gameObject.AddComponent<BigWorldCameraTrans>();
            }
        }

        private BigWorldCamera m_bigWorldCamera = null;
        public bool bigWorldCameraEnable
        {
            get
            {
                if (m_bigWorldCamera != null)
                {
                    return m_bigWorldCamera.enabled;
                }
                return false;
            }
            set
            {
                if (m_bigWorldCamera == null)
                {
                    m_bigWorldCamera = m_camera.gameObject.GetOrAddComponent<BigWorldCamera>();
                }
                m_bigWorldCamera.enabled = value;
            }
        }

        public void SetTarget(Transform trans,System.Action onComplete)
        {
            if (m_bigWorldCameraTrans == null)
            {
                return;
            }
            
            if (m_needTweener)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    m_bigWorldCameraTrans.SetTarget(trans, onComplete);
                }, 1f);
            }
            else
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            }
            m_needTweener = false;

        }

        public void SetTargetCamera(Transform trans)
        {
            if (m_bigWorldCameraTrans == null)
            {
                return;
            }
            TimeModule.Instance.SetTimeout(() =>
            {
                m_bigWorldCameraTrans.SetTarget(trans,null);
            }, 1f);
        }
        public void OnDestory()
        {
            m_bigWorldCameraTrans = null;
        }
    }
}
