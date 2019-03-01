using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexturePool:MonoBehaviour
{

    static List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

    public static RenderTexture Get(RenderTexture baseRenderTexture)
    {
        return Get(
            baseRenderTexture.width,
            baseRenderTexture.height,
            baseRenderTexture.depth,
            baseRenderTexture.format,
            baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear,
            baseRenderTexture.filterMode,
            baseRenderTexture.wrapMode
            );
    }

    public static RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
    {

        for (int i = 0; i < m_TemporaryRTs.Count; i++)
        {
            if (m_TemporaryRTs[i].width < width || m_TemporaryRTs[i].height < height || depthBuffer != m_TemporaryRTs[i].depth)
                continue;
            else
            {
                RenderTexture rt = m_TemporaryRTs[i];
                m_TemporaryRTs.Remove(m_TemporaryRTs[i]);
                return rt;
            }
        }


        var rc = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw); // add forgotten param rw
        rc.filterMode = filterMode;
        rc.wrapMode = wrapMode;
        rc.name = name;
        return rc;
    }

    public static void Release(RenderTexture rt)
    {
        if (rt == null)
            return;

        if (!m_TemporaryRTs.Contains(rt))
            m_TemporaryRTs.Add(rt);
    }
    public static void Delete(RenderTexture rt)
    {
        if (rt == null)
            return;

        DestroyImmediate(rt);
       // RenderTexture.ReleaseTemporary(rt);
    }
    public static void ReleaseAll()
    {
        var enumerator = m_TemporaryRTs.GetEnumerator();
        while (enumerator.MoveNext())
            RenderTexture.ReleaseTemporary(enumerator.Current);

        m_TemporaryRTs.Clear();
    }
}
