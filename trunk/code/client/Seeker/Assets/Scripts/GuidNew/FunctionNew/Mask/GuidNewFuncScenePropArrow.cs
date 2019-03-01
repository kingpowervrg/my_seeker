using EngineCore;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 道具提示箭头
    /// </summary>
    public class GuidNewFuncScenePropArrow : GuidNewFunctionBase
    {
        private string resName;
        private string iconName;
        private float m_delayTime;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.resName = param[0];
            this.iconName = param[1];
            this.iconName = this.iconName.Replace(':', '/');
            this.m_delayTime = float.Parse(param[2]);
        }

        private Transform arrowTran = null;

        private Transform m_arrowParent = null;
        private TweenScale tweener = null;
        public override void OnExecute()
        {
            base.OnExecute();
            if (this.m_delayTime > 0)
            {
                TimeModule.Instance.SetTimeout(TimeDelay, this.m_delayTime);
            }
            else
            {
                TimeDelay();
            }
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            arrowTran.gameObject.SetActive(false);
            arrowTran.SetParent(m_arrowParent);
            tweener.From = arrowTran.localPosition;
            tweener.To = arrowTran.localPosition - Vector3.up * 10f;
            //tweener.enabled = false;
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            OnDestory();
            if (this.m_delayTime > 0)
            {
                TimeModule.Instance.RemoveTimeaction(TimeDelay);
            }
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            arrowTran.gameObject.SetActive(false);
            arrowTran.SetParent(m_arrowParent);
            tweener.From = arrowTran.localPosition;
            tweener.To = arrowTran.localPosition - Vector3.up * 10f;
            //tweener.enabled = false;
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

        private void OnMaskClick(Vector2 pos, bool inner)
        {
            if (inner)
            {
                OnDestory();
            }
        }

        private void TimeDelay()
        {
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.resName);
            if (frame == null)
            {
                OnDestory();
                return;
            }
            arrowTran = frame.FrameRootTransform.Find(iconName);
            this.m_arrowParent = arrowTran.parent;
            GUIFrame guidFrame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            Transform guidTrans = guidFrame.FrameRootTransform.Find("guid/art");
            tweener = arrowTran.GetComponent<TweenScale>();
            //tweener.enabled = false;
            arrowTran.SetParent(guidTrans);
            tweener.From = arrowTran.localPosition;
            tweener.To = arrowTran.localPosition - Vector3.up * 10f;
            arrowTran.gameObject.SetActive(true);
            tweener.ResetAndPlay();
            GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
        }
    }
}
