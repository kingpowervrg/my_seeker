using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace SimpleNodeEditor
{
    public class ChatSystemManager
    {
        private static ChatSystemManager m_instance = null;

        public static ChatSystemManager Instacne
        {
            get {
                if (m_instance == null)
                {
                    m_instance = new ChatSystemManager();
                }
                return m_instance;
            }
        }
        
        public string PATH
        {
            get
            {
                return Application.dataPath + "/Res/chat.json";
            }
        }

        public string EDITORPATH
        {
            get
            {
                return Application.dataPath + "/Res/EditorChat.json";
            }
        }

        private string GetChatFileContent(string path)
        {
            if (!File.Exists(path))
            {
                FileStream filestream = File.Create(path);
                filestream.Close();
            }
            StreamReader reader = new StreamReader(path);
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        public ChatDataGroup chatGroup = null;
        public EditorChatDataGroup editorChatGroup = null;
        public void ReadChatData()
        {
            string content = GetChatFileContent(PATH);
            if (!string.IsNullOrEmpty(content))
            {
                chatGroup = fastJSON.JSON.ToObject(content) as ChatDataGroup;
            }
            else
            {
                chatGroup = new ChatDataGroup();
                chatGroup.m_chats = new Dictionary<int, ChatData>();
            }

            string editorContent = GetChatFileContent(EDITORPATH);
            if (!string.IsNullOrEmpty(editorContent))
            {
                editorChatGroup = fastJSON.JSON.ToObject(editorContent) as EditorChatDataGroup;
            }
            else
            {
                editorChatGroup = new EditorChatDataGroup();
                editorChatGroup.m_chats = new Dictionary<int, EditorChatData>();
            }
        }

        public void SaveChatData(List<BaseNode> nodes, Texture BG, int chatID,string describe)
        {
            //bool needAdd = true;
            string bgName = string.Empty;
            if (BG != null)
            {
                bgName = BG.name + ".png";
            }
            ChatData chatData = GetChatDataByNodes(nodes, bgName, chatID);
            if (chatGroup.m_chats.ContainsKey(chatID))
            {
                chatGroup.m_chats[chatID] = chatData;
            }
            else
            {
                chatGroup.m_chats.Add(chatID,chatData);
            }
            
            string chatContent = fastJSON.JSON.ToJSON(chatGroup);
            CreateJson(PATH,chatContent);

            string editorbgName = string.Empty;
            if (BG != null)
            {
                editorbgName = UnityEditor.AssetDatabase.GetAssetPath(BG);
            }
            SaveEditorChatData(nodes, editorbgName, chatID, describe);
        }

        public void CreateJson(string path, string jsonStr)
        {
            byte[] jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            using (FileStream fsWrite = new FileStream(path, FileMode.Create))
            {
                fsWrite.Write(jsonByte, 0, jsonByte.Length);
                fsWrite.Close();
            }

        }

        private ChatData GetChatDataByNodes(List<BaseNode> nodes,string BG,int chatID)
        {
            ChatData chatData = new ChatData();
            chatData.m_chatBG = BG;
            chatData.m_chatID = chatID;
            for (int i = 0; i < nodes.Count; i++)
            {
                ChatItemData itemData = GetChatItemDataByBaseNode(nodes[i]);
                chatData.m_chat.Add(itemData);
            }
            return chatData;
        }

        private ChatItemData GetChatItemDataByBaseNode(BaseNode node)
        {
            ChatItemData itemData = new ChatItemData();
            itemData.m_chatItemID = node.Id;
            for (int i = 0; i < node.Lets.Count; i++)
            {
                if (node.Lets[i].Type == LetTypes.OUTLET)
                {
                    for (int j = 0; j < node.Lets[i].Connections.Count; j++)
                    {
                        Connection con = node.Lets[i].Connections[j];
                        itemData.m_nextChatItemID.Add(con.Inlet.Owner.Id);
                    }
                }
            }
            //itemData.m_nextChatItemID
            if (node is ButtonNode)
            {
                ButtonNode btnNode = node as ButtonNode;
                itemData.m_chatHead = btnNode.headValue;
                itemData.m_chatContent = btnNode.contentValue;
                itemData.m_chatPosition = (int)btnNode.m_headPosition;
                itemData.m_chatType = (int)btnNode.m_nodeType;
                if (btnNode.imgValue_00 != null)
                {
                    itemData.m_chatKeyImages.Add(btnNode.imgValue_00.name + ".png");
                }
                if (btnNode.imgValue_01 != null)
                {
                    itemData.m_chatKeyImages.Add(btnNode.imgValue_01.name + ".png");
                }
                if (btnNode.imgValue_02 != null)
                {
                    itemData.m_chatKeyImages.Add(btnNode.imgValue_02.name + ".png");
                }
                if (btnNode.imgValue_03 != null)
                {
                    itemData.m_chatKeyImages.Add(btnNode.imgValue_03.name + ".png");
                }
            }
            else if (node is ChatOptionContentNode)
            {
                ChatOptionContentNode contentNode = (ChatOptionContentNode)node;
                itemData.m_chatContent = contentNode.contentValue;
            }
            else if (node is ChatOptionNode)
            {
                ChatOptionNode optionNode = (ChatOptionNode)node;
                itemData.m_chatContent = optionNode.tex.name + ".png";
            }
            return itemData;
        }

        #region 编辑器数据
        public void SaveEditorChatData(List<BaseNode> nodes, string BG, int chatID,string describe)
        {
            //bool needAdd = true;
            EditorChatData chatData = GetEditorChatDataByNodes(nodes, BG, chatID);
            chatData.m_describe = describe;
            if (editorChatGroup.m_chats.ContainsKey(chatID))
            {
                editorChatGroup.m_chats[chatID] = chatData;
            }
            else
            {
                editorChatGroup.m_chats.Add(chatID, chatData);
            }

            string chatContent = fastJSON.JSON.ToJSON(editorChatGroup);
            CreateJson(EDITORPATH, chatContent);
        }

        private EditorChatData GetEditorChatDataByNodes(List<BaseNode> nodes, string BG, int chatID)
        {
            EditorChatData chatData = new EditorChatData();
            chatData.m_chatBG = BG;
            chatData.m_chatID = chatID;
            for (int i = 0; i < nodes.Count; i++)
            {
                EditorChatItemData itemData = GetEditorChatItemDataByBaseNode(nodes[i]);
                chatData.m_chat.Add(itemData);
            }
            return chatData;
        }

        private EditorChatItemData GetEditorChatItemDataByBaseNode(BaseNode node)
        {
            EditorChatItemData itemData = new EditorChatItemData();
            itemData.m_chatItemID = node.Id;
            itemData.m_Position = new EditorVector2(node.Position);
            for (int i = 0; i < node.Lets.Count; i++)
            {
                if (node.Lets[i].Type == LetTypes.OUTLET)
                {
                    for (int j = 0; j < node.Lets[i].Connections.Count; j++)
                    {
                        Connection con = node.Lets[i].Connections[j];
                        EditorConnectionData connectPoint = new EditorConnectionData();
                        connectPoint.m_connectPoint = new EditorVector2[con.Points.Length];
                        for (int k = 0; k < con.Points.Length; k++)
                        {
                            connectPoint.m_connectPoint[k] = new EditorVector2(con.Points[k]);
                        }
                        connectPoint.m_nextItemID = con.Inlet.Owner.Id;
                        itemData.m_nextChatItemID.Add(connectPoint);
                    }
                }
            }
            //itemData.m_nextChatItemID
            if (node is ButtonNode)
            {
                ButtonNode btnNode = node as ButtonNode;
                itemData.m_chatHead = btnNode.headValue;
                itemData.m_chatContent = btnNode.contentValue;
                itemData.m_chatPosition = (int)btnNode.m_headPosition;
                itemData.m_chatType = (int)btnNode.m_nodeType;
                if (btnNode.imgValue_00 != null)
                {
                    itemData.m_chatKeyImages.Add(UnityEditor.AssetDatabase.GetAssetPath(btnNode.imgValue_00));
                }
                if (btnNode.imgValue_01 != null)
                {
                    itemData.m_chatKeyImages.Add(UnityEditor.AssetDatabase.GetAssetPath(btnNode.imgValue_01));
                }
                if (btnNode.imgValue_02 != null)
                {
                    itemData.m_chatKeyImages.Add(UnityEditor.AssetDatabase.GetAssetPath(btnNode.imgValue_02));
                }
                if (btnNode.imgValue_03 != null)
                {
                    itemData.m_chatKeyImages.Add(UnityEditor.AssetDatabase.GetAssetPath(btnNode.imgValue_03));
                }
            }
            else if (node is ChatOptionContentNode)
            {
                ChatOptionContentNode contentNode = (ChatOptionContentNode)node;
                itemData.m_chatContent = contentNode.contentValue;
                itemData.m_chatType = (int)ChatNodeType.Content_Word;
            }
            else if (node is ChatOptionNode)
            {
                ChatOptionNode optionNode = (ChatOptionNode)node;
                itemData.m_chatContent = optionNode.tex.name;
                itemData.m_chatType = (int)ChatNodeType.Content_Image;
            }
            return itemData;
        }
        #endregion
    }
}
