/********************************************************************
	created:  2018-4-4 11:24:25
	filename: InputModule.cs
	author:	  songguangze@fotoable.com
	
	purpose:  游戏输入Module
    remark:   基于EasyTouch的封装
*********************************************************************/
using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace EngineCore
{
    [EngineCoreModule(EngineCore.ModuleType.INPUT_MODULE)]
    public class InputModule : AbstractModule
    {
        private GameObject m_inputDispatcherGameObject = null;
        private EasyTouch m_easyTouchComponent = null;

        private static InputModule m_instance;

        public InputModule()
        {
            AutoStart = true;
            m_instance = this;
        }

        public override void Start()
        {
            base.Start();

            GameObject gameClientRoot = EngineCoreEvents.BridgeEvent.GetGameRootObject();
            InitInputModule(gameClientRoot);
        }

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        /// <param name="gameRoot"></param>
        private void InitInputModule(GameObject gameRoot)
        {
            m_inputDispatcherGameObject = gameRoot.FindOrAddGameobject("InputDispatcher");
            m_easyTouchComponent = m_inputDispatcherGameObject.GetOrAddComponent<EasyTouch>();

            EasyTouch.instance = m_easyTouchComponent;

            EasyTouch.On_SimpleTap += OnOneFingerTouchUpHandler;
            EasyTouch.On_PinchOut += OnPinOutHandler;
            EasyTouch.On_PinchIn += OnPinInHandler;
            EasyTouch.On_Swipe += OnSwipeHandler;
            EasyTouch.On_SwipeEnd += OnSwipeEndHandler;
            EasyTouch.On_SwipeStart += OnSwipeBeginHandler;
            EasyTouch.On_TouchUp += OnTouchupHandler;
            EasyTouch.On_TouchDown += OnTouchDownHandler;
            Enable = false;
        }

        #region Handle EasyTouch Raise Events
        /// <summary>
        /// 单指抬起
        /// </summary>
        /// <param name="gesture"></param>
        private void OnOneFingerTouchUpHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnOneFingerTouchup.SafeInvoke(gesture);
        }

        /// <summary>
        /// 双指外滑
        /// </summary>
        /// <param name="gesture"></param>
        private void OnPinOutHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnPinOut.SafeInvoke(gesture);
        }

        /// <summary>
        /// 双指内滑
        /// </summary>
        /// <param name="gesture"></param>
        private void OnPinInHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnPinIn.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指抬起
        /// </summary>
        /// <param name="gesture"></param>
        private void OnTouchUpHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnTouchup.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指一直在屏幕上
        /// </summary>
        /// <param name="gesture"></param>
        /// <returns></returns>
        private void OnTouchDownHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnTouchDown.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指滑动
        /// </summary>
        /// <param name="gesture"></param>
        private void OnSwipeHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnSwipe.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指滑动开始
        /// </summary>
        /// <param name="gesture"></param>
        private void OnSwipeBeginHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnSwipeBegin.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指滑动结束
        /// </summary>
        /// <param name="gesture"></param>
        private void OnSwipeEndHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnSwipeEnd.SafeInvoke(gesture);
        }

        /// <summary>
        /// 单指抬起
        /// </summary>
        /// <param name="gesture"></param>
        private void OnTouchupHandler(Gesture gesture)
        {
            EngineCoreEvents.InputEvent.OnTouchup.SafeInvoke(gesture);
        }


        #endregion


        protected override void setEnable(bool value)
        {
            base.setEnable(value);
            if (this.m_inputDispatcherGameObject != null)
            {
                if (value)
                    ResetEasyTouchCameras();

                EasyTouch.SetEnabled(value);
                this.m_inputDispatcherGameObject.SetActive(value);
                this.m_easyTouchComponent.enable = value;
            }
        }

        public override void Dispose()
        {
            EasyTouch.On_SimpleTap -= OnOneFingerTouchUpHandler;
            EasyTouch.On_PinchOut -= OnPinOutHandler;
            EasyTouch.On_PinchIn -= OnPinInHandler;
            EasyTouch.On_Swipe -= OnSwipeHandler;
            EasyTouch.On_SwipeEnd -= OnSwipeEndHandler;
            EasyTouch.On_SwipeStart -= OnSwipeBeginHandler;
            EasyTouch.On_TouchUp -= OnTouchupHandler;

        }

        /// <summary>
        /// 重置EasyTouch的摄像机
        /// </summary>
        private void ResetEasyTouchCameras()
        {
            for (int i = 0; i < EasyTouch.instance.touchCameras.Count; ++i)
            {

                EasyTouch.RemoveCamera(EasyTouch.GetCamera(i));
            }


            EasyTouch.AddCamera(Camera.main, false);
        }

        public override bool Enable
        {
            get
            {
                return base.Enable;
            }

            set
            {
                setEnable(value);
            }
        }



        public static InputModule Instance
        {
            get { return m_instance; }
        }

    }
}