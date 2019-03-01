using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Utils
{
    public class ManifestTreeNode
    {
        private const string SPLIT = ":";
        public int level;
        
        public ManifestTreeNode left;
        public ManifestTreeNode right;
        public ManifestTreeNode parent;

        private string _content;
        public string nodeKey;
        public string nodeValue;
        public string Content
        {
            set
            {
                if (value.StartsWith("- "))
                    value = value.Replace("- ", "");
                if (value.StartsWith("\"") && value.EndsWith("\""))
                    value = value.Replace("\"", "");
                if (value.Contains("\\u"))
                {
                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    char[] charArray = value.ToCharArray();
                    char c = charArray[0];
                    while (i < value.Length)
                    {
                        c = charArray[i];
                        if (c == '\\' && charArray[i + 1] == 'u')
                        {
                            string content = value.Substring(i+2, 4);
                            sb.Append(((char)System.Convert.ToInt32(content, 16)).ToString());
                            i += 6;
                        }
                        else
                        {
                            sb.Append(c);
                            i++;
                        }
                    }
                    value = sb.ToString();
                }

                _content = value;
                if (_content.Contains(SPLIT) && !this.parent.nodeKey.Equals("Assets"))
                {
                    nodeKey = _content.Substring(0, _content.IndexOf(SPLIT)).Trim();
                    nodeValue = _content.Substring(_content.IndexOf(SPLIT) + 1).Trim();
                }
                else
                {
                    nodeKey = _content;
                    nodeValue = "";
                }
            }
            get { return _content; }
        }
        public ManifestTreeNode this[string name]
        {
            get
            {
                ManifestTreeNode node = this.left;
                while(node != null)
                {
                    if (node.nodeKey.Equals(name))
                        return node;

                    node = node.right;
                }
                return null;
            }
        }
        public List<ManifestTreeNode> Children
        {
            get
            {
                List<ManifestTreeNode> list = new List<ManifestTreeNode>();
                if (this.left != null)
                {
                    ManifestTreeNode node = this.left;
                    list.Add(node);
                    while (node.right != null)
                    {
                        node = node.right;
                        list.Add(node);
                    }
                }
                return list;
            }
        }
    }
    public class ManifestDoc
    {
        public ManifestTreeNode root;
        private string[] mAllLines;
        public ManifestDoc(string path)
        {
            root = new ManifestTreeNode();
            root.Content = "Root";
            root.level = -1;
            using (StreamReader sr = new StreamReader(path))
            {
                mAllLines = sr.ReadToEnd().Replace("\r\n","\n").Split('\n');
                int start = 0;
                root.left = ParseNode(ref start, root);
            }
        }

        #region Parse
        private ManifestTreeNode ParseNode(ref int readIndex,ManifestTreeNode root)
        {
            if (readIndex >= mAllLines.Length)
                return null;

            int treeLv = GetTreeLevel(mAllLines[readIndex]);
            if (treeLv < root.level)
                return null;

            ManifestTreeNode node = new ManifestTreeNode();
            node.level = treeLv;
            node.parent = root;
            node.Content = mAllLines[readIndex];

            int nextLv = NextLineLevel(readIndex + 1);
            while (nextLv != -1)
            {
                if (nextLv > node.level)
                {
                    readIndex++;
                    node.left = ParseNode(ref readIndex, node);
                }
                else if (nextLv == node.level)
                {
                    readIndex++;
                    node.right = ParseNode(ref readIndex, root);
                }
                else
                {
                    return node;
                }
                nextLv = NextLineLevel(readIndex + 1);
            }

            return node;
        }
        private int NextLineLevel(int index)
        {
            if (index >= mAllLines.Length)
                return -1;
            return GetTreeLevel(mAllLines[index]);
        }
        private int GetTreeLevel(string line)
        {
            int treeLevel = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                    treeLevel++;
                else
                    break;
            }
            treeLevel /= 2;
            if (line.StartsWith("- "))
                treeLevel += 1;
            return treeLevel;
        }
        #endregion


        
        #region Test
        private string mOutputTreeStr = "";
        public void PrintTree()
        {
            mOutputTreeStr = "";
            mOutputTreeStr = PrintTreeRecursive(root, 0);
            using (StreamWriter sw = new StreamWriter("G:\\tree.txt"))
            {
                sw.Write(mOutputTreeStr);
            }
        }
        private string PrintTreeRecursive(ManifestTreeNode root, int level)
        {
            string content = "";
            string tabString = "";
            for (int i = 0; i < level; i++)
                tabString += "  ";
            if (string.IsNullOrEmpty(root.nodeValue))
                content += string.Format("{0}{1}\n", tabString, root.nodeKey);
            else
                content += string.Format("{0}{1}={2}\n",tabString,root.nodeKey,root.nodeValue);

            if (root.left != null)
                content += PrintTreeRecursive(root.left, level + 1);
            if (root.right != null)
                content += PrintTreeRecursive(root.right, level);
            return content;
        }
        #endregion
    }
}
