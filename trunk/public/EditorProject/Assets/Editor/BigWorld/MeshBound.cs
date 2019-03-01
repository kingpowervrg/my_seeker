using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshBound : EditorWindow {

    static MeshBound window;

    [MenuItem("Tools/BigWorld/生成围墙")]
    public static void CreateMesh()
    {
        Rect rect = new Rect(0, 0, 500, 400);
        window = (MeshBound)EditorWindow.GetWindowWithRect(typeof(MeshBound), rect, true, "创建网格");
        if (window.Init())
        {
            window.Show();
        }
    }

    public bool Init()
    {
        return true;
    }
    string m_meshHeiStr = "0.5";
    string m_effectDisStr = "0";

    float m_meshHei
    {
        get
        {
            return float.Parse(m_meshHeiStr);
        }
    }

    float m_effectDis
    {
        get
        {
            return float.Parse(m_effectDisStr);
        }
    }

    void OnGUI()
    {
        m_meshHeiStr = EditorGUILayout.TextField("网格高度:", m_meshHeiStr);
        m_effectDisStr = EditorGUILayout.TextField("网格距离:", m_effectDisStr);
        if (GUILayout.Button("生成墙面"))
        {
            window.GetMeshBound(MeshType.LockWall, "wall");
        }
        if (GUILayout.Button("生成解锁墙面"))
        {
            window.GetMeshBound(MeshType.UnLockEffectWall,"EffectWall");
        }
    }
    
    public void GetMeshBound(MeshType meshType,string name)
    {
        GameObject[] obj = Selection.gameObjects;
        for (int i = 0; i < obj.Length; i++)
        {
            MeshFilter render = obj[i].GetComponent<MeshFilter>();
            if (render == null || render.sharedMesh == null)
            {
                continue;
            }
            LineRenderer lineRender = obj[i].GetComponent<LineRenderer>();

            Mesh mesh = render.sharedMesh;
            List<List<Vector3>> vertices = MeshTools.TrianglesAndVerticesEdge(mesh.vertices, mesh.triangles);
            int index = 0;
            int maxCount = 0;
            for (int j = 0; j < vertices.Count; j++)
            {
                if (maxCount < vertices[j].Count)
                {
                    index = j;
                    maxCount = vertices[j].Count;
                }
            }
            CreateMesh(vertices[index], obj[i], obj[i].transform.parent, name,meshType);
        }
    }

    private void CreateMesh(List<Vector3> points,GameObject obj,Transform parent,string name,MeshType meshType)
    {
        //AssetDatabase.LoadAssetAtPath(typeof(Material),"Assets/Res/Maps/ChengShiDiTu_01/FBX/Materials/wall");
        Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Res/Maps/ChengShiDiTu_01/FBX/Materials/wall.mat");
        //Material material = new Material(Shader.Find("Unlit/Wall"));
        Vector3 center = Vector3.forward * points[0].y;

        Transform wall = parent.Find(name);
        if (wall != null)
        {
            GameObject.DestroyImmediate(wall.gameObject);
        }
        MeshFilter meshFilter = null;
        GameObject meshObj = new GameObject(name);
        meshFilter = meshObj.AddComponent<MeshFilter>();
        MeshRenderer meshRender = meshObj.AddComponent<MeshRenderer>();
        meshRender.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRender.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        wall = meshObj.transform;
        meshRender.sharedMaterial = material;
        wall.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        List<Vector3> meshPoints = new List<Vector3>();
        List<int> indecs = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        Mesh mesh = new Mesh();
        points.Add(points[0]);
        if (meshType == MeshType.UnLockEffectWall)
        {
            points = CreateMeshWall(points);
        }
        int pointCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = points[i] - center;
            meshPoints.Add(points[i]);
            meshPoints.Add(points[i] + Vector3.forward * m_meshHei);
            if (meshType == MeshType.UnLockEffectWall)
            {
                uv.Add(new Vector2(i % 2 == 0 ? 0 : 1, 0));
                uv.Add(new Vector2(i % 2 == 0 ? 0 : 1, 1));
               
            }
            else
            {
                uv.Add(new Vector2(i / (float)pointCount, 0));
                uv.Add(new Vector2(i / (float)pointCount, 1));
            }
           
            if (i < points.Count - 1)
            {
                indecs.Add(i * 2);
                indecs.Add(i * 2 + 1);
                indecs.Add(i * 2 + 2);

                indecs.Add(i * 2 + 1);
                indecs.Add(i * 2 + 2);
                indecs.Add(i * 2 + 3);
            }
        }
        mesh.SetVertices(meshPoints);
        mesh.SetIndices(indecs.ToArray(), MeshTopology.Triangles,0);
        mesh.SetUVs(0, uv);
        meshFilter.sharedMesh = mesh;
        wall.SetParent(parent,false);
        wall.localPosition = obj.transform.localPosition;
        wall.localScale = obj.transform.localScale;
    }

    private List<Vector3> CreateMeshWall(List<Vector3> points)
    {
        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (i == 0)
            {
                newPoints.Add(points[0]);
            }
            else
            {
                newPoints.Add(points[i]);
                newPoints.Add(points[i]);
                float dis = Vector3.Distance(points[i], points[i + 1]);
                float number = Mathf.FloorToInt(dis / m_effectDis);
                if (number > 1)
                {
                    Debug.Log("==== " + number);
                    for (int j = 1; j < number; j++)
                    {
                        float percent = (float)j / (float)number;
                        Vector3 tempVector = Vector3.Lerp(points[i], points[i + 1], percent); //( + ) * percent;
                        newPoints.Add(tempVector);
                        newPoints.Add(tempVector);
                        //points.Add(tempVector);
                    }
                }
            }
            
        }

        //newPoints.Add(points[0]);
        //for (int i = 1; i < points.Count - 1; i++)
        //{
        //    newPoints.Add(points[i]);
        //    newPoints.Add(points[i]);
        //}
        newPoints.Add(points[points.Count - 1]);
        return newPoints;
    }

    public enum MeshType
    {
        LockWall,
        UnLockEffectWall
    }

}
