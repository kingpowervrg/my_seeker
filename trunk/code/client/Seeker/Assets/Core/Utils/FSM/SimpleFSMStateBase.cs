/********************************************************************
	created:	2018-3-26 
	filename: SimpleFSMStateBase.cs
	author:	songguangze@outlook.com
	
	purpose:	状态机状态基类
*********************************************************************/
namespace EngineCore
{
    public class SimpleFSMStateBase
    {
        //状态标识
        private int _stateFlag;

        //数据
        private object _data;

        //状态参数
        private object _params;

        /// <summary>
        /// 状态开始
        /// </summary>
        /// <param name="stateFlag"></param>
        public virtual void BeginState(int stateFlag) { }

        /// <summary>
        /// 状态Tick
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Tick(float elapsed) { }

        /// <summary>
        /// 状态结束
        /// </summary>
        /// <param name="nNextState">下一个状态</param>
        public virtual void EndState(int nextState) { }

        /// <summary>
        /// 状态挂起
        /// </summary>
        public virtual void Suspend() { }

        /// <summary>
        /// 状态恢复
        /// </summary>
        public virtual void Resume() { }

        public int StateFlag
        {
            get { return _stateFlag; }
            set { _stateFlag = value; }
        }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public object Params
        {
            get { return _params; }
            set { _params = value; }
        }
    }
}
