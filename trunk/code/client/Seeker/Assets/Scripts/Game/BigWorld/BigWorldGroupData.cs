using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BigWorldGroupData : MonoBehaviour {

    public float m_startPos; //开始高度
    public float m_endPos; //结束高度

    public float totalTime = 5f;
    List<Material> childMat;
    private bool m_isPlaying = false;
    public Vector3 m_EffectScale = Vector3.one;
    public Vector3 m_FuncPos = Vector3.zero;
    private Action m_playOver = null;
    public void Start()
    {
        speed = (m_endPos - m_startPos) / totalTime;
    }

    public void PlayEffect(List<Material> mats,Action callback)
    {
        childMat = mats;
        this.m_isPlaying = true;
        this.m_playOver = callback;
    }

    public float timesection = 0f;
    public float speed = 0f;
    public void Update()
    {
        if (!m_isPlaying || childMat == null)
        {
            return;
        }
        timesection += Time.deltaTime * speed;
        for (int i = 0; i < childMat.Count; i++)
        {
            childMat[i].SetFloat("_Clamp", m_endPos - timesection);
        }
        if (timesection >=( m_endPos - m_startPos))
        {
            if (this.m_playOver != null)
            {
                this.m_playOver();
            }
        }
    }
}
