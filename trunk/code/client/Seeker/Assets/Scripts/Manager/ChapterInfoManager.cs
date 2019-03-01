using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class ChapterInfoManager : Singleton<ChapterInfoManager>
    {
        //Dictionary<long, ChapterInfo> m_chapter_infos_dict = new Dictionary<long, ChapterInfo>();

        //public void SetChapterInfo(long chapter_id_, ChapterInfo info_)
        //{
        //    if (!this.m_chapter_infos_dict.ContainsKey(chapter_id_))
        //        this.m_chapter_infos_dict.Add(chapter_id_, info_);
        //    else
        //    {
        //        this.m_chapter_infos_dict[chapter_id_] = info_;
        //    }
        //}

        //public ChapterInfo GetChapterInfo(long chapter_id_)
        //{
        //    ChapterInfo ret;
        //    if (this.m_chapter_infos_dict.TryGetValue(chapter_id_, out ret))
        //    {
        //        return ret;
        //    }

        //    return null;
        //}

        //public ChapterInfo FirstChapterInfo()
        //{
        //    if (this.m_chapter_infos_dict.Count <= 0)
        //        return null;


        //    foreach (ChapterInfo info in this.m_chapter_infos_dict.Values)
        //    {
        //        return info;
        //    }

        //    return null;
        //}

        private long m_cur_chapter_id;
        public long Cur_chapter_id
        {
            get { return m_cur_chapter_id; }
            set { m_cur_chapter_id = value; }
        }
    }
}
