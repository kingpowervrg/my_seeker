using System;
using System.Collections.Generic;

public class PersuadeGroupData
{
    //public Dictionary<int, PersuadeData> persuadeGroup { get; set; }
    public List<PersuadeData> persuadeGroup { get; set; }
    //public PersuadeGroupData()
    //{
    //    persuadeGroup = new Dictionary<int, PersuadeData>();
    //}
}

public class PersuadeData
{
    public int id { get; set; }
    public long npcId { get; set; }
    public string name { get; set; }
    public string introduce { get; set; }
    public long[] evidenceIds { get; set; }
    public List<PersuadeItemData> m_items { get; set; }

    //public PersuadeData()
    //{
    //    m_items = new List<PersuadeItemData>();
    //}
}

public class PersuadeItemData
{
    public int itemId { get; set; }
    public int talkType { get; set; }
    public int persuadeType { get; set; }
    public string content { get; set; }
    public long evidenceID { get; set; }
    public int feedbackIndex { get; set; }
    public int nextIndex { get; set; }
    public int forwardIndex { get; set; }
}
