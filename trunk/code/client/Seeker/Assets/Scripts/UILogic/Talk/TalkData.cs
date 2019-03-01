using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public enum TalkDialogEnum
    {
        NormalTalk,
        SceneTalk,
        AchiveTalk, //档案对话
        TaskEndTalk
    }


    /// <summary>
    /// 对话数据
    /// </summary>
    public class TalkData
    {
        public ConfChat chatData;

        public List<ConfChatItem> partData;
        //public long ID { get; set; }

        //public string bgImgPath { get; set; }

        //public List<TalkPartData> partData { get; set; }
    }

    public class TalkPartData
    {

        //public long partID { get; set; }

        //public string iconPath { get; set; }

        //public int iconPos { get; set; } //0左   1 右

        //public string name { get; set; }

        //public string talkContent { get; set; }

        //public long[] articleID { get; set; } //物件ID

        //public int nextType { get; set; }  //0 纯文字  1文字选择  2图片选择

        //public long[] nextID { get; set; }

        //public string[] nextBtn { get; set; }

    }

    public class TalkRewardData
    {
        public long[] ID;
    }


    public class TalkUIData
    {
        public TalkDialogEnum m_talk_type;
        public long talk_id;
    }

}

