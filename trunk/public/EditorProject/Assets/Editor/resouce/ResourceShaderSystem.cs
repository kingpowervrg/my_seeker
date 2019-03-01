using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace Resource
{
    public class ResourceShaderSystem
    {
        public Dictionary<string, List<string>> m_ShaderCount = new Dictionary<string, List<string>>();
        public int m_MeshRenderCount = 0;  //Mesh个数
        public int m_SkinnedMeshRenderCount = 0; //带骨骼的个数

        public List<string> m_LoseMesh = new List<string>(); //丢失的Mesh

        public List<string> m_LoseMeshMaterial = new List<string>();//丢失的材质

        public List<string> m_LoseExhibit = new List<string>(); //不存在的物件

        private void Clear()
        {
            m_LoseExhibit.Clear();
            m_ShaderCount.Clear();
            m_LoseMesh.Clear();
            m_LoseMeshMaterial.Clear();
            m_MeshRenderCount = 0;
            m_SkinnedMeshRenderCount = 0;
        }

        public void GetAllShader()
        {
            Clear();
            GetAllShader(ResourceData.m_AbsoluteResourceDir);
        }

        public void GetAllShader(string dirPath)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                GetAllShader(subDirs[i]);
            }
            string[] filePaths = Directory.GetFiles(dirPath);
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (filePaths[i].EndsWith(".prefab"))
                {
                    int assetIndex = filePaths[i].IndexOf("Assets");
                    string assetPath = filePaths[i].Substring(assetIndex);
                    CheckPrefab(0,assetPath);
                }
            }
        }

        public void GetAllShaderByMat()
        {
            Clear();
            GetAllShaderByMat(ResourceData.m_AbsoluteResourceDir);
        }

        public void GetAllShaderByMat(string dirPath)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                GetAllShaderByMat(subDirs[i]);
            }
            string[] filePaths = Directory.GetFiles(dirPath);
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (filePaths[i].EndsWith(".mat"))
                {
                    int assetIndex = filePaths[i].IndexOf("Assets");
                    string assetPath = filePaths[i].Substring(assetIndex);
                    Material obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Material)) as UnityEngine.Material;
                    
                    if (m_ShaderCount.ContainsKey(obj.shader.name))
                    {
                        m_ShaderCount[obj.shader.name].Add(assetPath);
                    }
                    else
                    {
                        m_ShaderCount.Add(obj.shader.name, new List<string> { assetPath });
                    }
                    //CheckPrefab(0, assetPath);
                }
            }
        }

        public void CensusExhibit()
        {
            Clear();
            for (int i = 0; i < ResourceData.exhibitItem.Count; i++)
            {
                CheckPrefab(ResourceData.exhibitItem[i].id, "Assets/" + ResourceData.exhibitItem[i].model + ".prefab");
            }
        }

        private void CheckPrefab(long id,string assetPath)
        {
            GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.GameObject)) as UnityEngine.GameObject;
            if (obj == null)
            {
                m_LoseExhibit.Add("id: " + id + " ==" + assetPath);
                return;
            }
            //网格丢失
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                if (meshFilter.sharedMesh == null)
                {
                    m_LoseMesh.Add(assetPath);
                }
            }
            //材质丢失
            MeshRenderer render = obj.GetComponent<MeshRenderer>();
            if (render != null)
            {
                m_MeshRenderCount++;
                CheckMesh(render, assetPath, 0);
            }
            SkinnedMeshRenderer skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                m_SkinnedMeshRenderCount++;
                CheckMesh(skinnedMesh,assetPath,1);
            }
        }

        private void CheckMesh(Renderer render,string assetPath,int type)
        {
            if (render.sharedMaterial == null)
            {
                m_LoseMeshMaterial.Add(assetPath);
            }
            else
            {
                string shaderName = render.sharedMaterial.shader.name;
                if (m_ShaderCount.ContainsKey(shaderName))
                {
                    m_ShaderCount[shaderName].Add(assetPath);
                }
                else
                {
                    m_ShaderCount.Add(shaderName,new List<string> { assetPath});
                }
            } 
        }

        private Renderer GetRenderForPrefab(string assetPath)
        {
            GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.GameObject)) as UnityEngine.GameObject;
            //材质丢失
            MeshRenderer render = obj.GetComponent<MeshRenderer>();
            if (render != null)
            {
                return render;
            }
            SkinnedMeshRenderer skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                return skinnedMesh;
            }
            return null;
        }

        private void ReplaceShader(List<string> assetPaths,string destShaderName)
        {
            for (int i = 0; i < assetPaths.Count; i++)
            {
                Renderer render = GetRenderForPrefab(assetPaths[i]);
                if (render == null)
                {
                    continue;
                }
                render.sharedMaterial.shader = Shader.Find(destShaderName);
            }
            UnityEditor.AssetDatabase.Refresh();
        }

        public void ReplaceShader()
        {
            foreach (var kv in m_ShaderCount)
            {
                for (int i = 0; i < ResourceData.m_srcShader.Count; i++)
                {
                    if (kv.Key.Equals(ResourceData.m_srcShader[i]))
                    {
                        ReplaceShader(kv.Value,ResourceData.m_destShader[i]);
                    }
                }
            }
        }
    }
}
