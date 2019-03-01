/********************************************************************
	created:  2018-3-27 11:27:29
	filename: ClientFSM.cs
	author:	  songguangze@outlook.com
	
	purpose:  客户端主状态机
*********************************************************************/
using EngineCore;

namespace SeekerGame
{
    public class ClientFSM : SimpleFSM
    {
        public void InitClientFSM()
        {
            AddState((int)ClientState.INIT, new GameInitState());
            AddState((int)ClientState.NORMAL, new GameState());
            AddState((int)ClientState.LOGIN, new GameLoginState());
            AddState((int)ClientState.PREGAME, new PreGameState());
            AddState((int)ClientState.PROLOGUE,new GamePrologueState());
        }

        public override bool GotoState(int stateFlag)
        {
            if (_currentState != null && _currentState.StateFlag.Equals(stateFlag))
            {
                return false;
            }

            return base.GotoState(stateFlag);
        }

        public bool GotoStateWithParam(int stateFlag, object param)
        {
            if (_currentState != null && _currentState.StateFlag.Equals(stateFlag))
            {
                return false;
            }

            base.GotoState(stateFlag, param);

            return true;
        }
        /// <summary>
        /// 游戏状态
        /// </summary>
        public enum ClientState
        {
            INIT = 0,   //游戏初始化
            PREGAME,    //预先游戏(更新,动态下载)
            PREVIEW,
            PRELOAD,
            LOGIN,      //登录状态
            SWITCHACCOUNT,
            CREATEROLE,
            NORMAL,     //正常游戏状态
            DISCONNECT,
            PROLOGUE, //序章
        }
    }
}