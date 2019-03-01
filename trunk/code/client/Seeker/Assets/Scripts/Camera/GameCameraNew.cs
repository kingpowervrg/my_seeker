/********************************************************************
	created:  2018-4-11 18:28:21
	filename: GameCamera.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏内摄像机
*********************************************************************/
using DG.Tweening;
using EngineCore;
using EngineCore.Utility;
using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace SeekerGame
{
    public class GameCameraNew : MonoBehaviour
    {
        private Camera m_gameSceneCamera = null;
        public SceneCameraParams_New sceneCameraParams = null;

        public const float ZOOM_SENSITIVE = 0.2f;
        public float X_AXIS_ROTATE_THRESHOLD = 150f;
        public float Y_AXIS_ROTATE_THRESHOLD = 70f;
        private float KICK_BACKFACTOR = 0.3f;

        private bool m_canSlide = true;
        private bool m_needResetCamera = true;

        public System.Action backAction;
        public virtual void OnEnable()
        {
            //if (sceneCameraParams == null)
            //{
            //    sceneCameraParams = GameSceneCamera.GetComponent<SceneCameraParams>();
            //    InitSceneEdge();
            //}
            InitSceneEdge();

            EngineCoreEvents.InputEvent.OnSwipe += OnHandSwipeHandler;
            EngineCoreEvents.InputEvent.OnPinIn += OnPinIn;
            EngineCoreEvents.InputEvent.OnPinOut += OnPinOut;
            GameEvents.MainGameEvents.OnCameraMove += MoveCamera;
            GameEvents.MainGameEvents.OnFingerForbidden += OnFingerForbidden;
            GameEvents.MainGameEvents.OnCameraZoomOrRotation += OnCameraZoomOrRotation;
            GameEvents.MainGameEvents.GetCameraBound = GetCameraBound;
            GameEvents.MainGameEvents.OnClearCameraStatus += OnClearCameraStatus;
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
            if (sceneCameraParams == null)
            {
                return;
            }
            OnHandSwipeHandler(gesture, 0.2f);
        }

        protected void OnHandSwipeHandler(Gesture gesture, float moveTime,bool needLimit = true)
        {
            if (m_inZooming || !m_canSlide || this.m_isKickBack)
                return;

            Vector2 swipeDelta = gesture.swipeVector;
            if (needLimit)
            {
                swipeDelta.x = Mathf.Sign(swipeDelta.x) * Mathf.Min(Mathf.Abs(swipeDelta.x), X_AXIS_ROTATE_THRESHOLD);
                swipeDelta.y = Mathf.Sign(swipeDelta.y) * Mathf.Min(Mathf.Abs(swipeDelta.y), Y_AXIS_ROTATE_THRESHOLD);
            }
            

            Vector3 rotateValue = Vector3.right * swipeDelta.y - Vector3.up * swipeDelta.x;
            rotateValue *= CameraManager.Instance.DragDelta * 0.1f;
            Vector3 cacheRotate = rotateValue;
            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;

            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref rotateValue);
            if (cameraDirection_0 != CameraDirection.None && cameraDirection_1 != CameraDirection.None)
            {
                if (!m_needKickBack)
                {
                    m_srcRotate = GameSceneCamera.transform.localEulerAngles;
                    EasyTouch.On_TouchUp -= OnTouchup;
                    EasyTouch.On_TouchUp += OnTouchup;
                }
                this.m_needKickBack = true;

                rotateValue = Vector3.Normalize(cacheRotate) * KICK_BACKFACTOR;
            }

            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
            Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + rotateValue;// + m_moveRotate;
            rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, moveTime).SetEase(Ease.OutSine);//.OnComplete(() =>
            EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
        }
        private Vector3 m_srcRotate = Vector3.zero;
        private bool m_needKickBack = false;// 是否需要回弹
        private bool m_isKickBack = false;
        private bool m_canZoom = true;
        public void SetSceneCameraParam(SceneCameraParams_New cameraParams)
        {
            this.sceneCameraParams = cameraParams;
        }

        public void SetCanZoom(bool canZoom)
        {
            this.m_canZoom = canZoom;
        }

        float time = 0f;
        Vector3 endForward = Vector3.zero;
        Vector3 startForward = Vector3.zero;
        protected void MoveCamera(GameObject obj)
        {
            if (sceneCameraParams == null)
                return;
            this.m_canSlide = false;
            Vector3 targetPos = Vector3.Normalize(obj.transform.position - transform.position);
            Vector3 cameraPos = Vector3.Normalize(transform.forward);
            Vector3 cameraMoveDir = targetPos - cameraPos;

            Vector3 targetXPos = targetPos;
            targetXPos.y = 0;
            Vector3 cameraXPos = cameraPos;
            cameraXPos.y = 0;
            float yAngle = Vector3.Angle(targetXPos,cameraXPos);
            if (cameraMoveDir.x < 0) //右边
            {
                yAngle = -yAngle;
            }
            
            Vector3 targetYPos = targetPos;
            targetYPos.x = 0;
            Vector3 cameraYPos = cameraPos;
            cameraYPos.x = 0;
            float xAngle = Vector3.Angle(targetYPos, cameraYPos);
            if (cameraMoveDir.y > 0)
            {
                xAngle = -xAngle;
            }
            Vector3 rotateValue = Vector3.right * xAngle + Vector3.up * yAngle;
            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref rotateValue);
            if (rotateTweener != null)
            {
                rotateTweener.Kill();
            }
            Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + rotateValue;// + m_moveRotate;
            rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, 1f).SetEase(Ease.OutSine).OnComplete(() => {
                this.m_canSlide = true;
            });

            //endForward = Vector3.Normalize(obj.transform.position - GameSceneCamera.transform.position);
            //GameSceneCamera.transform.forward = Vector3.Normalize(GameSceneCamera.transform.forward);
            //startForward = GameSceneCamera.transform.forward;

            //time = 0f;
            //TimeModule.Instance.SetTimeInterval(LerpMoveCamera, 0.01f);
        }


        private void OnPinIn(Gesture gesture)
        {
            if (!this.m_canZoom && gesture.deltaPinch >= 5f && backAction != null)
            {
                backAction();
            }
            if (!this.m_canZoom || !m_canSlide || sceneCameraParams == null)
            {
                return;
            }
            CameraZoomOut(gesture.deltaPinch, 0.02f);
            this.m_inZooming = true;

            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
        }

        private void OnPinOut(Gesture gesture)
        {
            if (!this.m_canZoom || !m_canSlide || sceneCameraParams == null)
            {
                return;
            }
            CameraZoomIn(gesture.deltaPinch, 0.02f);
            this.m_inZooming = true;

            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
        }

        private void OnTouchup(Gesture gesture)
        {
            if (m_needKickBack)
            {
                if (isCameraOutPut())
                {
                    m_needKickBack = false;
                    this.m_isKickBack = false;
                    EasyTouch.On_TouchUp -= OnTouchup;
                    return;
                }
                rotateTweener.Kill();
                this.m_isKickBack = true;
                rotateTweener = GameSceneCamera.transform.DOLocalRotate(m_srcRotate, 0.2f).OnComplete(() =>
                {
                    m_needKickBack = false;
                    this.m_isKickBack = false;
                });
                EasyTouch.On_TouchUp -= OnTouchup;
            }
            if (m_needZoomKickBack)
            {
                this.m_zoomTweener.Kill();
                this.m_isZoomKickBack = true;
                this.m_zoomTweener = GameSceneCamera.transform.DOMove(m_zoomKickBackPos, 0.2f).OnComplete(() =>
                {
                    m_needZoomKickBack = false;
                    this.m_isZoomKickBack = false;
                });
                EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            }
            if (!m_canSlide)
            {
                return;
            }
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            this.m_inZooming = false;
        }


        /// <summary>
        /// 场景放大
        /// </summary>
        /// <param name="delta"></param>
        private void CameraZoomIn(float delta, float moveTime)
        {
            if (GameSceneCamera.transform.position.z >= sceneCameraParams.ZNear)
            {
                CameraPreZoomKickBack();
                //Debug.Log("kick back ==");
                //return;
            }

            if (m_zoomTweener != null)
                m_zoomTweener.Kill();

            float pinInDelta = CameraManager.Instance.PinInDelta;
            if (this.m_needKickBack)
            {
                pinInDelta *= CameraManager.Instance.ZoomKick;
            }
            Vector3 cameraTargetPos = GameSceneCamera.transform.forward * delta * 0.03f * pinInDelta + GameSceneCamera.transform.position;
            if (cameraTargetPos.z > sceneCameraParams.ZNear)
            {
                pinInDelta = CameraManager.Instance.PinInDelta * CameraManager.Instance.ZoomKick;
                //cameraTargetPos = GameSceneCamera.transform.position + GameSceneCamera.transform.forward * ((sceneCameraParams.ZNear - GameSceneCamera.transform.position.z) / GameSceneCamera.transform.forward.z);
                cameraTargetPos = GameSceneCamera.transform.forward * delta * 0.03f * pinInDelta + GameSceneCamera.transform.position;
                CameraPreZoomKickBack();
            }

            DoZoomCamera(cameraTargetPos, moveTime);
            EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
        }

        private bool m_needZoomKickBack = false;
        private bool m_isZoomKickBack = false;
        private Vector3 m_zoomKickBackPos = Vector3.zero;
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
            Vector3 cameraTargetPos = GameSceneCamera.transform.position + cameraZoomDirection * delta * 0.03f * CameraManager.Instance.PinInDelta;
            if (cameraTargetPos.z < sceneCameraParams.CameraOriginPoint.z)
                cameraTargetPos = sceneCameraParams.CameraOriginPoint;

            //zoom camera
            DoZoomCamera(cameraTargetPos, moveTime);
            EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
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

        private void CameraPreZoomKickBack()
        {
            if (!this.m_needZoomKickBack)
            {
                this.m_needZoomKickBack = true;
                this.m_zoomKickBackPos = GameSceneCamera.transform.position;
                EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
                EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
            }

        }

        private bool m_inZooming = false;

        protected virtual void OnDisable()
        {
            if (this.m_zoomTweener != null)
                this.m_zoomTweener.Kill();

            if (this.rotateTweener != null)
                this.rotateTweener.Kill();
            //TimeModule.Instance.RemoveTimeaction(LerpMoveCamera);
            EngineCoreEvents.InputEvent.OnPinIn -= OnPinIn;
            EngineCoreEvents.InputEvent.OnPinOut -= OnPinOut;
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            EngineCoreEvents.InputEvent.OnSwipe -= OnHandSwipeHandler;
            GameEvents.MainGameEvents.OnCameraMove -= MoveCamera;
            GameEvents.MainGameEvents.OnFingerForbidden -= OnFingerForbidden;
            GameEvents.MainGameEvents.OnCameraZoomOrRotation -= OnCameraZoomOrRotation;
            GameEvents.MainGameEvents.OnClearCameraStatus -= OnClearCameraStatus;
            CameraManager.Instance.IsAutoPlay = false;
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
                OnHandSwipeHandler(gesture, moveTime,false);
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
                EngineCoreEvents.InputEvent.OnPinIn -= OnPinIn;
                EngineCoreEvents.InputEvent.OnPinOut -= OnPinOut;
                EngineCoreEvents.InputEvent.OnSwipe -= OnHandSwipeHandler;
            }
            else
            {
                EngineCoreEvents.InputEvent.OnPinIn += OnPinIn;
                EngineCoreEvents.InputEvent.OnPinOut += OnPinOut;
                EngineCoreEvents.InputEvent.OnSwipe += OnHandSwipeHandler;
            }
        }

        public Vector3 GetCameraBound()
        {
            float zoomFactor = sceneCameraParams.ZNear - GameSceneCamera.transform.position.z;
            zoomFactor = zoomFactor / 0.03f;

            float leftAngle = Mathf.Abs(sceneCameraParams.minAngleY_0 * 10f);
            float rightAngle = Mathf.Abs(sceneCameraParams.maxAngleY_0 * 10f);
            float centerAngle = Mathf.Min(leftAngle,rightAngle);
            return new Vector3(zoomFactor, centerAngle, centerAngle);
        }

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

        private bool isCameraOutPut()
        {
            float currentAngleX = transform.localEulerAngles.x;
            float minAngleX = sceneCameraParams.CurrentminAngleX;
            if (sceneCameraParams.CurrentminAngleX >= sceneCameraParams.CurrentmaxAngleX)
            {
                minAngleX = sceneCameraParams.CurrentminAngleX - 360f;
                if (currentAngleX > 180f)
                {
                    currentAngleX = currentAngleX - 360f;
                }
            }

            float currentAngleY = transform.localEulerAngles.y;
            float minAngleY = sceneCameraParams.CurrentminAngleY;
            if (sceneCameraParams.CurrentminAngleY >= sceneCameraParams.CurrentmaxAngleY)
            {
                minAngleY = sceneCameraParams.CurrentminAngleY - 360f;
                if (currentAngleY > 180f)
                {
                    currentAngleY = currentAngleY - 360f;
                }
            }

            if (currentAngleX > minAngleX && currentAngleX < sceneCameraParams.CurrentmaxAngleX)
            {
                if (currentAngleY > minAngleY && currentAngleY < sceneCameraParams.CurrentmaxAngleY)
                {
                    return true;
                }
            }
            return false;
        }

        public void IsCameraOverStep(ref CameraDirection direction_0, ref CameraDirection direction_1, ref Vector3 rotateValue)
        {
            direction_0 = CameraDirection.None;
            direction_1 = CameraDirection.None;
            float tempY = IsBoundsOutY(rotateValue.y);

            if (tempY > -0.001 && tempY < 0.001 && rotateValue.y != 0)
            {
                direction_0 = rotateValue.y > 0 ? CameraDirection.Right : CameraDirection.Left;
            }
            float tempX = IsBoundsOutX(rotateValue.x);

            if (tempX > -0.001 && tempX < 0.001 && rotateValue.x != 0)
            {
                direction_1 = rotateValue.x > 0 ? CameraDirection.Down : CameraDirection.Up;
            }
            rotateValue.y = tempY;
            rotateValue.x = tempX;
        }

        protected void OnClearCameraStatus()
        {
            this.m_needZoomKickBack = false;
            this.m_isZoomKickBack = false;
            this.m_isKickBack = false;
            this.m_needKickBack = false;
        }

        public Camera GameSceneCamera
        {
            get
            {
                m_gameSceneCamera = m_gameSceneCamera ?? Camera.main;
                return m_gameSceneCamera;
            }
        }

    }

    public enum CameraDirection
    {
        Left,
        Right,
        Up,
        Down,
        None
    }
}