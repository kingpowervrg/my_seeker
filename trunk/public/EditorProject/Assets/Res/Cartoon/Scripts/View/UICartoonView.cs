using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ENUM_CARTOON_ITEM_TYPE
{
    E_INVALID,
    E_SQUARE_FIXED, //固定正方
    E_SQUARE_DRAG, //可移动正方
    E_SQUARE_ROTATE, //可旋转正方
    E_LONGRECT_ROTATE, //可旋转长方形

}

public enum ENUM_ITEM_DIRECTION
{
    E_NONE,
    E_UP,
    E_RIGHT,
    E_DOWN,
    E_LEFT,
}

public enum ENUM_ROTATE_DIR
{
    E_CLOCKWISE,
    E_ANTI_CLOCKWISE,
}

public class UICartoonView : MonoBehaviour
{
    private class AnchorPos
    {
        public Vector2 m_pos;
        public int m_index_on_panel;
        public int cur_ocuppied_cartoon_item_id;
    }

    CartoonTemplate m_cartoon;
    Vector3[] m_play_rect_4_corners = new Vector3[4];
    private int anchor_w_size;
    private int anchor_h_size;

    private List<AnchorPos> m_anchor_positions;
    private Dictionary<int, CartoonFixedView> m_cartoon_views;
    private List<int> m_played_views;
    private int m_cur_index_playing;
    private float m_min_cartoon_width_half;
    private float m_min_cartoon_height_half;

    private void Start()
    {
        m_cartoon = this.gameObject.GetComponentInChildren<CartoonTemplate>();
        m_cartoon.Init();
        m_cartoon.m_play_rect.GetWorldCorners(m_play_rect_4_corners);

        this.InitCartoonItemAnchorPosition();
        this.InitCartoonItemPosition();
    }

    private void InitCartoonItemAnchorPosition()
    {
        float panel_width = m_cartoon.m_play_rect.sizeDelta.x;
        float panel_height = m_cartoon.m_play_rect.sizeDelta.y;

        if ((0 != ((int)panel_width) % ((int)(m_cartoon.m_min_width))) || (0 != ((int)panel_height) % ((int)(m_cartoon.m_min_height))))
        {
            Debug.LogError("可操作区域，不是最小卡通图尺寸的整数倍");
            return;
        }

        anchor_w_size = (int)(panel_width / m_cartoon.m_min_width);
        anchor_h_size = (int)(panel_height / m_cartoon.m_min_height);

        float half_w = panel_width * 0.5f;
        float half_h = panel_height * 0.5f;

        m_min_cartoon_width_half = m_cartoon.m_min_width * 0.5f;
        m_min_cartoon_height_half = m_cartoon.m_min_height * 0.5f;

        m_anchor_positions = new List<AnchorPos>();

        int index = 0;
        for (int j = anchor_h_size - 1; j >= 0; --j)
            for (int i = 0; i < anchor_w_size; ++i)
            {
                float pos_x = m_cartoon.m_min_width * i + m_cartoon.m_min_width * 0.5f - half_w;
                float pos_y = m_cartoon.m_min_height * j + m_cartoon.m_min_height * 0.5f - half_h;
                AnchorPos anchor = new AnchorPos();
                anchor.m_pos = new Vector2(pos_x, pos_y);
                anchor.m_index_on_panel = index;
                anchor.cur_ocuppied_cartoon_item_id = -1;
                m_anchor_positions.Add(anchor);
                ++index;
            }

    }




