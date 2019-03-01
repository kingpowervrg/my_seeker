using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 加载矩形遮罩
    /// </summary>
    public class GuidNewFuncLoadRectMaskByFrame : GuidNewFunctionBase
    {
        private int m_type = 0;
        private string frameName;
        private string itemName;
        private string eventName;
        private float lessPixel = 0f;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length > 0)
            {
                this.m_type = int.Parse(param[0]);
            }
            if (param.Length >= 4)
            {
                this.frameName = param[1];
                this.itemName = param[2];
                this.eventName = param[3];
            }
            if (param.Length >= 5)
            {
                this.lessPixel = float.Parse(param[4]);
            }
        }

        private Vector2 leftBottomPos;
        private Vector2 rightTopPos;

        private Vector2 minPos = Vector2.zero;
        private Vector2 maxPos = Vector2.zero;
        public override void OnExecute()
        {
            base.OnExecute();
            if (m_type == 0)
            {
                GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName);
                Transform tran = frame.FrameRootTransform.Find(itemName);
                RectTransform srcRect = tran.GetComponent<RectTransform>();
                Vector2[] cornPos = GuidTools.getCornPos(srcRect, this.lessPixel);

                GUIFrame frameGuid = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
                Transform maskRoot = frameGuid.FrameRootTransform.Find("guid/mask");

                leftBottomPos = maskRoot.InverseTransformPoint(cornPos[0]);
                rightTopPos = maskRoot.InverseTransformPoint(cornPos[2]);
            }
            else
            {
                Vector4 rectInfo = GameEvents.UI_Guid_Event.OnGetCurrentMaskInfo(1);
                leftBottomPos = Vector2.left * rectInfo.x + Vector2.up * rectInfo.y;
                rightTopPos = Vector2.left * rectInfo.z + Vector2.up * rectInfo.w;
            }
            Vector2 centerPos = new Vector2((leftBottomPos.x + rightTopPos.x) / 2f, (leftBottomPos.y + rightTopPos.y) / 2f);
            minPos.x = centerPos.x - Screen.width;
            minPos.y = centerPos.y - Screen.height;

            maxPos.x = centerPos.x + Screen.width;
            maxPos.y = centerPos.y + Screen.height;

            GuidNewModule.Instance.PushFunction(this);
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GuidNewModule.Instance.RemoveFunction(this);
            this.m_timesection = 0f;
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            ClearFunc();
        }

        private float m_timesection = 0f;
        private float m_costTime = 0.5f;
        private Vector4 m_tempVec = Vector4.zero;
        public override void Tick(float time)
        {
            base.Tick(time);
            this.m_timesection += time;
            Vector2 minVec = Vector2.zero;
            Vector2 maxVec = Vector2.zero;
            if (0 == this.m_type)
            {
                minVec = Vector2.Lerp(this.maxPos, this.rightTopPos, Mathf.Clamp01(this.m_timesection / this.m_costTime));
                maxVec = Vector2.Lerp(this.minPos, this.leftBottomPos, Mathf.Clamp01(this.m_timesection / this.m_costTime));
            }
            else
            {
                minVec = Vector2.Lerp(this.rightTopPos, this.maxPos, Mathf.Clamp01(this.m_timesection / this.m_costTime));
                maxVec = Vector2.Lerp(this.leftBottomPos, this.minPos, Mathf.Clamp01(this.m_timesection / this.m_costTime));
            }
            m_tempVec.x = minVec.x;
            m_tempVec.y = minVec.y;
            m_tempVec.z = maxVec.x;
            m_tempVec.w = maxVec.y;
            GameEvents.UI_Guid_Event.OnReflashCircleMask.SafeInvoke(this.m_tempVec, 1);
            if (m_timesection >= m_costTime)
            {
                GuidNewModule.Instance.RemoveFunction(this);
                this.m_timesection = 0f;
                this.m_tempVec = Vector4.zero;
                OnDestory();
            }
        }
    }
}
