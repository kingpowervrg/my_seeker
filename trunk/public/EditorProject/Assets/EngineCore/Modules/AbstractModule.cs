/********************************************************************
	created:  2014-7-23
	filename: AbstractModule.cs
	author:	  songguangze@outlook.com
	
	purpose:  Module抽象类
*********************************************************************/
using System;

namespace EngineCore
{
    public abstract class AbstractModule : IGameModule
    {
        protected ModuleType m_moduleType;

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Dispose() { }
        public virtual bool Enable
        {
            get { return _enable; }
            protected set
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
    }
}
