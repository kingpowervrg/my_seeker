/********************************************************************
	created:  2018-4-28 11:25:43
	filename: StringUtils.cs
	author:	  songguangze@outlook.com
	
	purpose:  字符串的帮助类，可提高性能
             https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EngineCore
{
    public static class StringUtils
    {
        /// <summary>
        /// More performant version of string.EndsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool EndsWithFast(this string content, string match)
        {
            int ap = content.Length - 1;
            int bp = match.Length - 1;

            while (ap >= 0 && bp >= 0 && content[ap] == match[bp])
            {
                ap--;
                bp--;
            }

            return (bp < 0 && content.Length >= match.Length) || (ap < 0 && match.Length >= content.Length);
        }

        /// <summary>
        /// More performant version of string.StartsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool StartsWithFast(this string content, string match)
        {
            int aLen = content.Length;
            int bLen = match.Length;
            int ap = 0, bp = 0;

            while (ap < aLen && bp < bLen && content[ap] == match[bp])
            {
                ap++;
                bp++;
            }

            return (bp == bLen && aLen >= bLen) || (ap == aLen && bLen >= aLen);
        }

        /// <summary>
        /// 获取指定开始，结束的中间部分
        /// </summary>
        public static string GetBetween(this string content, string startMatchString, string endMatchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(startMatchString) && content.Contains(endMatchString))
            {
                var startIndex = content.IndexOf(startMatchString) + startMatchString.Length;
                var endIndex = content.IndexOf(endMatchString, startIndex);
                return content.Substring(startIndex, endIndex - startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content before the specified match (on last occurence).
        /// </summary>
        public static string GetBeforeLast(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var endIndex = content.LastIndexOf(matchString);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content after the specified match (on last occurence).
        /// </summary>
        public static string GetAfter(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.LastIndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content after the specified match (on first occurence).
        /// </summary>
        public static string GetAfterFirst(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.IndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Splits the string using new line symbol as a separator.
        /// Will split by all type of new lines, independant of environment.
        /// </summary>
        public static string[] SplitByNewLine(this string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            // "\r\n"   (\u000D\u000A)  Windows
            // "\n"     (\u000A)        Unix
            // "\r"     (\u000D)        Mac
            // Not using Environment.NewLine here, as content could've been produced 
            // in not the same environment we running the program in.
            return content.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Performes <see cref="string.Trim"/> additionally removing any BOM and other service symbols.
        /// </summary>
        public static string TrimFull(this string source)
        {
#if UNITY_WEBGL // WebGL build under .NET 4.6 fails when using Trim with UTF-8 chars. (should be fixed in Unity 2018.1)
            var whitespaceChars = new System.Collections.Generic.List<char> {
                '\u0009','\u000A','\u000B','\u000C','\u000D','\u0020','\u0085','\u00A0',
                '\u1680','\u2000','\u2001','\u2002','\u2003','\u2004','\u2005','\u2006',
                '\u2007','\u2008','\u2009','\u200A','\u2028','\u2029','\u202F','\u205F',
                '\u3000','\uFEFF','\u200B',
            };
    
            // Trim start.
            if (string.IsNullOrEmpty(source)) return source;
            var c = source[0];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(1);
                c = source[0];
            }
    
            // Trim end.
            if (string.IsNullOrEmpty(source)) return source;
            c = source[source.Length - 1];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(0, source.Length - 1);
                c = source[source.Length - 1];
            }
    
            return source;
#else
            return source.Trim().Trim(new char[] { '\uFEFF', '\u200B' });
#endif
        }

        /// <summary>
        /// Checks whether string is null, empty or consists of whitespace chars.
        /// </summary>
        public static bool IsNullEmptyOrWhiteSpace(string content)
        {
            if (String.IsNullOrEmpty(content))
                return true;

            return String.IsNullOrEmpty(content.TrimFull());
        }

        public static bool IsLongNullEmpty(long[] content)
        {
            if (content == null || content.Length == 0)
            {
                return true;
            }
            return false;
        }


        public static bool IsIntNullEmpty(int[] content)
        {
            if (content == null || content.Length == 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 是否是中文
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsChinese(char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;
        }

        /// <summary>
        /// 中文字符的数量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ChineseCountInString(string str)
        {
            int chineseCharCount = 0;
            char[] strChars = str.ToCharArray();
            for (int i = 0; i < strChars.Length; ++i)
                chineseCharCount = IsChinese(strChars[i]) ? ++chineseCharCount : chineseCharCount;

            return chineseCharCount;
        }

        /// <summary>
        /// 是否是Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrlValid(string url, out string urlLink)
        {
            string pattern = @"(http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-]))?";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matchCollection = reg.Matches(url);

            urlLink = matchCollection.Count > 0 ? matchCollection[0].Value : string.Empty;
            return reg.IsMatch(url);
        }

       

    }
}