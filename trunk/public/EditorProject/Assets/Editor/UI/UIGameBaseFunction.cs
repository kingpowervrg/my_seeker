using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIGameBaseFunction
{
    [MenuItem("Tools/UI/ImageTrue")]
    public static void AddImageTrue()
    {
        GameObject[] obj = Selection.gameObjects;
        for (int i = 0; i < obj.Length; i++)
        {
            //Object o = obj[i];
            //if (o == null)
            //{
            //    continue;
            //}
            GameObject gameObj = obj[i];
            if (gameObj == null)
            {
                continue;
            }
            Image img = gameObj.GetComponent<Image>();
            if (img == null)
            {
                continue;
            }
            TweenAlpha tweenAlpha = gameObj.GetComponent<TweenAlpha>();
            if (tweenAlpha == null)
            {
                continue;
            }
            GameBaseFunction func = gameObj.AddComponent<GameBaseFunction>();
            tweenAlpha.AddTweenCompletedCallback(func.SetImageTrue);
            img.raycastTarget = false;
        }
    }

    //[MenuItem("Tools/UI/Find UIOutline Or UIShadow")]
    public static void FindUIWithOutlineOrShadow()
    {
        string uiPrefabRoot = "Assets/Res/Gui/GuiPrefab/";
        string[] allUIPrefab = Directory.GetFiles(uiPrefabRoot, "*.prefab", SearchOption.TopDirectoryOnly);


        for (int i = 0; i < allUIPrefab.Length; ++i)
        {
            GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(allUIPrefab[i]);
            Outline[] outlineComponents = uiPrefab.GetComponentsInChildren<Outline>(true);
            Shadow[] shadowComponent = uiPrefab.GetComponentsInChildren<Shadow>(true);

            if (outlineComponents.Length > 0)
            {
                Debug.Log(allUIPrefab[i]);
                for (int j = 0; j < outlineComponents.Length; ++j)
                    Debug.Log(outlineComponents[j].name);
            }

            if (shadowComponent.Length > 0)
            {
                Debug.Log(allUIPrefab[i]);
                for (int j = 0; j < shadowComponent.Length; ++j)
                    Debug.Log(shadowComponent[j].name);
            }


        }
    }

    [MenuItem("Tools/UI/查找超大尺寸的UI")]
    public static void FindSpriteOverSize()
    {
        int SIZE_WIDTH_THRESHOLD = 256;
        int SIZE_HEIGHT_THRESHOLD = 256;

        MethodInfo getWidthAndHeightMethod = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);

        string spriteRoot = "Assets/Res/Gui/UISprite/";
        string[] sprites = Directory.GetFiles(spriteRoot, "*.png", SearchOption.AllDirectories);
        StringBuilder sbOverSizeOutput = new StringBuilder($"width or height over size { SIZE_WIDTH_THRESHOLD},{SIZE_HEIGHT_THRESHOLD} :");
        sbOverSizeOutput.AppendLine();

        for (int i = 0; i < sprites.Length; ++i)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(sprites[i]) as TextureImporter;
            if (textureImporter.textureType == TextureImporterType.Sprite)
            {
                object[] param = new object[] { 0, 0 };
                getWidthAndHeightMethod.Invoke(textureImporter, param);
                int width = Convert.ToInt32(param[0]);
                int height = Convert.ToInt32(param[1]);

                if (width > SIZE_WIDTH_THRESHOLD || height > SIZE_HEIGHT_THRESHOLD)
                    sbOverSizeOutput.AppendLine($"{sprites[i]} size:({width},{height})");
            }
        }

        Debug.LogWarning(sbOverSizeOutput.ToString());
    }

    public static void ReplaceFont()
    {
        Font arialFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        Font font1 = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Gui/Fonts/font_1.ttf");
        Font font2 = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Gui/Fonts/font_2.ttf");
        Font font3 = AssetDatabase.LoadAssetAtPath<Font>("Assets/Res/Gui/Fonts/font_3.ttf");

        GameObject[] allUIGameobjectList = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < allUIGameobjectList.Length; ++i)
        {
            Text[] uiTextComponents = allUIGameobjectList[i].GetComponentsInChildren<Text>(true);

            for (int j = 0; j < uiTextComponents.Length; ++j)
            {
                Text textComponent = uiTextComponents[j];
                if (textComponent.font == font1)
                    textComponent.font = arialFont;
                else if (textComponent.font == font3)
                    textComponent.font = font1;
            }

            PrefabUtility.ReplacePrefab(allUIGameobjectList[i], PrefabUtility.GetPrefabParent(allUIGameobjectList[i]), ReplacePrefabOptions.ConnectToPrefab);
        }
    }
}