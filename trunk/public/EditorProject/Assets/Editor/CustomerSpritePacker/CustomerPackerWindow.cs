using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;


public class CustomerPackerWindow : EditorWindow
{
    protected const float k_ToolbarHeight = 17f;
    protected const float kSpacing = 5;
    protected const float k_ScrollbarMargin = 16f;
    protected const float kSliderMinW = 60;
    protected const float k_MaxZoom = 50f;
    protected const float kObjectFieldMiniThumbnailWidth = 32f;
    protected const float k_MinZoomPercentage = 0.9f;
    protected const float k_MouseZoomSpeed = 0.005f;
    protected const float k_WheelZoomSpeed = 0.03f;


    protected Texture2D m_Texture;
    protected Texture2D m_TextureAlphaOverride;
    protected float m_Zoom = -1f;
    protected Styles m_Styles;
    protected float m_MipLevel = 0;
    protected Rect m_TextureRect;
    protected Rect m_TextureViewRect;
    protected bool m_ShowAlpha = false;
    protected Vector2 m_ScrollPosition = new Vector2();


    protected Rect maxScrollRect
    {
        get
        {
            float halfWidth = m_Texture.width * .5f * m_Zoom;
            float halfHeight = m_Texture.height * .5f * m_Zoom;
            return new Rect(-halfWidth, -halfHeight, m_TextureViewRect.width + halfWidth * 2f, m_TextureViewRect.height + halfHeight * 2f);
        }
    }

    private class PackerWindowStyle
    {
        public static readonly GUIContent packLabel = new GUIContent("Pack");
        public static readonly GUIContent repackLabel = new GUIContent("Repack");
        public static readonly GUIContent viewAtlasLabel = new GUIContent("View Atlas:");
        public static readonly GUIContent windowTitle = new GUIContent("Sprite Packer");
        public static readonly GUIContent pageContentLabel = new GUIContent("Page {0}");
        public static readonly GUIContent packingDisabledLabel = new GUIContent("Legacy sprite packing is disabled. Enable it in Edit > Project Settings > Editor.");
        public static readonly GUIContent openProjectSettingButton = new GUIContent("Open Project Editor Settings");
    }

    struct Edge
    {
        public UInt16 v0;
        public UInt16 v1;
        public Edge(UInt16 a, UInt16 b)
        {
            v0 = a;
            v1 = b;
        }

        public override bool Equals(object obj)
        {
            Edge item = (Edge)obj;
            return (v0 == item.v0 && v1 == item.v1) || (v0 == item.v1 && v1 == item.v0);
        }

        public override int GetHashCode()
        {
            return (v0 << 16 | v1) ^ (v1 << 16 | v0).GetHashCode();
        }
    };

    private static string[] s_AtlasNamesEmpty = new string[1] { "Sprite atlas cache is empty" };
    private string[] m_AtlasNames = s_AtlasNamesEmpty;
    private int m_SelectedAtlas = 0;

    private static string[] s_PageNamesEmpty = new string[0];
    private string[] m_PageNames = s_PageNamesEmpty;
    private int m_SelectedPage = 0;

    private Sprite m_SelectedSprite = null;

    [MenuItem("Tools/自定义sprite packer")]
    static void CreateWindow()
    {
        Rect rect = new Rect(Screen.width >> 1, Screen.height >> 1, Screen.width, Screen.height);
        CustomerPackerWindow window = (CustomerPackerWindow)EditorWindow.GetWindowWithRect(typeof(CustomerPackerWindow), rect, true, "自定义sprite packer");
        //window.Init();
        window.Show();
    }

    void OnEnable()
    {
        minSize = new Vector2(400f, 256f);
        titleContent = PackerWindowStyle.windowTitle;

        Reset();
    }

    private void Reset()
    {
        RefreshAtlasNameList();
        RefreshAtlasPageList();

        m_SelectedAtlas = 0;
        m_SelectedPage = 0;
        m_SelectedSprite = null;
    }

    private void RefreshAtlasNameList()
    {
        m_AtlasNames = Packer.atlasNames;

        // Validate
        if (m_SelectedAtlas >= m_AtlasNames.Length)
            m_SelectedAtlas = 0;
    }

