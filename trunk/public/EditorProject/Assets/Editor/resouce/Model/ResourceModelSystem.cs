using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Resource
{
    public class ResourceModelSystem
    {
        public ResourceModelData m_resourceModelData;

        public ResourceModelSystem()
        {
            m_resourceModelData = new ResourceModelData();
        }

        public void LoadModel()
        {
            m_resourceModelData.Clear();
            LoadModel(ResourceData.m_AbsoluteResourceDir);
        }

        public void LoadModel(string dirPath)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                LoadModel(subDirs[i]);
            }
            string[] filePaths = Directory.GetFiles(dirPath);
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (filePaths[i].EndsWith(".fbx") || filePaths[i].EndsWith(".FBX"))
                {
                    int assetIndex = filePaths[i].IndexOf("Assets");
                    string assetPath = filePaths[i].Substring(assetIndex);
                    m_resourceModelData.AddModelPath(assetPath);
                }
            }
        }

        public void OnLightmapUV(bool flag,bool[] state)
        {
            for (int i = 0; i < m_resourceModelData.m_allNoLightmapUV.Count; i++)
            {
                if (state[i])
                {
                    ModelImporter modelImporter = ModelImporter.GetAtPath(m_resourceModelData.m_allNoLightmapUV[i]) as ModelImporter;
                    modelImporter.generateSecondaryUV = flag;
                    modelImporter.SaveAndReimport();
                }
                
            }
        }

        public void OnRW(bool flag, bool[] state)
        {
            for (int i = 0; i < m_resourceModelData.m_allOnWrite.Count; i++)
            {
                if (state[i])
                {
                    ModelImporter modelImporter = ModelImporter.GetAtPath(m_resourceModelData.m_allOnWrite[i]) as ModelImporter;
                    modelImporter.isReadable = flag;
                    modelImporter.SaveAndReimport();
                }
            }
        }

        public void OnBlendShap(bool flag, bool[] state)
        {
            for (int i = 0; i < m_resourceModelData.m_BlendShape.Count; i++)
            {
                if (state[i])
                {
                    ModelImporter modelImporter = ModelImporter.GetAtPath(m_resourceModelData.m_BlendShape[i]) as ModelImporter;
                    modelImporter.importBlendShapes = flag;
                    modelImporter.SaveAndReimport();
                }
            }
        }

        public void OnChangeResource(ResourceModelType modelType,bool[] state)
        {
            if (modelType == ResourceModelType.UV2)
            {
                OnLightmapUV(true,state);
            }
            else if (modelType == ResourceModelType.RW)
            {
                OnRW(false, state);
            }
            else if (modelType == ResourceModelType.BlendShape)
            {
                OnBlendShap(false, state);
            }
        }
    }
}
