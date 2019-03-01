using EngineCore;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using SeekerGame.Rendering;
namespace SeekerGame
{
    public class SpecialGameCamera : GameCameraNew
    {
        private static float SWPITE_VALID_AREA_RATIO = 0.08f;
        private static float SWIPE_THRESHOLD = 7;

        private static Rect Invalid_SwipteRect;

        public override void OnEnable()
        {

            Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
            Invalid_SwipteRect = new Rect(new Vector2(screenResolution.x * SWPITE_VALID_AREA_RATIO, screenResolution.y * SWPITE_VALID_AREA_RATIO + 40f), screenResolution * (1 - 2 * SWPITE_VALID_AREA_RATIO));
            InitSceneEdge();

            EngineCoreEvents.InputEvent.OnSwipe += OnHandSwipeHandler;
            EngineCoreEvents.InputEvent.OnTouchDown += OnTouchScreenHandler;
            EngineCoreEvents.InputEvent.OnTouchup += OnTouchUpHandler;
            GameEvents.MainGameEvents.OnCameraMove += MoveCamera;
            GameEvents.MainGameEvents.OnClearCameraStatus += OnClearCameraStatus;
           
        }

        private Gesture m_lastValidGesture = null;

        protected override void OnHandSwipeHandler(Gesture gesture)
        {
            if (gesture.swipe != EasyTouch.SwipeDirection.Other)
            {
               
                if (IsSwipeOnValidArea(gesture.position))
                {
                    this.m_lastValidGesture = gesture;
                    this.m_lastValidGesture.swipeVector = -this.m_lastValidGesture.swipeVector * SWIPE_THRESHOLD;
                }
            }
        }

        private void OnTouchScreenHandler(Gesture gesture)
        {
            if (!ScreenDrawer.instance.enabled)
            {
                OnHandSwipeHandler(gesture, 0.2f);
            }
            else if (this.m_lastValidGesture != null && IsSwipeOnValidArea(gesture.position))
                OnHandSwipeHandler(this.m_lastValidGesture, 0.2f);
        }

        private void OnTouchUpHandler(Gesture gesture)
        {
            this.m_lastValidGesture = null;
        }

        protected override void OnDisable()
        {
            EngineCoreEvents.InputEvent.OnSwipe -= OnHandSwipeHandler;
            EngineCoreEvents.InputEvent.OnTouchDown -= OnTouchScreenHandler;
            EngineCoreEvents.InputEvent.OnTouchup -= OnTouchUpHandler;
            GameEvents.MainGameEvents.OnCameraMove -= MoveCamera;
            GameEvents.MainGameEvents.OnClearCameraStatus -= OnClearCameraStatus;
            //GameEvents.MainGameEvents.OnFingerForbidden -= OnFingerForbidden;
        }

        private bool IsSwipeOnValidArea(Vector2 point)
        {
            return point.x > Invalid_SwipteRect.xMax || point.x < Invalid_SwipteRect.xMin || point.y < Invalid_SwipteRect.yMin || point.y > Invalid_SwipteRect.yMax;
        }
    }
}