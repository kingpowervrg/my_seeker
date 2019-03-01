using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace SeekerGame
{
    public class MD5URL
    {
        private string m_url;
        public string Url
        {
            get { return m_url; }
        }

        public MD5URL(string url_)
        {
            m_url = CommonTools.GetMd5Str1(url_);
        }

        public static bool operator ==(MD5URL md5_url, string url)
        {
            return md5_url.Url == CommonTools.GetMd5Str1(url);
        }

        public static bool operator !=(MD5URL md5_url, string url)
        {
            return md5_url.Url != CommonTools.GetMd5Str1(url);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class FriendIcon
    {
        private string m_url;
        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        //private MD5URL m_url;
        //public SeekerGame.MD5URL Url
        //{
        //    get { return m_url; }
        //    set { m_url = value; }
        //}
        public Texture m_tex;
    }
}
