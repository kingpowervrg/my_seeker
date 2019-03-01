using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekerGame
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UBSBridgeAttribute : Attribute
    {
        private readonly BridgeType m_bridgeType;

        public UBSBridgeAttribute(BridgeType moduleType)
        {
            this.m_bridgeType = moduleType;
        }

        public BridgeType Type
        {
            get { return this.m_bridgeType; }
        }

    }
}
