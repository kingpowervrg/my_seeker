using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

namespace SeekerGame
{
    public class IAPProduct
    {
        public long m_charge_id;
        public string m_unique_platform_id;
        public ProductType m_type;

        public IDs m_cross_platform_ids;
    }
}
