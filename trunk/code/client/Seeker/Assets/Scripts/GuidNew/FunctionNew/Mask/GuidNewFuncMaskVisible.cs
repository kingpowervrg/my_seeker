using EngineCore;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 控制遮罩显示隐藏
    /// </summary>
    public class GuidNewFuncMaskVisible : GuidNewFunctionBase
    {
        private int m_type = 0; //0 表示遮罩
        private bool m_visible;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_type = int.Parse(param[0]);
            this.m_visible = bool.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (this.m_type == 0)
            {
                GameEvents.UI_Guid_Event.OnMaskEnableClick.SafeInvoke(this.m_visible);
            }
            OnDestory();
        }
    }
}
