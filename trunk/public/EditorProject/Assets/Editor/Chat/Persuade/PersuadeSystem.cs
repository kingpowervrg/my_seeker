using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace SimpleNodeEditor
{
    public class PersuadeSystem
    {
        private static PersuadeSystem m_instance = null;

        public static PersuadeSystem Instacne
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new PersuadeSystem();
                }
                return m_instance;
            }
        }

        public string PATH
        {
            get
            {
                return Application.dataPath + "/Res/ClientConfig/persuade.json";
            }
        }

        public string EDITORPATH
        {
            get
            {
                return Application.dataPath + "/Res/EditorPersuade.json";
            }
        }

        private string GetFileContent(string path)
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

        public PersuadeGroupData persuadeGroup = null;
        public EditorPersuadeGroupData editorChatGroup = null;
        public void ReadData()
        {
            string content = GetFileContent(PATH);
            if (!string.IsNullOrEmpty(content))
            {
                persuadeGroup = fastJSON.JSON.ToObject(content) as PersuadeGroupData;
            }
            else
            {
                persuadeGroup = new PersuadeGroupData();
                //persuadeGroup.persuadeGroup = new Dictionary<int, PersuadeData>();
            }

            string editorContent = GetFileContent(EDITORPATH);
            if (!string.IsNullOrEmpty(editorContent))
            {
                editorChatGroup = fastJSON.JSON.ToObject(editorContent) as EditorPersuadeGroupData;
            }
            else
            {
                editorChatGroup = new EditorPersuadeGroupData();
                editorChatGroup.persuadeGroup = new Dictionary<int, EditorPersuadeData>();
            }
        }

        public void SaveData(List<BaseNode> nodes, int ID, EditorPersuadeData editorData, PersuadeData persuadeData)
        {
            //bool needAdd = true;
            string bgName = string.Empty;

            GetDataByNodes(nodes, ID, persuadeData);
            bool hasExist = false;
            for (int i = 0; i < persuadeGroup.persuadeGroup.Count; i++)
            {
                if (persuadeGroup.persuadeGroup[i].id == ID)
                {
                    persuadeGroup.persuadeGroup[i] = persuadeData;
                    hasExist = true;
                    break;
                }
            }
            if (!hasExist)
            {
                persuadeGroup.persuadeGroup.Add(persuadeData);
            }
            //if (persuadeGroup.persuadeGroup.ContainsKey(ID))
            //{
            //    persuadeGroup.persuadeGroup[ID] = persuadeData;
            //}
            //else
            //{
            //    persuadeGroup.persuadeGroup.Add(ID, persuadeData);
            //}

            string chatContent = fastJSON.JSON.ToJSON(persuadeGroup);
            CreateJson(PATH, chatContent);

            string editorbgName = string.Empty;
            
            SaveEditorData(nodes, ID, editorData);
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

        private List<BaseNode> m_nodes = null;
        private PersuadeData GetDataByNodes(List<BaseNode> nodes, int id, PersuadeData persuadeData)
        {
            //PersuadeData persuadeData = new PersuadeData();
            //chatData.m_chatBG = BG;
            m_nodes = nodes;
            persuadeData.id = id;
            for (int i = 0; i < nodes.Count; i++)
            {
                PersuadeItemData itemData = GetItemDataByBaseNode(nodes[i]);
                persuadeData.m_items.Add(itemData);
            }
            return persuadeData;
        }

        private PersuadeItemData GetItemDataByBaseNode(BaseNode node)
        {
            PersuadeItemData itemData = new PersuadeItemData();
            itemData.itemId = node.Id;
            itemData.nextIndex = -1;
            itemData.forwardIndex = -1;
            itemData.feedbackIndex = -1;
            for (int i = 0; i < node.Lets.Count; i++)
            {
                if (node.Lets[i].Type == LetTypes.OUTLET)
                {
                    for (int j = 0; j < node.Lets[i].Connections.Count; j++)
                    {
                        Connection con = node.Lets[i].Connections[j];
                        int nextIndex = -1;
                        for (int m = 0; m < m_nodes.Count; m++)
                        {
                            if (m_nodes[m].Id == con.Inlet.Owner.Id)
                            {
                                nextIndex = m;
                                break;
                            }
                        }
                        if (nextIndex == -1)
                        {
                            Debug.LogError("error nextindex");
                        }
                        if (con.Outlet.type == 0)
                        {
                            itemData.nextIndex = nextIndex;
                        }
                        else
                        {
                            itemData.feedbackIndex = nextIndex;
                        }
                        //itemData.m_nextChatItemID.Add(con.Inlet.Owner.Id);
                    }
                }
                else if (node.Lets[i].Type == LetTypes.INLET)
                {
                    for (int j = 0; j < node.Lets[i].Connections.Count; j++)
                    {
                        Connection con = node.Lets[i].Connections[j];
                        for (int m = 0; m < m_nodes.Count; m++)
                        {
                            if (m_nodes[m].Id == con.Outlet.Owner.Id)
                            {
                                itemData.forwardIndex = m;
                                break;
                            }
                        }
                    }
                }
            }
            if (node is PersuadeBaseNode)
            {
                PersuadeBaseNode persuadeNode = node as PersuadeBaseNode;
                itemData.talkType = (int)persuadeNode.m_talkType;
                itemData.persuadeType = (int)persuadeNode.m_type;
                itemData.content = persuadeNode.content;
                itemData.evidenceID = persuadeNode.evidenceID;
                //itemData.feedbackID = persuadeNode.feedbackID;
            }
            
            return itemData;
        }

        #region 编辑器数据
        public void SaveEditorData(List<BaseNode> nodes, int ID, EditorPersuadeData editorData)
        {
            //bool needAdd = true;
            GetEditorDataByNodes(nodes, ID, editorData);
            //persuadeData.m_describe = describe;
            if (editorChatGroup.persuadeGroup.ContainsKey(ID))
            {
                editorChatGroup.persuadeGroup[ID] = editorData;
            }
            else
            {
                editorChatGroup.persuadeGroup.Add(ID, editorData);
            }

            string chatContent = fastJSON.JSON.ToJSON(editorChatGroup);
            CreateJson(EDITORPATH, chatContent);
        }

        private EditorPersuadeData GetEditorDataByNodes(List<BaseNode> nodes, int ID, EditorPersuadeData persuadeData)
        {
            //EditorPersuadeData persuadeData = new EditorPersuadeData();
            persuadeData.id = ID;
            for (int i = 0; i < nodes.Count; i++)
            {
                EditorPersuadeItemData itemData = GetEditorItemDataByBaseNode(nodes[i]);
                persuadeData.m_editorItems.Add(itemData);
            }
            return persuadeData;
        }

        private EditorPersuadeItemData GetEditorItemDataByBaseNode(BaseNode node)
        {
            EditorPersuadeItemData itemData = new EditorPersuadeItemData();
            itemData.itemId = node.Id;
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
            if (node is PersuadeBaseNode)
            {
                PersuadeBaseNode persuadeNode = node as PersuadeBaseNode;
                itemData.talkType = (int)persuadeNode.m_talkType;
                itemData.persuadeType = (int)persuadeNode.m_type;
                itemData.content = persuadeNode.content;
                itemData.evidenceID = persuadeNode.evidenceID;
                //itemData.feedbackID = persuadeNode.feedbackID;
            }
            return itemData;
        }
        #endregion
    }
}
