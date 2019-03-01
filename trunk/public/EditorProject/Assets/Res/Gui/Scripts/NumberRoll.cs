using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using EngineCore;

public class NumberRoll : MonoBehaviour {

    private Image m_numberImg;
    public string[] m_NumberStr;
    public float m_speed;
    public int m_FinalIndex = 0;
    public float m_TotalTime = 1f;
    private int index = 0;

    public Sprite[] m_TemplateImage;
    public float m_startDelay = 0f;

    private float m_timesection = 0;
    private float m_totalTimeSection = 0;
    private bool isRun = false;
    private bool isTotalRun = true;
    private System.DateTime startTime;
    
    void Awake()
    {
        m_numberImg = GetComponent<Image>();
    }

	// Use this for initialization
	void Start () {
        startTime = System.DateTime.Now;
        if (m_NumberStr != null && m_NumberStr.Length > 0)
        {
            m_numberImg.sprite = m_TemplateImage[0];
        }
	}

    private void UpdateImage()
    {
        index++;
        if (index == m_NumberStr.Length)
        {
            index = 0;
        }
        m_numberImg.sprite = m_TemplateImage[index];//m_NumberStr[index].Replace(".png", "");
        
        
    }

    // Update is called once per frame
    void Update () {
       
        if (!isTotalRun)
        {
            return;
        }
        m_timesection += Time.deltaTime;
        m_totalTimeSection += Time.deltaTime;

        if (!isRun && m_totalTimeSection >= m_startDelay)
        {
            isRun = true;
            //m_totalTimeSection -= m_startDelay;
        }
        if (!isRun)
        {
            return;
        }
        if (m_timesection >= m_speed)
        {
            UpdateImage();
            m_timesection -= m_speed;
        }
        if (m_totalTimeSection >= m_TotalTime)
        {
            isTotalRun = false;
            //Debug.Log((System.DateTime.Now - startTime).TotalSeconds);
            m_numberImg.sprite = m_TemplateImage[m_FinalIndex];
            return;
        }
        

	}
}
