using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

static class DirtWordService
{
    static DirtyWordNode root = new DirtyWordNode();

    public static void Initialize(string[] words)
    {
        foreach (var i in words)
        {
            AddDirtWord(i.Trim('\r'), root);
        }
    }
    public static void AddDirtWord(string word, DirtyWordNode root)
    {
        DirtyWordNode curNode = root;
        char[] arr = word.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            char c = arr[i];
            curNode = curNode.AppendChild(c, i == arr.Length - 1);
        }
    }

    public static bool HasDirtWord(this string str)
    {
        List<DirtyWordNode> m;
        ReplaceDirtWord(str, out m);
        return m.Count > 0;
    }
    public static string ReplaceDirtWord(this string str)
    {
        List<DirtyWordNode> m;
        return ReplaceDirtWord(str, out m);
    }

    public static string ReplaceDirtWord(this string str, out List<DirtyWordNode> match)
    {
        return ReplaceDirtWord(str, root, out match);
    }

    public static string ReplaceDirtWord(this string str, DirtyWordNode root, out List<DirtyWordNode> match)
    {
        match = new List<DirtyWordNode>();
        if (str == null)
            return null;
        char[] arr = str.ToCharArray();
        int curIdx = 0;
        int wordIdx = 0;
        StringBuilder sb = new StringBuilder();
        DirtyWordNode curNode = root;
        DirtyWordNode old = curNode;
        while (curIdx < str.Length)
        {
            char c = arr[curIdx + wordIdx];
            old = curNode;
            curNode = curNode[c];
            if (curNode != null)
            {
                wordIdx++;
                if (curNode.Terminated)
                {
                    DirtyWordNode newNode = new DirtyWordNode();
                    newNode.Word = curNode.Word;
                    newNode.Index = curIdx;
                    match.Add(newNode);
                    for (int i = 0; i < curNode.Word.Length; i++)
                    {
                        sb.Append('*');
                    }
                    curIdx += wordIdx;
                    wordIdx = 0;
                    curNode = root;
                }
                else if (curIdx + wordIdx >= str.Length)
                {
                    for (int i = 0; i < wordIdx; i++)
                        sb.Append(arr[curIdx + i]);
                    curIdx += wordIdx;
                }
            }
            else
            {
                if (old != null && old != root)
                {
                    if (old.CanTerminate)
                    {
                        DirtyWordNode newNode = new DirtyWordNode();
                        newNode.Word = curNode.Word;
                        newNode.Index = curIdx;
                        match.Add(newNode);
                        for (int i = 0; i < old.Word.Length; i++)
                        {
                            sb.Append('*');
                        }
                    }
                    else
                        sb.Append(old.Word);
                    curNode = root;
                    curIdx += wordIdx;
                    wordIdx = 0;
                    old = null;
                }
                else
                {
                    sb.Append(c);
                    curNode = root;
                    curIdx++;
                    wordIdx = 0;
                }
            }
        }
        if (old != null && old != root)
        {
            if (old.CanTerminate)
            {
                DirtyWordNode newNode = new DirtyWordNode();
                newNode.Word = curNode.Word;
                newNode.Index = curIdx;
                match.Add(newNode);
                for (int i = 0; i < old.Word.Length; i++)
                {
                    sb.Append('*');
                }
            }
            else
                sb.Append(old.Word);
            curNode = root;
            curIdx += wordIdx;
            wordIdx = 0;
            old = null;
        }
        return sb.ToString();
    }
}

class DirtyWordNode
{
    Dictionary<char, DirtyWordNode> children = new Dictionary<char, DirtyWordNode>();
    string word = "";
    public char Character { get; set; }

    public bool CanTerminate { get; set; }

    public int Index { get; set; }
    public string Word { get { return word; } set { word = value; } }

    public bool Terminated { get { return children.Count == 0; } }

    public DirtyWordNode this[char c]
    {
        get
        {
            DirtyWordNode res;
            if (children.TryGetValue(c, out res))
                return res;
            else
                return null;
        }
    }

    public DirtyWordNode AppendChild(char c, bool canTerminate)
    {
        if (!children.ContainsKey(c))
        {
            DirtyWordNode node = new DirtyWordNode();
            node.Character = c;
            node.Word = word + c;
            node.CanTerminate = canTerminate;
            children.Add(c, node);
            return node;
        }
        else
            return children[c];
    }

}