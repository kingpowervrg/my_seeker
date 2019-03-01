using UnityEngine;
using UnityEditor;

namespace GOGUI
{
    [CustomEditor(typeof(AnimatedImage), true)]
    [CanEditMultipleObjects]
    public class AnimatedImageEditor : UnityEditor.UI.ImageEditor
    {
        bool showSyms = true;
        string newName = "";
        Sprite newSprite;
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {            
            serializedObject.Update();
            AnimatedImage img = target as AnimatedImage;
            img.FPS = EditorGUILayout.IntField("FPS", img.FPS);
            img.Delay = EditorGUILayout.FloatField("播放间隔", img.Delay);
            showSyms = EditorGUILayout.Foldout(showSyms, "图片集");
            if (showSyms)
            {                        
                Sprite[] arr = img.Sprites;
                if (arr == null)
                    arr = new Sprite[0];
                for (int i = 0; i < arr.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    arr[i] = EditorGUILayout.ObjectField(arr[i], typeof(Sprite), false, GUILayout.Height(64), GUILayout.Width(64)) as Sprite;
                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                    {
                        Sprite[] arr2 = arr;
                        ArrayUtility.RemoveAt(ref arr2, i);
                        arr = arr2;
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("添加图片", GUILayout.Width(50));
                GUILayout.FlexibleSpace();
                newSprite = EditorGUILayout.ObjectField(newSprite, typeof(Sprite), false, GUILayout.Height(64), GUILayout.Width(64)) as Sprite;
                bool hasSprite = newSprite;
                
                bool isValid = true;
                bool sameTexture = false;
                if (hasSprite)
                {
                    if (arr.Length > 0)
                    {
                        TextureImporter newTI = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newSprite)) as TextureImporter;
                        TextureImporter oldTI = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(arr[0])) as TextureImporter;
                        if (newTI == null || oldTI == null || (newTI.spritePackingTag != oldTI.spritePackingTag))
                        {
                            isValid = false;
                            sameTexture = true;
                        }
                    }
                }
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button("添加", GUILayout.Width(35f)) && isValid)
                {
                    if (arr.Length > 0)
                    {
                        Sprite[] arr2 = new Sprite[arr.Length + 1];
                        arr.CopyTo(arr2, 0);
                        arr2[arr.Length] = newSprite;
                        arr = arr2;
                    }
                    else
                    {
                        arr = new Sprite[1] { newSprite };
                    }
                    newName = "";
                    newSprite = null;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                if (sameTexture)
                {
                    EditorGUILayout.HelpBox("新添加的图片必须跟之前添加的图片在同一个图集当中！", MessageType.Error);
                }
                if (arr.Length == 0)
                {
                    EditorGUILayout.HelpBox("请添加至少1张图片到动画", MessageType.Info);
                }
                else
                    GUILayout.Space(4f);


                img.Sprites = arr;
            }
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}