using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraPoint : MonoBehaviour
{
    public GameObject m_cameraObj;
    public GameObject m_normalParticle;
    public GameObject m_playParticle;
    public bool m_canZoom;

    void Awake()
    {
        this.m_normalParticle.SetActive(true);
        this.m_playParticle.SetActive(false);
    }
}
