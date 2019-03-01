using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{
    [System.Serializable]
    public class SymbolData
    {
        [SerializeField]
        Sprite sprite;
        [SerializeField]
        string value;

        public Sprite Sprite
        {
            get
            { return sprite; }
            set
            {
                sprite = value;
            }
        }
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }

    public struct SymbolSpriteInfo
    {
        public Vector2[] UV { get; set; }

        public int Index { get; set; }

    }

    public struct ReplacementInfo
    {
        public SymbolSpriteInfo[] Symbols { get; set; }
        public string ReplacedText { get; set; }

        public int ReplacedSpriteLength { get; set; }

        public string OriginalText { get; set; }
    }
    /// <summary>
    /// 图文混排
    /// </summary>
    public class ImageFont : MonoBehaviour
    {
        [SerializeField]
        SymbolData[] data = new SymbolData[0];

        [System.NonSerialized]
        bool needRebuild;
        [System.NonSerialized]
        DirtyWordNode rootNode;
        [System.NonSerialized]
        Dictionary<string, Sprite> mapping;
        [System.NonSerialized]
        Texture tex;
        public SymbolData[] SymbolData { get { return data; } set { data = value; needRebuild = true; } }

        public Texture Texture
        {
            get
            {
                if (tex == null)
                {
                    if (data != null && data.Length > 0)
                        tex = data[0].Sprite.texture;
                }
                return tex;
            }
        }

        void OnEnable()
        {
            needRebuild = true;
        }
        public ReplacementInfo ReplaceText(string text, float imgSize = 0f)
        {
            if (needRebuild || rootNode == null)
            {
                RebuildMapping();
            }

            ReplacementInfo res = new ReplacementInfo();
            res.OriginalText = text;
            List<DirtyWordNode> replacement;
            text.ReplaceDirtWord(rootNode, out replacement);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            res.Symbols = new SymbolSpriteInfo[replacement.Count];

            int curIdx = 0;
            int newIdx = 0;
            int appendLength = 0;
            int idx = 0;
            foreach (var i in replacement)
            {
                SymbolSpriteInfo info = new SymbolSpriteInfo();

                sb.Append(text.Substring(curIdx, i.Index - curIdx));
                int prefixLength = 0;
                if (imgSize != 0)
                {
                    int b = sb.Length;
                    sb.Append("<size=");
                    sb.Append(imgSize);
                    sb.Append('>');
                    prefixLength = sb.Length - b;
                }
                appendLength += prefixLength;
                newIdx += i.Index - curIdx + prefixLength;
                info.Index = newIdx;
                newIdx++;

                Sprite sp = mapping[i.Word];
                info.UV = CalcUV(sp);

                Rect r = sp.rect;
                float ratio = r.width / r.height;
                string rep = string.Format("<quad width={0:0.00}>", ratio);
                res.ReplacedSpriteLength = rep.Length;
                sb.Append(rep);
                curIdx = i.Index + i.Word.Length;
                res.Symbols[idx++] = info;
                if (imgSize != 0)
                {
                    sb.Append("</size>");
                    appendLength += 7;
                    newIdx += 7;
                }
            }
            sb.Append(text.Substring(curIdx));
            res.ReplacedText = sb.ToString();

            return res;
        }

        void RebuildMapping()
        {
            needRebuild = false;
            mapping = new Dictionary<string, Sprite>();
            rootNode = new DirtyWordNode();
            if (data != null)
            {
                foreach (var i in data)
                {
                    mapping[i.Value] = i.Sprite;
                    DirtWordService.AddDirtWord(i.Value, rootNode);
                }
            }
        }

        Vector2[] CalcUV(Sprite sprite)
        {
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var i in sprite.uv)
            {
                if (i.x < minX)
                    minX = i.x;
                if (i.x > maxX)
                    maxX = i.x;
                if (i.y < minY)
                    minY = i.y;
                if (i.y > maxY)
                    maxY = i.y;
            }
            return new Vector2[4]{
                new Vector2(minX,maxY),
                new Vector2(maxX,maxY),
                new Vector2(maxX,minY),
                new Vector2(minX,minY)
            };
        }
    }
}