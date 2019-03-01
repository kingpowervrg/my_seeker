namespace fastJSON
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class JSON
    {
        public static JSONParameters Parameters = new JSONParameters();

        public static string Beautify(string input)
        {
            return Formatter.PrettyPrint(input);
        }

        internal static long CreateLong(out long num, string s, int index, int count)
        {
            num = 0L;
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
                        num *= 10L;
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

        public static T DeepCopy<T>(T obj)
        {
            return new deserializer(Parameters).ToObject<T>(ToJSON(obj));
        }

        public static object DeepCopy(object obj)
        {
            return new deserializer(Parameters).ToObject(ToJSON(obj));
        }

        public static object FillObject(object input, string json)
        {
            Dictionary<string, object> d = Reflection.Instance.GetJsonDictionaryFromCache(json);
            if (d == null)
            {
                return null;
            }
            return new deserializer(Parameters).ParseDictionary(d, null, input.GetType(), input);
        }

        public static object Parse(string json)
        {
            return new JsonParser(json, Parameters.IgnoreCaseOnDeserialize).Decode();
        }

        public static void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
            Reflection.Instance.RegisterCustomType(type, serializer, deserializer);
        }

        public static string ToJSON(object obj)
        {
            return ToJSON(obj, Parameters);
        }

        public static string ToJSON(object obj, JSONParameters param)
        {
            param.FixValues();
            Type genericTypeDefinition = null;
            if (obj == null)
            {
                return "null";
            }
            if (obj.GetType().IsGenericType)
            {
                genericTypeDefinition = Reflection.Instance.GetGenericTypeDefinition(obj.GetType());
            }
            if ((genericTypeDefinition == typeof(Dictionary<,>)) || (genericTypeDefinition == typeof(List<>)))
            {
                param.UsingGlobalTypes = false;
            }
            if (param.EnableAnonymousTypes)
            {
                param.UseExtensions = false;
                param.UsingGlobalTypes = false;
            }
            return new JSONSerializer(param).ConvertToJSON(obj);
        }

        public static string ToNiceJSON(object obj, JSONParameters param)
        {
            return Beautify(ToJSON(obj, param));
        }

        public static object ToObject(string json)
        {
            return new deserializer(Parameters).ToObject(json, null);
        }

        public static T ToObject<T>(string json)
        {
            return new deserializer(Parameters).ToObject<T>(json);
        }

        public static T ToObject<T>(string json, JSONParameters param)
        {
            return new deserializer(param).ToObject<T>(json);
        }

        public static object ToObject(string json, JSONParameters param)
        {
            return new deserializer(param).ToObject(json, null);
        }

        public static object ToObject(string json, Type type)
        {
            return new deserializer(Parameters).ToObject(json, type);
        }
    }
}

