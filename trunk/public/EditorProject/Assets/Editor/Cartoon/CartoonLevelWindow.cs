using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class CartoonLevelWindow : EditorWindow
{
    public Action<long> OnLevelSelected;

    List<string> m_names = new List<string>();

    Vector2 scrollPosition = new Vector2(0, 0);

    public static CartoonLevelWindow CreateWindow()
    {
        Rect rect = new Rect(0, 0, 400, 700);
        CartoonLevelWindow window = (CartoonLevelWindow)EditorWindow.GetWindowWithRect(typeof(CartoonLevelWindow), rect, true, "漫画编辑器");
        window.Init();
        window.Show();

        return window;
    }

    void Init()
    {
        m_names = CartoonJsonUtil.GetLevelJsonFileNamesWithoutEx();

    }

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < m_names.Count; ++i)

        {
            GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
            GUILayout.Space(7);
            if (GUILayout.Button(m_names[i]))
            {
                if (null != OnLevelSelected)
                {
                    OnLevelSelected(long.Parse(m_names[i]));
                    this.Close();
                }
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}

