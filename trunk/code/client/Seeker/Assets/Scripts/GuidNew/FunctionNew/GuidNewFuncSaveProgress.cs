
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 保存当前进度
    /// </summary>
    public class GuidNewFuncSaveProgress : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            this.m_guidBase.OnSaveCurrentProgress();
            OnDestory();
        }

    }
}
