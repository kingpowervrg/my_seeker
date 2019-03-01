using UnityEngine;
using HedgehogTeam.EasyTouch;
using DG.Tweening;
using System;
using EngineCore;
using EngineCore.Utility;

namespace SeekerGame
{
    public class BigWorldCamera : MonoBehaviour
    {
        private Camera GameSceneCamera = null;
        private bool m_inZooming = false;
        private BigWorldCameraParam sceneCameraParams = null;
        private Transform m_trans = null;
        private void OnEnable()
        {
            if (GameSceneCamera == null)
            {
                GameSceneCamera = Camera.main;
            }
            if (sceneCameraParams == null)
            {
                sceneCameraParams = gameObject.GetComponent<BigWorldCameraParam>();
            }
            if (m_trans == null)
            {
                m_trans = transform.Find("GameObject");
            }
            EngineCoreEvents.InputEvent.OnSwipe += OnSwipte;
            EngineCoreEvents.InputEvent.OnPinIn += OnPinIn;
            EngineCoreEvents.InputEvent.OnPinOut += OnPinOut;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnOneFingerTouchup;
            //GameEvents.BigWorld_Event.OnCameraCollider += OnCameraCollider;
            currentPos = transform.position;
        }

        private void OnSwipte(Gesture gesture)
        {
            if (m_inZooming)
                return;
            Vector2 swipeDelta = gesture.swipeVector;
            Vector2 moveDelta = Vector2.zero;
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                moveDelta.x = -swipeDelta.x * 0.005f;
            else
                moveDelta.y = -swipeDelta.y * 0.008f;

            Vector3 tempPos = transform.position + Vector3.right * moveDelta.x + Vector3.forward * moveDelta.y;
            m_trans.position = tempPos;
            //Debug.Log("clear target");
            m_targetPos = Vector3.zero;
            if (sceneCameraParams.IsOutBounds(m_trans) >= 0)
            {
                m_trans.localPosition = Vector3.zero;
                return;
            }

            currentPos = tempPos;
        }

        private void OnPinIn(Gesture gesture)
        {
            m_inZooming = true;
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
            Vector3 tempPos = transform.position - gesture.deltaPinch * transform.forward * 0.1f;
            m_trans.position = tempPos;
            int boundIndex = sceneCameraParams.IsOutBounds(m_trans);
            if (boundIndex >= 0)
            {
                Vector3 wallPoint = Vector3.zero;
                Vector3 wallNormal = Vector3.zero;
                if (boundIndex == 3)
                {
                    wallPoint = sceneCameraParams.m_zuoPoint;
                    wallNormal = sceneCameraParams.m_zuoNormal;
                }
                else if (boundIndex == 2)
                {
                    wallPoint = sceneCameraParams.m_youPoint;
                    wallNormal = sceneCameraParams.m_youNormal;
                }
                if (boundIndex == 3 || boundIndex == 2)
                {
                    Vector3 crossPoint = GetIntersectWithLineAndPlane(tempPos, tempPos - transform.position, wallNormal, wallPoint);

                    float distance = Vector3.Distance(crossPoint,tempPos);
                    //currentPos = crossPoint;

                    m_trans.localPosition = Vector3.zero;
                    if (boundIndex == 3)
                    {
                        m_trans.localPosition += Vector3.right * 2f;
                    }
                    else if (boundIndex == 2)
                    {
                        m_trans.localPosition -= Vector3.right * 2f;
                    }

                    Vector3 nexCrossPoint = GetIntersectWithLineAndPlane(m_trans.position, transform.forward, wallNormal, wallPoint);
                    m_targetPos = crossPoint + Vector3.Normalize(nexCrossPoint - crossPoint) * distance ;
                    currentPos = m_targetPos;
                   
                }
                
                m_trans.localPosition = Vector3.zero;
                return;
            }
            //Debug.Log("clear target");
            m_targetPos = Vector3.zero;
            currentPos = tempPos;
        }

        private void OnPinOut(Gesture gesture)
        {
            m_inZooming = true;
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
            EngineCoreEvents.InputEvent.OnTouchup += OnTouchup;
            Vector3 tempPos = transform.position + gesture.deltaPinch * transform.forward * 0.1f;
            m_trans.position = tempPos;
            //Debug.Log("clear target");
            m_targetPos = Vector3.zero;
            if (sceneCameraParams.IsOutBounds(m_trans) >= 0)
            {
                m_trans.localPosition = Vector3.zero;
                return;
            }
            currentPos = tempPos;
        }

        private void OnTouchup(Gesture gesture)
        {
            m_inZooming = false;
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchup;
        }
        
        private void OnOneFingerTouchup(Gesture gesture)
        {
            GameEvents.BigWorld_Event.OnClickScreen.SafeInvoke();
            //ClickBuild();
        }

        Vector3 currentPos = Vector3.zero;
        Vector3 m_targetPos = Vector3.zero;

        void LateUpdate()
        {
            if (transform.position != currentPos)
            {
                transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
                GameEvents.BigWorld_Event.OnReflashScreen.SafeInvoke();
            }
            //else if (m_targetPos != Vector3.zero)
            //{
            //    currentPos = m_targetPos;
            //    m_targetPos = Vector3.zero;
            //    Debug.Log("reset targetPos=== " + m_targetPos);
            //}

        }

        private void OnDisable()
        {
            EngineCoreEvents.InputEvent.OnSwipe -= OnSwipte;
            EngineCoreEvents.InputEvent.OnPinIn -= OnPinIn;
            EngineCoreEvents.InputEvent.OnPinOut -= OnPinOut;
            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnOneFingerTouchup;
            //GameEvents.BigWorld_Event.OnCameraCollider -= OnCameraCollider;
        }

        private void ClickBuild()
        {
            Ray ray = GameSceneCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit))
            {
                Transform parent = rayhit.collider.gameObject.transform.parent;
                if (parent != null)
                {
                    GameEvents.BigWorld_Event.OnClick.SafeInvoke(parent.name);
                }
            }
        }

        public Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            //print(d);
            return d * direct.normalized + point;
        }

        //public Vector3 GetPointForPanel(Vector3)
        //{
        //}
    }
}
