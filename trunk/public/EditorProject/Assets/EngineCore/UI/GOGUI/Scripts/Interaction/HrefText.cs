using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace GOGUI
{
    /// <summary>
    /// 超链接 文本格式 如: start<url info=mm>[木剑]</url>武器en
    /// </summary>
    public class HrefText
    {

        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private readonly List<HrefInfo> m_hrefInfoList = new List<HrefInfo>();

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static readonly Regex m_hrefRegex =
            new Regex(@"<url info=([^>\n\s]+)>(.*?)(</url>)", RegexOptions.Singleline);

        private static readonly Regex m_hrefColor =
            new Regex(@"<color=([^>\n\s]+)>(.*?)(</color>)", RegexOptions.Singleline);      
		
		private  int m_charNum = 4;// 每个字符占4位
        public HrefText()
        {

        }
        public List<HrefInfo> getHrefList
        {
            get
            {
                return m_hrefInfoList;
            }
        }
        public string getResolveText(string _str)
        {
            return resolveText(_str);
        }
        protected string resolveText(string _text)
        {
            StringBuilder m_textBuilder = new StringBuilder();
            var indexText = 0;
			m_hrefInfoList.Clear();            
			foreach (Match match in m_hrefRegex.Matches(_text))
            {
                m_textBuilder.Append(_text.Substring(indexText, match.Index - indexText));
                m_textBuilder.Append("<color=yellow>");  // 超链接颜色

                var group = match.Groups[1];
                HrefInfo _hrefInfo = new HrefInfo();
                _hrefInfo.startIndex = m_textBuilder.Length * m_charNum;// 超链接里的文本起始顶点索引 
               _hrefInfo.endIndex = (m_textBuilder.Length + match.Groups[2].Length - 1) * m_charNum + 3;
                _hrefInfo.name = group.Value;
                m_hrefInfoList.Add(_hrefInfo);

                m_textBuilder.Append(match.Groups[2].Value);
                m_textBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            m_textBuilder.Append(_text.Substring(indexText, _text.Length - indexText));
            return m_textBuilder.ToString();
        }
    }
    /// <summary>
    /// 超链接信息类
    /// </summary>
    public class HrefInfo
    {
        public int startIndex;

        public int endIndex;

        public string name;

		public Color32 color;

        public readonly BetterList<Rect> boxes = new BetterList<Rect>();
    }
}

