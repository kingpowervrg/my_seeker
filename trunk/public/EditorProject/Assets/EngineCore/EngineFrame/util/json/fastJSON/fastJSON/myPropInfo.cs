namespace fastJSON
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct myPropInfo
    {
        public System.Type pt;
        public System.Type bt;
        public System.Type changeType;
        public System.Reflection.FieldInfo fieldInfo;
        public System.Reflection.PropertyInfo propertyInfo;
        public Reflection.GenericGetter getter;
        public System.Type[] GenericTypes;
        public string Name;
        public myPropInfoType Type;
        public myPropInfoFlags Flags;
        public bool IsClass;
        public bool IsValueType;
        public bool IsGenericType;
        public object getValue(object obj)
        {
            if (fieldInfo != null)
                return fieldInfo.GetValue(obj);
            if (propertyInfo != null)
                return propertyInfo.GetValue(obj, null);
            return null;
        }

        public object setValue(object obj, object value)
        {
            if (fieldInfo != null)
                fieldInfo.SetValue(obj, value);
            if (propertyInfo != null)
                propertyInfo.SetValue(obj, value, null);
            return obj;
        }
    }
}

