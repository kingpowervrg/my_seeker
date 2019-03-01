using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CartoonTemplate : MonoBehaviour
{
    public float m_min_width = 150.0f;
    public float m_min_height = 150.0f;

    public long m_template_id = 1001L;

    public RectTransform m_play_rect;
    //[HideInInspector]
    public List<CartoonFixed> m_cartoon_items;


    public void Init()
    {
        m_play_rect = this.transform.Find("Play_Rect").GetComponent<RectTransform>();
        m_cartoon_items = new List<CartoonFixed>();
        foreach (Transform child in m_play_rect.transform)
        {
            CartoonFixed item = child.GetComponent<CartoonFixed>();
            m_cartoon_items.Add(item);
        }
    }

    public void LoadVideos(CartoonItemWithClips videos_)
    {
        if (this.m_template_id != videos_.Item_id)
        {
            Debug.LogError(string.Format("{0}号动漫加载的是{1}号动漫的视频", m_template_id, videos_.Item_id));
            return;
        }

        if (null == m_cartoon_items || 0 == m_cartoon_items.Count)
            this.Init();

        if (m_cartoon_items.Count != videos_.M_Items.Count)
        {
            Debug.LogError(string.Format("{0}号动漫有模块{1}个，加载的视频模块有{2}个", m_template_id, m_cartoon_items.Count, videos_.M_Items.Count));
            return;
        }

        for (int i = 0; i < m_cartoon_items.Count && i < videos_.M_Items.Count; ++i)
        {
            CartoonFixed item = m_cartoon_items[i];
            CartoonClips video = videos_.M_Items[i];

            item.m_videos = video.M_clips;

        }
    }

}

