using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncResourceLoadRes : GuidNewFunctionBase
    {
        private string resName;
        private bool isLoad;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.resName = param[0];
            this.isLoad = bool.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (isLoad)
            {
                Object obj = Resources.Load(this.resName);
                GuidNewNodeManager.Instance.SetCommonObj(this.resName, obj);
            }
            else
            {
                Object obj = GuidNewNodeManager.Instance.GetCommonObj(this.resName) as Object;
                Resources.UnloadAsset(obj);
                GuidNewNodeManager.Instance.RemoveCommonObj(this.resName);
            }
            OnDestory();
        }
    }
}
