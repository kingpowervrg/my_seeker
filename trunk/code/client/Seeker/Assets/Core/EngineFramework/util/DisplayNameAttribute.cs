using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOEngine.Implement
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayNameAttribute : Attribute
    {
        private string _displayName;
        public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();

        public DisplayNameAttribute()
            : this(string.Empty)
        {
        }

        public DisplayNameAttribute(string displayName)
        {
            this._displayName = displayName;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            DisplayNameAttribute attribute = obj as DisplayNameAttribute;
            return ((attribute != null) && (attribute.DisplayName == this.DisplayName));
        }

        public override int GetHashCode()
        {
            return this.DisplayName.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }

        public virtual string DisplayName
        {
            get
            {
                return this.DisplayNameValue;
            }
        }

        protected string DisplayNameValue
        {
            get
            {
                return this._displayName;
            }
            set
            {
                this._displayName = value;
            }
        }
    }
}
