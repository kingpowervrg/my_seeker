using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 增加物品
/// </summary>
public class SceneAddItem : EditorWindow {

    public delegate void DelSelectItem(List<BaseItem> selectItems);
    public DelSelectItem delSelectItem;
    List<bool> itemState = new List<bool>();
    List<Texture> itemIcon = new List<Texture>();
    List<BaseItem> m_baseItems;

    List<BaseItem> m_selectItems = new List<BaseItem>();
    public void Init(List<BaseItem> baseItems)
    {
        m_baseItems = baseItems;
        if (m_baseItems == null)
        {
            return;
        }
        for (int i = 0; i < m_baseItems.Count; i++)
        {
            itemState.Add(false);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/"+m_baseItems[i].icon + ".JPG");
            if (tex == null)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/" + m_baseItems[i].icon + ".png");
            }
            itemIcon.Add(tex);
        }

    }
    Vector2 scrollPos = Vector2.zero;
    
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("确定"))
        {
            BtnSelectItem();
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUIItem();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    void GUIItem()
    {
        for (int i = 0; i < itemState.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button(itemIcon[i],GUILayout.Width(30),GUILayout.Height(30));
            itemState[i] = EditorGUILayout.Toggle(m_baseItems[i].id + "/" + m_baseItems[i].descs, itemState[i]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("---------------------------------------------");

        }

    }

    void BtnSelectItem()
    {
        for (int i = 0; i < itemState.Count; i++)
        {
            if (itemState[i])
            {
                m_selectItems.Add(m_baseItems[i]);
            }
        }

        if (delSelectItem != null && m_selectItems != null)
        {
            delSelectItem(m_selectItems);
        }
        this.Close();
    }

    //void OnDisable()
    //{
    //    m_selectItems.Clear();
    //}
}
