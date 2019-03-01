/********************************************************************
	created:  2018-6-27 20:19:46
	filename: PackBuildinShaders.cs
	author:	  songguangze@outlook.com
	
	purpose:  设置AlwaysBuildinShader
*********************************************************************/
using UnityEditor;
using UnityEngine;
namespace EngineCore.Editor
{
    public class PackBuildinShaders
    {
        private static string[] AlwaysBuildinShaders = new string[]
        {
            "Legacy Shaders/Diffuse",
            "Mobile/Diffuse",
            "Sprites/Default",
            "UI/Default",
            "Legacy Shaders/Transparent/Cutout/Diffuse",
            "Legacy Shaders/Transparent/Diffuse",
            "Particles/Additive",
            "UI/DefaultETC1",
        };

        /// <summary>
        /// 修改GraphicSetting
        /// </summary>
        public static void PackAlwaysBuildinShaders()
        {
            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            SerializedProperty dataPoint;
            while (it.NextVisible(true))
            {
                if (it.name == "m_AlwaysIncludedShaders")
                {
                    it.ClearArray();

                    for (int i = 0; i < AlwaysBuildinShaders.Length; i++)
                    {
                        it.InsertArrayElementAtIndex(i);
                        dataPoint = it.GetArrayElementAtIndex(i);
                        dataPoint.objectReferenceValue = Shader.Find(AlwaysBuildinShaders[i]);
                    }

                    graphicsSettings.ApplyModifiedProperties();
                }
            }

            graphicsSettings.UpdateIfRequiredOrScript();
        }

    }
}