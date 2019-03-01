#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EngineCore.Editor
{
    public class JsonObjectEditWindow : EditorWindow
    {
        private object mCurStringInterface = null;                          //当前编辑的根StringInterface//
        private JsonObjectUIHelper mCurStringInterfaceHelper = null;        //当前SI的辅助对象//


        // Update is called once per frame
        void Update()
        {
            if (null != mCurStringInterfaceHelper)
            {
                if (mCurStringInterfaceHelper.DoesNeedStarted() && !Application.isPlaying)
                    return;
                if (mCurStringInterfaceHelper.IsDirty)
                {
                    GOESelectWindow.SaveHelperConfiguration(mCurStringInterfaceHelper);
                }
                if (mCurStringInterfaceHelper.OnUpdate(mCurStringInterface))
                {
                    Repaint();
                    ShowInPropertyWindow();
                }
            }
        }

        void OnSelectionChange()
        {
            if (null != mCurStringInterfaceHelper)
            {
                mCurStringInterfaceHelper.OnSelectionChange(mCurStringInterface);
            }
            Repaint();
        }

        //设置当前StringInterface//
        public void SetCurStringInterface(object si, JsonObjectUIHelper sih)
        {
            mCurStringInterface = si;
            mCurStringInterfaceHelper = sih;
        }

        static JsonObjectPropertyWindow wp = null;
        void ShowInPropertyWindow()
        {
            //显示属性窗口//
            if (null == wp)
                wp = EditorWindow.GetWindow(typeof(JsonObjectPropertyWindow), false, "属性") as JsonObjectPropertyWindow;
            wp.Repaint();
            wp.SetCurStringInterface(mCurStringInterface, mCurStringInterfaceHelper);
        }

        private Vector2 m_scrollPositionSelect; //滚动位置//
        GUILayoutOption[] voptionsNoMaxWidth = {
        GUILayout.MaxWidth (10000),
        GUILayout.ExpandHeight (true),
        GUILayout.ExpandWidth (true)};
        void OnGUI()
        {
            //由m_CurStringInterfaceHelper生成界面//
            //只要求helper不为空//
            if (null != mCurStringInterfaceHelper)
            {
                //开始滚动区//
                m_scrollPositionSelect = GUILayout.BeginScrollView(m_scrollPositionSelect, false, true, voptionsNoMaxWidth);

                mCurStringInterfaceHelper.MakeEditUI(mCurStringInterface);

                //结束滚动区//
                GUILayout.EndScrollView();
            }
        }
    }
}
#endif
