using EngineCore;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 判断UI是否关闭
    /// </summary>
    public class GuidNewFuncUIIsHide : GuidNewFunctionBase
    {
        private string m_resName;
        private float m_delayTime;
        private bool m_needForceHide = false;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_resName = param[0];
            this.m_delayTime = float.Parse(param[1]);
            if (param.Length >= 3)
            {
                this.m_needForceHide = bool.Parse(param[2]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (!this.m_needForceHide)
            {
                GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(this.m_resName);
                if (frame == null)
                {
                    OnDestory();
                    return;
                }
            }
            GameEvents.UI_Guid_Event.OnCloseUI += OnCloseUI;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnCloseUI -= OnCloseUI;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnCloseUI -= OnCloseUI;
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
        }


        private void OnCloseUI(GUIFrame frame)
        {
            if (frame.ResName.Equals(this.m_resName))
            {
                if (this.m_delayTime > 0f)
                {
                    TimeModule.Instance.SetTimeout(TimeDelay, this.m_delayTime);
                }
                else
                {
                    TimeDelay();
                }
            }
        }

        private void TimeDelay()
        {
            OnDestory();
        }
    }
}
