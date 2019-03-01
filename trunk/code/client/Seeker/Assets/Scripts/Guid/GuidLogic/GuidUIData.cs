using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using HedgehogTeam.EasyTouch;

namespace SeekerGame
{
    public class GuidUIData
    {
        private bool m_hasEvent = false;
        private GuidEnum m_operaType = GuidEnum.Guid_Click;
        private float m_TypeValue;
        private EventPassType m_eventPassType;

        public EventPassType eventpassType
        {
            get {
                return m_eventPassType;
            }
        }

        public GuidUIData(bool hasEvent, GuidEnum operaType, float typeValue,EventPassType eventPassType)
        {
            this.m_hasEvent = hasEvent;
            this.m_operaType = operaType;
            this.m_TypeValue = typeValue;
            this.m_eventPassType = eventPassType;
        }

        List<GuidMaskData> m_rectData = new List<GuidMaskData>(); //矩形数据
        List<GuidCircleData> m_CircleData = new List<GuidCircleData>(); //圆形数据

        public bool HasEvent => m_hasEvent;
        public GuidEnum OperaType => m_operaType;
        public float TypeValue => m_TypeValue;
        public List<GuidMaskData> RectData => m_rectData;
        public List<GuidCircleData> CircleData => m_CircleData;

        public void Clear()
        {
            RectData.Clear();
            CircleData.Clear();
        }

        public void AddRectData(GuidMaskData data)
        {
            RectData.Add(data);
        }

        public void AddCircleData(GuidCircleData data)
        {
            CircleData.Add(data);
        }

        public bool CheckBtnLegal()
        {
            if (m_operaType != GuidEnum.Guid_Click)
            {
                return false;
            }

            if (eventpassType == EventPassType.None)
            {
                GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
                return false;
            }
            else if(eventpassType == EventPassType.Scene_Event)
            {
                Gesture gesture = new Gesture();
                GameObject obj = gesture.GetCurrentPickedObject();
                if (obj != null)
                {
                    gesture.pickedObject = obj;
                }
                EngineCoreEvents.InputEvent.OnOneFingerTouchup.SafeInvoke(gesture);
                GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
                return false;
            }
            //if (!m_hasEvent)
            //{
            //    GameEvents.UI_Guid_Event.OnNextGuid.SafeInvoke();
            //    return false;
            //}
            return true;
        }

        public string CheckLegalPoint(Vector2 worldPos)
        {
            for (int i = 0; i < m_rectData.Count; i++)
            {
                GuidMaskData rectData = m_rectData[i];
                if (GuidTools.isInnerRect(rectData, worldPos))
                {
                    //MaskClick(rectData.btnName);
                    return rectData.btnName;
                }
            }

            for (int i = 0; i < m_CircleData.Count; i++)
            {
                GuidCircleData circle = m_CircleData[i];
                if (GuidTools.isInnerCircle(circle, worldPos))
                {
                    //MaskClick(circle.btnName);
                    return circle.btnName;
                }
            }
            return null;
        }
    }

    public enum GuidMaskType
    {
        Circle = 0,
        Rect
    }

    public class GuidMaskCommonData
    {
        public List<GuidMaskData> m_maskdata = new List<GuidMaskData>();
        public List<Vector2> m_artPos = new List<Vector2>(); //美术位置
        //public List<string> m_artPath = new List<string>(); //美术路径
        public ConfGuid m_confGuid = null;
        //public bool canSkip;
        public bool hasEvent; //是否有点击事件
        public float m_TypeValue;
        public EventPassType eventPassType;
        public GuidEnum m_operaType = GuidEnum.Guid_Click;
    }

    public class GuidMaskData
    {
        public Vector2 leftBottom;
        public Vector2 leftTop;
        public Vector2 rightTop;
        public Vector2 rightBottom;

        public string btnName;
        public GuidMaskType maskType;

        public GuidMaskData()
        {
            maskType = GuidMaskType.Rect;
        }
    }

    //圆形
    public class GuidCircleData
    {
        public Vector2 centerPos;
        public float radius;
        public string btnName;
    }

    public class GuidMaskBoardData
    {
        public Vector4[] circleCenter = new Vector4[4];
        public float[] circleRadius = new float[4];

        public Vector4[] rectCenter = new Vector4[4];
        public float[] rectWidth = new float[4];
        public float[] rectHeigh = new float[4];

        public GuidMaskBoardData(Vector4[] circleCenter, float[] circleRadius, Vector4[] rectCenter, float[] rectWidth, float[] rectHeigh)
        {
            this.circleCenter = circleCenter;
            this.circleRadius = circleRadius;
            this.rectCenter = rectCenter;
            this.rectWidth = rectWidth;
            this.rectHeigh = rectHeigh;
        }
    }
}