    private void RefreshAtlasPageList()
    {
        if (m_AtlasNames.Length > 0)
        {
            string atlas = m_AtlasNames[m_SelectedAtlas];
            Texture2D[] textures = Packer.GetTexturesForAtlas(atlas);
            m_PageNames = new string[textures.Length];
            for (int i = 0; i < textures.Length; ++i)
                m_PageNames[i] = string.Format(PackerWindowStyle.pageContentLabel.text, i + 1);
        }
        else
        {
            m_PageNames = s_PageNamesEmpty;
        }

        // Validate
        if (m_SelectedPage >= m_PageNames.Length)
            m_SelectedPage = 0;
    }

    private void OnAtlasNameListChanged()
    {
        if (m_AtlasNames.Length > 0)
        {
            string[] atlasNames = Packer.atlasNames;
            string curAtlasName = m_AtlasNames[m_SelectedAtlas];
            string newAtlasName = (atlasNames.Length <= m_SelectedAtlas) ? null : atlasNames[m_SelectedAtlas];
            if (curAtlasName.Equals(newAtlasName))
            {
                RefreshAtlasNameList();
                RefreshAtlasPageList();
                m_SelectedSprite = null;
                return;
            }
        }

        Reset();
    }

    private bool ValidateIsPackingEnabled()
    {
        if (EditorSettings.spritePackerMode != SpritePackerMode.BuildTimeOnly
            && EditorSettings.spritePackerMode != SpritePackerMode.AlwaysOn)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label(PackerWindowStyle.packingDisabledLabel);
            if (GUILayout.Button(PackerWindowStyle.openProjectSettingButton))
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
            EditorGUILayout.EndVertical();
            return false;
        }

