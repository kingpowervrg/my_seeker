using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Resource
{
    public class ResourceExhibitSystem
    {
        private SceneDataJson sceneData;

        public void LoadSceneData()
        {
            if (sceneData != null)
            {
                return;
            }
            StreamReader reader = new StreamReader(ResourceData.sceneDataConfigPath);
            string content = reader.ReadToEnd();
            reader.Close();
            sceneData = fastJSON.JSON.ToObject<SceneDataJson>(content);
        }

        public List<string> emptyDataNames = new List<string>();
        public List<string> emptyDataPaths = new List<string>();

        public void CheckSceneData()
        {
            emptyDataNames.Clear();
            emptyDataPaths.Clear();
            List<BaseItem> baseItems = ResourceData.exhibitItem;
            for (int i = 0; i < baseItems.Count; i++)
            {
                bool flag = CheckSceneData(baseItems[i]);
                if (!flag)
                {
                    emptyDataNames.Add(baseItems[i].id + " ----" + baseItems[i].model);
                    emptyDataPaths.Add("Assets/" + baseItems[i].model + ".prefab");
                }
            }
        }

        private bool CheckSceneData(BaseItem baseItem)
        {
            for (int j = 0; j < sceneData.sceneDatas.Count; j++)
            {
                SceneItemServerJson itemserver = sceneData.sceneDatas[j];
                for (int m = 0; m < itemserver.items.Count; m++)
                {
                    if (baseItem.id == itemserver.items[m].itemID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

