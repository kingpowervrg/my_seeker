using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{
    class AdvancedTextImage : MaskableGraphic
    {
        BetterList<UIVertex> vertexStream = new BetterList<UIVertex>();
        Texture tex;
        bool isDirty;
        public Texture ImageTexture
        {
            get { return tex; }
            set
            {
                if (tex != value)
                {
                    tex = value;
                    SetMaterialDirty();
                    SetVerticesDirty();
                }
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return tex;
            }
        }
        public BetterList<UIVertex> ImageVertices
        {
            set
            {
                vertexStream = value;
                isDirty = true;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            AdvancedText.FillUIQuad(vh, vertexStream);
        }

        void Update()
        {
            if (isDirty)
            {
                SetVerticesDirty();
                isDirty = false;
            }
        }
    }
    /// <summary>
    /// 图文混排
    /// </summary>
    [AddComponentMenu("UI/Advanced Text", 11)]
    public class AdvancedText : Text, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        VertexHelper _toFill;
        [SerializeField]
        ImageFont m_ImageFont;
        [SerializeField]
        string originalText;
        [SerializeField]
        float imageSize = 0;
        [SerializeField]
        bool isPureEmoji;
        [SerializeField]
        bool raycastTarget;
        [SerializeField]
        string m_prefixFlag = string.Empty;
        [SerializeField]
        string m_postfixFlag = string.Empty;

        [System.NonSerialized]
        AdvancedTextImage m_CachedInputRenderer;
        [System.NonSerialized]
        RectTransform caretRectTrans;
        //[System.NonSerialized]
        //UIVertex[] m_TempVerts = new UIVertex[4];
        [System.NonSerialized]
        ReplacementInfo info = default(ReplacementInfo);
        [System.NonSerialized]
        RectTransform cachedTransform;

        private HrefText m_hrefText;
        bool isTextDirty = true;
        BetterList<UIVertex> vertImg = new BetterList<UIVertex>();
        BetterList<UIVertex> vertTxt = new BetterList<UIVertex>();
        Dictionary<int, List<UIVertex>> vertCache;

        private Action<string> m_hrefClickEvent;
        Vector2 lastExtents;
        bool isVerticisDirty = false;
        RectTransform CachedRectTransform
        {
            get
            {
                if (!cachedTransform)
                    cachedTransform = gameObject.GetComponent<RectTransform>();

                return cachedTransform;
            }
        }

        public ImageFont ImageFont
        {
            get { return m_ImageFont; }
            set
            {
                m_ImageFont = value;
                isTextDirty = true;
                SetVerticesDirty();
                SetLayoutDirty();
                //引擎里面m_Text未赋值就已经布局了, 在赋值的时候重新调用一次
                GenerateText();
                CheckImage();
            }
        }

        public float ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                isTextDirty = true;
                SetVerticesDirty();
                SetLayoutDirty();
                //引擎里面m_Text未赋值就已经布局了, 在赋值的时候重新调用一次
                GenerateText();
                CheckImage();
            }
        }

        public bool IgnoreVertexCache { get; set; }

        public override string text
        {
            get
            {
                return originalText;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(originalText))
                        return;
                    originalText = "";
                    isTextDirty = true;
                    SetVerticesDirty();
                    SetLayoutDirty();
                    GenerateText();
                }
                else if (originalText != value)
                {
                    originalText = value;

                    if (!string.IsNullOrEmpty(m_prefixFlag))
                        originalText = m_prefixFlag + value;

                    if (!string.IsNullOrEmpty(m_postfixFlag))
                        originalText = originalText + m_postfixFlag;

                    isTextDirty = true;
                    SetVerticesDirty();
                    SetLayoutDirty();
                    GenerateText();
                }
            }
        }

        public bool RayCastTarget
        {
            get { return raycastTarget; }
            set
            {
                raycastTarget = value;
                CheckImage();
                if (m_CachedInputRenderer)
                {
                    m_CachedInputRenderer.raycastTarget = value;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            isTextDirty = true;
            CheckImage();
        }

        void CheckImage()
        {
            if (Application.isPlaying)
            {
                if (m_CachedInputRenderer == null)
                {
                    GameObject go = new GameObject("Images");
                    go.hideFlags = HideFlags.DontSave;
                    go.transform.SetParent(transform);
                    go.transform.SetAsFirstSibling();
                    go.layer = gameObject.layer;
                    caretRectTrans = go.AddComponent<RectTransform>();
                    caretRectTrans.anchorMin = Vector2.zero;
                    caretRectTrans.anchorMax = Vector2.one;
                    caretRectTrans.pivot = CachedRectTransform.pivot;
                    caretRectTrans.anchoredPosition3D = Vector3.zero;
                    caretRectTrans.sizeDelta = Vector2.zero;
                    caretRectTrans.localScale = Vector2.one;
                    m_CachedInputRenderer = go.AddComponent<AdvancedTextImage>();
                    m_CachedInputRenderer.raycastTarget = raycastTarget;

                    // Needed as if any layout is present we want the caret to always be the same as the text area.
                    //go.AddComponent<LayoutElement>().ignoreLayout = true;
                }
                base.raycastTarget = false;
                if (m_ImageFont != null)
                {
                    m_CachedInputRenderer.ImageTexture = m_ImageFont.Texture;
                }
            }
        }
        void GenerateText()
        {
            bool regenText = isTextDirty || lastExtents != rectTransform.rect.size;
            if (isTextDirty)
            {
                isTextDirty = false;
                if (m_hrefText == null)
                {
                    m_hrefText = new HrefText();
                }
                if (info.OriginalText != originalText)
                {
                    m_hrefText.getHrefList.Clear();
                    string m_outText = m_hrefText.getResolveText(originalText);

                    if (m_ImageFont)
                        info = m_ImageFont.ReplaceText(m_outText, imageSize);
                    else
                    {
                        return;
                    }
                }
                m_Text = info.ReplacedText;
            }
            if (info.Symbols == null)
                return;
            if (regenText)
            {
                lastExtents = rectTransform.rect.size;
                List<UIVertex> textVertList = null;
                if (!isPureEmoji || IgnoreVertexCache || vertCache == null || !vertCache.TryGetValue(info.Symbols.Length, out textVertList))
                {
                    textVertList = new List<UIVertex>();
                    OnTextFillVBO(textVertList);

                    // 处理超链接包含的点击区域
                    foreach (var hrefInfo in m_hrefText.getHrefList)
                    {
                        hrefInfo.boxes.Clear();
                        if (hrefInfo.startIndex >= textVertList.Count)
                        {
                            continue;
                        }

                        // 将超链接里面的文本顶点索引坐标加入到点击区域内
                        var pos = textVertList[hrefInfo.startIndex];
                        var bounds = new Bounds(pos.position, Vector3.zero);
                        for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
                        {
                            if (i >= textVertList.Count)
                            {
                                break;
                            }

                            pos = textVertList[i];
                            if (pos.position.x < bounds.min.x) // 换行重新添加点击区域
                            {
                                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                                bounds = new Bounds(pos.position, Vector3.zero);
                            }
                            else
                            {
                                bounds.Encapsulate(pos.position); //再次扩展范围
                            }
                        }
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                    }

                    if (isPureEmoji && !IgnoreVertexCache)
                    {
                        if (vertCache == null)
                            vertCache = new Dictionary<int, List<UIVertex>>();
                        vertCache[info.Symbols.Length] = textVertList;
                    }
                }
                Vector3 offset = new Vector3(0, -fontSize / 4f, 0);
                int newIdx = 0;
                int skip = 0;
#if !UNITY_5_2
                int totalSkip = 0;
#endif
                SymbolSpriteInfo lastSprite = default(SymbolSpriteInfo);
                int counter = 0;
                bool isLastSkip = false;
                vertImg.Clear();
                vertTxt.Clear();

                for (int i = 0; i < textVertList.Count; i++)
                {
                    if (skip > 0)
                    {
#if UNITY_5_2
                        if (skip <= 4 && skip > 0)
#else
                        int processed = totalSkip - skip;
                        if (processed < 4)
#endif
                        {
                            UIVertex vert = new UIVertex();
                            UIVertex old = textVertList[i];
#if UNITY_5_2
                            int idx = 4 - skip;
#else
                            int idx = processed;
#endif
                            vert.position = old.position + offset;
                            vert.uv0 = lastSprite.UV[idx];
                            vert.normal = old.normal;
                            vert.tangent = old.tangent;
                            Color32 col = new Color32(255, 255, 255, 255);
                            col.a = old.color.a;
                            vert.color = col;
                            vertImg.Add(vert);
                            /*m_TempVerts[idx] = vert;
                            if (idx == 3)
                            {                                
                                vhImg.AddUIVertexQuad(m_TempVerts);
                            }*/
                            counter++;
                        }
                        skip--;
                        continue;
                    }
                    if (newIdx < info.Symbols.Length)
                    {
                        int idx = counter / 4;
                        lastSprite = info.Symbols[newIdx];
                        if (idx == lastSprite.Index)
                        {
#if UNITY_5_2
                            skip = info.ReplacedSpriteLength * 4 - 1;
#else
                            totalSkip = info.ReplacedSpriteLength * 4;
                            i--;
                            skip = totalSkip;
#endif
                            newIdx++;
                            continue;
                        }
                    }
                    int tmpIdx = counter % 4;
                    if (tmpIdx == 0)
                    {
                        //filter junk vertices
                        if (textVertList[i].position == textVertList[i + 1].position)
                        {
                            isLastSkip = true;
                        }
                        else
                            isLastSkip = false;
                    }
                    if (!isLastSkip)
                        vertTxt.Add(textVertList[i]);
                    /*m_TempVerts[tmpIdx] = textVertList[i];
                    if (tmpIdx == 3)
                    {
                        if (_toFill != null)
                        {
                            _toFill.AddUIVertexQuad(m_TempVerts);
                        }
                    }*/
                    counter++;
                }
            }
            else
            {
                Color32 col = color;
                for (int i = 0; i < vertImg.size; i++)
                {
                    var vert = vertImg[i];
                    vert.color = new Color32(255, 255, 255, col.a);
                    vertImg[i] = vert;
                }
                for (int i = 0; i < vertTxt.size; i++)
                {
                    var vert = vertTxt[i];
                    vert.color = col;
                    vertTxt[i] = vert;
                }
            }
            if (_toFill != null)
                FillUIQuad(_toFill, vertTxt);
            if (m_CachedInputRenderer)
                m_CachedInputRenderer.ImageVertices = vertImg;
        }

        internal static void FillUIQuad(VertexHelper vh, BetterList<UIVertex> uivert)
        {
            int startIndex = 0;
            for (int i = 0; i < uivert.size; i++)
            {
                if (i % 4 == 0)
                {
                    startIndex = ((i / 4) - 1) * 4;
                    if (startIndex >= 0)
                    {
                        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                    }
                }
                vh.AddVert(uivert[i]);
            }

            //last quad
            startIndex = ((uivert.size / 4) - 1) * 4;
            if (startIndex >= 0)
            {
                vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            isVerticisDirty = false;
            _toFill = toFill;
            _toFill.Clear();
            GenerateText();
        }

        void OnTextFillVBO(List<UIVertex> vbo)
        {
            if (font == null)
                return;

            // We dont care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(m_Text, settings);

            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
            refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Pre-allocate Memory
            vbo.Capacity = verts.Count;
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    UIVertex uiv = verts[i];
                    uiv.position *= unitsPerPixel;
                    uiv.position.x += roundingOffset.x;
                    uiv.position.y += roundingOffset.y;
                    vbo.Add(uiv);
                }
            }
            else
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    UIVertex uiv = verts[i];
                    uiv.position *= unitsPerPixel;
                    vbo.Add(uiv);
                }
            }
            m_DisableFontTextureRebuiltCallback = false;
        }

        protected override void UpdateGeometry()
        {
            //字体重构的时候会调用此方法，需要重新生成文字
            if (!isVerticisDirty)
                isTextDirty = true;
            cachedTextGenerator.Invalidate();
            base.UpdateGeometry();

        }
        public override void SetAllDirty()
        {
            //字体重构的时候会调用此方法，需要重新生成文字
            isTextDirty = true;

            cachedTextGenerator.Invalidate();
            if (m_CachedInputRenderer != null)
            {
                m_CachedInputRenderer.SetVerticesDirty();
            }
            base.SetAllDirty();
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

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            isVerticisDirty = true;
            if (m_CachedInputRenderer != null)
            {
                m_CachedInputRenderer.SetVerticesDirty();
            }
        }

        public override void SetLayoutDirty()
        {
            base.SetLayoutDirty();
            isVerticisDirty = true;
            if (m_CachedInputRenderer != null)
            {
                m_CachedInputRenderer.SetLayoutDirty();
            }
        }
        public Action<string> HrefClickEvent
        {
            set
            {
                m_hrefClickEvent = value;
            }
            get
            {
                return m_hrefClickEvent;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out pos);

            foreach (var hrefInfo in m_hrefText.getHrefList)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.size; ++i)
                {
                    if (boxes[i].Contains(pos))
                    {
                        //if (HrefClickEvent != null)
                        //    HrefClickEvent.SafeInvoke(hrefInfo.name);
                        return;
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}