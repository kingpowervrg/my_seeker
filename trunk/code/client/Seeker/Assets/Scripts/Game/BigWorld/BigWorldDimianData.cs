using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BigWorldDimianData : MonoBehaviour {

    public Vector3 m_centerPos = Vector3.zero;
    public float m_maxDis = 1f;

    Action callback = null;
    Material mat = null;
    private bool isPlayEffect = false;
    public void PlayEffect(Action cb,Material mat)
    {
        callback = cb;
        this.mat = mat;
        this.mat.SetVector("_centerWorldPos", m_centerPos);
        this.mat.SetFloat("_maxDis", m_maxDis);
        isPlayEffect = true;
        
    }
    float timesection = 0f;
	// Update is called once per frame
	void Update () {

        if (!isPlayEffect)
        {
            return;
        }
        timesection += Time.deltaTime;
        if (timesection <= 1f)
        {
            mat.SetFloat("_Factor", timesection);
            return;
        }
        if (callback != null)
        {
            callback();
        }
        isPlayEffect = false;
        
	}
}
