using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    public class PersuadeEditor : EditorWindow
    {
        public Color ConnectionColor = Color.green;

        protected List<BaseNode> m_nodes = new List<BaseNode>();

        protected int NodeID = 0;
        private int chatID = 1000;
        private PersuadeData m_persuadeData = null;

        private Texture m_bgImg = null;
        private string m_describeLab = string.Empty;
        protected MouseModes m_currentMouseMode = MouseModes.IDLE;
        protected Let m_currentSelectedLet = null;

        protected Vector2 m_startMousePos = Vector2.zero;

        public static System.Action<EditorPersuadeData> changeNewChat;
        //public Inlet MasterInlet = null;
        //public Outlet MasterOutlet = null;

        private List<Vector2> m_livePoints = new List<Vector2>();

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void Construct()
        {
            changeNewChat = CreateChatNodeByEditorData;
            PersuadeSystem.Instacne.ReadData();
            //ChatSystemManager.Instacne.ReadChatData();
            chatID += PersuadeSystem.Instacne.persuadeGroup.persuadeGroup.Count;
            m_persuadeData = new PersuadeData();
        }

        //void OnHierarchyChange()
        //{
        //    Construct();
        //}

        static void ShowEditor()
        {
            EditorWindow.GetWindow<PersuadeEditor>();
        }

        void OnFocus()
        {
            wantsMouseMove = true;
            //Construct();
        }

        void OnLetPressed(object sender, LetTypes type)
        {
            m_currentSelectedLet = (Let)sender;
            m_startMousePos = Event.current.mousePosition;
            m_currentMouseMode = MouseModes.CONNECTING;
        }

        void OnLetDrag(object sender, LetTypes type)
        {
            if (sender != m_currentSelectedLet)
            {
                Let senderLet = (Let)sender;
                if (senderLet.Owner != m_currentSelectedLet.Owner)
                {
                    if ((m_currentSelectedLet.Type == LetTypes.INLET && type == LetTypes.OUTLET) ||
                        (m_currentSelectedLet.Type == LetTypes.OUTLET && type == LetTypes.INLET))
                    {
                        // Valid connection
                    }
                }
            }
        }

        void OnLetUp(object sender, LetTypes type)
        {
            if (sender != m_currentSelectedLet)
            {
                Let senderLet = (Let)sender;
                if (senderLet.Owner != m_currentSelectedLet.Owner)
                {
                    if ((m_currentSelectedLet.Type == LetTypes.INLET && type == LetTypes.OUTLET) ||
                        (m_currentSelectedLet.Type == LetTypes.OUTLET && type == LetTypes.INLET))
                    {
                        Let inlet = null;
                        Let outlet = null;

                        if (m_currentSelectedLet.Type == LetTypes.INLET)
                        {
                            inlet = m_currentSelectedLet;
                            outlet = (Let)sender;
                        }
                        else
                        {
                            outlet = m_currentSelectedLet;
                            inlet = (Let)sender;
                        }

                        Connection connection = new Connection((Inlet)inlet, (Outlet)outlet, m_livePoints);
                        inlet.Connections.Add(connection);
                        outlet.Connections.Add(connection);

                        if (Application.isPlaying)
                        {
                            ((Outlet)outlet).MakeConnections();
                            //   ((Outlet)outlet).Emit += ((Inlet)inlet).Slot;
                        }
                        //if ( !inlet.Contains(outlet) && 
                        //    !outlet.Contains(inlet) )
                        //{

                        //}
                        //else
                        //{
                        //    Debug.LogError("has let ====");
                        //}

                        m_livePoints.Clear();
                        m_currentMouseMode = MouseModes.IDLE;
                    }
                }
            }
        }

        BaseNode CreateNode(Vector2 pos, System.Type nodeType, int nodeID = -1)
        {
            // TODO : make this better ( for example, get the first available NodeID )

            NodeID++;
            BaseNode simpleNode = System.Activator.CreateInstance(nodeType) as BaseNode;
            //GameObject nodeObject = new GameObject("Node");
            //BaseNode simpleNode = (BaseNode) nodeObject.AddComponent(nodeType);

            simpleNode.Construct();
            if (nodeID > 0)
            {
                simpleNode.Id = nodeID;
            }
            else
            {
                simpleNode.Id = chatID + NodeID;
            }

            simpleNode.Position = new Vector2(pos.x, pos.y);
            m_nodes.Add(simpleNode);

            for (int i = 0; i < simpleNode.Lets.Count; i++)
            {
                simpleNode.Lets[i].LetClicked += OnLetPressed;
                simpleNode.Lets[i].LetDrag += OnLetDrag;
                simpleNode.Lets[i].LetUp += OnLetUp;
            }
            return simpleNode;
            //simpleNode.transform.parent = Root.transform;
        }

        void CreateNodeMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            Vector2 mousePos = Event.current.mousePosition;

            // this is making the assumption that all assemblies we need are already loaded.
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    var attribs = type.GetCustomAttributes(typeof(NodeMenuItem), false);
                    if (attribs.Length > 0)
                    {
                        for (int i = 0; i < attribs.Length; i++)
                        {
                            string name = ((NodeMenuItem)attribs[i]).Name;
                            if (name.Contains("Persuade"))
                            {
                                System.Type nodeType = ((NodeMenuItem)attribs[i]).Type;

                                genericMenu.AddItem(new GUIContent(name), false, () =>
                                {
                                    CreateNode(mousePos, nodeType);
                                });
                            }
                           
                        }
                    }

                }
            }

            genericMenu.ShowAsContext();
        }

        void BreakConnectionMenu(Inlet inlet, Outlet outlet)
        {
            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent("Break Connection"), false, () =>
            {
                inlet.RemoveLet(outlet);
                outlet.RemoveLet(inlet);
            });

            genericMenu.ShowAsContext();
        }

        private void SetPersuadeData(PersuadeData persuade)
        {
            persuade.npcId = npcId;
            persuade.name = persuadeName;
            persuade.introduce = introduce;
            string[] evids = evidenceIds.Split(',');
            persuade.evidenceIds = new long[evids.Length];
            for (int i = 0; i < evids.Length; i++)
            {
                persuade.evidenceIds[i] = long.Parse(evids[i]);
            }
        }

        private long npcId;
        private string evidenceIds;
        private string persuadeName;
        private string introduce;
        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 80, 30), "New"))
            {
                OnDestroy();
                Clear();
                chatID = 1000 + PersuadeSystem.Instacne.persuadeGroup.persuadeGroup.Count;
                Repaint();
            }
            if (GUI.Button(new Rect(10, 50, 80, 30), "Open"))
            {
                ChatJsonWindow.Open(JsonWindow.Persuade);
            }
            if (GUI.Button(new Rect(10, 90, 80, 30), "Save"))
            {
                EditorPersuadeData editorPersuade = new EditorPersuadeData();
                SetPersuadeData(editorPersuade);
                editorPersuade.m_describe = m_describeLab;
                PersuadeSystem.Instacne.SaveEditorData(m_nodes,chatID, editorPersuade);
            }

            if (GUI.Button(new Rect(10, 130, 80, 30), "Ouput"))
            {
                EditorPersuadeData editorPersuade = new EditorPersuadeData();
                SetPersuadeData(editorPersuade);
                editorPersuade.m_describe = m_describeLab;
                PersuadeData persuadeData = new PersuadeData();
                SetPersuadeData(persuadeData);
                PersuadeSystem.Instacne.SaveData(m_nodes, chatID, editorPersuade, persuadeData);
            }
            GUI.BeginGroup(new Rect(100, 5, 500, 400));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID:", GUILayout.MaxWidth(150));
            chatID = EditorGUILayout.IntField(chatID);
            EditorGUILayout.EndHorizontal();
            //m_bgImg = EditorGUILayout.ObjectField(m_bgImg, typeof(Texture), GUILayout.MaxWidth(180)) as Texture;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("描述(策划看):", GUILayout.MaxWidth(150));
            m_describeLab = EditorGUILayout.TextArea(m_describeLab);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("名称:", GUILayout.MaxWidth(150));
            persuadeName = EditorGUILayout.TextArea(persuadeName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("开头介绍:", GUILayout.MaxWidth(150));
            introduce = EditorGUILayout.TextArea(introduce);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("关键证人ID:", GUILayout.MaxWidth(150));
            npcId = EditorGUILayout.LongField(npcId);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("证据ID数组0,1,2:", GUILayout.MaxWidth(150));
            evidenceIds = EditorGUILayout.TextField(evidenceIds);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUI.EndGroup();
            //GUI.BeginGroup(new Rect(100, 15, 200, 50));
            //GUI.EndGroup();
            var nodesToRemove = new List<int>();
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i] == null)
                {
                    nodesToRemove.Add(i);
                }
            }

            for (int i = 0; i < nodesToRemove.Count; i++)
            {
                m_nodes.RemoveAt(nodesToRemove[i] - i);
            }

            BeginWindows();

            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Draw();
            }

            EndWindows();

            bool isConnectionSelected = false;
            Connection connectionSelected = null;
            float minDistance = float.MaxValue;

            // Collect connections
            List<Connection> connections = new List<Connection>();
            int selectedConnection = -1;
            for (int i = 0; i < m_nodes.Count; i++)
            {
                for (int j = 0; j < m_nodes[i].Lets.Count; j++)
                {
                    Let outlet = m_nodes[i].Lets[j];
                    if (outlet.Type == LetTypes.OUTLET)
                    {
                        for (int k = 0; k < outlet.Connections.Count; k++)
                        {
                            Connection connection = outlet.Connections[k];
                            connections.Add(connection);

                            List<Vector2> points = new List<Vector2>();
                            points.Add(new Vector2(connection.Inlet.Position.center.x, connection.Inlet.Position.center.y));
                            for (int l = 0; l < connection.Points.Length; l++)
                            {
                                points.Add(connection.Points[l]);
                            }
                            points.Add(new Vector2(connection.Outlet.Position.center.x, connection.Outlet.Position.center.y));

                            for (int l = 0; l < points.Count - 1; l++)
                            {
                                float distance = MouseDistanceToLine(points[l], points[l + 1]);

                                if (distance < 20.0f)
                                {
                                    if (distance < minDistance)
                                    {
                                        minDistance = distance;
                                        isConnectionSelected = true;
                                        connectionSelected = connection;
                                        selectedConnection = connections.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Draw connections 
            for (int i = 0; i < connections.Count; i++)
            {
                Connection connection = connections[i];

                List<Vector2> points = new List<Vector2>();
                points.Add(connection.Inlet.Position.center);
                for (int j = 0; j < connection.Points.Length; j++)
                {
                    points.Add(connection.Points[j]);
                }
                points.Add(connection.Outlet.Position.center);

                for (int j = 0; j < points.Count - 1; j++)
                {
                    if (i != selectedConnection)
                    {
                        DrawLine(points[j], points[j + 1], ConnectionColor);
                    }
                    else
                    {
                        DrawLine(points[j], points[j + 1], Color.blue);
                    }
                }

            }

            // Process events
            if (Event.current.type == EventType.MouseMove)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if (m_nodes[i].MouseOver(Event.current.mousePosition))
                    {
                        handled = true;

                        break;
                    }
                }

                if (!handled)
                {
                    // Do something
                }

                Repaint();
            }
            else if (Event.current.type == EventType.MouseDown && m_currentMouseMode != MouseModes.CONNECTING)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if (m_nodes[i].MouseDown(Event.current.mousePosition, Event.current.button))
                        handled = true;
                }

                if (!handled && Event.current.button == 1)
                {
                    if (!isConnectionSelected)
                    {
                        CreateNodeMenu();
                    }
                    else
                    {
                        BreakConnectionMenu(connectionSelected.Inlet, connectionSelected.Outlet);
                    }
                }
                else if (!handled && Event.current.button == 0)
                {
                    m_startMousePos = Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDown && m_currentMouseMode == MouseModes.CONNECTING)
            {
                if (Event.current.button == 0)
                {
                    m_livePoints.Add(Event.current.mousePosition);
                    Repaint();
                }
                else if (Event.current.button == 1)
                {
                    m_currentMouseMode = MouseModes.IDLE;
                    m_livePoints.Clear();
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if (m_nodes[i].MouseDrag(Event.current.mousePosition))
                    {
                        handled = true;
                        break;
                    }
                }

                if (!handled)
                {
                    if (Event.current.shift)
                    {
                        Vector2 offset = Event.current.mousePosition - m_startMousePos;
                        for (int i = 0; i < m_nodes.Count; i++)
                        {
                            m_nodes[i].Position += offset;
                        }

                        Repaint();

                        m_startMousePos = Event.current.mousePosition;
                        handled = true;
                    }
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    m_nodes[i].MouseUp(Event.current.mousePosition);
                }
            }

            if (m_currentMouseMode == MouseModes.CONNECTING)
            {
                List<Vector2> points = new List<Vector2>();
                points.Add(m_startMousePos);
                for (int i = 0; i < m_livePoints.Count; i++)
                {
                    points.Add(m_livePoints[i]);
                }
                points.Add(Event.current.mousePosition);

                for (int i = 0; i < points.Count - 1; i++)
                {
                    DrawConnectingCurve(points[i], points[i + 1]);
                }

                Repaint();
            }

            List<BaseNode> nodesToDelete = new List<BaseNode>();
            foreach (BaseNode node in m_nodes)
            {
                if (!node.Valid)
                {
                    nodesToDelete.Add(node);
                }
            }

            foreach (BaseNode node in nodesToDelete)
            {
                m_nodes.Remove(node);

                node.BreakAllLets();

                // DestroyImmediate(node.gameObject);
            }

            if (nodesToDelete.Count > 0)
                Repaint();
        }

        void OnDestroy()
        {
            // Delete all listeners
            foreach (BaseNode simpleNode in m_nodes)
            {
                for (int i = 0; i < simpleNode.Lets.Count; i++)
                {
                    simpleNode.Lets[i].LetClicked -= OnLetPressed;
                    simpleNode.Lets[i].LetDrag -= OnLetDrag;
                    simpleNode.Lets[i].LetUp -= OnLetUp;
                }
            }
        }

        float MouseDistanceToLine(Vector2 start, Vector2 end)
        {
            return DistancePointLine(Event.current.mousePosition, start, end);
        }

        void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            Color guiColor = Handles.color;
            Handles.color = color;
            Handles.DrawLine(start, end);
            GUI.color = guiColor;
        }

        void DrawConnectingCurve(Vector2 start, Vector2 end)
        {
            /*
            Vector3 startPos = new Vector3(start.x, start.y, 0);
            Vector3 endPos = new Vector3(end.x, end.y, 0);

            Vector3 startTan = startPos + Vector3.left * 50;
            Vector3 endTan = endPos + Vector3.right * 50;

            if (m_currentSelectedLet.Type == LetTypes.OUTLET)
            {
                startTan = startPos + Vector3.right * 50;
                endTan = endPos + Vector3.left * 50;
            }
             Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.red, null, 4);
             */


            Color guiColor = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(start, end);
            GUI.color = guiColor;
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }

        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;
            if (magnitude > 1E-06f)
            {
                lhs = (Vector3)(lhs / magnitude);
            }
            float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + ((Vector3)(lhs * num2)));
        }

        private void Clear()
        {
            m_bgImg = null;
            m_describeLab = string.Empty;
            m_livePoints.Clear();
            m_currentMouseMode = MouseModes.IDLE;
            m_nodes.Clear();
        }

        public void CreateChatNodeByEditorData(EditorPersuadeData chatData)
        {
            OnDestroy();
            Clear();

            m_describeLab = chatData.m_describe;
            npcId = chatData.npcId;
            persuadeName = chatData.name;
            introduce = chatData.introduce;
            evidenceIds = string.Empty;
            for (int i = 0; i < chatData.evidenceIds.Length; i++)
            {
                evidenceIds += chatData.evidenceIds[i];
                if (i != chatData.evidenceIds.Length - 1)
                {
                    evidenceIds += ",";
                }
            }
            //evidenceIds = chat
            chatID = chatData.id;
            for (int i = 0; i < chatData.m_editorItems.Count; i++)
            {
                EditorPersuadeItemData chatItemData = chatData.m_editorItems[i];
                System.Type type = typeof(PersuadeBaseNode);
                PersuadeBaseNode contentNode = CreateNode(new Vector2(chatItemData.m_Position.x, chatItemData.m_Position.y), type, chatItemData.itemId) as PersuadeBaseNode;
                contentNode.m_talkType = (PersuadeTalkType)chatItemData.talkType;
                contentNode.m_type = (PersuadeType)chatItemData.persuadeType;
                contentNode.content = chatItemData.content;
                contentNode.evidenceID = chatItemData.evidenceID;
            }
            CreateConnectLine(chatData);
            Repaint();
        }

        private void CreateConnectLine(EditorPersuadeData chatData)
        {
            for (int i = 0; i < chatData.m_editorItems.Count; i++)
            {
                
                EditorPersuadeItemData chatItemData = chatData.m_editorItems[i];
                for (int j = 0; j < chatItemData.m_nextChatItemID.Count; j++)
                {
                    for (int k = 0; k < m_nodes.Count; k++)
                    {
                        if (m_nodes[k].Id == chatItemData.m_nextChatItemID[j].m_nextItemID)
                        {
                            Inlet inlet = (Inlet)m_nodes[k].Lets[0];
                            EditorVector2[] editorVec = chatItemData.m_nextChatItemID[j].m_connectPoint;
                            List<Vector2> linePoints = new List<Vector2>();
                            for (int m = editorVec.Length - 1; m >= 0; m--)
                            {
                                linePoints.Add(new Vector2(editorVec[m].x, editorVec[m].y));
                            }
                            Outlet outlet = (Outlet)m_nodes[i].Lets[1];
                            if ((m_nodes[k] as PersuadeBaseNode).m_type == PersuadeType.正向反馈)
                            {
                                outlet = (Outlet)m_nodes[i].Lets[2];
                            }
                            Connection connection = new Connection(inlet, outlet, linePoints);
                            inlet.Connections.Add(connection);

                            outlet.Connections.Add(connection);

                            break;
                        }
                    }
                    
                }

            }
        }
    }
}
