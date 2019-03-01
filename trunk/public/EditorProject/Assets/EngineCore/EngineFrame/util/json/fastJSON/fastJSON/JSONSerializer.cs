namespace fastJSON
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Reflection;

    internal sealed class JSONSerializer
    {
        private StringBuilder _before = new StringBuilder();
        private bool _circular;
        private Dictionary<object, int> _cirobj = new Dictionary<object, int>();
        private int _current_depth;
        private Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private readonly int _MAX_DEPTH = 20;
        private StringBuilder _output = new StringBuilder();
        private JSONParameters _params;
        private bool _TypesWritten;
        private bool _useEscapedUnicode;

        internal JSONSerializer(JSONParameters param)
        {
            this._params = param;
            this._useEscapedUnicode = this._params.UseEscapedUnicode;
        }

        internal string ConvertToJSON(object obj)
        {
            this.WriteValue(obj);
            if ((this._params.UsingGlobalTypes && (this._globalTypes != null)) && (this._globalTypes.Count > 0))
            {
                StringBuilder builder = this._before;
                if (this._circular)
                {
                    builder.Append("\"$circular\":true,");
                }
                builder.Append("\"$types\":{");
                bool flag = false;
                foreach (KeyValuePair<string, int> pair in this._globalTypes)
                {
                    if (flag)
                    {
                        builder.Append(',');
                    }
                    flag = true;
                    builder.Append('"');
                    builder.Append(pair.Key);
                    builder.Append("\":\"");
                    builder.Append(pair.Value);
                    builder.Append('"');
                }
                builder.Append("},");
                builder.Append(this._output.ToString());
                return builder.ToString();
            }
            return this._output.ToString();
        }

        private void WriteArray(IEnumerable array)
        {
            this._output.Append('[');
            bool flag = false;
            foreach (object obj2 in array)
            {
                if (flag)
                {
                    this._output.Append(',');
                }
                this.WriteValue(obj2);
                flag = true;
            }
            this._output.Append(']');
        }

        private void WriteBytes(byte[] bytes)
        {
            this.WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
        }

        private void WriteCustom(object obj)
        {
            Serialize serialize;
            Reflection.Instance._customSerializer.TryGetValue(obj.GetType(), out serialize);
            this.WriteStringFast(serialize(obj));
        }

        private void WriteDateTime(DateTime dateTime)
        {
            DateTime time = dateTime;
            if (this._params.UseUTCDateTime)
            {
                time = dateTime.ToUniversalTime();
            }
            this._output.Append('"');
            this._output.Append(time.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            this._output.Append('-');
            this._output.Append(time.Month.ToString("00", NumberFormatInfo.InvariantInfo));
            this._output.Append('-');
            this._output.Append(time.Day.ToString("00", NumberFormatInfo.InvariantInfo));
            this._output.Append(' ');
            this._output.Append(time.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
            this._output.Append(':');
            this._output.Append(time.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
            this._output.Append(':');
            this._output.Append(time.Second.ToString("00", NumberFormatInfo.InvariantInfo));
            if (this._params.UseUTCDateTime)
            {
                this._output.Append('Z');
            }
            this._output.Append('"');
        }

        private void WriteDictionary(IDictionary dic)
        {
            this._output.Append('[');
            bool flag = false;
            foreach (DictionaryEntry entry in dic)
            {
                if (flag)
                {
                    this._output.Append(',');
                }
                this._output.Append('{');
                this.WritePair("k", entry.Key);
                this._output.Append(",");
                this.WritePair("v", entry.Value);
                this._output.Append('}');
                flag = true;
            }
            this._output.Append(']');
        }

        private void WriteEnum(Enum e)
        {
            if (this._params.UseValuesOfEnums)
            {
                this.WriteValue(Convert.ToInt32(e));
            }
            else
            {
                this.WriteStringFast(e.ToString());
            }
        }

        private void WriteGuid(Guid g)
        {
            if (!this._params.UseFastGuid)
            {
                this.WriteStringFast(g.ToString());
            }
            else
            {
                this.WriteBytes(g.ToByteArray());
            }
        }

        private void WriteNV(NameValueCollection nameValueCollection)
        {
            this._output.Append('{');
            bool flag = false;
            foreach (string str in nameValueCollection)
            {
                if (flag)
                {
                    this._output.Append(',');
                }
                this.WritePair(str, nameValueCollection[str]);
                flag = true;
            }
            this._output.Append('}');
        }

        private void WriteObject(object obj)
        {
            int num = 0;
            if (!this._cirobj.TryGetValue(obj, out num))
            {
                this._cirobj.Add(obj, this._cirobj.Count + 1);
            }
            else if (this._current_depth > 0)
            {
                this._circular = true;
                this._output.Append("{\"$i\":" + num + "}");
                return;
            }
            if (!this._params.UsingGlobalTypes)
            {
                this._output.Append('{');
            }
            else if (!this._TypesWritten)
            {
                this._output.Append('{');
                this._before = this._output;
                this._output = new StringBuilder();
            }
            else
            {
                this._output.Append('{');
            }
            this._TypesWritten = true;
            this._current_depth++;
            if (this._current_depth > this._MAX_DEPTH)
            {
                throw new Exception("Serializer encountered maximum depth of " + this._MAX_DEPTH);
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Type t = obj.GetType();
            bool flag = false;
            if (this._params.UseExtensions)
            {
                if (!this._params.UsingGlobalTypes)
                {
                    this.WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                }
                else
                {
                    int num2 = 0;
                    string typeAssemblyName = Reflection.Instance.GetTypeAssemblyName(t);
                    if (!this._globalTypes.TryGetValue(typeAssemblyName, out num2))
                    {
                        num2 = this._globalTypes.Count + 1;
                        this._globalTypes.Add(typeAssemblyName, num2);
                    }
                    this.WritePairFast("$type", num2.ToString());
                }
                flag = true;
            }
            if (t == typeof(UnityEngine.Color))
            {
                UnityEngine.Color color = (UnityEngine.Color)obj;
                if (flag)
                {
                    this._output.Append(',');
                }
                this.WritePair("value", string.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a));                        
            }
            else
            {
                PropertyInfo[] gettersArray = Reflection.Instance.GetGetters(t, this._params);
                int length = gettersArray.Length;
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo getters = gettersArray[i];
                    ParameterInfo[] pis = getters.GetIndexParameters();
                    if (pis != null && pis.Length > 0)
                        continue;
                    object obj2 = getters.GetValue(obj, null);
                    if (((obj2 != null) && !(obj2 is DBNull)) || this._params.SerializeNullValues)
                    {
                        if (flag)
                        {
                            this._output.Append(',');
                        }
                        this.WritePair(getters.Name, obj2);
                        if ((obj2 != null) && this._params.UseExtensions)
                        {
                            Type type = obj2.GetType();
                            if (type == typeof(object))
                            {
                                dic.Add(getters.Name, type.ToString());
                            }
                        }
                        flag = true;
                    }
                }
            }
            if ((dic.Count > 0) && this._params.UseExtensions)
            {
                this._output.Append(",\"$map\":");
                this.WriteStringDictionary(dic);
            }
            this._current_depth--;
            this._output.Append('}');
            this._current_depth--;
        }

        private void WritePair(string name, object value)
        {
            if (((value != null) && !(value is DBNull)) || this._params.SerializeNullValues)
            {
                this.WriteStringFast(name);
                this._output.Append(':');
                this.WriteValue(value);
            }
        }

        private void WritePairFast(string name, string value)
        {
            if ((value != null) || this._params.SerializeNullValues)
            {
                this.WriteStringFast(name);
                this._output.Append(':');
                this.WriteStringFast(value);
            }
        }

        private void WriteSD(StringDictionary stringDictionary)
        {
            this._output.Append('{');
            bool flag = false;
            foreach (DictionaryEntry entry in stringDictionary)
            {
                if (flag)
                {
                    this._output.Append(',');
                }
                this.WritePair((string) entry.Key, entry.Value);
                flag = true;
            }
            this._output.Append('}');
        }

        private void WriteString(string s)
        {
            this._output.Append('"');
            int startIndex = -1;
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if (this._useEscapedUnicode)
                {
                    if (((ch < ' ') || (ch >= '\x0080')) || ((ch == '"') || (ch == '\\')))
                    {
                        goto Label_007A;
                    }
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    continue;
                }
                if ((((ch != '\t') && (ch != '\n')) && ((ch != '\r') && (ch != '"'))) && (ch != '\\'))
                {
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    continue;
                }
            Label_007A:
                if (startIndex != -1)
                {
                    this._output.Append(s, startIndex, i - startIndex);
                    startIndex = -1;
                }
                switch (ch)
                {
                    case '\t':
                    {
                        this._output.Append(@"\t");
                        continue;
                    }
                    case '\n':
                    {
                        this._output.Append(@"\n");
                        continue;
                    }
                    case '\r':
                    {
                        this._output.Append(@"\r");
                        continue;
                    }
                    case '"':
                    case '\\':
                    {
                        this._output.Append('\\');
                        this._output.Append(ch);
                        continue;
                    }
                }
                if (this._useEscapedUnicode)
                {
                    this._output.Append(@"\u");
                    this._output.Append(((int) ch).ToString("X4", NumberFormatInfo.InvariantInfo));
                }
                else
                {
                    this._output.Append(ch);
                }
            }
            if (startIndex != -1)
            {
                this._output.Append(s, startIndex, s.Length - startIndex);
            }
            this._output.Append('"');
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            this._output.Append('{');
            bool flag = false;
            foreach (DictionaryEntry entry in dic)
            {
                if (flag)
                {
                    this._output.Append(',');
                }
                this.WritePair((string) entry.Key, entry.Value);
                flag = true;
            }
            this._output.Append('}');
        }

        private void WriteStringFast(string s)
        {
            this._output.Append('"');
            this._output.Append(s);
            this._output.Append('"');
        }

        private void WriteValue(object obj)
        {
            if ((obj == null) || (obj is DBNull))
            {
                this._output.Append("null");
            }
            else if ((obj is string) || (obj is char))
            {
                this.WriteString(obj.ToString());
            }
            else if (obj is Guid)
            {
                this.WriteGuid((Guid) obj);
            }
            else if (obj is bool)
            {
                this._output.Append(((bool) obj) ? "true" : "false");
            }
            else if ((((obj is int) || (obj is long)) || ((obj is double) || (obj is decimal))) || ((((obj is float) || (obj is byte)) || ((obj is short) || (obj is sbyte))) || (((obj is ushort) || (obj is uint)) || (obj is ulong))))
            {
                this._output.Append(((IConvertible) obj).ToString(NumberFormatInfo.InvariantInfo));
            }
            else if (obj is DateTime)
            {
                this.WriteDateTime((DateTime) obj);
            }
            else if ((!this._params.KVStyleStringDictionary && (obj is IDictionary)) && (obj.GetType().IsGenericType && (obj.GetType().GetGenericArguments()[0] == typeof(string))))
            {
                this.WriteStringDictionary((IDictionary) obj);
            }
            else if (obj is IDictionary)
            {
                this.WriteDictionary((IDictionary) obj);
            }
            else if (obj is byte[])
            {
                this.WriteBytes((byte[]) obj);
            }
            else if (obj is StringDictionary)
            {
                this.WriteSD((StringDictionary) obj);
            }
            else if (obj is NameValueCollection)
            {
                this.WriteNV((NameValueCollection) obj);
            }
            else if (obj is IEnumerable)
            {
                this.WriteArray((IEnumerable) obj);
            }
            else if (obj is Enum)
            {
                this.WriteEnum((Enum) obj);
            }
            else if (Reflection.Instance.IsTypeRegistered(obj.GetType()))
            {
                this.WriteCustom(obj);
            }
            else
            {
                this.WriteObject(obj);
            }
        }
    }
}

