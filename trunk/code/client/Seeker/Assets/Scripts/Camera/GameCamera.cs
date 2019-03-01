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
    public class GameCamera : MonoBehaviour
    {
        private Camera m_gameSceneCamera = null;
        public SceneCameraParams sceneCameraParams = null;
        public const float ZOOM_SENSITIVE = 0.2f;
        public float X_AXIS_ROTATE_THRESHOLD = 150f;
        public float Y_AXIS_ROTATE_THRESHOLD = 70f;
        private float KICK_BACKFACTOR = 2f;

        private bool m_canSlide = true;
        private bool m_needResetCamera = true;

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
            sceneCameraParams = sceneCameraParams ?? GameSceneCamera.GetComponent<SceneCameraParams>();
            if (sceneCameraParams && this.m_needResetCamera)
            {
                this.m_needResetCamera = false;
                //sceneCameraParams = GameSceneCamera.GetComponent<SceneCameraParams>();
                this.transform.LookAt(sceneCameraParams.SceneCenterTransform);
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

        protected void OnHandSwipeHandler(Gesture gesture, float moveTime)
        {
            if (m_inZooming || !m_canSlide || this.m_isKickBack)
                return;

            if (rotateTweener != null)
                rotateTweener.Kill();

            Vector2 swipeDelta = gesture.swipeVector;
            swipeDelta.x = Mathf.Sign(swipeDelta.x) * Mathf.Min(Mathf.Abs(swipeDelta.x), X_AXIS_ROTATE_THRESHOLD);
            swipeDelta.y = Mathf.Sign(swipeDelta.y) * Mathf.Min(Mathf.Abs(swipeDelta.y), Y_AXIS_ROTATE_THRESHOLD);

            Vector3 rotateValue = Vector3.right * swipeDelta.y - Vector3.up * swipeDelta.x;

            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
            Vector3 cacheRotate = rotateValue;
            //Debug.Log("rotateValue" + rotateValue);
            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref rotateValue);

            if (cameraDirection_0 != CameraDirection.None && cameraDirection_1 != CameraDirection.None)
            {
                //Debug.Log("rotateValue" + rotateValue + "  targetDummy_0 : " + targetDummy_0 + "targetDummy_1 :  " + targetDummy_1 + " d_0 : " + cameraDirection_0 + "   d_1 : " + cameraDirection_1);
                if (!m_needKickBack)
                {
                    m_srcRotate = GameSceneCamera.transform.localEulerAngles;
                    EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
                    EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
                }
                this.m_needKickBack = true;

                rotateValue = Vector3.Normalize(cacheRotate) * KICK_BACKFACTOR;
            }
            m_currentDir = rotateValue * 0.1f * CameraManager.Instance.DragDelta;// + m_moveRotate;
            rotateTweener.Kill();
            Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + rotateValue * 0.1f * CameraManager.Instance.DragDelta;// + m_moveRotate;
            m_cacheRotate = targetRotate;

            rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, moveTime).SetEase(Ease.OutSine).OnUpdate(OnRotateTweenerTweening);//.OnComplete(() =>
            

            EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
            //Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + rotateValue * 0.1f * CameraManager.Instance.DragDelta;

            //rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, moveTime).OnUpdate(OnRotateTweenerTweening);
        }

        private Vector3 m_srcRotate = Vector3.zero; //回弹之前的速度
        private bool m_needKickBack = false;// 是否需要回弹
        private bool m_isKickBack = false;
        private Vector3 m_cacheRotate = Vector3.zero;
        //private Vector3 m_moveRotate = Vector3.zero;
        private Vector3 m_currentDir = Vector3.zero;
        private bool m_canZoom = true;
        public void SetSceneCameraParam(SceneCameraParams cameraParams)
        {
            this.sceneCameraParams = cameraParams;
            
        }

        public void SetCanZoom(bool canZoom)
        {
            this.m_canZoom = canZoom;
        }

        private bool IsBoundsInCameraView(ref Vector3 rotateValue)
        {
            Bounds bound0 = default(Bounds);
            Bounds bound1 = default(Bounds);
            if (Mathf.Abs(rotateValue.x) < Mathf.Abs(rotateValue.y) && (Mathf.Atan(Mathf.Abs(rotateValue.x / rotateValue.y))) * Mathf.Rad2Deg <= 10f)
            {
                bound1 = GetRotateY(rotateValue.y);
                rotateValue.x = 0f;

                return CameraUtility.IsBoundsInCameraView(bound0, bound1);
            }
            else if (Mathf.Abs(rotateValue.y) < Mathf.Abs(rotateValue.x) && Mathf.Atan(Mathf.Abs(rotateValue.y / rotateValue.x)) * Mathf.Rad2Deg <= 10f)
            {
                bound0 = GetRotateX(rotateValue.x);
                rotateValue.y = 0f;
                return CameraUtility.IsBoundsInCameraView(bound0, bound1);
            }
            bound0 = GetRotateX(rotateValue.x);
            bound1 = GetRotateY(rotateValue.y);
            return CameraUtility.IsBoundsInCameraView(bound0, bound1);
        }

        private Bounds GetRotateX(float x)
        {
            Bounds bound0 = default(Bounds);
            if (x > 0)
            {
                bound0 = sceneCameraParams.SceneDownBounds;
            }
            else if (x < 0)
            {
                bound0 = sceneCameraParams.SceneUpBounds;
            }
            return bound0;
        }

        private Bounds GetRotateY(float y)
        {
            Bounds bound1 = default(Bounds);
            if (y > 0)
            {
                bound1 = sceneCameraParams.SceneRightBounds;
            }
            else if (y < 0)
            {
                bound1 = sceneCameraParams.SceneLeftBounds;
            }
            return bound1;
        }


        float time = 0f;
        Vector3 endForward = Vector3.zero;
        Vector3 startForward = Vector3.zero;
        private void MoveCamera(GameObject obj)
        {
            if (sceneCameraParams == null)
                return;
            this.m_canSlide = false;
            endForward = Vector3.Normalize(obj.transform.position - GameSceneCamera.transform.position);
            GameSceneCamera.transform.forward = Vector3.Normalize(GameSceneCamera.transform.forward);
            startForward = GameSceneCamera.transform.forward;
            //endForward.z = startForward.z;
            //Debug.Log("  " + (tempForward - startForward));
            time = 0f;
            TimeModule.Instance.SetTimeInterval(LerpMoveCamera, 0.01f);
            //GameSceneCamera.transform.forward = tempForward;
        }

        private void LerpMoveCamera()
        {
            time += 0.01f;
            Vector3 tempForward = endForward - GameSceneCamera.transform.forward;//Vector3.Normalize(GameSceneCamera.transform.forward);

            if (IsCameraBoundMoving(tempForward))
            {
                //Debug.Log("breake   " + time);
                TimeModule.Instance.RemoveTimeaction(LerpMoveCamera);
                time = 0f;
                this.m_canSlide = true;
                return;
            }
            if (time >= 1f)
            {
                TimeModule.Instance.RemoveTimeaction(LerpMoveCamera);
                GameSceneCamera.transform.forward = endForward;
                time = 0f;
                this.m_canSlide = true;
                //Debug.Log("stop   " + time);
                return;
            }
            GameSceneCamera.transform.forward = Lerp(startForward, endForward, Mathf.Clamp01(time));
            //Debug.Log("forward : " + GameSceneCamera.transform.forward + " VLerp:" + Vector3.Lerp(startForward, endForward, Mathf.Clamp01(time)));
            //Debug.Log("startForward : " + startForward + "   endForward:" + endForward + "  Self:" + GameSceneCamera.transform.forward);
        }

        private Vector3 Lerp(Vector3 s, Vector3 e, float t)
        {
            float x = s.x * (1 - t) + e.x * t;
            float y = s.y * (1 - t) + e.y * t;
            float z = s.z * (1 - t) + e.z * t;
            return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
        }

        private bool IsCameraBoundMoving(Vector3 tempVector)
        {
            int tempNum = 0;
            if ((IsSceneRightBounds(tempVector) || IsSceneLeftBounds(tempVector)))
            {
                endForward = new Vector3(GameSceneCamera.transform.forward.x, endForward.y, GameSceneCamera.transform.forward.z);
                startForward = new Vector3(GameSceneCamera.transform.forward.x, startForward.y, GameSceneCamera.transform.forward.z);

                tempNum++;
            }
            if (IsSceneUpBounds(tempVector) || IsSceneDownBounds(tempVector))
            {
                endForward = new Vector3(endForward.x, GameSceneCamera.transform.forward.y, GameSceneCamera.transform.forward.z); //endForward
                startForward = new Vector3(startForward.x, GameSceneCamera.transform.forward.y, GameSceneCamera.transform.forward.z); //startForward
                tempNum++;
            }
            return tempNum == 2;
        }

        private bool IsSceneRightBounds(Vector3 tempVector)
        {
            return tempVector.x > 0 && CameraUtility.IsBoundsInCameraView(sceneCameraParams.SceneRightBounds);
        }

        private bool IsSceneLeftBounds(Vector3 tempVector)
        {
            return tempVector.x < 0 && CameraUtility.IsBoundsInCameraView(sceneCameraParams.SceneLeftBounds);
        }

        private bool IsSceneUpBounds(Vector3 tempVector)
        {
            return tempVector.y > 0 && CameraUtility.IsBoundsInCameraView(sceneCameraParams.SceneUpBounds);
        }

        private bool IsSceneDownBounds(Vector3 tempVector)
        {
            return tempVector.y < 0 && CameraUtility.IsBoundsInCameraView(sceneCameraParams.SceneDownBounds);
        }

        private void OnRotateTweenerTweening()
        {
            m_currentDir = m_cacheRotate - GameSceneCamera.transform.localEulerAngles; //当前方向
            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
            EngineCoreEvents.InputEvent.OnTouchScene.SafeInvoke();
            //m_currentDir = m_moveRotate;
            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref m_currentDir);

            if (m_currentDir != Vector3.zero && cameraDirection_0 == CameraDirection.None && cameraDirection_1 == CameraDirection.None)
            {
                if (m_needKickBack)
                {
                    this.m_isKickBack = false;
                    this.m_needKickBack = false;
                    EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
                }
            }
            else
            {
                if (!m_needKickBack && !m_isKickBack && !CameraManager.Instance.IsAutoPlay)
                {
                    m_currentDir = Vector3.Normalize(m_currentDir);
                    this.rotateTweener.Kill();
                    if (m_currentDir != Vector3.zero)
                    {
                        Vector3 targetRotate = GameSceneCamera.transform.localEulerAngles + m_currentDir * CameraManager.Instance.DragDelta;
                        rotateTweener = GameSceneCamera.transform.DOLocalRotate(targetRotate, 0.2f).SetEase(Ease.OutSine).OnUpdate(CameraMoveUpdate);
                    }
                }
                return;
            }
        }

        private void CameraMoveUpdate()
        {
            CameraDirection cameraDirection_0 = CameraDirection.None;
            CameraDirection cameraDirection_1 = CameraDirection.None;
            IsCameraOverStep(ref cameraDirection_0, ref cameraDirection_1, ref m_currentDir);
            if (cameraDirection_0 != CameraDirection.None || cameraDirection_1 != CameraDirection.None)
            {
                this.rotateTweener.Kill();
            }
        }

        private void OnPinIn(Gesture gesture)
        {
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
                rotateTweener.Kill();
                this.m_isKickBack = true;
                rotateTweener = GameSceneCamera.transform.DOLocalRotate(m_srcRotate, 0.2f).OnComplete(() =>
                {
                    m_needKickBack = false;
                    this.m_isKickBack = false;
                });
                EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
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
                Quaternion q = Quaternion.Slerp(GameSceneCamera.transform.rotation, sceneCameraParams.CameraOriginRotation, rotateLerpFactor);

                rotateTweener = GameSceneCamera.transform.DORotateQuaternion(q, 0.2f).OnUpdate(() =>
                {
                    if (GameSceneCamera.transform.position.z <= sceneCameraParams.ZFar)
                    {
                        rotateTweener.Kill();
                    }
                });
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
                Debug.Log("CameraPreZoomKickBack=====");
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
            TimeModule.Instance.RemoveTimeaction(LerpMoveCamera);
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

            Bounds leftBounds = sceneCameraParams.SceneLeftBounds;
            Vector3 leftDir = leftBounds.max - GameSceneCamera.transform.position;
            float leftAngle = Vector3.Angle(Vector3.Normalize(GameSceneCamera.transform.forward), Vector3.Normalize(leftDir)) / 0.08f;
            Vector3 rightDir = sceneCameraParams.SceneRightBounds.max - GameSceneCamera.transform.position;
            float rightAngle = Vector3.Angle(Vector3.Normalize(GameSceneCamera.transform.forward), Vector3.Normalize(rightDir)) / 0.08f;

            //Debug.Log("zoomFactor : " + zoomFactor + "  leftAngle : " + leftAngle + "  rightAngle : " + rightAngle);
            return new Vector3(zoomFactor, leftAngle, rightAngle);
        }

        public void IsCameraOverStep(ref CameraDirection direction_0, ref CameraDirection direction_1, ref Vector3 rotateValue)
        {
            //x > 0  up  y<0 right 
            direction_0 = CameraDirection.None;
            direction_1 = CameraDirection.None;
            targetDummy_0 = GetRotateX(rotateValue.x);
            targetDummy_1 = GetRotateY(rotateValue.y);
            if (CameraUtility.IsBoundsInCameraView(targetDummy_1))
            {
                direction_0 = rotateValue.y > 0 ? CameraDirection.Right : CameraDirection.Left;
                rotateValue.y = 0;
            }

            if (CameraUtility.IsBoundsInCameraView(targetDummy_0))
            {
                direction_1 = rotateValue.x > 0 ? CameraDirection.Down : CameraDirection.Up;
                rotateValue.x = 0;
            }
        }

        private void OnClearCameraStatus()
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

}