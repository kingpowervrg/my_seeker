
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncCompleteGuidByGroup : GuidNewFunctionBase
    {
        private int group;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.group = int.Parse(param[0]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GuidNewManager.Instance.SetProgressByIndex(group);
            GuidNewManager.Instance.OnGroupComplete(group);
            OnDestory();
        }

    }
}
