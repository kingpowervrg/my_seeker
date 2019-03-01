using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ChatDataGroup
{
    public Dictionary<int, ChatData> m_chats { get; set; }// = new Dictionary<int, ChatData>();
}

public class ChatData
{
    public int m_chatID { get; set; }
    public string m_chatBG { get; set; }
    public List<ChatItemData> m_chat { get; set; }

    public ChatData()
    {
        m_chat = new List<ChatItemData>();
    }
}

public class ChatItemData
{
    public int m_chatItemID { get; set; }
    public string m_chatContent { get; set; } //对话内容
    public string m_chatHead { get; set; } //对话头像
    public int m_chatPosition { get; set; } //头像位置

    public int m_chatType { get; set; }

    public List<int> m_nextChatItemID { get; set; } //下一个ID
    public List<string> m_chatKeyImages { get; set; } //关键物证

    public ChatItemData()
    {
        m_nextChatItemID = new List<int>();
        m_chatKeyImages = new List<string>();
    }
}


#region 编辑器内容

public class EditorChatDataGroup
{
    public Dictionary<int, EditorChatData> m_chats { get; set; }// = new Dictionary<int, ChatData>();
}

public class EditorChatData
{
    public int m_chatID { get; set; }
    public string m_chatBG { get; set; }
    public string m_describe { get; set; }
    public List<EditorChatItemData> m_chat { get; set; }

    public EditorChatData()
    {
        m_chat = new List<EditorChatItemData>();
    }
}

public class EditorChatItemData
{
    public int m_chatItemID { get; set; }
    public string m_chatContent { get; set; } //对话内容
    public string m_chatHead { get; set; } //对话头像
    public int m_chatPosition { get; set; } //头像位置

    public int m_chatType { get; set; }

    public List<EditorConnectionData> m_nextChatItemID { get; set; } //下一个ID
    public List<string> m_chatKeyImages { get; set; } //关键物证

    public EditorVector2 m_Position { get; set; }

    public EditorChatItemData()
    {
        m_nextChatItemID = new List<EditorConnectionData>();
        m_chatKeyImages = new List<string>();
    }
}

public class EditorConnectionData
{
    public int m_nextItemID { get; set; }
    public EditorVector2[] m_connectPoint { get; set; }
}

public class EditorVector2
{
    public float x { get; set; }
    public float y { get; set; }

    public EditorVector2()
    {
    }

    public EditorVector2(Vector2 v)
    {
        this.x = v.x;
        this.y = v.y;
    }
}
#endregion
