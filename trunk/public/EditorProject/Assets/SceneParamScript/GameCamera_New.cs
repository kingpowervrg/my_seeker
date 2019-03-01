/********************************************************************
	created:  2018-4-11 18:28:21
	filename: GameCamera.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏内摄像机
*********************************************************************/
using DG.Tweening;
using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace SeekerGame
{
    public class GameCamera_New : MonoBehaviour
    {
        private Camera m_gameSceneCamera = null;
        public SceneCameraParams_New sceneCameraParams = null;

        //private SceneCameraParams_New m_newSceneCameraParams = null;

        public const float ZOOM_SENSITIVE = 0.2f;
        public float X_AXIS_ROTATE_THRESHOLD = 150f;
        public float Y_AXIS_ROTATE_THRESHOLD = 70f;
        private float KICK_BACKFACTOR = 2f;
        private bool m_canSlide = true;
        private bool m_needResetCamera = true;
        public virtual void OnEnable()
        {

            InitSceneEdge();
            EasyTouch.On_Swipe += OnHandSwipeHandler;
            EasyTouch.On_PinchIn += OnPinIn;
            EasyTouch.On_PinchOut += OnPinOut;
        }

        public void SetSceneCameraParam(SceneCameraParams_New cameraParams)
        {
            this.sceneCameraParams = cameraParams;

        }

        private Vector3 m_targetEulerAngle = Vector3.zero;

        protected bool InitSceneEdge()
        {
            sceneCameraParams = sceneCameraParams ?? GameSceneCamera.GetComponent<SceneCameraParams_New>();
            if (sceneCameraParams && this.m_needResetCamera)
            {
                this.m_needResetCamera = false;
                //sceneCameraParams = GameSceneCamera.GetComponent<SceneCameraParams>();
                //this.transform.LookAt(sceneCameraParams.SceneCenterTransform);
                this.m_targetEulerAngle = transform.rotation.eulerAngles;

                return true;
            }

            return false;
        }
        private Tween rotateTweener = null;
        Bounds targetDummy_0 = default(Bounds);
        Bounds targetDummy_1 = default(Bounds);

        private Tween m_zoomTweener = null;

        protected virtual void OnHandSwipeHandler(Gesture gesture)
        {
            OnHandSwipeHandler(gesture, CameraManager.moveTime);
        }
        //private Vector3 m_currentDir
        protected void OnHandSwipeHandler(Gesture gesture, float moveTime)
        {
            if (m_inZooming || !m_canSlide || this.m_isKickBack)
                return;

            //if (rotateTweener != null)
            //    rotateTweener.Kill();

            Vector2 swipeDelta = gesture.swipeVector;
            swipeDelta.x = Mathf.Sign(swipeDelta.x) * Mathf.Min(Mathf.Abs(swipeDelta.x), X_AXIS_ROTATE_THRESHOLD);
            swipeDelta.y = Mathf.Sign(swipeDelta.y) * Mathf.Min(Mathf.Abs(swipeDelta.y), Y_AXIS_ROTATE_THRESHOLD);

            Vector3 rotateValue = Vector3.right * swipeDelta.y - Vector3.up * swipeDelta.x;
            rotateValue *= CameraManager.DragDelta * 0.1f;
            Debug.Log("srcrotateValue : " + rotateValue);
           CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
            //Vector3 cacheRotate = rotateValue;
            //if (rotateTweener != null && !rotateTweener.IsComplete())
            //{
            //    Vector3 tempVec = (targetRotate - GameSceneCamera.transform.localEulerAngles) * 10f * CameraManager.DragDelta;
            //    tempVec.z = 0;
            //    rotateValue += tempVec;
            //    Debug.Log("contiune tweener ==");
            //}
            IsCameraOverStep(ref cameraDirection_0,ref cameraDirection_1,ref rotateValue);
            
            if (cameraDirection_0 != CameraDirection.None && cameraDirection_1 != CameraDirection.None)
            {
                //if (!m_needKickBack)
                //{
                //    m_srcRotate = GameSceneCamera.transform.localEulerAngles;
                //    EasyTouch.On_TouchUp -= OnTouchup;
                //    EasyTouch.On_TouchUp += OnTouchup;
                //    Debug.Log("need kickback ====");
                //}
                //this.m_needKickBack = true;

                //rotateValue = Vector3.Normalize(cacheRotate) * KICK_BACKFACTOR;
            }
            Debug.Log("rotateValue : " + rotateValue + " d_0 : " + cameraDirection_0 + "   d_1 : " + cameraDirection_1);
            m_currentDir = rotateValue * 0.1f * CameraManager.DragDelta;// + m_moveRotate;

            rotateTweener.Kill();
            targetRotate = GameSceneCamera.transform.localEulerAngles + rotateValue;// * 0.1f ;// + m_moveRotate;
            m_cacheRotate = targetRotate;
            rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, moveTime).SetEase((Ease)CameraManager.ease).OnComplete(()=> {
                m_moveRotate = Vector3.zero;
                Debug.Log("rotateTweener complete");
            });
        }
        #region new
        Vector3 targetRotate;
        #endregion

        private Vector3 RotateTest = Vector3.zero;

        private Vector3 m_srcRotate = Vector3.zero; //回弹之前的速度
        private bool m_needKickBack = false;// 是否需要回弹
        private bool m_isKickBack = false;
        private Vector3 m_cacheRotate = Vector3.zero;
        private Vector3 m_moveRotate = Vector3.zero;

        private Vector3 Lerp(Vector3 s, Vector3 e, float t)
        {
            float x = s.x * (1 - t) + e.x * t;
            float y = s.y * (1 - t) + e.y * t;
            float z = s.z * (1 - t) + e.z * t;
            return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
        }

        private Vector3 m_currentDir = Vector3.zero;

        private void OnRotateTweenerTweening()
        {
            m_moveRotate = m_cacheRotate - GameSceneCamera.transform.localEulerAngles; //当前方向
            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
           
            m_currentDir = m_moveRotate;
            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref m_currentDir);

            if (m_currentDir != Vector3.zero && cameraDirection_0 == CameraDirection.None && cameraDirection_1 == CameraDirection.None)
            {
                if (m_needKickBack)
                {
                    this.m_isKickBack = false;
                    this.m_needKickBack = false;
                    EasyTouch.On_TouchUp -= OnTouchup;
                    Debug.Log("cancel kickback === " + cameraDirection_0 + "  " + cameraDirection_1 + "  " + m_moveRotate);
                }
            }
            else
            {
                if (!m_needKickBack && !m_isKickBack)
                {
                    //Debug.Log("current dir : " + m_currentDir);
                    m_currentDir = Vector3.Normalize(m_currentDir);
                    this.rotateTweener.Kill();
                    Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + m_currentDir * CameraManager.DragDelta;
                    m_moveRotate = Vector3.zero;
                    rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, 0.2f).SetEase((Ease)CameraManager.ease).OnUpdate(CameraMoveUpdate);
                    //Debug.Log("collider : " + targetRotate + "  " + m_currentDir * 10);
                }
                return;
            }
        }

        private void CameraMoveUpdate()
        {
            //CameraDirection cameraDirection_0 = CameraDirection.None;
            //CameraDirection cameraDirection_1 = CameraDirection.None;
            //IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref m_currentDir);
            //if (cameraDirection_0 != CameraDirection.None || cameraDirection_1 != CameraDirection.None)
            //{
            //    Debug.Log("CameraMoveUpdate complete");
            //    this.rotateTweener.Kill();
            //    m_moveRotate = Vector3.zero;
            //}
        }

        private void OnPinIn(Gesture gesture)
        {
            if (!m_canSlide && this.m_isZoomKickBack)
            {
                return;
            }
            CameraZoomOut(gesture.deltaPinch, 0.02f);
            this.m_inZooming = true;
            EasyTouch.On_TouchUp -= OnTouchup;
            EasyTouch.On_TouchUp += OnTouchup;
        }

        private void OnPinOut(Gesture gesture)
        {
            if (!m_canSlide && this.m_isZoomKickBack)
            {
                return;
            }
            CameraZoomIn(gesture.deltaPinch, 0.02f);
            this.m_inZooming = true;
            EasyTouch.On_TouchUp -= OnTouchup;
            EasyTouch.On_TouchUp += OnTouchup;
        }

        private void OnTouchup(Gesture gesture)
        {
            if (m_needKickBack)
            {
                rotateTweener.Kill();
                this.m_isKickBack = true;
                rotateTweener = GameSceneCamera.transform.DOLocalRotate(m_srcRotate, 0.2f).OnComplete(() =>
                {
                    m_needKickBack = false;
                    this.m_isKickBack = false;
                    Debug.Log("cancel kickback === ");
                });
                EasyTouch.On_TouchUp -= OnTouchup;
            }
            if (m_needZoomKickBack)
            {
                //Debug.Log("play CameraPreZoomKickBack");
                this.m_zoomTweener.Kill();
                this.m_isZoomKickBack = true;
                this.m_zoomTweener = GameSceneCamera.transform.DOMove(m_zoomKickBackPos, 0.2f).OnComplete(() =>
                {
                   // Debug.Log("CameraPreZoomKickBack complete");
                    m_needZoomKickBack = false;
                    this.m_isZoomKickBack = false;
                });
                EasyTouch.On_TouchUp -= OnTouchup;
            }
            if (!m_canSlide)
            {
                return;
            }
            
            EasyTouch.On_TouchUp -= OnTouchup;
            //EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            this.m_inZooming = false;
        }

        private bool m_needZoomKickBack = false;
        private bool m_isZoomKickBack = false;
        private Vector3 m_zoomKickBackPos = Vector3.zero;
        /// <summary>
        /// 场景放大
        /// </summary>
        /// <param name="delta"></param>
        private void CameraZoomIn(float delta, float moveTime)
        {
            if (GameSceneCamera.transform.position.z >= sceneCameraParams.ZNear)
            {
                CameraPreZoomKickBack();
                //return;
            }

            if (m_zoomTweener != null)
                m_zoomTweener.Kill();
            float pinInDelta = CameraManager.PinInDelta;
            if (this.m_needKickBack)
            {
                pinInDelta *= CameraManager.ZoomKick;
            }
            Vector3 cameraTargetPos = GameSceneCamera.transform.forward * delta * 0.03f * pinInDelta + GameSceneCamera.transform.position;
            if (cameraTargetPos.z > sceneCameraParams.ZNear)
            {
                pinInDelta = CameraManager.PinInDelta * CameraManager.ZoomKick;
                //cameraTargetPos = GameSceneCamera.transform.position + GameSceneCamera.transform.forward * ((sceneCameraParams.ZNear - GameSceneCamera.transform.position.z) / GameSceneCamera.transform.forward.z);
                cameraTargetPos = GameSceneCamera.transform.forward * delta * 0.03f * pinInDelta + GameSceneCamera.transform.position;
                CameraPreZoomKickBack();
            }

            DoZoomCamera(cameraTargetPos, moveTime);
            //EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
        }

        private void CameraPreZoomKickBack()
        {
            if (!this.m_needZoomKickBack)
            {
                Debug.Log("Register CameraPreZoomKickBack");
                this.m_needZoomKickBack = true;
                this.m_zoomKickBackPos = GameSceneCamera.transform.position;
                EasyTouch.On_TouchUp -= OnTouchup;
                EasyTouch.On_TouchUp += OnTouchup;
            }
            
        }

        /// <summary>
        /// 场景缩小
        /// </summary>
        /// <param name="delta"></param>
        private void CameraZoomOut(float delta, float moveTime)
        {
            if (GameSceneCamera.transform.position.z <= sceneCameraParams.ZFar)
                return;

            if (m_zoomTweener != null)
                m_zoomTweener.Kill();

            Vector3 cameraZoomDirection = (sceneCameraParams.CameraOriginPoint - GameSceneCamera.transform.position).normalized;
            Vector3 cameraTargetPos = GameSceneCamera.transform.position + cameraZoomDirection * delta * 0.03f * CameraManager.PinInDelta;
            if (cameraTargetPos.z < sceneCameraParams.CameraOriginPoint.z)
                cameraTargetPos = sceneCameraParams.CameraOriginPoint;

            //zoom camera
            DoZoomCamera(cameraTargetPos, moveTime);
            //EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
            //correct angle
            float rotateLerpFactor = (GameSceneCamera.transform.position.z - cameraTargetPos.z) / (GameSceneCamera.transform.position.z - sceneCameraParams.ZFar);
            if (rotateLerpFactor <= 1 && rotateLerpFactor >= 0)
            {
                if (rotateTweener != null)
                    rotateTweener.Kill();
                //Quaternion q = Quaternion.Slerp(GameSceneCamera.transform.rotation, sceneCameraParams.CameraOriginRotation, rotateLerpFactor);

                //rotateTweener = GameSceneCamera.transform.DORotateQuaternion(q, 0.2f).OnUpdate(() =>
                //{
                //    if (GameSceneCamera.transform.position.z <= sceneCameraParams.ZFar)
                //    {
                //        rotateTweener.Kill();
                //    }
                //});
            }

        }

        private void DoZoomCamera(Vector3 cameraTargetPos, float moveTime)
        {
            if ((GameSceneCamera.transform.position - cameraTargetPos).sqrMagnitude <= 0.1f)
                return;

            this.m_zoomTweener = GameSceneCamera.transform.DOMove(cameraTargetPos, moveTime).OnUpdate(() =>
            {
                this.m_inZooming = true;
                if (GameSceneCamera.transform.position.z <= sceneCameraParams.ZFar || GameSceneCamera.transform.position.z >= sceneCameraParams.ZNear)
                {
                    m_zoomTweener.Kill();
                    CameraPreZoomKickBack();

                    this.m_inZooming = false;
                }
            }).OnComplete(() =>
            {
                this.m_inZooming = false;
            });
        }


        private Vector3 GetZoomPolePosition(Vector3 currentPosition, Vector3 direction, float poleZ)
        {
            return Vector3.zero;
        }


        private bool m_inZooming = false;

        protected virtual void OnDisable()
        {
            if (this.m_zoomTweener != null)
                this.m_zoomTweener.Kill();

            if (this.rotateTweener != null)
                this.rotateTweener.Kill();
            EasyTouch.On_Swipe -= OnHandSwipeHandler;
            EasyTouch.On_PinchIn -= OnPinIn;
            EasyTouch.On_PinchOut -= OnPinOut;
        }

        private void OnCameraZoomOrRotation(int type, Gesture gesture, float moveTime)
        {
            if (type == 0) //放大
            {
                CameraZoomIn(gesture.deltaPinch, moveTime);
            }
            else if (type == 1) //缩小
            {
                CameraZoomOut(gesture.deltaPinch, moveTime);
            }
            else if (type == 2) //旋转
            {
                OnHandSwipeHandler(gesture, moveTime);
            }
        }

        /// <summary>
        /// 禁用手指
        /// </summary>
        /// <param name="isForbidden"></param>
        private void OnFingerForbidden(bool isForbidden)
        {
            if (isForbidden)
            {
                //EngineCoreEvents.InputEvent.OnPinIn -= OnPinIn;
                //EngineCoreEvents.InputEvent.OnPinOut -= OnPinOut;
                //EngineCoreEvents.InputEvent.OnSwipe -= OnHandSwipeHandler;
            }
            else
            {
                //EngineCoreEvents.InputEvent.OnPinIn += OnPinIn;
                //EngineCoreEvents.InputEvent.OnPinOut += OnPinOut;
                //EngineCoreEvents.InputEvent.OnSwipe += OnHandSwipeHandler;
            }
        }


        public Camera GameSceneCamera
        {
            get
            {
                m_gameSceneCamera = m_gameSceneCamera ?? Camera.main;
                return m_gameSceneCamera;
            }
        }

        #region new 

        //public Vector3 GetKickBackDirection(CameraDirection cameraDirection_0,CameraDirection cameraDirection_1,Vector3 rotateValue)
        //{
        //    rotateValue = Vector3.Normalize(rotateValue);
        //    rotateValue = rotateValue* KICK_BACKFACTOR;
        //}
        private float IsBoundsOutX(float x)
        {
            if (x < 0)
            {
                float tempX = sceneCameraParams.CurrentminAngleX - transform.localEulerAngles.x;
                if (Mathf.Abs(tempX) > X_AXIS_ROTATE_THRESHOLD)
                {
                    //小于0的角度
                    tempX -= 360f;
                }
                if (tempX >= 0)
                {
                    return 0;
                }
                return Mathf.Max(x, tempX);
            }
            else if (x > 0)
            {
                float tempX = sceneCameraParams.CurrentmaxAngleX - transform.localEulerAngles.x;
                if (Mathf.Abs(tempX) > X_AXIS_ROTATE_THRESHOLD)
                {
                    //小于0的角度
                    tempX += 360f;
                }
                if (tempX <= 0)
                {
                    return 0;
                }
                return Mathf.Min(x, tempX);
            }
            return x;
        }

        private float IsBoundsOutY(float y)
        {
            if (y > 0)
            {
                float tempY = sceneCameraParams.CurrentmaxAngleY - transform.localEulerAngles.y;
                if (Mathf.Abs(tempY) > Y_AXIS_ROTATE_THRESHOLD)
                {
                    //小于0的角度
                    tempY += 360f;
                }
                if (tempY <= 0)
                {
                    return 0;
                }
                return Mathf.Min(y, tempY);
            }
            else if (y < 0)
            {
                float tempY = sceneCameraParams.CurrentminAngleY - transform.localEulerAngles.y;
                if (tempY > Y_AXIS_ROTATE_THRESHOLD)
                {
                    //小于0的角度
                    tempY -= 360f;
                }
                if (tempY >= 0)
                {
                    return 0;
                }
                return Mathf.Max(y, tempY);
            }
            return y;
        }


        public void IsCameraOverStep(ref CameraDirection direction_0, ref CameraDirection direction_1,ref Vector3 rotateValue)
        {
            //x > 0  up  y<0 right 
            direction_0 = CameraDirection.None;
            direction_1 = CameraDirection.None;
            float tempY = IsBoundsOutY(rotateValue.y);
            
            if (tempY > -0.001 && tempY < 0.001 && rotateValue.y != 0)
            {
                direction_0 = rotateValue.y > 0 ? CameraDirection.Right : CameraDirection.Left;
            }
            float tempX = IsBoundsOutX(rotateValue.x);
            
            if (tempX > -0.001  && tempX < 0.001 && rotateValue.x != 0)
            {
                direction_1 = rotateValue.x > 0 ? CameraDirection.Down : CameraDirection.Up;
            }
            rotateValue.y = tempY;
            rotateValue.x = tempX;
        }
        #endregion
    }

    //越界方向
    //public struct CameraOverStep
    //{
    //    public CameraDirection direction_0;
    //    public CameraDirection direction_1;
    //}
}
