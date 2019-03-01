using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class SceneLightingMap : EditorWindow {

    public ObjLightMapping light;
    public int width = 64;
    public int heigh = 64;

    public int lightWidth = 1024;
    public int lightHeigh = 1024;
    //[MenuItem("Tools/LightingMap")]
    static void CreateWindow()
    {
        Rect rect = new Rect(0, 0, 400, 400);
        SceneLightingMap window = (SceneLightingMap)EditorWindow.GetWindowWithRect(typeof(SceneLightingMap), rect, true, "场景编辑器");
        window.Init();
        window.Show();
    }

    void Init()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            return;
        }
        lightMapping = LightmapSettings.lightmaps[0].lightmapColor;
        lightDir = LightmapSettings.lightmaps[0].lightmapDir;
         light = AssetDatabase.LoadAssetAtPath<ObjLightMapping>("Assets/testLight.asset");
    }
    Texture2D lightMapping;
    Texture2D lightDir;
    GameObject testObj;
    //LightmapData lightData;
    void OnGUI()
    {
        lightMapping = (Texture2D)EditorGUILayout.ObjectField(lightMapping, typeof(Texture2D), true);
        testObj = (GameObject)EditorGUILayout.ObjectField(testObj, typeof(GameObject), true);
        if (GUILayout.Button("获取"))
        {
            getObjLightMapping();
            CreateObjLighting();
        }
    }

    void getObjLightMapping()
    {
        
        if (testObj == null)
        {
            return;
        }
        MeshRenderer render = testObj.GetComponent<MeshRenderer>();
        if (light == null)
        {
            light = new ObjLightMapping();
            AssetDatabase.CreateAsset(light, "Assets/testLight.asset");
            AssetDatabase.Refresh();
        }
        //Material mat;
        //mat.SetTextureOffset
        light.id = 0;
        light.lightIndex = render.lightmapIndex;
        light.lightOffset = render.lightmapScaleOffset;
    }
    
    void CreateObjLighting()
    {
        if (light == null)
        {
            return;
        }
        int startX = (int)(Mathf.Abs(light.lightOffset.z) * lightWidth);
        int startY = (int)(Mathf.Abs(light.lightOffset.w) * lightHeigh);
        float www = light.lightOffset.w * lightHeigh;
        int scaleX = (int)(light.lightOffset.x * lightWidth);
        int scaleY = (int)(light.lightOffset.y * lightHeigh);

        width = scaleX;
        heigh = scaleY;
        Texture2D tex = new Texture2D(width, heigh);

        for (int i = startX; i < startX + scaleX; i++)
        {
            for (int j = startY; j < startY + scaleY; j++)
            {
                tex.SetPixel(i - startX,j - startY, lightMapping.GetPixel(i,j));
            }
        }
        tex.Apply();
        byte[] texByte = tex.EncodeToPNG();
        FileStream file = File.Open(Application.dataPath + "/" + testObj.name + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(texByte);
        file.Close();
        writer.Close();
    }


}
