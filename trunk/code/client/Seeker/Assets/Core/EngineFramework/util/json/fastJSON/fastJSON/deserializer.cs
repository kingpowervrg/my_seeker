namespace fastJSON
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    internal class deserializer
    {
        private Dictionary<object, int> _circobj = new Dictionary<object, int>();
        private bool _circular;
        private Dictionary<int, object> _cirrev = new Dictionary<int, object>();
        private JSONParameters _params;
        private bool _usingglobals;

        public deserializer(JSONParameters param)
        {
            this._params = param;
        }

        private object ChangeType(object value, Type conversionType)
        {
            if (conversionType == typeof(int))
            {
                return (int) ((long) value);
            }
            if (conversionType == typeof(long))
            {
                return (long) value;
            }
            if (conversionType == typeof(string))
            {
                return (string) value;
            }
            if (conversionType == typeof(Guid))
            {
                return this.CreateGuid((string) value);
            }
            if (conversionType.IsEnum)
            {
                return this.CreateEnum(conversionType, value);
            }
            if (conversionType == typeof(UnityEngine.Color))
            {
                Dictionary<string,object> dic = value as Dictionary<string,object>;
                string[] vals = ((string)dic["value"]).Split(',');
                return new UnityEngine.Color(float.Parse(vals[0]), float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3]));
            }
            if (Reflection.Instance.IsTypeRegistered(conversionType))
            {
                return Reflection.Instance.CreateCustom((string) value, conversionType);
            }
            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        private object CreateArray(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            Array array = Array.CreateInstance(bt, data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                object obj2 = data[i];
                if (obj2 is IDictionary)
                {
                    array.SetValue(this.ParseDictionary((Dictionary<string, object>) obj2, globalTypes, bt, null), i);
                }
                else
                {
                    array.SetValue(this.ChangeType(obj2, bt), i);
                }
            }
            return array;
        }


        private DateTime CreateDateTime(string value)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            bool flag = false;
            CreateInteger(out num, value, 0, 4);
            CreateInteger(out num2, value, 5, 2);
            CreateInteger(out num3, value, 8, 2);
            CreateInteger(out num4, value, 11, 2);
            CreateInteger(out num5, value, 14, 2);
            CreateInteger(out num6, value, 0x11, 2);
            if (value[value.Length - 1] == 'Z')
            {
                flag = true;
            }
            if (!this._params.UseUTCDateTime && !flag)
            {
                return new DateTime(num, num2, num3, num4, num5, num6);
            }
            DateTime time = new DateTime(num, num2, num3, num4, num5, num6, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        private object CreateDictionary(List<object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            IDictionary dictionary = (IDictionary) Reflection.Instance.FastCreateInstance(pt);
            Type type = null;
            Type type2 = null;
            if (types != null)
            {
                type = types[0];
                type2 = types[1];
            }
            foreach (Dictionary<string, object> dictionary2 in reader)
            {
                object obj2 = dictionary2["k"];
                object obj3 = dictionary2["v"];
                if (obj2 is Dictionary<string, object>)
                {
                    obj2 = this.ParseDictionary((Dictionary<string, object>) obj2, globalTypes, type, null);
                }
                else
                {
                    obj2 = this.ChangeType(obj2, type);
                }
                if (obj3 is Dictionary<string, object>)
                {
                    obj3 = this.ParseDictionary((Dictionary<string, object>) obj3, globalTypes, type2, null);
                }
                else
                {
                    obj3 = this.ChangeType(obj3, type2);
                }
                dictionary.Add(obj2, obj3);
            }
            return dictionary;
        }

        private object CreateEnum(Type pt, object v)
        {
            return Enum.Parse(pt, v.ToString());
        }

        private object CreateGenericList(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            IList list = (IList) Reflection.Instance.FastCreateInstance(pt);
            foreach (object obj2 in data)
            {
                if (obj2 is IDictionary)
                {
                    list.Add(this.ParseDictionary((Dictionary<string, object>) obj2, globalTypes, bt, null));
                }
                else if (obj2 is List<object>)
                {
                    list.Add(((List<object>) obj2).ToArray());
                }
                else
                {
                    list.Add(this.ChangeType(obj2, bt));
                }
            }
            return list;
        }

        private Guid CreateGuid(string s)
        {
            if (s.Length > 30)
            {
                return new Guid(s);
            }
            return new Guid(Convert.FromBase64String(s));
        }

        private static int CreateInteger(out int num, string s, int index, int count)
        {
            num = 0;
            bool flag = false;
            int num2 = 0;
            while (num2 < count)
            {
                char ch = s[index];
                switch (ch)
                {
                    case '-':
                        flag = true;
                        break;

                    case '+':
                        flag = false;
                        break;

                    default:
                        num *= 10;
                        num += ch - '0';
                        break;
                }
                num2++;
                index++;
            }
            if (flag)
            {
                num = -num;
            }
            return num;
        }

        private NameValueCollection CreateNV(Dictionary<string, object> d)
        {
            NameValueCollection values = new NameValueCollection();
            foreach (KeyValuePair<string, object> pair in d)
            {
                values.Add(pair.Key, (string) pair.Value);
            }
            return values;
        }

        private StringDictionary CreateSD(Dictionary<string, object> d)
        {
            StringDictionary dictionary = new StringDictionary();
            foreach (KeyValuePair<string, object> pair in d)
            {
                dictionary.Add(pair.Key, (string) pair.Value);
            }
            return dictionary;
        }

        private object CreateStringKeyDictionary(Dictionary<string, object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            IDictionary dictionary = (IDictionary) Reflection.Instance.FastCreateInstance(pt);
            Type bt = null;
            Type type = null;
            if (types != null)
            {
                bt = types[0];
                type = types[1];
            }
            foreach (KeyValuePair<string, object> pair in reader)
            {
                string key = pair.Key;
                object obj2 = null;
                if (pair.Value is Dictionary<string, object>)
                {
                    obj2 = this.ParseDictionary((Dictionary<string, object>) pair.Value, globalTypes, type, null);
                }
                else if ((types != null) && type.IsArray)
                {
                    obj2 = this.CreateArray((List<object>) pair.Value, type, type.GetElementType(), globalTypes);
                }
                else if (pair.Value is IList)
                {
                    obj2 = this.CreateGenericList((List<object>) pair.Value, type, bt, globalTypes);
                }
                else
                {
                    obj2 = this.ChangeType(pair.Value, type);
                }
                dictionary.Add(key, obj2);
            }
            return dictionary;
        }

        internal object ParseDictionary(Dictionary<string, object> d, Dictionary<string, object> globaltypes, Type type, object input)
        {
            object obj2 = "";
            if (type == typeof(NameValueCollection))
            {
                return this.CreateNV(d);
            }
            if (type == typeof(StringDictionary))
            {
                return this.CreateSD(d);
            }
            if (!this._circular)
            {
                this._circular = d.TryGetValue("$circular", out obj2);
            }
            if (d.TryGetValue("$i", out obj2))
            {
                object obj3 = null;
                this._cirrev.TryGetValue((int) ((long) obj2), out obj3);
                return obj3;
            }
            if (d.TryGetValue("$types", out obj2))
            {
                this._usingglobals = true;
                globaltypes = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> pair in (Dictionary<string, object>) obj2)
                {
                    globaltypes.Add((string) pair.Value, pair.Key);
                }
            }
            bool flag = d.TryGetValue("$type", out obj2);
            if (!flag && (type == typeof(object)))
            {
                return d;
            }
            if (flag)
            {
                if (this._usingglobals)
                {
                    object obj4 = "";
                    if ((globaltypes != null) && globaltypes.TryGetValue((string) obj2, out obj4))
                    {
                        obj2 = obj4;
                    }
                }
                type = Reflection.Instance.GetTypeFromCache((string) obj2);
            }
            if (type == null)
            {
                throw new Exception("Cannot determine type:" + obj2);
            }
            string fullName = type.FullName;
            object key = input;
            if (key == null)
            {
                if (this._params.ParametricConstructorOverride)
                {
                    key = FormatterServices.GetUninitializedObject(type);
                }
                else
                {
                    key = Reflection.Instance.FastCreateInstance(type);
                }
            }
            if (this._circular)
            {
                int num = 0;
                if (!this._circobj.TryGetValue(key, out num))
                {
                    num = this._circobj.Count + 1;
                    this._circobj.Add(key, num);
                    this._cirrev.Add(num, key);
                }
            }
            Dictionary<string, myPropInfo> props = Reflection.Instance.Getproperties(type, fullName, this._params.IgnoreCaseOnDeserialize, Reflection.Instance.IsTypeRegistered(type));
            foreach (string str2 in d.Keys)
            {
                myPropInfo info;
                string str3 = str2;
                if (this._params.IgnoreCaseOnDeserialize)
                {
                    str3 = str3.ToLower();
                }
                if (str3 == "$map")
                {
                    this.ProcessMap(key, props, (Dictionary<string, object>) d[str3]);
                    continue;
                }
                if (props.TryGetValue(str3, out info) && ((info.Flags & (myPropInfoFlags.CanWrite | myPropInfoFlags.Filled)) != 0))
                {
                    object v = d[str3];
                    if (v != null)
                    {
                        object obj7 = null;
                        switch (info.Type)
                        {
                            case myPropInfoType.Int:
                                obj7 = (int) ((long) v);
                                break;

                            case myPropInfoType.Long:
                                obj7 = (long) v;
                                break;

                            case myPropInfoType.String:
                                obj7 = (string) v;
                                break;

                            case myPropInfoType.Bool:
                                obj7 = (bool) v;
                                break;

                            case myPropInfoType.DateTime:
                                obj7 = this.CreateDateTime((string) v);
                                break;

                            case myPropInfoType.Enum:
                                obj7 = this.CreateEnum(info.pt, v);
                                break;

                            case myPropInfoType.Guid:
                                obj7 = this.CreateGuid((string) v);
                                break;

                            case myPropInfoType.Array:
                                if (!info.IsValueType)
                                {
                                    obj7 = this.CreateArray((List<object>) v, info.pt, info.bt, globaltypes);
                                }
                                break;

                            case myPropInfoType.ByteArray:
                                obj7 = Convert.FromBase64String((string) v);
                                break;

                            case myPropInfoType.Dictionary:
                            case myPropInfoType.Hashtable:
                                obj7 = this.CreateDictionary((List<object>) v, info.pt, info.GenericTypes, globaltypes);
                                break;

                            case myPropInfoType.StringKeyDictionary:
                                obj7 = this.CreateStringKeyDictionary((Dictionary<string, object>) v, info.pt, info.GenericTypes, globaltypes);
                                break;

                            case myPropInfoType.NameValue:
                                obj7 = this.CreateNV((Dictionary<string, object>) v);
                                break;

                            case myPropInfoType.StringDictionary:
                                obj7 = this.CreateSD((Dictionary<string, object>) v);
                                break;

                            case myPropInfoType.Custom:
                                obj7 = Reflection.Instance.CreateCustom((string) v, info.pt);
                                break;

                            default:
                                if ((info.IsGenericType && !info.IsValueType) && (v is List<object>))
                                {
                                    obj7 = this.CreateGenericList((List<object>) v, info.pt, info.bt, globaltypes);
                                }
                                else if (info.IsClass && (v is Dictionary<string, object>))
                                {
                                    obj7 = this.ParseDictionary((Dictionary<string, object>)v, globaltypes, info.pt, null/*info.getter(key)*/);
                                }
                                else if (v is List<object>)
                                {
                                    obj7 = this.CreateArray((List<object>) v, info.pt, typeof(object), globaltypes);
                                }
                                else if (info.IsValueType)
                                {
                                    obj7 = this.ChangeType(v, info.changeType);
                                }
                                else
                                {
                                    obj7 = v;
                                }
                                break;
                        }
                        key = info.setValue(key, obj7);
                    }
                }
            }
            return key;
        }

        private void ProcessMap(object obj, Dictionary<string, myPropInfo> props, Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> pair in dic)
            {
                myPropInfo info = props[pair.Key];
                object obj2 = info.getValue(obj);
                if (Type.GetType((string) pair.Value) == typeof(Guid))
                {
                    info.setValue(obj, this.CreateGuid((string) obj2));
                }
            }
        }

        private object RootDictionary(object parse, Type type)
        {
            Type[] genericArguments = Reflection.Instance.GetGenericArguments(type);
            Type conversionType = null;
            Type type3 = null;
            if (genericArguments != null)
            {
                conversionType = genericArguments[0];
                type3 = genericArguments[1];
            }
            if (parse is Dictionary<string, object>)
            {
                IDictionary dictionary = (IDictionary) Reflection.Instance.FastCreateInstance(type);
                foreach (KeyValuePair<string, object> pair in (Dictionary<string, object>) parse)
                {
                    object obj2;
                    object key = this.ChangeType(pair.Key, conversionType);
                    if (pair.Value is Dictionary<string, object>)
                    {
                        obj2 = this.ParseDictionary(pair.Value as Dictionary<string, object>, null, type3, null);
                    }
                    else if (type3.IsArray)
                    {
                        obj2 = this.CreateArray((List<object>) pair.Value, type3, type3.GetElementType(), null);
                    }
                    else if (pair.Value is IList)
                    {
                        obj2 = this.CreateGenericList((List<object>) pair.Value, type3, conversionType, null);
                    }
                    else
                    {
                        obj2 = this.ChangeType(pair.Value, type3);
                    }
                    dictionary.Add(key, obj2);
                }
                return dictionary;
            }
            if (parse is List<object>)
            {
                return this.CreateDictionary(parse as List<object>, type, genericArguments, null);
            }
            return null;
        }

        private object RootHashTable(List<object> o)
        {
            Hashtable hashtable = new Hashtable();
            foreach (Dictionary<string, object> dictionary in o)
            {
                object key = dictionary["k"];
                object obj3 = dictionary["v"];
                if (key is Dictionary<string, object>)
                {
                    key = this.ParseDictionary((Dictionary<string, object>) key, null, typeof(object), null);
                }
                if (obj3 is Dictionary<string, object>)
                {
                    obj3 = this.ParseDictionary((Dictionary<string, object>) obj3, null, typeof(object), null);
                }
                hashtable.Add(key, obj3);
            }
            return hashtable;
        }

        private object RootList(object parse, Type type)
        {
            Type[] genericArguments = Reflection.Instance.GetGenericArguments(type);
            IList list = (IList) Reflection.Instance.FastCreateInstance(type);
            foreach (object obj2 in (IList) parse)
            {
                this._usingglobals = false;
                object obj3 = obj2;
                if (obj2 is Dictionary<string, object>)
                {
                    obj3 = this.ParseDictionary(obj2 as Dictionary<string, object>, null, genericArguments[0], null);
                }
                else
                {
                    obj3 = this.ChangeType(obj2, genericArguments[0]);
                }
                list.Add(obj3);
            }
            return list;
        }

        public T ToObject<T>(string json)            
        {
            Type type = typeof(T);
            object obj2 = this.ToObject(json, type);
            if (type.IsArray && ((obj2 as ICollection).Count == 0))
            {
                return (T)(object)Array.CreateInstance(type.GetElementType(), 0);
            }
            return (T) obj2;
        }

        public object ToObject(string json)
        {
            return this.ToObject(json, null);
        }

        public object ToObject(string json, Type type)
        {
            this._params.FixValues();
            Type genericTypeDefinition = null;
            if ((type != null) && type.IsGenericType)
            {
                genericTypeDefinition = Reflection.Instance.GetGenericTypeDefinition(type);
            }
            if ((genericTypeDefinition == typeof(Dictionary<,>)) || (genericTypeDefinition == typeof(List<>)))
            {
                this._params.UsingGlobalTypes = false;
            }
            this._usingglobals = this._params.UsingGlobalTypes;
            object parse = new JsonParser(json, this._params.IgnoreCaseOnDeserialize).Decode();
            if (parse == null)
            {
                return null;
            }
            if (parse is IDictionary)
            {
                if ((type != null) && (genericTypeDefinition == typeof(Dictionary<,>)))
                {
                    return this.RootDictionary(parse, type);
                }
                return this.ParseDictionary(parse as Dictionary<string, object>, null, type, null);
            }
            if (parse is List<object>)
            {
                if ((type != null) && (genericTypeDefinition == typeof(Dictionary<,>)))
                {
                    return this.RootDictionary(parse, type);
                }
                if ((type != null) && (genericTypeDefinition == typeof(List<>)))
                {
                    return this.RootList(parse, type);
                }
                if (type == typeof(Hashtable))
                {
                    return this.RootHashTable((List<object>) parse);
                }
                return (parse as List<object>).ToArray();
            }
            if ((type != null) && (parse.GetType() != type))
            {
                return this.ChangeType(parse, type);
            }
            return parse;
        }
    }
}

