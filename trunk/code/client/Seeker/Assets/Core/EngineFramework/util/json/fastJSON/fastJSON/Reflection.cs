namespace fastJSON
{
    using GOEngine.Implement;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;

    internal sealed class Reflection
    {
        private SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
        internal SafeDictionary<Type, Deserialize> _customDeserializer = new SafeDictionary<Type, Deserialize>();
        internal SafeDictionary<Type, Serialize> _customSerializer = new SafeDictionary<Type, Serialize>();
        private SafeDictionary<Type, Type> _genericTypeDef = new SafeDictionary<Type, Type>();
        private SafeDictionary<Type, Type[]> _genericTypes = new SafeDictionary<Type, Type[]>();
        private SafeDictionary<Type, PropertyInfo[]> _getterscache = new SafeDictionary<Type, PropertyInfo[]>();
        private SafeDictionary<string, Dictionary<string, myPropInfo>> _propertycache = new SafeDictionary<string, Dictionary<string, myPropInfo>>();
        private SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
        private SafeDictionary<int, Dictionary<string, object>> parseCache = new SafeDictionary<int, Dictionary<string, object>>();
        private static readonly Reflection instance = new Reflection();

        private Reflection()
        {

        }

        internal object CreateCustom(string v, Type type)
        {
            Deserialize deserialize;
            this._customDeserializer.TryGetValue(type, out deserialize);
            return deserialize(v);
        }

        private myPropInfo CreateMyProp(Type t, string name, bool customType)
        {
            myPropInfo info = new myPropInfo();
            myPropInfoType unknown = myPropInfoType.Unknown;
            myPropInfoFlags flags = myPropInfoFlags.CanWrite | myPropInfoFlags.Filled;
            if ((t == typeof(int)) || (t == typeof(int?)))
            {
                unknown = myPropInfoType.Int;
            }
            else if ((t == typeof(long)) || (t == typeof(long?)))
            {
                unknown = myPropInfoType.Long;
            }
            else if (t == typeof(string))
            {
                unknown = myPropInfoType.String;
            }
            else if ((t == typeof(bool)) || (t == typeof(bool?)))
            {
                unknown = myPropInfoType.Bool;
            }
            else if ((t == typeof(DateTime)) || (t == typeof(DateTime?)))
            {
                unknown = myPropInfoType.DateTime;
            }
            else if (t.IsEnum)
            {
                unknown = myPropInfoType.Enum;
            }
            else if ((t == typeof(Guid)) || (t == typeof(Guid?)))
            {
                unknown = myPropInfoType.Guid;
            }
            else if (t == typeof(StringDictionary))
            {
                unknown = myPropInfoType.StringDictionary;
            }
            else if (t == typeof(NameValueCollection))
            {
                unknown = myPropInfoType.NameValue;
            }
            else if (t.IsArray)
            {
                info.bt = t.GetElementType();
                if (t == typeof(byte[]))
                {
                    unknown = myPropInfoType.ByteArray;
                }
                else
                {
                    unknown = myPropInfoType.Array;
                }
            }
            else if (t.Name.Contains("Dictionary"))
            {
                info.GenericTypes = Instance.GetGenericArguments(t);
                if ((info.GenericTypes.Length > 0) && (info.GenericTypes[0] == typeof(string)))
                {
                    unknown = myPropInfoType.StringKeyDictionary;
                }
                else
                {
                    unknown = myPropInfoType.Dictionary;
                }
            }
            else if (t == typeof(Hashtable))
            {
                unknown = myPropInfoType.Hashtable;
            }
            else if (customType)
            {
                unknown = myPropInfoType.Custom;
            }
            info.IsClass = t.IsClass;
            info.IsValueType = t.IsValueType;
            if (t.IsGenericType)
            {
                info.IsGenericType = true;
                info.bt = t.GetGenericArguments()[0];
            }
            info.pt = t;
            info.Name = name;
            info.changeType = this.GetChangeType(t);
            info.Type = unknown;
            info.Flags = flags;
            return info;
        }

        private Dictionary<Type, ConstructorInfo> tcdic = new Dictionary<Type, ConstructorInfo>();
        internal object FastCreateInstance(Type objtype)
        {
            ConstructorInfo construct;
            if (!tcdic.TryGetValue(objtype, out construct))
            {
                construct = objtype.GetConstructor(new Type[0]);
                tcdic.Add(objtype, construct);
            }
            return construct.Invoke(null);
        }

        private Type GetChangeType(Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                return Instance.GetGenericArguments(conversionType)[0];
            }
            return conversionType;
        }

        public Type[] GetGenericArguments(Type t)
        {
            Type[] genericArguments = null;
            if (!this._genericTypes.TryGetValue(t, out genericArguments))
            {
                genericArguments = t.GetGenericArguments();
                this._genericTypes.Add(t, genericArguments);
            }
            return genericArguments;
        }

        public Type GetGenericTypeDefinition(Type t)
        {
            Type genericTypeDefinition = null;
            if (!this._genericTypeDef.TryGetValue(t, out genericTypeDefinition))
            {
                genericTypeDefinition = t.GetGenericTypeDefinition();
                this._genericTypeDef.Add(t, genericTypeDefinition);
            }
            return genericTypeDefinition;
        }

        internal PropertyInfo[] GetGetters(Type type, JSONParameters param)
        {
            PropertyInfo[] gettersArray = null;
            if (!this._getterscache.TryGetValue(type, out gettersArray))
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> list = new List<PropertyInfo>();
                foreach (PropertyInfo info in properties)
                {
                    if (!info.CanWrite && !param.ShowReadOnlyProperties)
                    {
                        continue;
                    }
                    JsonFieldTypes fieldP = JsonFieldAttribute.GetFieldFlag(info);
                    if (param.IgnoreAttributes != null && fieldP != JsonFieldTypes.Null)
                    {
                        bool flag = false;

                        foreach (JsonFieldTypes type2 in param.IgnoreAttributes)
                        {
                            if (fieldP == type2)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            continue;
                        }
                    }
                    list.Add(info);
                }
                gettersArray = list.ToArray();
                this._getterscache.Add(type, gettersArray);
            }
            return gettersArray;
        }

        public Dictionary<string, myPropInfo> Getproperties(Type type, string typename, bool IgnoreCaseOnDeserialize, bool customType, bool withField = false)
        {
            Dictionary<string, myPropInfo> dictionary = null;
            if (!this._propertycache.TryGetValue(typename, out dictionary))
            {
                dictionary = new Dictionary<string, myPropInfo>();
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    myPropInfo info2 = this.CreateMyProp(info.PropertyType, info.Name, customType);
                    info2.Flags |= myPropInfoFlags.CanWrite;
                    info2.propertyInfo = info;
                    if (IgnoreCaseOnDeserialize)
                    {
                        dictionary.Add(info.Name.ToLower(), info2);
                    }
                    else
                    {
                        dictionary.Add(info.Name, info2);
                    }
                }
                if (withField)
                {
                    foreach (FieldInfo info3 in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        myPropInfo info4 = this.CreateMyProp(info3.FieldType, info3.Name, customType);
                        info4.fieldInfo = info3;
                        if (IgnoreCaseOnDeserialize)
                        {
                            dictionary.Add(info3.Name.ToLower(), info4);
                        }
                        else
                        {
                            dictionary.Add(info3.Name, info4);
                        }
                    }
                }
                this._propertycache.Add(typename, dictionary);
            }
            return dictionary;
        }

        public Dictionary<string, object> GetJsonDictionaryFromCache(string json)
        {
            int hashCode = json.GetHashCode();
            Dictionary<string, object> res;
            if (parseCache.TryGetValue(hashCode, out res))
                return res;
            else
            {
                res = new JsonParser(json, JSON.Parameters.IgnoreCaseOnDeserialize).Decode() as Dictionary<string, object>;
                parseCache[hashCode] = res;
                return res;
            }
        }

        public void ClearParseCache()
        {
            parseCache.Clear();
        }

        internal string GetTypeAssemblyName(Type t)
        {
            string str = "";
            if (this._tyname.TryGetValue(t, out str))
            {
                return str;
            }
            string assemblyQualifiedName = t.AssemblyQualifiedName;
            this._tyname.Add(t, assemblyQualifiedName);
            return assemblyQualifiedName;
        }

        public void AddMapping(string name, Type type)
        {
            _typecache[name] = type;
        }
        internal Type GetTypeFromCache(string typename)
        {
            Type type = null;
            if (this._typecache.TryGetValue(typename, out type))
            {
                return type;
            }
            else
            {
                string tempname = typename.Substring(0, typename.IndexOf(","));
                if (this._typecache.TryGetValue(tempname, out type))
                    return type;
            }
            Type type2 = Type.GetType(typename);
            if (type2 == null)
            {
                string tempname = typename.Substring(0, typename.IndexOf(","));
                Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblys)
                {
                    type2 = assembly.GetType(tempname);
                    if (type2 != null)
                        break;
                }
                type2 = Type.GetType(tempname);
            }
            this._typecache.Add(typename, type2);
            return type2;
        }

        internal bool IsTypeRegistered(Type t)
        {
            Serialize serialize;
            if (this._customSerializer.Count == 0)
            {
                return false;
            }
            return this._customSerializer.TryGetValue(t, out serialize);
        }

        internal void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
            if (((type != null) && (serializer != null)) && (deserializer != null))
            {
                this._customSerializer.Add(type, serializer);
                this._customDeserializer.Add(type, deserializer);
                Instance.ResetPropertyCache();
            }
        }

        internal void ResetPropertyCache()
        {
            this._propertycache = new SafeDictionary<string, Dictionary<string, myPropInfo>>();
        }

        public static Reflection Instance
        {
            get
            {
                return instance;
            }
        }

        private delegate object CreateObject();

        internal delegate object GenericGetter(object obj);

        internal delegate object GenericSetter(object target, object value);
    }
}

