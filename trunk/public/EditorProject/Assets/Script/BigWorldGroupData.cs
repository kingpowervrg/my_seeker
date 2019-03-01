using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWorldGroupData : MonoBehaviour {

    public float m_startPos; //开始高度
    public float m_endPos; //结束高度

    public float totalTime = 5f;
    public Vector3 m_EffectScale = Vector3.zero;
    public Vector3 m_FuncPos = Vector3.zero;
    List<Material> childMat;
    public void Start()
    {
        childMat = new List<Material>();
        MeshRenderer[] render = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < render.Length; i++)
        {
            Material mat = render[i].material;
            if (mat.name.Contains("UnLock"))
            {
                childMat.Add(mat);
                BigWorldModelData modeldata = render[i].gameObject.GetComponent<BigWorldModelData>();
                mat.SetFloat("_MaxHei", modeldata.maxHei);
            }
        }
        speed = (m_endPos - m_startPos) / totalTime;
    }
    public float timesection = 0f;
    public float speed = 0f;
    public void Update()
    {
        timesection += Time.deltaTime * speed;
        for (int i = 0; i < childMat.Count; i++)
        {
            childMat[i].SetFloat("_Clamp", m_endPos - timesection);
        }
        if (timesection >=( m_endPos - m_startPos))
        {
            timesection = 0f;
        }
    }
}
