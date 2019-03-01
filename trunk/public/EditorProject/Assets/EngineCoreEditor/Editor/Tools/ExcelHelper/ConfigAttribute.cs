using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace conf
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public class ConfigAttribute : Attribute
	{
        private string _fieldFlag;
        public ConfigAttribute(string parm)
        {
            this._fieldFlag = parm;
        }

        public static string GetFieldFlag(System.Reflection.PropertyInfo pro)
        {
            object[] atts = pro.GetCustomAttributes(typeof(ConfigAttribute), false);
            if (atts.Length > 0)
            {
                ConfigAttribute attr = atts[0] as ConfigAttribute;
                return attr._fieldFlag;
            }
            return null;
        }
	}
}
