using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOGUI;
using UnityEngine;
using SeekerGame;

class CartoonLongRotateView : CartoonFixedView
{
    private readonly Quaternion C_LEFT_ANG = Quaternion.Euler(new Vector3(0, 0, 0));
    private readonly Quaternion C_DOWN_ANG = Quaternion.Euler(new Vector3(0, 0, 90));
    private readonly Quaternion C_RIGHT_ANG = Quaternion.Euler(new Vector3(0, 0, 180));
    private readonly Quaternion C_UP_ANG = Quaternion.Euler(new Vector3(0, 0, 270));

    /// <summary>
    /// <我的块序号，我当前方向，我要按何种方式旋转>
    /// </summary>
    protected Func<CartoonLongRotateView, CartoonRotate, ENUM_ROTATE_DIR, bool> IsCanDrag;
    protected Action<CartoonLongRotateView, CartoonRotate> OnDragging;
    //protected Action<CartoonLongRotateView, CartoonRotate> OnDragEnd;

    public int m_tail_index;
    public int Tail_index
    {
        get { return m_tail_index; }
        set { m_tail_index = value; }
    }

    private Camera m_cam;

    public override void SetModel(CartoonFixed model_)
    {
        base.SetModel(model_);
        //DragEventTriggerListener.Get(gameObject).onDragStart = DragBegin;
        DragEventTriggerListener.Get(gameObject).onDrag = Dragging;
        DragEventTriggerListener.Get(gameObject).onDragEnd = DragEnd;

        this.m_dir = this.CalcCurDir();

        m_cam = CameraManager.Instance.UICamera;
    }

    public override List<int> SetAnchorPosAndReturnOccupyIndex(Vector2 anchor_pos_, int index_, int w_, int h_)
    {

        if (ENUM_ITEM_DIRECTION.E_LEFT == this.m_dir)
        {
            if (index_ % w_ <= w_ - this.m_model.m_width_unit)
            {
                this.Item_index = CartoonUtil.GetRightIndex(index_, this.m_model.m_width_unit, w_, h_);
                this.m_rect.anchoredPosition = new Vector2(anchor_pos_.x + (this.m_model.m_width_unit - 1) * (this.m_rect.sizeDelta.x / this.m_model.m_width_unit), anchor_pos_.y);

                List<int> ret = new List<int>();

                ret.Add(this.Item_index);
                ret.Add(index_);



                this.Tail_index = ret.Last();
                return ret;

            }
            Debug.LogError(string.Format("序号{0} 指向左侧的转动长方形，位置摆放错误", index_));

        }
        else if (ENUM_ITEM_DIRECTION.E_UP == this.m_dir)
        {
            if (index_ / w_ <= h_ - this.m_model.m_width_unit)
            {
                this.Item_index = CartoonUtil.GetDownIndex(index_, this.m_model.m_width_unit, w_, h_);
                this.m_rect.anchoredPosition = new Vector2(anchor_pos_.x, anchor_pos_.y - (this.m_model.m_width_unit - 1) * (this.m_rect.sizeDelta.x / this.m_model.m_width_unit));

                List<int> ret = new List<int>();

                ret.Add(this.Item_index);
                ret.Add(index_);


                this.Tail_index = ret.Last();
                return ret;

            }



        }
        else if (ENUM_ITEM_DIRECTION.E_RIGHT == this.m_dir)
        {
            if (index_ % w_ <= w_ - this.m_model.m_width_unit)
            {
                this.Item_index = index_;
                this.m_rect.anchoredPosition = anchor_pos_;

                List<int> ret = new List<int>();


                ret.Add(this.Item_index);
                ret.Add(CartoonUtil.GetRightIndex(index_, this.m_model.m_width_unit, w_, h_));


                this.Tail_index = ret.Last();
                return ret;

            }

        }
        else if (ENUM_ITEM_DIRECTION.E_DOWN == this.m_dir)
        {
            if (index_ / w_ <= h_ - this.m_model.m_width_unit)
            {
                this.Item_index = index_;
                this.m_rect.anchoredPosition = anchor_pos_;

                List<int> ret = new List<int>();

                ret.Add(this.Item_index);
                ret.Add(CartoonUtil.GetDownIndex(index_, this.m_model.m_width_unit, w_, h_));


                this.Tail_index = ret.Last();
                return ret;

            }

        }

        return null;
    }

