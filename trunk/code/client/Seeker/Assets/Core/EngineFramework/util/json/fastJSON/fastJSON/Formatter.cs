namespace fastJSON
{
    using System;
    using System.Text;

    internal static class Formatter
    {
        public static string Indent = "   ";

        public static void AppendIndent(StringBuilder sb, int count)
        {
            while (count > 0)
            {
                sb.Append(Indent);
                count--;
            }
        }

        public static string PrettyPrint(string input)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            int length = input.Length;
            char[] chArray = input.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                char ch = chArray[i];
                if (ch == '"')
                {
                    bool flag = true;
                    while (flag)
                    {
                        sb.Append(ch);
                        ch = chArray[++i];
                        switch (ch)
                        {
                            case '\\':
                            {
                                sb.Append(ch);
                                ch = chArray[++i];
                                continue;
                            }
                            case '"':
                                flag = false;
                                break;
                        }
                    }
                }
                switch (ch)
                {
                    case ',':
                    {
                        sb.Append(ch);
                        sb.AppendLine();
                        AppendIndent(sb, count);
                        continue;
                    }
                    case ':':
                    {
                        sb.Append(" : ");
                        continue;
                    }
                    case '[':
                    case '{':
                    {
                        sb.Append(ch);
                        sb.AppendLine();
                        AppendIndent(sb, ++count);
                        continue;
                    }
                    case ']':
                    case '}':
                    {
                        sb.AppendLine();
                        AppendIndent(sb, --count);
                        sb.Append(ch);
                        continue;
                    }
                }
                if (!char.IsWhiteSpace(ch))
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}

