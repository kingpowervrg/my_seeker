using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Texture2Sprite
{
    class Tex2SpriteWindow : EditorWindow
    {
        [MenuItem("Tools/图片批量变sprite")]
        static void CreateWindow()
        {
            Rect rect = new Rect(Screen.width >> 1, Screen.height >> 1, 300, 300);
            Tex2SpriteWindow window = (Tex2SpriteWindow)EditorWindow.GetWindowWithRect(typeof(Tex2SpriteWindow), rect, true, "图片编辑器");
            //window.Init();
            window.Show();
        }

        private string atlas_name = "";
        string selection_path = "";
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("输入图集名称:");

            atlas_name = EditorGUILayout.TextField(atlas_name);

            EditorGUILayout.Space();

            if (GUILayout.Button("转换"))
            {
                //Object topObj = Selection.activeObject;//这个函数可以得到你选中的对象
                string[] objs = Selection.assetGUIDs;
                //UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
                //Object topObj = arr[0];

                if (null != objs && objs.Length > 0)
                {
                    selection_path = AssetDatabase.GUIDToAssetPath(objs[0]);//AssetDatabase.GetAssetPath(topObj);

                    if (!selection_path.Contains("UISprite"))
                    {
                        this.ShowNotification(new GUIContent("请选择UISprite下的文件夹"));
                        return;
                    }
                }

                Rect rect = new Rect(Screen.width >> 1, Screen.height >> 1, 1200, 300);
                Tex2SpriteConfirmWindow window = (Tex2SpriteConfirmWindow)EditorWindow.GetWindowWithRect(typeof(Tex2SpriteConfirmWindow), rect, true, "图片编辑器");
                window.Init(selection_path, atlas_name);
                window.Show();

                EditorUtility.SetDirty(this);

            }
        }



    }


    public class Tex2SpriteConfirmWindow : EditorWindow
    {
        private string m_path = "";
        private string m_atlas_name = "";
        public void Init(string path_, string atlas_name_)
        {
            m_path = path_;
            m_atlas_name = atlas_name_;
            EditorUtility.SetDirty(this);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            GUIStyle bb = new GUIStyle();
            bb.normal.background = null; //这是设置背景填充的
            bb.normal.textColor = new Color(1, 0, 0);   //设置字体颜色的
            bb.fontSize = 24; //当然，这是字体颜色

            EditorGUILayout.LabelField(string.Format("确定将{0}下的图片\n改变成图集为{1}的sprite吗？", m_path, m_atlas_name), bb);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("是"))
            {
                Object[] m_objects = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);//选择的所以对象

                foreach (Object targetObj in m_objects)
                {
                    string path = AssetDatabase.GetAssetPath(targetObj);
                    TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (TextureImporterType.Sprite == texture.textureType && texture.spritePackingTag == m_atlas_name)
                        continue;

                    texture.textureType = TextureImporterType.Sprite;
                    texture.spritePackingTag = m_atlas_name;
                    //texture.spritePixelsPerUnit = 1;
                    //texture.filterMode = FilterMode.Trilinear;
                    texture.mipmapEnabled = false;
                    //texture.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                    AssetDatabase.ImportAsset(path);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                this.Close();

                Tex2SpriteWindow window = (Tex2SpriteWindow)EditorWindow.GetWindow(typeof(Tex2SpriteWindow));
                window.Close();
            }


            if (GUILayout.Button("否"))
            {
                this.Close();
                Tex2SpriteWindow window = (Tex2SpriteWindow)EditorWindow.GetWindow(typeof(Tex2SpriteWindow));
                window.Close();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
