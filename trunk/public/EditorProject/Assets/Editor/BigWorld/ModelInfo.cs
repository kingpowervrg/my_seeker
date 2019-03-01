using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelInfo : MonoBehaviour {

    [MenuItem("Tools/BigWorld/生成模型高度")]
    public static void GetModelHei()
    {
        GameObject[] selectObj = Selection.gameObjects;
        for (int i = 0; i < selectObj.Length; i++)
        {
            GameObject objRoot = selectObj[i];
            MeshRenderer[] childRender = objRoot.GetComponentsInChildren<MeshRenderer>();
            float groupMinHei = 0f;
            float groupMaxHei = 0f;
            for (int j = 0; j < childRender.Length; j++)
            {
                if (childRender[j].name.Equals("center") || childRender[j].name.Equals("Cube"))
                {
                    continue;
                }
                GameObject obj = childRender[j].gameObject;
                MeshFilter filter = obj.GetComponent<MeshFilter>();
                if (filter == null)
                {
                    continue;
                }
                Mesh mesh = filter.sharedMesh;
                Vector3 meshSize = mesh.bounds.size;
                Vector3 scale = obj.transform.lossyScale;
                float yyy = meshSize.z * scale.y;
                BigWorldModelData modelParam = obj.GetComponent<BigWorldModelData>();
                if (modelParam == null)
                {
                    modelParam = obj.AddComponent<BigWorldModelData>();
                }
                modelParam.maxHei = yyy;
                float childMinHei = obj.transform.position.y - yyy / 2f;
                float childMaxHei = obj.transform.position.y + yyy / 2f;
                if (j == 0)
                {
                    groupMinHei = childMinHei;
                    groupMaxHei = childMaxHei;
                }
                else
                {
                    groupMinHei = Mathf.Min(groupMinHei,childMinHei);
                    groupMaxHei = Mathf.Max(groupMaxHei, childMaxHei);
                }
            }
            BigWorldGroupData groupdata = objRoot.GetComponent<BigWorldGroupData>();
            if (groupdata == null)
            {
                groupdata = objRoot.AddComponent<BigWorldGroupData>();
                groupdata.m_startPos = 6.3f;//groupMinHei - 0.5f;
                groupdata.m_endPos = groupMaxHei + 0.1f;
            }


        }
    }

    [MenuItem("Tools/BigWorld/生成地板数据")]
    public static void CreateModelMaxWidth()
    {
        GameObject[] selectObj = Selection.gameObjects;
        for (int i = 0; i < selectObj.Length; i++)
        {
            GameObject currentObj = selectObj[i];
            BigWorldModelData modelData = currentObj.GetComponent<BigWorldModelData>();
            if (currentObj != null)
            {
                GameObject.DestroyImmediate(modelData);
            }
            MeshFilter filter = currentObj.GetComponent<MeshFilter>();
            if (filter == null)
            {
                continue;
            }
            Mesh mesh = filter.sharedMesh;
            Vector3 meshSize = mesh.bounds.size;
            Vector3 scale = currentObj.transform.lossyScale;
            float maxDis = Mathf.Max(meshSize.x * scale.x,meshSize.z * scale.z);
            //Quaternion quater = currentObj.transform.rotation;
            //Debug.Log(quater.eulerAngles);
            Vector3 centerPos = currentObj.transform.rotation * mesh.bounds.center;//Quaternion.Euler(90,180,0) * currentObj.transform.position;
            BigWorldDimianData dimianData = currentObj.GetComponent<BigWorldDimianData>();
            if (dimianData == null)
            {
                dimianData = currentObj.AddComponent<BigWorldDimianData>();
            }
            dimianData.m_centerPos = centerPos;
            dimianData.m_maxDis = maxDis;

            //Debug.Log("center : " + GetCenterPos(mesh));
        }
    }

    private static Vector3 GetCenterPos(Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);
        float maxX = -9999f;
        float minX = 9999f;
        float maxZ = -9999f;
        float minZ = 9999f;
        float maxY = -9999f;
        float minY = 99999f;
        for (int i = 0; i < vertices.Count; i++)
        {
            maxX = Mathf.Max(vertices[i].x,maxX);
            minX = Mathf.Min(vertices[i].x, minX);

            maxZ = Mathf.Max(vertices[i].z, maxZ);
            minZ = Mathf.Min(vertices[i].z, minZ);

            maxY = Mathf.Max(vertices[i].y, maxY);
            minY = Mathf.Min(vertices[i].y, minY);
        }
        return new Vector3((maxX + minX)/2f,(maxY + minY)/2f,(maxZ + minZ)/2f);
    }

    private static void GetModel(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector2 xbound = Vector2.zero;
        Vector2 ybound = Vector2.zero;
        Vector2 zbound = Vector2.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 0)
            {
                xbound.x = vertices[i].x;
                xbound.y = vertices[i].x;

                ybound.x = vertices[i].y;
                ybound.y = vertices[i].y;

                zbound.x = vertices[i].z;
                zbound.y = vertices[i].z;
            }
            else
            {
                xbound.x = Mathf.Min(xbound.x,vertices[i].x);
                xbound.y = Mathf.Max(xbound.y, vertices[i].x);

                ybound.x = Mathf.Min(ybound.x, vertices[i].y);
                ybound.y = Mathf.Max(ybound.y, vertices[i].y);

                zbound.x = Mathf.Min(zbound.x, vertices[i].z);
                zbound.y = Mathf.Max(zbound.y, vertices[i].z);
            }
        }
        Debug.Log("X len : " +( xbound.y - xbound.x));
        Debug.Log("Y len : " + (ybound.y - ybound.x));
        Debug.Log("Z len : " + (zbound.y - zbound.x));
    }
}
