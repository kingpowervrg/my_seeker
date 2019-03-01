using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Resource
{
    public class ResourceUISystem
    {
        public Dictionary<string, List<string>> m_sameUI = new Dictionary<string, List<string>>();
        private Dictionary<int, string> m_allUI = new Dictionary<int, string>();

        public void CheckUIDepth()
        {
            m_sameUI.Clear();
            m_allUI.Clear();
            GetAllPrefab(ResourceData.m_AbsoluteResourceDir);
        }

        private void GetAllPrefab(string dirPath)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                GetAllPrefab(subDirs[i]);
            }
            string[] filePaths = Directory.GetFiles(dirPath);
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (filePaths[i].EndsWith(".prefab"))
                {
                    int assetIndex = filePaths[i].IndexOf("Assets");
                    string assetPath = filePaths[i].Substring(assetIndex);
                    GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.GameObject)) as UnityEngine.GameObject;
                    Canvas canvas = obj.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        int sortOrder = canvas.sortingOrder;
                        if (m_allUI.ContainsKey(sortOrder))
                        {
                            string sortOrderStr = sortOrder.ToString();
                            if (m_sameUI.ContainsKey(sortOrderStr))
                            {
                                m_sameUI[sortOrderStr].Add(assetPath);
                            }
                            else
                            {
                                m_sameUI.Add(sortOrderStr, new List<string>() { m_allUI[sortOrder],assetPath });
                            }
                        }
                        else
                        {
                            m_allUI.Add(sortOrder, assetPath);
                        }
                    }
                }
            }
        }
    }
}
