/********************************************************************
	created:  2014-7-23
	filename: AbstractModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  Module抽象类
*********************************************************************/

namespace EngineCore
{
    public abstract class AbstractModule : IGameModule
    {
        protected ModuleType m_moduleType;
        protected bool m_isStarted = false;

        public virtual void Start()
        {
            this.m_isStarted = true;
        }


        public virtual void Update()
        {
        }



        public virtual void Dispose() { }
        public virtual bool Enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                    setEnable(value);
            }
        }

        public byte ModuleType
        {
            get
            {
                return (byte)m_moduleType;
            }
        }

        public virtual bool AutoStart
        {
            get;
            protected set;
        }

        private bool _enable = true;
        protected virtual void setEnable(bool value)
        {
            _enable = value;
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void OnApplicationPause()
        {

        }

        public virtual void OnApplicationResume()
        {

        }

        public bool IsModuleStarted
        {
            get { return this.m_isStarted; }
        }
    }
}
