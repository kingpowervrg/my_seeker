using EngineCore;
using UnityEngine;


namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 清空单机数据,进入到登陆界面
    /// </summary>
    public class GuidNewFuncChangeGameStateToLogin : GuidNewFunctionBase
    {
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            ClearPlayerInfo();
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }

        private void ClearPlayerInfo()
        {
            GlobalInfo.MY_PLAYER_INFO.ClearBagInfo();
            GlobalInfo.MY_PLAYER_INFO.ClearSkyEye();
#if OFFICER_SYS
            GlobalInfo.MY_PLAYER_INFO.ClearOfficerInfo();
#endif
            GlobalInfo.MY_PLAYER_INFO.ClearPlayerChapterSystem();
            GlobalInfo.MY_PLAYER_INFO.ClearPlayerTaskSystem();
            PlayerInfoManager.Instance.ClearPlayerInfo();
            m_guidBase.ForceFuncFinish();
            GameRoot.instance.GameFSM.GotoStateWithParam((int)ClientFSM.ClientState.LOGIN, ENUM_ACCOUNT_TYPE.E_GUID);
        }
    }
}
