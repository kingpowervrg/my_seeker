using System.Collections.Generic;
using UnityEngine;

public class MeshTools
{
    /// <summary>
    /// 网格系统边缘查找,支持多边缘
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    /// <returns></returns>
    public static List<List<Vector3>> TrianglesAndVerticesEdge(Vector3[] vertices, int[] triangles)
    {
        List<Vector2Int> edgeLines = TrianglesEdgeAnalysis(triangles);
        List<List<Vector3>> result = SpliteLines(edgeLines, vertices);
        return result;
    }

    /// <summary>
    /// 三角面组边缘提取
    /// </summary>
    /// <param name="triangles"></param>
    /// <param name="edges"></param>
    /// <param name="invalidFlag"></param>
    /// <returns></returns>
    public static List<Vector2Int> TrianglesEdgeAnalysis(int[] triangles)
    {
        int[,] edges = new int[triangles.Length, 2];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    int index = (j + k) % 3;
                    edges[i + j, k] = triangles[i + index];
                }
            }
        }
        bool[] invalidFlag = new bool[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            for (int j = i + 1; j < triangles.Length; j++)
            {
                if ((edges[i, 0] == edges[j, 0] && edges[i, 1] == edges[j, 1]) || (edges[i, 0] == edges[j, 1] && edges[i, 1] == edges[j, 0]))
                {
                    invalidFlag[i] = true;
                    invalidFlag[j] = true;
                }
            }
        }
        List<Vector2Int> edgeLines = new List<Vector2Int>();
        for (int i = 0; i < triangles.Length; i++)
        {
            if (!invalidFlag[i])
            {
                edgeLines.Add(new Vector2Int(edges[i, 0], edges[i, 1]));
            }
        }
        if (edgeLines.Count == 0)
        {
            Debug.Log("Calculate wrong, there is not any valid line");
        }
        return edgeLines;
    }

    /// <summary>
    /// 边缘排序与分离
    /// </summary>
    /// <param name="edgeLines"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static List<List<Vector3>> SpliteLines(List<Vector2Int> edgeLines, Vector3[] vertices)
    {
        List<List<Vector3>> result = new List<List<Vector3>>();

        List<int> edgeIndex = new List<int>();
        int startIndex = edgeLines[0].x;
        edgeIndex.Add(edgeLines[0].x);
        int removeIndex = 0;
        int currentIndex = edgeLines[0].y;

        while (true)
        {
            edgeLines.RemoveAt(removeIndex);
            edgeIndex.Add(currentIndex);

            bool findNew = false;
            for (int i = 0; i < edgeLines.Count && !findNew; i++)
            {
                if (currentIndex == edgeLines[i].x)
                {
                    currentIndex = edgeLines[i].y;
                    removeIndex = i;
                    findNew = true;
                }
                else if (currentIndex == edgeLines[i].y)
                {
                    currentIndex = edgeLines[i].x;
                    removeIndex = i;
                    findNew = true;
                }
            }

            if (findNew && currentIndex == startIndex)
            {
                Debug.Log("Complete Closed curve");
                edgeLines.RemoveAt(removeIndex);
                List<Vector3> singleVertices = new List<Vector3>();
                for (int i = 0; i < edgeIndex.Count; i++)
                    singleVertices.Add(vertices[edgeIndex[i]]);
                result.Add(singleVertices);

                if (edgeLines.Count > 0)
                {
                    edgeIndex = new List<int>();
                    startIndex = edgeLines[0].x;
                    edgeIndex.Add(edgeLines[0].x);
                    removeIndex = 0;
                    currentIndex = edgeLines[0].y;
                }
                else
                {
                    break;
                }
            }
            else if (!findNew)
            {
                Debug.Log("Complete curve, but not closed");
                List<Vector3> singleVertices = new List<Vector3>();
                for (int i = 0; i < edgeIndex.Count; i++)
                    singleVertices.Add(vertices[edgeIndex[i]]);
                result.Add(singleVertices);

                if (edgeLines.Count > 0)
                {
                    edgeIndex = new List<int>();
                    startIndex = edgeLines[0].x;
                    edgeIndex.Add(edgeLines[0].x);
                    removeIndex = 0;
                    currentIndex = edgeLines[0].y;
                }
                else
                {
                    break;
                }
            }
        }

        return result;
    }
}
