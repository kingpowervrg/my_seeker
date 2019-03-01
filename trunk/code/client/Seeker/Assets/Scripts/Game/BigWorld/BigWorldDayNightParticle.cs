using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeekerGame;

public class BigWorldDayNightParticle : MonoBehaviour
{
    public Color[] m_color;
    MeshRenderer meshRender = null;
    ParticleSystemRenderer particleRender = null;
    void OnEnable()
    {
        if (m_color == null || m_color.Length == 0)
        {
            return;
        }
        GameEvents.BigWorld_Event.OnReflashTime += OnReflashTime;
        float lerp = CommonTools.GetCurrentTimePercent();
        meshRender = gameObject.GetComponent<MeshRenderer>();
        if (meshRender != null)
        {
            return;
        }
        //ParticleSystem particleSys = gameObject.GetComponent<ParticleSystem>();
        particleRender = gameObject.GetComponent<ParticleSystemRenderer>();
        

    }

    void OnReflashTime(int index,float lerp)
    {
        if (m_color == null || m_color.Length == 0)
        {
            return;
        }
        if (meshRender != null && meshRender.sharedMaterial != null)
        {
            meshRender.sharedMaterial.SetColor("_TintColor", m_color[index]);
            meshRender.sharedMaterial.SetColor("_NextColor", m_color[(index + 1) % (m_color.Length - 1)]);
            meshRender.sharedMaterial.SetFloat("_lerp", lerp);
            return;
        }
        if (particleRender != null && meshRender.sharedMaterial != null)
        {
            meshRender.sharedMaterial.SetColor("_TintColor", m_color[index]);
            meshRender.sharedMaterial.SetColor("_NextColor", m_color[(index + 1) % (m_color.Length - 1)]);
            particleRender.sharedMaterial.SetFloat("_lerp", lerp);
            return;
        }
    }

    void OnDisable()
    {
        GameEvents.BigWorld_Event.OnReflashTime -= OnReflashTime;
    }
}

//namespace SeekerGame
//{
    
//}

