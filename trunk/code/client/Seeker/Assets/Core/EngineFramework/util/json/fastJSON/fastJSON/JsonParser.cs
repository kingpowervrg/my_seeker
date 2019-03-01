namespace fastJSON
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal sealed class JsonParser
    {
        private bool _ignorecase;
        private int index;
        private readonly string json;
        private Token lookAheadToken = Token.None;
        private readonly StringBuilder s = new StringBuilder();

        internal JsonParser(string json, bool ignorecase)
        {
            this.json = json;
            this._ignorecase = ignorecase;
        }

        private void ConsumeToken()
        {
            this.lookAheadToken = Token.None;
        }

        private long CreateLong(string s)
        {
            long num = 0L;
            bool flag = false;
            foreach (char ch in s)
            {
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
            }
            if (!flag)
            {
                return num;
            }
            return -num;
        }

        public object Decode()
        {
            return this.ParseValue();
        }

        private Token LookAhead()
        {
            if (this.lookAheadToken != Token.None)
            {
                return this.lookAheadToken;
            }
            return (this.lookAheadToken = this.NextTokenCore());
        }

        private Token NextToken()
        {
            Token token = (this.lookAheadToken != Token.None) ? this.lookAheadToken : this.NextTokenCore();
            this.lookAheadToken = Token.None;
            return token;
        }

        private Token NextTokenCore()
        {
            char ch;
            do
            {
                ch = this.json[this.index];
            }
            while (((ch <= ' ') && (((ch == ' ') || (ch == '\t')) || ((ch == '\n') || (ch == '\r')))) && (++this.index < this.json.Length));
            if (this.index == this.json.Length)
            {
                throw new Exception("Reached end of string unexpectedly");
            }
            ch = this.json[this.index];
            this.index++;
            switch (ch)
            {
                case '{':
                    return Token.Curly_Open;

                case '}':
                    return Token.Curly_Close;

                case 't':
                    if ((((this.json.Length - this.index) < 3) || (this.json[this.index] != 'r')) || ((this.json[this.index + 1] != 'u') || (this.json[this.index + 2] != 'e')))
                    {
                        break;
                    }
                    this.index += 3;
                    return Token.True;

                case 'n':
                    if ((((this.json.Length - this.index) >= 3) && (this.json[this.index] == 'u')) && ((this.json[this.index + 1] == 'l') && (this.json[this.index + 2] == 'l')))
                    {
                        this.index += 3;
                        return Token.Null;
                    }
                    break;

                case '"':
                    return Token.String;

                case '+':
                case '-':
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return Token.Number;

                case ',':
                    return Token.Comma;

                case ':':
                    return Token.Colon;

                case '[':
                    return Token.Squared_Open;

                case ']':
                    return Token.Squared_Close;

                case 'f':
                    if (((((this.json.Length - this.index) < 4) || (this.json[this.index] != 'a')) || ((this.json[this.index + 1] != 'l') || (this.json[this.index + 2] != 's'))) || (this.json[this.index + 3] != 'e'))
                    {
                        break;
                    }
                    this.index += 4;
                    return Token.False;
            }
            throw new Exception("Could not find token at index " + --this.index);
        }

        private List<object> ParseArray()
        {
            List<object> list = new List<object>();
            this.ConsumeToken();
            while (true)
            {
                switch (this.LookAhead())
                {
                    case Token.Squared_Close:
                        this.ConsumeToken();
                        return list;

                    case Token.Comma:
                    {
                        this.ConsumeToken();
                        continue;
                    }
                }
                list.Add(this.ParseValue());
            }
        }

        private object ParseNumber()
        {
            long num2;
            this.ConsumeToken();
            int startIndex = this.index - 1;
            bool flag = false;
            do
            {
                if (this.index == this.json.Length)
                {
                    break;
                }
                char ch = this.json[this.index];
                if (((ch < '0') || (ch > '9')) && (((ch != '.') && (ch != '-')) && (((ch != '+') && (ch != 'e')) && (ch != 'E'))))
                {
                    break;
                }
                if (((ch == '.') || (ch == 'e')) || (ch == 'E'))
                {
                    flag = true;
                }
            }
            while (++this.index != this.json.Length);
            if (flag)
            {
                return double.Parse(this.json.Substring(startIndex, this.index - startIndex), NumberFormatInfo.InvariantInfo);
            }
            return JSON.CreateLong(out num2, this.json, startIndex, this.index - startIndex);
        }

        private Dictionary<string, object> ParseObject()
        {
            Token token;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            this.ConsumeToken();
        Label_000C:
            token = this.LookAhead();
            if (token != Token.Curly_Close)
            {
                if (token != Token.Comma)
                {
                    string str = this.ParseString();
                    if (this._ignorecase)
                    {
                        str = str.ToLower();
                    }
                    if (this.NextToken() != Token.Colon)
                    {
                        throw new Exception("Expected colon at index " + this.index);
                    }
                    object obj2 = this.ParseValue();
                    dictionary[str] = obj2;
                }
                else
                {
                    this.ConsumeToken();
                }
                goto Label_000C;
            }
            this.ConsumeToken();
            return dictionary;
        }

        private uint ParseSingleChar(char c1, uint multipliyer)
        {
            uint num = 0;
            if ((c1 >= '0') && (c1 <= '9'))
            {
                return ((uint)(c1 - '0') * multipliyer);
            }
            if ((c1 >= 'A') && (c1 <= 'F'))
            {
                return (uint) (((c1 - 'A') + 10) * multipliyer);
            }
            if ((c1 >= 'a') && (c1 <= 'f'))
            {
                num = (uint) (((c1 - 'a') + 10) * multipliyer);
            }
            return num;
        }

        private string ParseString()
        {
            this.ConsumeToken();
            this.s.Length = 0;
            int startIndex = -1;
            while (this.index < this.json.Length)
            {
                char ch = this.json[this.index++];
                if (ch == '"')
                {
                    if (startIndex != -1)
                    {
                        if (this.s.Length == 0)
                        {
                            return this.json.Substring(startIndex, (this.index - startIndex) - 1);
                        }
                        this.s.Append(this.json, startIndex, (this.index - startIndex) - 1);
                    }
                    return this.s.ToString();
                }
                if (ch != '\\')
                {
                    if (startIndex == -1)
                    {
                        startIndex = this.index - 1;
                    }
                }
                else
                {
                    if (this.index == this.json.Length)
                    {
                        break;
                    }
                    if (startIndex != -1)
                    {
                        this.s.Append(this.json, startIndex, (this.index - startIndex) - 1);
                        startIndex = -1;
                    }
                    switch (this.json[this.index++])
                    {
                        case '"':
                            goto Label_015A;

                        case '/':
                            goto Label_0180;

                        case '\\':
                            goto Label_016D;

                        case 'r':
                            this.s.Append('\r');
                            break;

                        case 't':
                            this.s.Append('\t');
                            break;

                        case 'u':
                        {
                            int num2 = this.json.Length - this.index;
                            if (num2 >= 4)
                            {
                                uint num3 = this.ParseUnicode(this.json[this.index], this.json[this.index + 1], this.json[this.index + 2], this.json[this.index + 3]);
                                this.s.Append((char) num3);
                                this.index += 4;
                            }
                            break;
                        }
                        case 'n':
                            this.s.Append('\n');
                            break;

                        case 'b':
                            goto Label_0193;

                        case 'f':
                            goto Label_01A5;
                    }
                }
                continue;
            Label_015A:
                this.s.Append('"');
                continue;
            Label_016D:
                this.s.Append('\\');
                continue;
            Label_0180:
                this.s.Append('/');
                continue;
            Label_0193:
                this.s.Append('\b');
                continue;
            Label_01A5:
                this.s.Append('\f');
            }
            throw new Exception("Unexpectedly reached end of string");
        }

        private uint ParseUnicode(char c1, char c2, char c3, char c4)
        {
            uint num = this.ParseSingleChar(c1, 0x1000);
            uint num2 = this.ParseSingleChar(c2, 0x100);
            uint num3 = this.ParseSingleChar(c3, 0x10);
            uint num4 = this.ParseSingleChar(c4, 1);
            return (((num + num2) + num3) + num4);
        }

        private object ParseValue()
        {
            switch (this.LookAhead())
            {
                case Token.Curly_Open:
                    return this.ParseObject();

                case Token.Squared_Open:
                    return this.ParseArray();

                case Token.String:
                    return this.ParseString();

                case Token.Number:
                    return this.ParseNumber();

                case Token.True:
                    this.ConsumeToken();
                    return true;

                case Token.False:
                    this.ConsumeToken();
                    return false;

                case Token.Null:
                    this.ConsumeToken();
                    return null;
            }
            throw new Exception("Unrecognized token at index" + this.index);
        }

        private enum Token
        {
            Colon = 4,
            Comma = 5,
            Curly_Close = 1,
            Curly_Open = 0,
            False = 9,
            None = -1,
            Null = 10,
            Number = 7,
            Squared_Close = 3,
            Squared_Open = 2,
            String = 6,
            True = 8
        }
    }
}

