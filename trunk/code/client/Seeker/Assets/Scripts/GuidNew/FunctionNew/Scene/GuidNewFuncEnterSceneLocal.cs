using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 直接从本地进入场景
    /// 需要提前设置好物件数据
    /// </summary>
    public class GuidNewFuncEnterSceneLocal : GuidNewFunctionBase
    {
        private int m_sceneId;
        private bool m_needPolice;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                m_sceneId = int.Parse(param[0]);
            }
            if (param.Length >= 2)
            {
                m_needPolice = bool.Parse(param[1]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            string exhibitSceneIDStr = GuidNewNodeManager.Instance.GetCommonParams(GuidNewNodeManager.sceneExhibit);
            string[] exhibitSplit = exhibitSceneIDStr.Split('|');
            List<long> exhibitSceneIdArray = new List<long>();
            for (int i = 0; i < exhibitSplit.Length; i++)
            {
                exhibitSceneIdArray.Add(long.Parse(exhibitSplit[i]));
            }
            SceneModule.Instance.OnEnterLocalSceneByID(this.m_sceneId, new List<long>(), exhibitSceneIdArray, this.m_needPolice);
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }
    }
}
