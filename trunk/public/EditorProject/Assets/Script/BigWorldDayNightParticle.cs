using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BigWorldDayNightParticle : MonoBehaviour {

    public float lerp = 0;
    public int index = 0;
    public Color[] m_color;
    void OnEnable()
    {
        if (m_color == null || m_color.Length == 0)
        {
            return;
        }
        MeshRenderer meshRender = gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            meshRender.sharedMaterial.SetColor("_TintColor",m_color[index]);
            meshRender.sharedMaterial.SetColor("_NextColor", m_color[(index + 1)%m_color.Length]);
            meshRender.sharedMaterial.SetFloat("_lerp", lerp);
            return;
        }
        //ParticleSystem particleSys = gameObject.GetComponent<ParticleSystem>();
        ParticleSystemRenderer particleRender = gameObject.GetComponent<ParticleSystemRenderer>();
        if (particleRender != null)
        {
            meshRender.sharedMaterial.SetColor("_TintColor", m_color[index]);
            meshRender.sharedMaterial.SetColor("_NextColor", m_color[(index + 1) % m_color.Length]);
            particleRender.sharedMaterial.SetFloat("_lerp", lerp);
            return;
        }
       
    }
}
