using UnityEngine;
using UnityEngine.EventSystems;
using GOGUI;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewMaskSystem
    {

        private Vector2 m_LeftBottom, m_RightTop;
        private List<Vector2[]> m_RectWorldPos = new List<Vector2[]>();
        private List<Vector3> m_CircleWorldPos = new List<Vector3>();

        //private List<Vector4> m_circleCenter = new List<Vector4>();
        private List<Vector4> m_rectInfo = new List<Vector4>();

        //private List<float> m_Radius = new List<float>(); // 缓存
        private Material m_maskMat = null;
        private Transform m_maskRoot;
        private GameImage m_maskImg;

        private MaskAnimType m_animType = MaskAnimType.ToOut;
        private string eventName = string.Empty;
        private EventTriggerListener m_eventListener = null;

        #region 1123新增修改
        int m_shaderCircleID = Shader.PropertyToID("_circleInfo");
        int m_shaderRectID_0 = Shader.PropertyToID("_rectInfo0");
        int m_shaderRectID_1 = Shader.PropertyToID("_rectInfo1");
        private bool m_maskCanHide = true;
        #endregion

        public GuidNewMaskSystem(Material mat, GameImage maskRoot, EventTriggerListener listener)
        {
            this.m_LeftBottom = new Vector2(-Screen.width / 2f, -Screen.height / 2f);
            this.m_RightTop = new Vector2(Screen.width / 2f, Screen.height / 2f);
            this.m_maskMat = mat;
            this.m_maskImg = maskRoot;
            this.m_maskRoot = maskRoot.gameObject.transform;
            this.m_eventListener = listener;
            GameEvents.UI_Guid_Event.OnClearGuid += OnClearGuid;
            RegisterEvent();
            //Shader.PropertyToID()
            // GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
        }

        private void OnClearGuid()
        {
            ClearMask();
        }

        public void OnDestory()
        {
            GameEvents.UI_Guid_Event.OnClearGuid -= OnClearGuid;
            UnRegisterEvent();
            // GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

        public void ClearMask()
        {
            m_RectWorldPos.Clear();
            m_CircleWorldPos.Clear();
            //m_circleCenter = new List<Vector4> { Vector4.zero};
            m_rectInfo = new List<Vector4> { Vector4.zero};
            //m_Radius = new List<float> { 0f };
            eventName = string.Empty;
            ClearEffect();
            //this.m_maskMat.SetVectorArray("circleCenter", m_circleCenter.ToArray());
            //this.m_maskMat.SetFloatArray("circleRadius", m_Radius.ToArray());
            ClearRectInfo();
            ClearCircleMask();
            //this.m_maskMat.SetVectorArray("rectInfo", m_rectInfo.ToArray());
            this.m_maskImg.Visible = false;
        }

        private void ClearMaskNotEventName()
        {
            m_RectWorldPos.Clear();
            m_CircleWorldPos.Clear();
            m_rectInfo = new List<Vector4> { Vector4.zero };
            ClearEffect();
            ClearRectInfo();
            ClearCircleMask();
            this.m_maskImg.Visible = false;
        }

        private void ClearEventName()
        {
            eventName = string.Empty;
        }

        public void OnMaskClick(Vector2 worldPos)
        {
            if (!this.m_maskCanHide)
            {
                return;
            }
            if (m_CircleWorldPos.Count + m_RectWorldPos.Count == 1 && !string.IsNullOrEmpty(eventName))
            {
                //点击事件
                if (GuidNewTools.InnerEmpty(m_CircleWorldPos, m_RectWorldPos,worldPos))
                {
                    GuidNewTools.PassEvent(eventName, m_eventListener.PointerEventData, ExecuteEvents.pointerClickHandler, true);
                    ClearMask();
                    GameEvents.UI_Guid_Event.OnMaskClick.SafeInvoke(worldPos, true);
                    return;
                }
            }
            else
            {
                //点击空白消失
                ClearMask();
            }
            GameEvents.UI_Guid_Event.OnMaskClick.SafeInvoke(worldPos, false);
        }

        public void OnMaskPressDown(Vector2 worldPos)
        {
            if (GuidNewTools.InnerEmpty(m_CircleWorldPos, m_RectWorldPos, worldPos))
            {
                GuidNewTools.PassEvent(eventName, m_eventListener.PointerEventData, ExecuteEvents.pointerDownHandler, true,1);
                GameEvents.UI_Guid_Event.OnMaskTalkVisible.SafeInvoke(false);
                ClearMaskNotEventName();
                return;
            }
        }

        public void OnMaskPressUp(Vector2 worldPos)
        {
            GuidNewTools.PassEvent(eventName, m_eventListener.PointerEventData, ExecuteEvents.pointerUpHandler, true, 2);
            ClearEventName();
            GameEvents.UI_Guid_Event.OnMaskClick.SafeInvoke(worldPos, true);
            //if (GuidNewTools.InnerEmpty(m_CircleWorldPos, m_RectWorldPos, worldPos))
            //{

            //    //GameEvents.UI_Guid_Event.OnMaskClick.SafeInvoke(worldPos, true);
            //    //ClearMask();
            //    //GameEvents.UI_Guid_Event.OnMaskClick.SafeInvoke(worldPos, true);
            //    return;
            //}
        }

        public void AddSingleEmpty(Vector2[] pos,MaskEmptyType type,MaskAnimType animType,string eventName)
        {
            this.eventName = eventName;
            this.m_animType = animType;
            if (type == MaskEmptyType.Circle)
            {
                AddCircle(new List<Vector2[]> { pos });
                //AddSingleCircle(pos);
            }
            else
            {
                AddRectEmpty(new List<Vector2[]>() { pos});
            }
        }

        public void AddMutliEmpty(List<Vector2[]> pos,List<MaskEmptyType> type)
        {
            if (pos.Count != type.Count)
            {
                Debug.LogError("mask error");
            }
            this.m_animType = MaskAnimType.ToOut;
            List<Vector2[]> circlePos = new List<Vector2[]>();
            List<Vector2[]> rectPos = new List<Vector2[]>();
            for (int i = 0; i < pos.Count; i++)
            {
                if (type[i] == MaskEmptyType.Circle)
                {
                    circlePos.Add(pos[i]);
                }
                else if (type[i] == MaskEmptyType.Rect)
                {
                    rectPos.Add(pos[i]);
                }
            }
            AddCircle(circlePos);
            AddRectEmpty(rectPos);
        }


        public void AddCircle(List<Vector2[]> pos)
        {
            if (pos.Count == 0)
            {
                return;
            }
            this.m_CircleWorldPos.Clear();
            for (int i = 0; i < pos.Count; i++)
            {
                m_CircleWorldPos.Add(GuidNewTools.GetWorldCirclePos(pos[i]));
            }
            Vector2[] emptyPos = GuidNewTools.GetEmptyPos(pos[0], m_maskRoot);
            float Radius = Mathf.Min(emptyPos[1].x, emptyPos[1].y) / 2f;
            this.m_circleInfo = new Vector4(emptyPos[0].x, emptyPos[0].y, Radius,0f);
            if (m_animType == MaskAnimType.ToInner)
            {
                this.tempRadius = Screen.width / 2f;
                this.maxRadius = Radius;
            }
            else
            {
                this.tempRadius = 0f;
                this.maxRadius = Radius;
            }
            this.startCircleTime = 0f;
            //TimeModule.Instance.SetTimeout(TickCircle,0.03f,true);
            TimeModule.Instance.SetTimeInterval(TickCircle,0.02f);
        }

        private void TickCircle()
        {
            startCircleTime += 0.01f;
            if (startCircleTime >= maxCircleTime)
            {
                LoadCircleEffect(m_circleInfo.x, m_circleInfo.y, m_circleInfo.z,15000);
                this.m_maskMat.SetVector("_circleInfo", m_circleInfo);
                TimeModule.Instance.RemoveTimeaction(TickCircle);
                GameEvents.UI_Guid_Event.OnMaskTickComplete.SafeInvoke();
            }
            else
            {
                m_circleInfo.z = Mathf.Lerp(tempRadius,maxRadius,startCircleTime / maxCircleTime);
                this.m_maskMat.SetVector("_circleInfo", m_circleInfo);
            }
        }

        private void ClearCircleMask()
        {
            this.startCircleTime = 0f;
            this.m_circleInfo = Vector4.zero;
            this.m_maskMat.SetVector(m_shaderCircleID, Vector4.zero);
        }
        Vector4 m_circleInfo = Vector4.zero;
        float tempRadius = 0f;
        float maxRadius = 0f;
        float startCircleTime = 0f;
        float maxCircleTime = 0.2f;

        private float m_maxTime = 0.2f;
        private float lerpTime = 0f;
        private List<Vector2> tempLeftPos = new List<Vector2>();
        private List<Vector2> tempRightPos = new List<Vector2>();
        public void AddRectEmpty(List<Vector2[]> pos)
        {
            if (pos.Count == 0)
            {
                return;
            }
            
            m_rectInfo.Clear();
            m_RectWorldPos.Clear();
            m_RectWorldPos.AddRange(pos);
            lerpTime = 0f;
            tempLeftPos.Clear();
            tempRightPos.Clear();
            for (int i = 0; i < pos.Count; i++)
            {
                Vector2[] emptyPos = GuidNewTools.GetEmptyPos(pos[i], m_maskRoot);
                m_rectInfo.Add(new Vector4(emptyPos[0].x - emptyPos[1].x / 2f, emptyPos[0].y - emptyPos[1].y / 2f, emptyPos[0].x + emptyPos[1].x / 2f, emptyPos[0].y + emptyPos[1].y / 2f));
                if (m_animType == MaskAnimType.ToInner)
                {
                    OnReflashCircleMask(new Vector4(m_LeftBottom.x, m_LeftBottom.y, m_RightTop.x, m_RightTop.y),1);
                    OnReflashCircleMask(Vector4.zero, 0);
                    tempLeftPos.Add(m_LeftBottom);
                    tempRightPos.Add(m_RightTop);
                }
                else
                {
                    tempLeftPos.Add(emptyPos[0]);
                    tempRightPos.Add(emptyPos[0]);
                }
            }
            this.m_maskCanHide = true;
            TimeModule.Instance.SetTimeout(OnRectInnerAnim, 0.02f, true);
            //TimeModule.Instance.SetTimeInterval(OnRectInnerAnim,0.02f);
            //Vector2 centerPos = 
        }

        private void OnRectInnerAnim()
        {
            lerpTime += 0.01f;
            if (lerpTime >= m_maxTime)
            {
                SetRectInfo(m_rectInfo.ToArray());
                //this.m_maskMat.SetVectorArray("rectInfo", m_rectInfo.ToArray());
                for (int i = 0; i < m_rectInfo.Count; i++)
                {
                    LoadRectEffect(m_rectInfo[i],10000 + i);
                }
                TimeModule.Instance.RemoveTimeaction(OnRectInnerAnim);
                GameEvents.UI_Guid_Event.OnMaskTickComplete.SafeInvoke();
                Debug.Log("rect mask complete===============");
                return;
            }
            Vector4[] tempRect = new Vector4[m_rectInfo.Count];
            for (int i = 0; i < m_rectInfo.Count; i++)
            {
                Vector2 leftV = Vector2.Lerp(tempLeftPos[i], Vector2.right * m_rectInfo[i].x + Vector2.up * m_rectInfo[i].y, lerpTime/ m_maxTime);
                Vector2 rightV = Vector2.Lerp(tempRightPos[i], Vector2.right * m_rectInfo[i].z + Vector2.up * m_rectInfo[i].w, lerpTime/ m_maxTime);
                Vector4 tempVec = new Vector4(leftV.x, leftV.y, rightV.x, rightV.y);
                tempRect[i] = tempVec;
            }
            SetRectInfo(tempRect);
            //this.m_maskMat.SetVectorArray("rectInfo", tempRect);
        }

        private void LoadRectEffect(Vector4 m_rectInfo,long id)
        {
            float centerX = (m_rectInfo.x + m_rectInfo.z) / 2f;
            float centerY = (m_rectInfo.y + m_rectInfo.w) / 2f;
            Vector2 center = Vector2.right * centerX + Vector2.up * centerY;
            float width = m_rectInfo.z - m_rectInfo.x;
            float hei = m_rectInfo.w - m_rectInfo.y;
            if (width < 300f && hei < 50f && width / hei > 3f)
            {
                id += 400;
                //float scaleX = width / 128f;
                //float scaleY = hei / 24f;
                //Vector2 scale = Vector2.right * scaleX + Vector2.up * scaleY;
                //GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(id, "UI_xinshouyindao_fang05.prefab", center, scale);
                LoadEffect(id, width / 128f, hei / 24f, center, "UI_xinshouyindao_fang05.prefab");
            }
            else if (width > 250f && hei > 250f)
            {
                id += 300;
                LoadEffect(id, width / 365f, hei / 357f, center, "UI_xinshouyindao_fang04.prefab");
            }
            else if (width / hei >= 8f)
            {
                id += 200;
                LoadEffect(id, width / 791f, hei / 38f, center, "UI_xinshouyindao_fang03.prefab");
            }
            else if (width / hei >= 2f)
            {
                id += 100;
                LoadEffect(id, width / 197f, hei / 96f, center, "UI_xinshouyindao_fang02.prefab");
            }
            else if (hei / width >= 2f)
            {
                id += 450;
                LoadEffect(id, hei / 215f,width / 97f, center, "UI_xinshouyindao_fang06.prefab");
            }
            else
            {
                LoadEffect(id, width / 96f, hei / 95f, center, "UI_xinshouyindao_fang01.prefab");
            }
        }

        private void LoadEffect(long id,float scaleX,float scaleY,Vector2 center,string resName)
        {

            Vector2 scale = Vector2.right * scaleX + Vector2.up * scaleY;
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(id, resName, center, scale,0f);
        }

        private void LoadCircleEffect(float centerPosX,float centerPosY,float radius,long id)
        {
            float scaleRadius = radius / 65f * 2f;
            Vector2 scale = Vector2.right * scaleRadius + Vector2.up * scaleRadius;
            Vector2 centerPos = Vector2.right * centerPosX + Vector2.up * centerPosY;
            GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(id, "UI_xinshouyindao_yuan.prefab", centerPos, scale,0f);
        }

        private void SetRectInfo(Vector4[] rectInfo)
        {
            this.m_maskMat.SetVector("_rectInfo0", rectInfo.Length > 0? rectInfo[0]:Vector4.zero);
            this.m_maskMat.SetVector("_rectInfo1", rectInfo.Length > 1 ? rectInfo[1] : Vector4.zero);
        }

        private void ClearRectInfo()
        {
            this.m_maskMat.SetVector("_rectInfo0", Vector4.zero);
            this.m_maskMat.SetVector("_rectInfo1", Vector4.zero);
        }

        private void ClearEffect()
        {
            GameEvents.UI_Guid_Event.OnClearMaskEffect.SafeInvoke(false);
        }

        #region 1123新增功能

        private void RegisterEvent()
        {
            GameEvents.UI_Guid_Event.OnReflashCircleMask += OnReflashCircleMask;
            GameEvents.UI_Guid_Event.OnGetCurrentMaskInfo = OnGetCurrentMaskInfo;
        }

        private void UnRegisterEvent()
        {
            GameEvents.UI_Guid_Event.OnReflashCircleMask -= OnReflashCircleMask;
        }

        private void OnReflashCircleMask(Vector4 circlePos,int type)
        {
            this.m_maskCanHide = false;
            this.m_maskImg.Visible = true;
            if (type == 0)
            {
                this.m_maskMat.SetVector(m_shaderCircleID, circlePos);
            }
            else
            {
                this.m_maskMat.SetVector(m_shaderRectID_0, circlePos);
            }
        }

        private Vector4 OnGetCurrentMaskInfo(int maskType)
        {
            if (0 == maskType)
            {
                return this.m_maskMat.GetVector(m_shaderCircleID);
            }
            else if (1 == maskType)
            {
                return this.m_maskMat.GetVector(m_shaderRectID_0);
            }
            return this.m_maskMat.GetVector(m_shaderRectID_1);
        }
        #endregion
    }
}