    private void InitCartoonItemPosition()
    {
        m_cartoon_views = new Dictionary<int, CartoonFixedView>();
        m_played_views = new List<int>();
        m_cur_index_playing = 0;

        int index = 0;
        for (int i = 0; i < m_cartoon.m_cartoon_items.Count; ++i)
        {
            CartoonFixed item = m_cartoon.m_cartoon_items[i];
            item.Init();
            item.m_item_id = i;

            CartoonFixedView item_view = null;

            if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_FIXED == item.m_item_type)
            {
                item_view = item.gameObject.AddComponent<CartoonFixedView>();
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_DRAG == item.m_item_type)
            {
                item_view = item.gameObject.AddComponent<CartoonDragView>();
                ((CartoonDragView)item_view).ReisterMoving(Moving);
                ((CartoonDragView)item_view).ReisterMoveEnd(MoveEnd);
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_ROTATE == item.m_item_type)
            {
                item_view = item.gameObject.AddComponent<CartoonSquareRotateView>();
                ((CartoonSquareRotateView)item_view).RegisterCanTurn(CanTurnAround);
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_LONGRECT_ROTATE == item.m_item_type)
            {
                item_view = item.gameObject.AddComponent<CartoonLongRotateView>();
                ((CartoonLongRotateView)item_view).RegisterCanDrag(CanDrag);
                ((CartoonLongRotateView)item_view).RegisterDragging(Dragging);
            }


            if (null == item_view)
            {
                Debug.LogError("找不到漫画种类" + item.name);
                return;
            }

            item_view.SetModel(item);

            m_cartoon_views[item.m_item_id] = item_view;

            item_view.OnVideoFinished = PlayNext;

            AnchorPos pos_on_panel = m_anchor_positions[index];

            while (-1 != pos_on_panel.cur_ocuppied_cartoon_item_id)
            {
                ++index;
                pos_on_panel = m_anchor_positions[index];
            }

            if (!this.ChangeAnchorPos(item_view, item, index, pos_on_panel.m_pos))
                return;

        }

