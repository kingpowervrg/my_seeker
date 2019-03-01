using System;
using System.Collections.Generic;

//public enum PersuadeTalkType
//{
//    Self,
//    NPC
//}

public class PersuadeGroupData
{
    public List<PersuadeData> persuadeGroup { get; set; }
    //public Dictionary<int, PersuadeData> persuadeGroup { get; set; }

    public PersuadeGroupData()
    {
        //persuadeGroup = new Dictionary<int, PersuadeData>();
        persuadeGroup = new List<PersuadeData>();
    }
}

public class PersuadeData
{
    public int id { get; set; }
    public long npcId { get; set; }
    public string name { get; set; }
    public string introduce { get; set; }
    public long[] evidenceIds { get; set; }
    //public Dictionary<int, PersuadeItemData> m_items { get; set; }
    public List<PersuadeItemData> m_items { get; set; }

    public PersuadeData()
    {
        //m_items = new Dictionary<int, PersuadeItemData>();
        m_items = new List<PersuadeItemData>();
    }
}

public class PersuadeItemData
{
    public int itemId { get; set; }
    public int talkType { get; set; }  //自己  NPC
    public int persuadeType { get; set; } //正常对话 反驳对话 反馈对话
    public string content { get; set; }
    public long evidenceID { get; set; }  //证据ID
    public int feedbackIndex { get; set; } //正向反馈ID
    public int nextIndex { get; set; }
    public int forwardIndex { get; set; }
}

#region 编辑器数据
public class EditorPersuadeGroupData
{
    public Dictionary<int, EditorPersuadeData> persuadeGroup { get; set; }

    public EditorPersuadeGroupData()
    {
        persuadeGroup = new Dictionary<int, EditorPersuadeData>();
    }
}

public class EditorPersuadeData : PersuadeData
{
    //public int id { get; set; }
    //public long npcId { get; set; }
    //public long[] evidenceIds { get; set; }
    public string m_describe { get; set; }
    public List<EditorPersuadeItemData> m_editorItems { get; set; }

    public EditorPersuadeData()
    {
        this.m_editorItems = new List<EditorPersuadeItemData>();
    }
}

public class EditorPersuadeItemData : PersuadeItemData
{
    //public int itemId { get; set; }
    //public int talkType { get; set; }  //自己  NPC
    //public int persuadeType { get; set; } //正常对话 反驳对话 反馈对话
    //public string content { get; set; }
    //public long evidenceID { get; set; }  //证据ID
    //public int feedbackID { get; set; } //正向反馈ID
    //public int nextID { get; set; }

    public EditorVector2 m_Position { get; set; }

    public List<EditorConnectionData> m_nextChatItemID { get; set; } //下一个ID

    public EditorPersuadeItemData()
    {
        m_nextChatItemID = new List<EditorConnectionData>();
    }
}

#endregion