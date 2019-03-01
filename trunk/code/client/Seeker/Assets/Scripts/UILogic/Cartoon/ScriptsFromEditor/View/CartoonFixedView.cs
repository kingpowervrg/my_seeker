using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

public class CartoonFixedView : MonoBehaviour
{

    public RectTransform m_rect;
    public ENUM_ITEM_DIRECTION m_dir = ENUM_ITEM_DIRECTION.E_NONE;
    public int m_item_index; //我在panel的摆放位置序号
    public int Item_index
    {
        set { m_item_index = value; }
        get { return m_item_index; }
    }

    public Action<int> OnVideoFinished;

    protected CartoonFixed m_model;

    private CartoonPlayer m_player;

    private int m_cur_exit;

    private Quaternion m_ori_rot;

    public CartoonFixedView()
    {

    }

    public virtual void SetModel(CartoonFixed model_)
    {
        this.m_model = model_;

        m_player = this.transform.GetComponentInChildren<CartoonPlayer>();
        m_player.Init(this.m_model.GetFirstClip(), (int)this.m_rect.sizeDelta.x, (int)this.m_rect.sizeDelta.y, OnCartoonPlayerFinished);

    }

    void Awake()
    {
        m_rect = this.GetComponent<RectTransform>();
        m_ori_rot = m_rect.localRotation;
    }

    public virtual List<int> SetAnchorPosAndReturnOccupyIndex(Vector2 anchor_pos_, int index_, int w_, int h_)
    {
        if (0 <= index_ && index_ < w_ * h_)
        {
            this.Item_index = index_;
            this.m_rect.anchoredPosition = anchor_pos_;
            return new List<int> { index_ };

        }

        return null;

    }


    public void Play(int pre_exit_, bool is_first = false)
    {
        VideoClip cur_clip;
        int cur_exit_;
        if (this.m_model.GetVideo(pre_exit_, out cur_clip, out cur_exit_, is_first))
        {
            this.m_player.Play(cur_clip);
            m_cur_exit = cur_exit_;
        }
        else
        {
            Debug.LogError(string.Format("上一个出口 {0}， View {1} 没有对应的入口视频", pre_exit_, this.Item_index));
        }

    }

    public virtual void Reset()
    {
        m_rect.localRotation = m_ori_rot;
        this.m_player.Reset();
    }

    private void OnCartoonPlayerFinished()
    {
        if (null != OnVideoFinished)
        {
            OnVideoFinished(m_cur_exit);
        }
    }

}