    public void RegisterCanDrag(Func<CartoonLongRotateView, CartoonRotate, ENUM_ROTATE_DIR, bool> judge_)
    {
        IsCanDrag = judge_;
    }


    public void RegisterDragging(Action<CartoonLongRotateView, CartoonRotate> act_)
    {
        OnDragging = act_;
    }


    //public void DragBegin(GameObject go, Vector2 pos)
    //{



    //}


    public void Dragging(GameObject go, Vector2 delta, Vector2 pos)
    {
        Vector2 From = pos + delta;

        Vector2 local_pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_rect, From, m_cam , out local_pos))
        {
            From = local_pos;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_rect, pos, m_cam, out local_pos))
            {
                Vector2 to = local_pos;

                Vector3 normal = Vector3.Cross(to, From);

                Vector3 delta_ang = new Vector3(0, 0, Vector2.Angle(From, to) * normal.normalized.z);

                //Debug.Log("delta ang " + delta_ang);

                if (null != IsCanDrag)
                {
                    ENUM_ROTATE_DIR r_dir = delta_ang.z < 0.0f ? ENUM_ROTATE_DIR.E_CLOCKWISE : ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE;
                    if (false == IsCanDrag(this, (CartoonRotate)this.m_model, r_dir))
                    {
                        return;
                    }
                }

                transform.Rotate(delta_ang, Space.World);

                ENUM_ITEM_DIRECTION next_dir = this.CalcCurDir();

                if (this.m_dir != next_dir)
                {
                    this.m_dir = next_dir;

                    if (null != OnDragging)
                    {
                        OnDragging(this, (CartoonRotate)this.m_model);
                    }
                }
            }
        }
    }
    public void DragEnd(GameObject go, Vector2 pos)
    {

        GoBack();
        //if (null != OnDragEnd)
        //{
        //    OnDragEnd(this, (CartoonRotate)this.m_model);
        //}
    }

    private ENUM_ITEM_DIRECTION CalcCurDir()
    {
        float cur_ang = this.transform.localRotation.eulerAngles.z % 360.0f;

        if (cur_ang < 0.0f)
        {
            cur_ang += 360.0f;
        }

        if (0 <= cur_ang && cur_ang < 45.0f || 315.0f <= cur_ang && cur_ang < 360.0f)
        {
            return ENUM_ITEM_DIRECTION.E_LEFT;
        }
        else if (45.0f <= cur_ang && cur_ang < 135.0f)
        {
            return ENUM_ITEM_DIRECTION.E_DOWN;
        }
        else if (135.0f <= cur_ang && cur_ang < 225.0f)
        {
            return ENUM_ITEM_DIRECTION.E_RIGHT;
        }
        else if (225.0f <= cur_ang && cur_ang < 315.0f)
        {
            return ENUM_ITEM_DIRECTION.E_UP;
        }

        return ENUM_ITEM_DIRECTION.E_NONE;

    }

    private void GoBack()
    {
        switch (this.m_dir)
        {
            case ENUM_ITEM_DIRECTION.E_LEFT:
                {
                    CustomTweenRotation.Begin(this.gameObject, 0.5f, C_LEFT_ANG);
                }
                break;
            case ENUM_ITEM_DIRECTION.E_DOWN:
                {
                    CustomTweenRotation.Begin(this.gameObject, 0.5f, C_DOWN_ANG);
                }
                break;
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                {
                    CustomTweenRotation.Begin(this.gameObject, 0.5f, C_RIGHT_ANG);
                }
                break;
            case ENUM_ITEM_DIRECTION.E_UP:
                {
                    CustomTweenRotation.Begin(this.gameObject, 0.5f, C_UP_ANG);
                }
                break;
        }
    }

    public override void Reset()
    {
        base.Reset();
        this.m_dir = this.CalcCurDir();
    }

}

