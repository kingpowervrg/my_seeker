using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GOGUI;
using EngineCore;

public class NumberRoll : MonoBehaviour {

    //private Image m_numberImg;
    private LazyLoadImage m_Image;
    public string[] m_NumberStr;
    public float m_speed;
    public int m_FinalIndex = 0;
    public float m_TotalTime = 1f;
    private int index = 0;

    public float m_startDelay = 0f;
    // Use this for initialization
    void Start () {
        m_Image = GetComponent<LazyLoadImage>();
        if (m_NumberStr != null && m_NumberStr.Length > 0)
        {
            m_Image.SpriteName = m_NumberStr[0];
        }
        TimeModule.Instance.SetTimeout(()=> {
            TimeModule.Instance.SetTimeout(StopImage, m_TotalTime);
            TimeModule.Instance.SetTimeout(UpdateImage, m_speed, true, true);
        },m_startDelay);

        
	}

    private void UpdateImage()
    {
        index++;
        if (index == m_NumberStr.Length)
        {
            index = 0;
        }
        m_Image.SpriteName = m_NumberStr[index];
        
        //if (isStop && index == m_FinalIndex)
        //{
        //    TimeModule.Instance.RemoveTimeaction(UpdateImage);
        //}
    }
    private bool isStop = false;

    private void StopImage()
    {
        isStop = true;
        TimeModule.Instance.RemoveTimeaction(UpdateImage);

        m_Image.SpriteName = m_NumberStr[m_FinalIndex];
        //if (index == m_FinalIndex)
        //{
        //}
    }

    void Disable()
    {
        Debug.Log("remove UpdateImage");
        TimeModule.Instance.RemoveTimeaction(StopImage);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
