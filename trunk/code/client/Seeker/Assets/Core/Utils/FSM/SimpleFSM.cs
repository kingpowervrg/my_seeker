/********************************************************************
	created:2018-3-26
	filename: SimpleFSM.cs
	author:	songguangze@outlook.com
	
	purpose:	状态机基类
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class SimpleFSM
    {
        //状态列表
        private Dictionary<int, SimpleFSMStateBase> _states = new Dictionary<int, SimpleFSMStateBase>();

        //当前状态
        protected SimpleFSMStateBase _currentState;


        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateFlag"></param>
        /// <param name="state"></param>
        /// <param name="userData"></param>
        public void AddState(int stateFlag, SimpleFSMStateBase state, object userData)
        {
            if (state != null)
            {
                state.StateFlag = stateFlag;
                state.Data = userData;
                _states.Add(stateFlag, state);
            }
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateFlag"></param>
        /// <param name="state"></param>
        public void AddState(int stateFlag, SimpleFSMStateBase state)
        {
            AddState(stateFlag, state, null);
        }


        /// <summary>
        /// 状态切换
        /// </summary>
        /// <param name="stateFlag"></param>
        /// <returns></returns>
        public virtual bool GotoState(int stateFlag)
        {
            //结束当前状态
            if (_currentState != null)
            {
                _currentState.EndState(stateFlag);
                _currentState = null;
            }

            if (_states.ContainsKey(stateFlag))
            {
                _currentState = _states[stateFlag];
                _currentState.Params = null;
                _currentState.BeginState(stateFlag);
                return true;
            }
            return false;
        }

        public virtual void GotoState(int stateFlag, object param)
        {

            if (_currentState != null)
            {
                _currentState.EndState(stateFlag);
                _currentState = null;
            }

            if (_states.ContainsKey(stateFlag))
            {
                _currentState = _states[stateFlag];
                _currentState.Params = param;
                _currentState.BeginState(stateFlag);

            }
        }

        /// <summary>
        /// 状态机Tick
        /// </summary>
        /// <param name="elapsed"></param>
        public void Tick(float elapsed)
        {
            if (_currentState != null)
                _currentState.Tick(elapsed);
        }

        public SimpleFSMStateBase CurrentState
        {
            get { return _currentState; }
        }
    }
}