        return true;
    }

    private Rect DoToolbarGUI()
    {
        Rect toolbarRect = new Rect(0, 0, position.width, k_ToolbarHeight);

        if (Event.current.type == EventType.Repaint)
        {
            EditorStyles.toolbar.Draw(toolbarRect, false, false, false, false);
        }

        bool wasEnabled = GUI.enabled;
        GUI.enabled = m_AtlasNames.Length > 0;
        toolbarRect = DoAlphaZoomToolbarGUI(toolbarRect);
        GUI.enabled = wasEnabled;

        Rect drawRect = new Rect(kSpacing, 0, 0, k_ToolbarHeight);
        toolbarRect.width -= drawRect.x;

        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.packLabel).x;
            DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawRect) =>
            {
                if (GUI.Button(adjustedDrawRect, PackerWindowStyle.packLabel, EditorStyles.toolbarButton))
                {
                    Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
                    m_SelectedSprite = null;
                    RefreshAtlasPageList();
                    RefreshState();
                }
            });

            using (new EditorGUI.DisabledScope(Packer.SelectedPolicy == Packer.kDefaultPolicy))
            {
                drawRect.x += drawRect.width;
                drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.repackLabel).x;
                DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawRect) =>
                {
                    if (GUI.Button(adjustedDrawRect, PackerWindowStyle.repackLabel, EditorStyles.toolbarButton))
                    {
                        Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true, Packer.Execution.ForceRegroup);
                        m_SelectedSprite = null;
                        RefreshAtlasPageList();
                        RefreshState();
                    }
                });
            }
        }

        const float kAtlasNameWidth = 100;
        const float kPagesWidth = 70;
        const float kPolicyWidth = 100;

        float viewAtlasWidth = GUI.skin.label.CalcSize(PackerWindowStyle.viewAtlasLabel).x;
        float totalWidth = viewAtlasWidth + kAtlasNameWidth + kPagesWidth + kPolicyWidth;

        drawRect.x += kSpacing; // leave some space from previous control for cosmetic
        toolbarRect.width -= kSpacing;
        float availableWidth = toolbarRect.width;

        using (new EditorGUI.DisabledScope(m_AtlasNames.Length == 0))
        {
            drawRect.x += drawRect.width;
            drawRect.width = viewAtlasWidth / totalWidth * availableWidth;
            DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawArea) =>
            {
                GUI.Label(adjustedDrawArea, PackerWindowStyle.viewAtlasLabel);
            });

            EditorGUI.BeginChangeCheck();
            drawRect.x += drawRect.width;
            drawRect.width = kAtlasNameWidth / totalWidth * availableWidth;
            DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawArea) =>
            {
                m_SelectedAtlas = EditorGUI.Popup(adjustedDrawArea, m_SelectedAtlas, m_AtlasNames, EditorStyles.toolbarPopup);
            });
            if (EditorGUI.EndChangeCheck())
            {
                RefreshAtlasPageList();
                m_SelectedSprite = null;
            }

            EditorGUI.BeginChangeCheck();
            drawRect.x += drawRect.width;
            drawRect.width = kPagesWidth / totalWidth * availableWidth;
            DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawArea) =>
            {
                m_SelectedPage = EditorGUI.Popup(adjustedDrawArea, m_SelectedPage, m_PageNames, EditorStyles.toolbarPopup);
            });

            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedSprite = null;
            }
        }

        EditorGUI.BeginChangeCheck();
        string[] policies = Packer.Policies;
        int selectedPolicy = Array.IndexOf(policies, Packer.SelectedPolicy);
        drawRect.x += drawRect.width;
        drawRect.width = kPolicyWidth / totalWidth * availableWidth;
        DrawToolBarWidget(ref drawRect, ref toolbarRect, (adjustedDrawArea) =>
        {
            selectedPolicy = EditorGUI.Popup(adjustedDrawArea, selectedPolicy, policies, EditorStyles.toolbarPopup);
        });

        if (EditorGUI.EndChangeCheck())
        {
            Packer.SelectedPolicy = policies[selectedPolicy];
        }

        return toolbarRect;
    }

    void OnSelectionChange()
    {
        if (Selection.activeObject == null)
            return;

        Sprite selectedSprite = Selection.activeObject as Sprite;
        if (selectedSprite != m_SelectedSprite)
        {
            if (selectedSprite != null)
            {
                string selAtlasName;
                Texture2D selAtlasTexture;
                Packer.GetAtlasDataForSprite(selectedSprite, out selAtlasName, out selAtlasTexture);

                int selAtlasIndex = m_AtlasNames.ToList().FindIndex(delegate (string s) { return selAtlasName == s; });
                if (selAtlasIndex == -1)
                    return;
                int selAtlasPage = Packer.GetTexturesForAtlas(selAtlasName).ToList().FindIndex(delegate (Texture2D t) { return selAtlasTexture == t; });
                if (selAtlasPage == -1)
                    return;

                m_SelectedAtlas = selAtlasIndex;
                m_SelectedPage = selAtlasPage;
                RefreshAtlasPageList();
            }

            m_SelectedSprite = selectedSprite;

            Repaint();
        }
    }

    private void RefreshState()
    {
        // Check if atlas name list changed
        string[] atlasNames = Packer.atlasNames;
        if (!atlasNames.SequenceEqual(m_AtlasNames))
        {
            if (atlasNames.Length == 0)
            {
                Reset();
                return;
            }
            else
            {
                OnAtlasNameListChanged();
            }
        }

        if (m_AtlasNames.Length == 0)
        {
            SetNewTexture(null);
            return;
        }

        // Validate selections
        if (m_SelectedAtlas >= m_AtlasNames.Length)
            m_SelectedAtlas = 0;
        string curAtlasName = m_AtlasNames[m_SelectedAtlas];

        Texture2D[] textures = Packer.GetTexturesForAtlas(curAtlasName);
        if (m_SelectedPage >= textures.Length)
            m_SelectedPage = 0;

        SetNewTexture(textures[m_SelectedPage]);

        // check if the atlas has alpha as an external texture (as in ETC1 atlases with alpha)
        Texture2D[] alphaTextures = Packer.GetAlphaTexturesForAtlas(curAtlasName);
        Texture2D selectedAlphaTexture = (m_SelectedPage < alphaTextures.Length) ? alphaTextures[m_SelectedPage] : null;
        SetAlphaTextureOverride(selectedAlphaTexture);
    }

    public void OnGUI()
    {
        if (!ValidateIsPackingEnabled())
            return;

        Matrix4x4 oldHandlesMatrix = Handles.matrix;
        InitStyles();

        RefreshState();

        // Top menu bar
        Rect toolbarRect = DoToolbarGUI();

        if (m_Texture == null)
            return;

        // Texture view
        EditorGUILayout.BeginHorizontal();
        m_TextureViewRect = new Rect(0f, toolbarRect.yMax, position.width - k_ScrollbarMargin, position.height - k_ScrollbarMargin - toolbarRect.height);
        GUILayout.FlexibleSpace();
        DoTextureGUI();
        //string info = string.Format("{1}x{2}, {0}", TextureUtil.GetTextureFormatString(m_Texture.format), m_Texture.width, m_Texture.height);
        //EditorGUI.DropShadowLabel(new Rect(m_TextureViewRect.x, m_TextureViewRect.y + 10, m_TextureViewRect.width, 20), info);
        EditorGUILayout.EndHorizontal();

        Handles.matrix = oldHandlesMatrix;
    }

    //private void DrawLineUtility(Vector2 from, Vector2 to)
    //{
    //    SpriteEditorUtility.DrawLine(new Vector3(from.x * m_Texture.width + 1f / m_Zoom, from.y * m_Texture.height + 1f / m_Zoom, 0.0f), new Vector3(to.x * m_Texture.width + 1f / m_Zoom, to.y * m_Texture.height + 1f / m_Zoom, 0.0f));
    //}

    private Edge[] FindUniqueEdges(UInt16[] indices)
    {
        Edge[] allEdges = new Edge[indices.Length];
        int tris = indices.Length / 3;
        for (int i = 0; i < tris; ++i)
        {
            allEdges[i * 3] = new Edge(indices[i * 3], indices[i * 3 + 1]);
            allEdges[i * 3 + 1] = new Edge(indices[i * 3 + 1], indices[i * 3 + 2]);
            allEdges[i * 3 + 2] = new Edge(indices[i * 3 + 2], indices[i * 3]);
        }

        Edge[] uniqueEdges = allEdges.GroupBy(x => x).Where(x => x.Count() == 1).Select(x => x.First()).ToArray();
        return uniqueEdges;
    }

    //protected  void DrawGizmos()
    //{
    //    if (m_SelectedSprite != null && m_Texture != null)
    //    {
    //        Vector2[] uvs = SpriteUtility.GetSpriteUVs(m_SelectedSprite, true);
    //        UInt16[] indices = m_SelectedSprite.triangles;
    //        Edge[] uniqueEdges = FindUniqueEdges(indices); // Assumes that our mesh has no duplicate vertices

    //        SpriteEditorUtility.BeginLines(new Color(0.3921f, 0.5843f, 0.9294f, 0.75f)); // Cornflower blue :)
    //        foreach (Edge e in uniqueEdges)
    //            DrawLineUtility(uvs[e.v0], uvs[e.v1]);
    //        SpriteEditorUtility.EndLines();
    //    }
    //}


    protected Rect DoAlphaZoomToolbarGUI(Rect area)
    {
        int mipCount = 1;
        if (m_Texture != null)
            mipCount = Mathf.Max(mipCount, 0 /*TextureUtil.GetMipmapCount(m_Texture)*/);

        Rect drawArea = new Rect(area.width, 0, 0, area.height);
        using (new EditorGUI.DisabledScope(mipCount == 1))
        {
            drawArea.width = m_Styles.largeMip.image.width;
            drawArea.x -= drawArea.width;
            GUI.Box(drawArea, m_Styles.largeMip, m_Styles.preLabel);

            drawArea.width = kSliderMinW;
            drawArea.x -= drawArea.width;
            m_MipLevel = Mathf.Round(GUI.HorizontalSlider(drawArea, m_MipLevel, mipCount - 1, 0, m_Styles.preSlider, m_Styles.preSliderThumb));

            drawArea.width = m_Styles.smallMip.image.width;
            drawArea.x -= drawArea.width;
            GUI.Box(drawArea, m_Styles.smallMip, m_Styles.preLabel);
        }

        drawArea.width = kSliderMinW;
        drawArea.x -= drawArea.width;
        m_Zoom = GUI.HorizontalSlider(drawArea, m_Zoom, GetMinZoom(), k_MaxZoom, m_Styles.preSlider, m_Styles.preSliderThumb);

        drawArea.width = kObjectFieldMiniThumbnailWidth;
        drawArea.x -= drawArea.width + kSpacing;
        m_ShowAlpha = GUI.Toggle(drawArea, m_ShowAlpha, m_ShowAlpha ? m_Styles.alphaIcon : m_Styles.RGBIcon, "toolbarButton");

        // Returns the area that is not used
        return new Rect(area.x, area.y, drawArea.x, area.height);

    }


    static void DrawToolBarWidget(ref Rect drawRect, ref Rect toolbarRect, Action<Rect> drawAction)
    {
        toolbarRect.width -= drawRect.width;
        if (toolbarRect.width < 0)
            drawRect.width += toolbarRect.width;

        if (drawRect.width > 0)
            drawAction(drawRect);
    }

    protected void SetNewTexture(Texture2D texture)
    {
        if (texture != m_Texture)
        {
            m_Texture = texture;
            m_Zoom = -1;
            m_TextureAlphaOverride = null;
        }
    }

    protected void SetAlphaTextureOverride(Texture2D alphaTexture)
    {
        if (alphaTexture != m_TextureAlphaOverride)
        {
            m_TextureAlphaOverride = alphaTexture;
            m_Zoom = -1;
        }
    }

    protected void InitStyles()
    {
        if (m_Styles == null)
            m_Styles = new Styles();
    }


    protected void DoTextureGUI()
    {
        if (m_Texture == null)
            return;

        // zoom startup init
        if (m_Zoom < 0f)
            m_Zoom = GetMinZoom();

        // Texture rect in view space
        m_TextureRect = new Rect(
                m_TextureViewRect.width / 2f - (m_Texture.width * m_Zoom / 2f),
                m_TextureViewRect.height / 2f - (m_Texture.height * m_Zoom / 2f),
                (m_Texture.width * m_Zoom),
                (m_Texture.height * m_Zoom)
                );

        HandleScrollbars();
        SetupHandlesMatrix();
        HandleZoom();
        HandlePanning();
        DrawScreenspaceBackground();
        if (Event.current.type == EventType.Repaint)
        {
            Vector2 cur_pos = m_TextureViewRect.position;
            cur_pos += -m_ScrollPosition;
            m_TextureViewRect.position = cur_pos;

            EditorGUI.DrawPreviewTexture(m_TextureViewRect, m_Texture);
        }
        //GUIClip.Push(m_TextureViewRect, -m_ScrollPosition, Vector2.zero, false);

        //if (Event.current.type == EventType.Repaint)
        //{
        //    DrawTexturespaceBackground();
        //    DrawTexture();
        //    DrawGizmos();
        //}

        //DoTextureGUIExtras();

        //GUIClip.Pop();
    }

    protected float GetMinZoom()
    {
        if (m_Texture == null)
            return 1.0f;
        // Case 654327: Add k_MaxZoom size to min check to ensure that min zoom is smaller than max zoom
        return Mathf.Min(m_TextureViewRect.width / m_Texture.width, m_TextureViewRect.height / m_Texture.height, k_MaxZoom) * k_MinZoomPercentage;
    }

    protected void HandleScrollbars()
    {
        Rect horizontalScrollBarPosition = new Rect(m_TextureViewRect.xMin, m_TextureViewRect.yMax, m_TextureViewRect.width, k_ScrollbarMargin);
        m_ScrollPosition.x = GUI.HorizontalScrollbar(horizontalScrollBarPosition, m_ScrollPosition.x, m_TextureViewRect.width, maxScrollRect.xMin, maxScrollRect.xMax);

        Rect verticalScrollBarPosition = new Rect(m_TextureViewRect.xMax, m_TextureViewRect.yMin, k_ScrollbarMargin, m_TextureViewRect.height);
        m_ScrollPosition.y = GUI.VerticalScrollbar(verticalScrollBarPosition, m_ScrollPosition.y, m_TextureViewRect.height, maxScrollRect.yMin, maxScrollRect.yMax);
    }

    protected void SetupHandlesMatrix()
    {
        // Offset from top left to center in view space
        Vector3 handlesPos = new Vector3(m_TextureRect.x, m_TextureRect.yMax, 0f);
        // We flip Y-scale because Unity texture space is bottom-up
        Vector3 handlesScale = new Vector3(m_Zoom, -m_Zoom, 1f);

        // Handle matrix is for converting between view and texture space coordinates, without taking account the scroll position.
        // Scroll position is added separately so we can use it with GUIClip.
        Handles.matrix = Matrix4x4.TRS(handlesPos, Quaternion.identity, handlesScale);
    }

    protected void HandleZoom()
    {
        bool zoomMode = Event.current.alt && Event.current.button == 1;
        if (zoomMode)
        {
            EditorGUIUtility.AddCursorRect(m_TextureViewRect, MouseCursor.Zoom);
        }

        if (
            ((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && zoomMode) ||
            ((Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt)
            )
        {
            Repaint();
        }

        if (Event.current.type == EventType.ScrollWheel || (Event.current.type == EventType.MouseDrag && Event.current.alt && Event.current.button == 1))
        {
            float zoomMultiplier = 1f - Event.current.delta.y * (Event.current.type == EventType.ScrollWheel ? k_WheelZoomSpeed : -k_MouseZoomSpeed);

            // Clamp zoom
            float wantedZoom = m_Zoom * zoomMultiplier;

            float currentZoom = Mathf.Clamp(wantedZoom, GetMinZoom(), k_MaxZoom);

            if (currentZoom != m_Zoom)
            {
                m_Zoom = currentZoom;

                // We need to fix zoomMultiplier if we clamped wantedZoom != currentZoom
                if (wantedZoom != currentZoom)
                    zoomMultiplier /= wantedZoom / currentZoom;

                m_ScrollPosition *= zoomMultiplier;

                // Zooming towards mouse cursor
                float xRatio = Event.current.mousePosition.x / m_TextureViewRect.width - 0.5f;
                float yRatio = Event.current.mousePosition.y / m_TextureViewRect.height - 0.5f;
                float diffX = xRatio * (zoomMultiplier - 1);
                float diffY = yRatio * (zoomMultiplier - 1);

                Rect scrollRect = maxScrollRect;
                m_ScrollPosition.x += (diffX * (scrollRect.width / 2.0f));
                m_ScrollPosition.y += (diffY * (scrollRect.height / 2.0f));

                Event.current.Use();
            }
        }
    }

    protected void HandlePanning()
    {
        // You can pan by holding ALT and using left button or NOT holding ALT and using right button. ALT + right is reserved for zooming.
        bool panMode = (!Event.current.alt && Event.current.button > 0 || Event.current.alt && Event.current.button <= 0);
        if (panMode && GUIUtility.hotControl == 0)
        {
            EditorGUIUtility.AddCursorRect(m_TextureViewRect, MouseCursor.Pan);

            if (Event.current.type == EventType.MouseDrag)
            {
                m_ScrollPosition -= Event.current.delta;
                Event.current.Use();
            }
        }

        //We need to repaint when entering or exiting the pan mode, so the mouse cursor gets refreshed.
        if (
            ((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && panMode) ||
            (Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt
            )
        {
            Repaint();
        }
    }

    protected void DrawScreenspaceBackground()
    {
        if (Event.current.type == EventType.Repaint)
            m_Styles.preBackground.Draw(m_TextureViewRect, false, false, false, false);
    }

} // class


public class Styles
{
    public readonly GUIStyle dragdot = "U2D.dragDot";
    public readonly GUIStyle dragdotDimmed = "U2D.dragDotDimmed";
    public readonly GUIStyle dragdotactive = "U2D.dragDotActive";
    public readonly GUIStyle createRect = "U2D.createRect";
    public readonly GUIStyle preToolbar = "preToolbar";
    public readonly GUIStyle preButton = "preButton";
    public readonly GUIStyle preLabel = "preLabel";
    public readonly GUIStyle preSlider = "preSlider";
    public readonly GUIStyle preSliderThumb = "preSliderThumb";
    public readonly GUIStyle preBackground = "preBackground";
    public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";
    public readonly GUIStyle pivotdot = "U2D.pivotDot";

    public readonly GUIStyle dragBorderdot = new GUIStyle();
    public readonly GUIStyle dragBorderDotActive = new GUIStyle();

    public readonly GUIStyle toolbar;
    public readonly GUIContent alphaIcon;
    public readonly GUIContent RGBIcon;
    public readonly GUIStyle notice;

    public readonly GUIContent smallMip;
    public readonly GUIContent largeMip;

    public Styles()
    {

        toolbar = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("In BigTitle"));
        toolbar.margin.top = 0;
        toolbar.margin.bottom = 0;
        alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
        RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
        preToolbar.border.top = 0;
        createRect.border = new RectOffset(3, 3, 3, 3);

        notice = new GUIStyle(GUI.skin.label);
        notice.alignment = TextAnchor.MiddleCenter;
        notice.normal.textColor = Color.yellow;

        dragBorderdot.fixedHeight = 5f;
        dragBorderdot.fixedWidth = 5f;
        dragBorderdot.normal.background = EditorGUIUtility.whiteTexture;

        dragBorderDotActive.fixedHeight = dragBorderdot.fixedHeight;
        dragBorderDotActive.fixedWidth = dragBorderdot.fixedWidth;
        dragBorderDotActive.normal.background = EditorGUIUtility.whiteTexture;

        smallMip = EditorGUIUtility.IconContent("PreTextureMipMapLow");
        largeMip = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
    }
}

