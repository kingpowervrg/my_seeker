using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LightMapLoading : MonoBehaviour {

    private string configPath;
    private Dictionary<string, SceneItemJson> itemjsons = new Dictionary<string, SceneItemJson>();
	// Use this for initialization
	void Start () {

        LoadPath();

    }

    private void LoadPath()
    {
        configPath = Application.dataPath + "/Res/SceneConfig/" + SceneManager.GetActiveScene().name;
        string[] files = Directory.GetFiles(configPath);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".meta") && files[i].Contains(".json"))
            {
                StreamReader reader = new StreamReader(files[i]);
                string fileContent = reader.ReadToEnd();
                reader.Close();
                SceneItemJson itemjson = fastJSON.JSON.ToObject<SceneItemJson>(fileContent);
                FileInfo myfile = new FileInfo(files[i]);
                Debug.Log(myfile.Name);
                itemjsons.Add(myfile.Name,itemjson);
            }
        }
        GameObject[] exhibits = GameObject.FindGameObjectsWithTag("exhibit");
        for (int i = 0; i < exhibits.Length; i++)
        {
            string[] exhibitInfo = exhibits[i].name.Split('/');
            if (itemjsons.ContainsKey(exhibitInfo[0]))
            {
                int jjj = int.Parse(exhibitInfo[2]);
                ItemPosInfoJson lightmapjson = GetLightMapByID(itemjsons[exhibitInfo[0]],long.Parse(exhibitInfo[1]), jjj);
                MeshRenderer render = exhibits[i].GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.material.shader = Shader.Find("Seeker/Exhibit/Diffuse");
                    render.sharedMaterial.SetTextureOffset("_LightTex", new Vector2(lightmapjson.offsetX, lightmapjson.offsetY));
                    render.sharedMaterial.SetTextureScale("_LightTex", new Vector2(lightmapjson.tilingX, lightmapjson.tilingY));
                }
            }
        }
    }

    private ItemPosInfoJson GetLightMapByID(SceneItemJson itemjson, long id,int index)
    {
        for (int i = 0; i < itemjson.items.Count; i++)
        {
            if (itemjson.items[i].itemID == id)
            {
                return itemjson.items[i].itemPos[index];
            }
        }
        return null;
    }

	// Update is called once per frame
	void Update () {
		
	}
   
}
