using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOGUI;
using UnityEngine;
using SeekerGame;
class CartoonSquareRotateView : CartoonFixedView
{
    private readonly Quaternion C_LEFT_ANG = Quaternion.Euler(new Vector3(0, 0, 0));
    private readonly Quaternion C_DOWN_ANG = Quaternion.Euler(new Vector3(0, 0, 90));
    private readonly Quaternion C_RIGHT_ANG = Quaternion.Euler(new Vector3(0, 0, 180));
    private readonly Quaternion C_UP_ANG = Quaternion.Euler(new Vector3(0, 0, 270));

    protected Func<CartoonSquareRotateView, CartoonRotate, ENUM_ROTATE_DIR, bool> IsCanDrag;

    private Camera m_cam;

    public override void SetModel(CartoonFixed model_)
    {
        base.SetModel(model_);
        DragEventTriggerListener.Get(gameObject).onDrag = Dragging;
        DragEventTriggerListener.Get(gameObject).onDragEnd = DragEnd;
        this.m_dir = this.CalcCurDir();
        m_cam = CameraManager.Instance.UICamera;
    }
    public void RegisterCanTurn(Func<CartoonSquareRotateView, CartoonRotate, ENUM_ROTATE_DIR, bool> fun_)
    {
        IsCanDrag = fun_;
    }
    protected virtual void DirChanged()
    {
    }

    //private void OnClick(GameObject obj)
    //{

    //    ((CartoonRotate)(this.m_model)).ClockwiseRotate();

    //    Vector3 next_ang = this.transform.localEulerAngles + new Vector3(0.0f, 0.0f, -90.0f);

    //    CustomTweenRotation.Begin(this.gameObject, 0.5f, Quaternion.Euler(next_ang));

    //}

    public void Dragging(GameObject go, Vector2 delta, Vector2 pos)
    {
        Vector2 From = pos + delta;

        Vector2 local_pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_rect, From, m_cam, out local_pos))
        {
            From = local_pos;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_rect, pos, m_cam, out local_pos))
            {
                Vector2 to = local_pos;

                Vector3 normal = Vector3.Cross(to, From);
                Vector3 normal_one = normal.normalized;
                Vector3 delta_ang = new Vector3(0, 0, Vector2.Angle(From, to) * normal_one.z);

                //Debug.Log("delta ang " + delta_ang + "normal one " + normal_one.z);

                if (null != IsCanDrag)
                {
                    ENUM_ROTATE_DIR r_dir = delta_ang.z < 0.0f ? ENUM_ROTATE_DIR.E_CLOCKWISE : ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE;
                    if (false == IsCanDrag(this, (CartoonRotate)this.m_model, r_dir))
                    {
                        if (CanTurnButNotChangeDir(delta_ang.z, r_dir))
                        {
                            transform.Rotate(delta_ang, Space.World);
                        }
                        return;
                    }
                }

                transform.Rotate(delta_ang, Space.World);

                ENUM_ITEM_DIRECTION next_dir = this.CalcCurDir();

                if (this.m_dir != next_dir)
                {
                    this.m_dir = next_dir;

                    if (normal_one.z < 0)
                    {
                        ((CartoonRotate)(this.m_model)).ClockwiseRotate();
                    }
                    else
                    {
                        ((CartoonRotate)(this.m_model)).AntiClockwiseRotate();
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

    private bool CanTurnButNotChangeDir(float delta_ang_, ENUM_ROTATE_DIR turn_dir_)
    {
        float next_ang = CalcCurAng() + delta_ang_;

        ENUM_ITEM_DIRECTION cur_dir = this.CalcCurDir();

        switch (cur_dir)
        {
            case ENUM_ITEM_DIRECTION.E_DOWN:
                if (ENUM_ROTATE_DIR.E_CLOCKWISE == turn_dir_)
                {
                    if (90.0f <= next_ang && next_ang <= 135.0f)
                        return true;
                }
                else
                {
                    if (45.0f <= next_ang && next_ang <= 90.0f)
                        return true;
                }
                break;
            case ENUM_ITEM_DIRECTION.E_LEFT:
                if (ENUM_ROTATE_DIR.E_CLOCKWISE == turn_dir_)
                {
                    if (0.0f <= next_ang && next_ang <= 45.0f)
                        return true;
                }
                else
                {
                    if (315.0f <= next_ang && next_ang <= 360.0f)
                        return true;
                }
                break;
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                if (ENUM_ROTATE_DIR.E_CLOCKWISE == turn_dir_)
                {
                    if (180.0f <= next_ang && next_ang <= 225.0f)
                        return true;
                }
                else
                {
                    if (135.0f <= next_ang && next_ang <= 180.0f)
                        return true;
                }
                break;
            case ENUM_ITEM_DIRECTION.E_UP:
                if (ENUM_ROTATE_DIR.E_CLOCKWISE == turn_dir_)
                {
                    if (270.0f <= next_ang && next_ang <= 315.0f)
                        return true;
                }
                else
                {
                    if (225.0f <= next_ang && next_ang <= 270.0f)
                        return true;
                }
                break;
        }

        return false;
    }

    private float CalcTheAng(float ang_)
    {
        float cur_ang = ang_ % 360.0f;

        if (cur_ang < 0.0f)
        {
            cur_ang += 360.0f;
        }

        return cur_ang;
    }

    private float CalcCurAng()
    {
        float cur_ang = this.transform.localRotation.eulerAngles.z % 360.0f;

        if (cur_ang < 0.0f)
        {
            cur_ang += 360.0f;
        }

        return cur_ang;
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