        for (int i = 0; i < m_anchor_positions.Count; ++i)
        {
            Debug.Log(string.Format("序列{0}，cartoon id {1}", i, m_anchor_positions[i].cur_ocuppied_cartoon_item_id));
        }
    }



    private void ResetCartoonItemPosition()
    {
        m_cartoon_views = new Dictionary<int, CartoonFixedView>();
        m_played_views = new List<int>();
        m_cur_index_playing = 0;

        int index = 0;
        for (int i = 0; i < m_cartoon.m_cartoon_items.Count; ++i)
        {
            CartoonFixed item = m_cartoon.m_cartoon_items[i];
            item.Init();
            item.m_item_id = i;

            CartoonFixedView item_view = null;

            if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_FIXED == item.m_item_type)
            {
                item_view = item.gameObject.GetComponent<CartoonFixedView>();
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_DRAG == item.m_item_type)
            {
                item_view = item.gameObject.GetComponent<CartoonDragView>();
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_ROTATE == item.m_item_type)
            {
                item_view = item.gameObject.GetComponent<CartoonSquareRotateView>();
            }
            else if (ENUM_CARTOON_ITEM_TYPE.E_LONGRECT_ROTATE == item.m_item_type)
            {
                item_view = item.gameObject.GetComponent<CartoonLongRotateView>();
            }


            if (null == item_view)
            {
                Debug.LogError("找不到漫画的view" + item.name);
                return;
            }

            m_cartoon_views[item.m_item_id] = item_view;

            AnchorPos pos_on_panel = m_anchor_positions[index];

            while (-1 != pos_on_panel.cur_ocuppied_cartoon_item_id)
            {
                ++index;
                pos_on_panel = m_anchor_positions[index];
            }

            if (!this.ChangeAnchorPos(item_view, item, index, pos_on_panel.m_pos))
                return;

        }

        for (int i = 0; i < m_anchor_positions.Count; ++i)
        {
            Debug.Log(string.Format("序列{0}，cartoon id {1}", i, m_anchor_positions[i].cur_ocuppied_cartoon_item_id));
        }
    }

    public void Moving(CartoonDragView view_, CartoonFixed model_)
    {
        int my_index = view_.Item_index;
        int my_id = model_.m_item_id;
        AnchorPos exchanged = GetNearestNeighbour(my_id, my_index, view_.m_rect.anchoredPosition);

        if (null != exchanged)
        {
            int exchanged_id = exchanged.cur_ocuppied_cartoon_item_id;
            int exchanged_index = exchanged.m_index_on_panel;

            CartoonFixedView exchange_view = m_cartoon_views[exchanged_id];
            CartoonFixed exchange_item = m_cartoon.m_cartoon_items[exchanged_id];

            //来我的位置
            this.ChangeAnchorPos(exchange_view, exchange_item, my_index, m_anchor_positions[my_index].m_pos);

            //他的anchor记录换成我
            m_anchor_positions[exchanged_index].cur_ocuppied_cartoon_item_id = my_id;
            //我的anchor记录换成他
            view_.Item_index = exchanged_index;
        }
    }

    public void MoveEnd(CartoonDragView view_, CartoonFixed model_)
    {
        this.ChangeAnchorPos(view_, model_, view_.Item_index, m_anchor_positions[view_.Item_index].m_pos);

        for (int i = 0; i < m_anchor_positions.Count; ++i)
        {
            Debug.Log(string.Format("序列{0}，cartoon id {1}", i, m_anchor_positions[i].cur_ocuppied_cartoon_item_id));
        }
    }

    private bool ChangeAnchorPos(CartoonFixedView item_view, CartoonFixed item, int next_index, Vector2 next_anchor_pos)
    {
        List<int> occupy_indexes = item_view.SetAnchorPosAndReturnOccupyIndex(next_anchor_pos, next_index, anchor_w_size, anchor_h_size);

        if (null == occupy_indexes)
        {
            Debug.LogError("此次移动会超过边界");
            return false;
        }

        foreach (int occupy_idx in occupy_indexes)
        {
            m_anchor_positions[occupy_idx].cur_ocuppied_cartoon_item_id = item.m_item_id;
        }

        return true;
    }

    private AnchorPos GetNearestNeighbour(int my_id_, int my_index_, Vector2 anchor_pos_)
    {
        Vector2 my_ori_anchor_pos = m_anchor_positions[my_index_].m_pos;

        if (Mathf.Abs(anchor_pos_.x - my_ori_anchor_pos.x) > m_min_cartoon_width_half
            || Mathf.Abs(anchor_pos_.y - my_ori_anchor_pos.y) > m_min_cartoon_height_half)
        {
            //出轨

            for (int i = 0; i < m_anchor_positions.Count; ++i)
            {
                if (i == my_index_)
                    continue;

                AnchorPos neighbour = m_anchor_positions[i];
                if (neighbour.cur_ocuppied_cartoon_item_id == my_id_)
                {
                    Debug.LogError(string.Format("index {0},id {1}", my_index_, my_id_));
                    continue;
                }

                Vector2 neighbour_pos = neighbour.m_pos;

                if (Mathf.Abs(anchor_pos_.x - neighbour_pos.x) < m_min_cartoon_width_half &&
                    Mathf.Abs(anchor_pos_.y - neighbour_pos.y) < m_min_cartoon_height_half)
                {
                    ENUM_CARTOON_ITEM_TYPE c_type = m_cartoon.m_cartoon_items[neighbour.cur_ocuppied_cartoon_item_id].m_item_type;

                    if (ENUM_CARTOON_ITEM_TYPE.E_SQUARE_DRAG != c_type)
                        continue;


                    return neighbour;
                }


            }

            return null;
        }
        else
        {
            return null;
        }


    }



    #region 旋转



    private bool CanBeExchanged(int index_)
    {
        int model_id = this.m_anchor_positions[index_].cur_ocuppied_cartoon_item_id;

        return ENUM_CARTOON_ITEM_TYPE.E_SQUARE_DRAG == this.m_cartoon.m_cartoon_items[model_id].m_item_type;
    }

    private enum ENUM_JUDGE_TYPE
    {
        E_X,
        E_Y,
    }

    private bool JudgeOverflow(Vector3[] mine_, Vector3[] limit_, ENUM_JUDGE_TYPE type_)
    {
        Vector2 corner0 = mine_[0];
        Vector2 corner1 = mine_[1];


        Vector2 limit_left_top = limit_[1];
        Vector3 limit_right_bottom = limit_[3];

        if (ENUM_JUDGE_TYPE.E_X == type_)
        {
            if ((int)corner0.x < (int)limit_left_top.x || (int)corner1.x < (int)limit_left_top.x
            || (int)corner0.x > (int)limit_right_bottom.x || (int)corner1.x > (int)limit_right_bottom.x)


            {
                return false;
            }
        }
        else if (ENUM_JUDGE_TYPE.E_Y == type_)
        {
            if ((int)corner0.y > (int)limit_left_top.y || (int)corner1.y > (int)limit_left_top.y
            || (int)corner0.y < (int)limit_right_bottom.y || (int)corner1.y < (int)limit_right_bottom.y)
            {
                return false;
            }
        }

        return true;
    }

    private bool JudgeCanDrag(CartoonLongRotateView view_, ENUM_ITEM_DIRECTION neighbour_dir)
    {
        Vector3[] fourCornersArray = new Vector3[4];
        view_.m_rect.GetWorldCorners(fourCornersArray);

        //Vector2[] fourScreenCornersArray = new Vector2[4];
        //for (int i = 0; i < fourCornersArray.Length; ++i)
        //{
        //    fourScreenCornersArray[i] = m_cam.WorldToScreenPoint(fourCornersArray[i]);
        //}

        switch (neighbour_dir)
        {
            case ENUM_ITEM_DIRECTION.E_UP:
            case ENUM_ITEM_DIRECTION.E_DOWN:
                {
                    return JudgeOverflow(fourCornersArray, m_play_rect_4_corners, ENUM_JUDGE_TYPE.E_Y);
                }

            case ENUM_ITEM_DIRECTION.E_LEFT:
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                {
                    return JudgeOverflow(fourCornersArray, m_play_rect_4_corners, ENUM_JUDGE_TYPE.E_X);
                }
            default:
                return false;
        }

    }

    private bool JudgeCanDrag(int view_index_, int model_width, ENUM_ITEM_DIRECTION neighbour_dir)
    {
        int neighbour_index = -1;

        switch (neighbour_dir)
        {
            case ENUM_ITEM_DIRECTION.E_UP:
                neighbour_index = CartoonUtil.GetUpIndex(view_index_, model_width, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_DOWN:
                neighbour_index = CartoonUtil.GetDownIndex(view_index_, model_width, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_LEFT:
                neighbour_index = CartoonUtil.GetLeftIndex(view_index_, model_width, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                neighbour_index = CartoonUtil.GetRightIndex(view_index_, model_width, this.anchor_w_size, this.anchor_h_size);
                break;
        }

        if (-1 == neighbour_index)
        {
            return false;
        }

        return CanBeExchanged(neighbour_index);
    }

    private bool CanTurnAround(CartoonSquareRotateView view_, CartoonRotate model_, ENUM_ROTATE_DIR r_dir_)
    {

        int pre_idx = view_.Item_index - 1;

        while (pre_idx > 0)
        {
            if (this.m_anchor_positions[pre_idx].cur_ocuppied_cartoon_item_id != model_.m_item_id)
            {
                //发现了前面的块

                int path = 0;
                CartoonFixed pre_model = this.m_cartoon.m_cartoon_items[this.m_anchor_positions[pre_idx].cur_ocuppied_cartoon_item_id];

                List<int> pre_video_exits = new List<int>();
                pre_video_exits.AddRange(pre_model.Video_entrance_2_exit.Values);
                List<int> pre_door_indexes = pre_model.DoorIndexCopy;

                List<int> cur_video_entrances = new List<int>();
                cur_video_entrances.AddRange(model_.Video_entrance_2_exit.Keys);
                List<int> cur_door_indexes = model_.DoorIndexCopy;

                if (ENUM_ROTATE_DIR.E_CLOCKWISE == r_dir_)
                {
                    CartoonUtil.ClockwiseRotate(cur_door_indexes);
                }
                else
                {
                    CartoonUtil.AntiClockwiseRotate(cur_door_indexes);
                }

                //前一个块，有向右的出口3, 4 则本块8，7位必须有视频
                //右侧两出口
                int pre_right_out_3 = pre_door_indexes[2];
                int pre_right_out_4 = pre_door_indexes[3];
                if (pre_video_exits.Contains(pre_right_out_3))
                {
                    //前一个块，有出口3
                    //检查本块是否有入口8的视频
                    int cur_left_in_8 = cur_door_indexes[7];
                    if (cur_video_entrances.Contains(cur_left_in_8))
                        ++path;
                }
                if (pre_video_exits.Contains(pre_right_out_4))
                {
                    //前一个块，有出口4
                    //检查本块是否有入口7的视频
                    int cur_left_in_7 = cur_door_indexes[6];
                    if (cur_video_entrances.Contains(cur_left_in_7))
                        ++path;
                }

                //前一个块，有向上的出口1，2 则本块6，5位必须有视频

                //上方两出口
                int pre_up_out_1 = pre_door_indexes[0];
                int pre_up_out_2 = pre_door_indexes[1];
                if (pre_video_exits.Contains(pre_up_out_1))
                {
                    //前一个块，有出口3
                    //检查本块是否有入口8的视频
                    int cur_down_in_6 = cur_door_indexes[5];
                    if (cur_video_entrances.Contains(cur_down_in_6))
                        ++path;
                }
                if (pre_video_exits.Contains(pre_up_out_2))
                {
                    //前一个块，有出口4
                    //检查本块是否有入口7的视频
                    int cur_down_in_5 = cur_door_indexes[4];
                    if (cur_video_entrances.Contains(cur_down_in_5))
                        ++path;
                }

                //前一个块，有向下的出口6, 5 则本块1，2位必须有视频

                //下方两出口
                int pre_down_out_6 = pre_door_indexes[5];
                int pre_down_out_5 = pre_door_indexes[4];
                if (pre_video_exits.Contains(pre_down_out_6))
                {
                    //前一个块，有出口3
                    //检查本块是否有入口8的视频
                    int cur_up_in_1 = cur_door_indexes[0];
                    if (cur_video_entrances.Contains(cur_up_in_1))
                        ++path;
                }
                if (pre_video_exits.Contains(pre_down_out_5))
                {
                    //前一个块，有出口4
                    //检查本块是否有入口7的视频
                    int cur_up_in_2 = cur_door_indexes[1];
                    if (cur_video_entrances.Contains(cur_up_in_2))
                        ++path;
                }

                return path > 0;
            }

            --pre_idx;
        }



        return true;
    }



    //private bool CanDrag(CartoonLongRotateView view_, CartoonRotate model_, ENUM_ROTATE_DIR r_dir_)
    //{

    //    switch (view_.m_dir)
    //    {
    //        case ENUM_ITEM_DIRECTION.E_LEFT:
    //            {
    //                switch (r_dir_)
    //                {
    //                    case ENUM_ROTATE_DIR.E_CLOCKWISE:
    //                        {
    //                            //up
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_UP);
    //                        }
    //                    case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
    //                        {
    //                            //down
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_DOWN);
    //                        }
    //                }
    //            }
    //            break;
    //        case ENUM_ITEM_DIRECTION.E_UP:
    //            {
    //                switch (r_dir_)
    //                {
    //                    case ENUM_ROTATE_DIR.E_CLOCKWISE:
    //                        {
    //                            //right
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_RIGHT);
    //                        }
    //                    case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
    //                        {
    //                            //left
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_LEFT);
    //                        }
    //                }
    //            }
    //            break;
    //        case ENUM_ITEM_DIRECTION.E_RIGHT:
    //            {
    //                switch (r_dir_)
    //                {
    //                    case ENUM_ROTATE_DIR.E_CLOCKWISE:
    //                        {
    //                            //down
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_DOWN);
    //                        }
    //                    case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
    //                        {
    //                            //up
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_UP);
    //                        }
    //                }
    //            }
    //            break;
    //        case ENUM_ITEM_DIRECTION.E_DOWN:
    //            {
    //                switch (r_dir_)
    //                {
    //                    case ENUM_ROTATE_DIR.E_CLOCKWISE:
    //                        {
    //                            //left
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_LEFT);
    //                        }
    //                    case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
    //                        {
    //                            //right
    //                            return JudgeCanDrag(view_.Item_index, model_.m_width_unit, ENUM_ITEM_DIRECTION.E_RIGHT);
    //                        }
    //                }
    //            }
    //            break;

    //    }

    //    return false;
    //}

    private bool CanDrag(CartoonLongRotateView view_, CartoonRotate model_, ENUM_ROTATE_DIR r_dir_)
    {

        switch (view_.m_dir)
        {
            case ENUM_ITEM_DIRECTION.E_LEFT:
                {
                    switch (r_dir_)
                    {
                        case ENUM_ROTATE_DIR.E_CLOCKWISE:
                            {
                                //up
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_UP);
                            }
                        case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
                            {
                                //down
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_DOWN);
                            }
                    }
                }
                break;
            case ENUM_ITEM_DIRECTION.E_UP:
                {
                    switch (r_dir_)
                    {
                        case ENUM_ROTATE_DIR.E_CLOCKWISE:
                            {
                                //right
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_RIGHT);
                            }
                        case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
                            {
                                //left
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_LEFT);
                            }
                    }
                }
                break;
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                {
                    switch (r_dir_)
                    {
                        case ENUM_ROTATE_DIR.E_CLOCKWISE:
                            {
                                //down
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_DOWN);
                            }
                        case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
                            {
                                //up
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_UP);
                            }
                    }
                }
                break;
            case ENUM_ITEM_DIRECTION.E_DOWN:
                {
                    switch (r_dir_)
                    {
                        case ENUM_ROTATE_DIR.E_CLOCKWISE:
                            {
                                //left
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_LEFT);
                            }
                        case ENUM_ROTATE_DIR.E_ANTI_CLOCKWISE:
                            {
                                //right
                                return JudgeCanDrag(view_, ENUM_ITEM_DIRECTION.E_RIGHT);
                            }
                    }
                }
                break;

        }

        return false;
    }
    private void Dragging(CartoonLongRotateView view_, CartoonRotate model_)
    {
        int exchanged_index = -1;

        switch (view_.m_dir)
        {
            case ENUM_ITEM_DIRECTION.E_LEFT:
                exchanged_index = CartoonUtil.GetLeftIndex(view_.Item_index, model_.m_width_unit, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_UP:
                exchanged_index = CartoonUtil.GetUpIndex(view_.Item_index, model_.m_width_unit, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_RIGHT:
                exchanged_index = CartoonUtil.GetRightIndex(view_.Item_index, model_.m_width_unit, this.anchor_w_size, this.anchor_h_size);
                break;
            case ENUM_ITEM_DIRECTION.E_DOWN:
                exchanged_index = CartoonUtil.GetDownIndex(view_.Item_index, model_.m_width_unit, this.anchor_w_size, this.anchor_h_size);
                break;

        }

        if (-1 == exchanged_index)
        {
            return;
        }

        if (false == this.CanBeExchanged(exchanged_index))
            return;

        AnchorPos exchanged = this.m_anchor_positions[exchanged_index];
        int exchanged_id = exchanged.cur_ocuppied_cartoon_item_id;

        CartoonFixedView exchange_view = m_cartoon_views[exchanged_id];
        CartoonFixed exchange_item = m_cartoon.m_cartoon_items[exchanged_id];

        //我的钉子点不动，尾巴被替换
        int my_tail_index = view_.Tail_index;
        int my_id = model_.m_item_id;

        //来我的位置
        this.ChangeAnchorPos(exchange_view, exchange_item, my_tail_index, m_anchor_positions[my_tail_index].m_pos);

        //目的地记录换成我
        m_anchor_positions[exchanged_index].cur_ocuppied_cartoon_item_id = my_id;
        //我的anchor记录换成目的地的
        view_.Tail_index = exchanged_index;
    }


    public void OnPlayClick()
    {
        this.PlayVideo(0, true);
    }


    public void OnResetClick()
    {
        this.m_cur_index_playing = 0;
        this.m_played_views.Clear();
        foreach (var view in this.m_cartoon_views.Values)
        {
            view.Reset();
        }
        this.InitCartoonItemAnchorPosition();
        this.ResetCartoonItemPosition();

    }

    private void PlayVideo(int pre_exit_, bool is_first = false)
    {
        AnchorPos ap = this.m_anchor_positions[this.m_cur_index_playing];

        int cartoon_id = ap.cur_ocuppied_cartoon_item_id;

        CartoonFixedView cartoon_view = this.m_cartoon_views[cartoon_id];

        cartoon_view.Play(pre_exit_, is_first);

        m_played_views.Add(cartoon_id);

        ++this.m_cur_index_playing;
    }

    private void PlayNext(int pre_exit_)
    {
        if (0 == pre_exit_)
        {
            Debug.Log("游戏失败了！！！！！！！！！");
            return;
        }
        else if (9 == pre_exit_)
        {
            Debug.Log("游戏胜利了！！！！！！！！！");
            return;
        }

        while (this.m_cur_index_playing < this.m_anchor_positions.Count)
        {
            AnchorPos ap = this.m_anchor_positions[this.m_cur_index_playing];

            int cartoon_id = ap.cur_ocuppied_cartoon_item_id;

            if (m_played_views.Contains(cartoon_id))
            {
                ++this.m_cur_index_playing;
                continue;
            }

            this.PlayVideo(pre_exit_);

            break;
        }


    }

    //private void Exchange( int exchanged_id, int exchanged_index, int my_id, int my_index)
    //{
    //    CartoonFixedView exchange_view = m_cartoon_views[exchanged_id];
    //    CartoonFixed exchange_item = m_cartoon.m_cartoon_items[exchanged_id];

    //    //来我的位置
    //    this.ChangeAnchorPos(exchange_view, exchange_item, my_index, m_anchor_positions[my_index].m_pos);

    //    //目的地记录换成我
    //    m_anchor_positions[exchanged_index].cur_ocuppied_cartoon_item_id = my_id;
    //    //我的anchor记录换成目的地的
    //    view_.Item_index = exchanged_index;
    //}

    #endregion

}

