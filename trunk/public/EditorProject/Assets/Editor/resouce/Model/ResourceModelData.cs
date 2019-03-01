using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Resource
{
    public class ResourceModelData
    {
        public List<string> m_allModelPath = new List<string>();

        public List<string> m_allNoLightmapUV = new List<string>(); //没有勾选UV

        public List<string> m_allOnWrite = new List<string>(); //所有打开写入

        public List<string> m_BlendShape = new List<string>(); //所有打开BlendShape

        public void Clear()
        {
            m_allModelPath.Clear();
            m_allNoLightmapUV.Clear();
            m_allOnWrite.Clear();
            m_BlendShape.Clear();
        }

        public void AddModelPath(string modelPath)
        {
            AddModelObj(modelPath);
            m_allModelPath.Add(modelPath);
        }

        public List<ModelImporter> m_allModelObj = new List<ModelImporter>();
        public void AddModelObj(string modelPath)
        {
            ModelImporter modelImporter = ModelImporter.GetAtPath(modelPath) as ModelImporter;
            if (modelImporter.isReadable)
            {
                m_allOnWrite.Add(modelPath);
            }
            if (!modelImporter.generateSecondaryUV)
            {
                m_allNoLightmapUV.Add(modelPath);
            }
            if (modelImporter.importBlendShapes)
            {
                m_BlendShape.Add(modelPath);
            }
        }
    }
}
