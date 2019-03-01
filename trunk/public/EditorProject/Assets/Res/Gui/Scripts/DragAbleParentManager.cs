using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class DragAbleParentManager : MonoBehaviour
{

    private enum ENUM_RECT_DIR
    {
        E_UP,
        E_DOWN,
        E_LEFT,
        E_RIGHT,
        E_LEFT_UP,
        E_RIGHT_UP,
        E_LEFT_DOWN,
        E_RIGHT_DOWN,

    }

    public List<DragAbleParent> m_drag_able_parents_list = new List<DragAbleParent>();
    public int m_mx_size = 3;
    public float m_straight_near_dis = 16.0f;
    private float m_sqrt_straight_near_dis;
    public float m_oblique_near_dis = 10.0f;
    private float m_sqrt_oblique_near_dis;

    Dictionary<int, Dictionary<int, string>> m_split_index_mx = new Dictionary<int, Dictionary<int, string>>();
    private const int m_mx_max_size = 4;

    private Vector2 m_logic_rect_size;

    private RectTransform canvasRectTrans;

    void Start()
    {
        m_sqrt_straight_near_dis = (float)(m_straight_near_dis * m_straight_near_dis);
        m_sqrt_oblique_near_dis = (float)(m_oblique_near_dis * m_oblique_near_dis);
        canvasRectTrans = DragAble.FindInParents<Canvas>(this.gameObject).transform as RectTransform;

        foreach (var item in this.GetComponentsInChildren<DragAbleParent>())
        {
            foreach (DragAble d in item.m_child_icon_list)
            {
                d.ReisterMoveEnd(JudgeCombine);
            }
            m_drag_able_parents_list.Add(item);
        }

        float r_w = m_drag_able_parents_list[0].m_child_logic_rect_list[0].rect.width;
        float r_h = m_drag_able_parents_list[0].m_child_logic_rect_list[0].rect.height;

        m_logic_rect_size = new Vector2(r_w, r_h);

        Dictionary<int, string> row0 = new Dictionary<int, string>();
        row0[0] = "00"; row0[1] = "01"; row0[2] = "02"; row0[3] = "03";
        Dictionary<int, string> row1 = new Dictionary<int, string>();
        row0[0] = "10"; row0[1] = "11"; row0[2] = "12"; row0[3] = "13";
        Dictionary<int, string> row2 = new Dictionary<int, string>();
        row0[0] = "20"; row0[1] = "21"; row0[2] = "22"; row0[3] = "23";
        Dictionary<int, string> row3 = new Dictionary<int, string>();
        row0[0] = "30"; row0[1] = "31"; row0[2] = "32"; row0[3] = "33";

        m_split_index_mx[0] = row0;
        m_split_index_mx[0] = row1;
        m_split_index_mx[0] = row2;
        m_split_index_mx[0] = row3;
    }

    private void JudgeCombine(DragAbleParent moved_dp_)
    {
        List<RectTransform> move_end_logic_rects = moved_dp_.m_child_logic_rect_list;
        List<DragAble> move_end_icons = moved_dp_.m_child_icon_list;

        Vector2 offset = Vector2.zero;


        foreach (RectTransform move_end_logic_rect in move_end_logic_rects)
        {
            Dictionary<string, ENUM_RECT_DIR> neighbours = this.GetNeighbours(move_end_logic_rect.gameObject.name);

            if (0 == neighbours.Count)
            {
                Debug.LogError(string.Format("拼图 {0} 没有邻居", move_end_logic_rect.gameObject.name));
                continue;
            }

            foreach (DragAbleParent judge_dp in m_drag_able_parents_list)
            {
                if (judge_dp == moved_dp_)
                    continue;

                foreach (RectTransform cur_logic_rect in judge_dp.m_child_logic_rect_list)
                {
                    string cur_name = cur_logic_rect.gameObject.name;

                    if (neighbours.ContainsKey(cur_name))
                    {
                        if (this.IsNear(move_end_logic_rect, cur_logic_rect, neighbours[cur_name], out offset))
                        {
                            this.MoveAndCombine(moved_dp_, judge_dp, offset);
                            return;

                        }
                    }
                }

            }
        }

    }

    private Dictionary<string, ENUM_RECT_DIR> GetNeighbours(string obj_name_)
    {
        Dictionary<string, ENUM_RECT_DIR> ret = new Dictionary<string, ENUM_RECT_DIR>();

        int i_name = int.Parse(obj_name_);

        int row = i_name / 10;
        int col = i_name % 10;

        int row_converted;
        int col_converted;

        //上
        string neighbour_name = null;
        row_converted = row - 1;
        col_converted = col;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_UP;
        }

        //下
        neighbour_name = null;
        row_converted = row + 1;
        col_converted = col;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_DOWN;
        }

        //左
        neighbour_name = null;
        row_converted = row;
        col_converted = col - 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT;
        }

        //右  
        neighbour_name = null;
        row_converted = row;
        col_converted = col + 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT;
        }

        //左上
        neighbour_name = null;
        row_converted = row - 1;
        col_converted = col - 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT_UP;

        }

        //右上
        neighbour_name = null;
        row_converted = row - 1;
        col_converted = col + 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT_UP;
        }

        //左下
        neighbour_name = null;
        row_converted = row + 1;
        col_converted = col - 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_LEFT_DOWN;
        }

        //右下
        neighbour_name = null;
        row_converted = row + 1;
        col_converted = col + 1;
        if (this.IsValid(row_converted, col_converted))
        {
            neighbour_name = string.Format("{0}{1}", row_converted, col_converted);
            ret[neighbour_name] = ENUM_RECT_DIR.E_RIGHT_DOWN;
        }

        return ret;
    }

    private void MoveAndCombine(DragAbleParent move_end_dp_, DragAbleParent neighbour_, Vector2 offset)
    {
        move_end_dp_.m_self_rtf.anchoredPosition += offset;
        this.Combine(move_end_dp_, neighbour_);
    }

    private void Combine(DragAbleParent combined_, DragAbleParent combine_)
    {
        foreach (RectTransform combine_logic_rect in combine_.m_child_logic_rect_list)
        {
            combined_.m_child_logic_rect_list.Add(combine_logic_rect);
            combine_logic_rect.SetParent(combined_.transform, true);
        }

        foreach (DragAble combine_icon in combine_.m_child_icon_list)
        {
            combined_.m_child_icon_list.Add(combine_icon);
            combine_icon.transform.SetParent(combined_.transform, true);
        }

        m_drag_able_parents_list.Remove(combine_);
        GameObject.DestroyImmediate(combine_.gameObject);

    }

    private bool IsValid(int row_, int col_)
    {
        if (0 <= row_ && row_ < m_mx_size && 0 <= col_ && col_ < m_mx_size)
            return true;

        return false;
    }

    private Vector2 CalcMyAimCoord(Vector2 neighbour_coord_, ENUM_RECT_DIR dir_)
    {
        Vector2 aim_coord = Vector2.zero;

        if (ENUM_RECT_DIR.E_UP == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x, neighbour_coord_.y - m_logic_rect_size.y);
        }
        else if (ENUM_RECT_DIR.E_DOWN == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x, neighbour_coord_.y + m_logic_rect_size.y);
        }
        else if (ENUM_RECT_DIR.E_LEFT == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y);
        }
        else if (ENUM_RECT_DIR.E_RIGHT == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y);
        }
        if (ENUM_RECT_DIR.E_LEFT_UP == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y - m_logic_rect_size.y);
        }
        else if (ENUM_RECT_DIR.E_RIGHT_UP == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y - m_logic_rect_size.y);
        }
        else if (ENUM_RECT_DIR.E_LEFT_DOWN == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x + m_logic_rect_size.x, neighbour_coord_.y + m_logic_rect_size.y);
        }
        else if (ENUM_RECT_DIR.E_RIGHT_DOWN == dir_)
        {
            aim_coord = new Vector2(neighbour_coord_.x - m_logic_rect_size.x, neighbour_coord_.y + m_logic_rect_size.y);
        }


        return aim_coord;
    }

    /// <summary>
    /// 邻居在我的哪个方向，是否算靠的很近
    /// </summary>
    /// <param name="moved_"></param>
    /// <param name="neighbour_"></param>
    /// <param name="dir_"></param>
    /// <param name="offset_"></param>
    /// <returns></returns>
    private bool IsNear(RectTransform moved_, RectTransform neighbour_, ENUM_RECT_DIR dir_, out Vector2 offset_)
    {
        offset_ = Vector3.zero;
        Vector2 moved_coord = this.GetLocalPositionInCanvas(moved_.gameObject);
        Vector2 neighbour_coord = this.GetLocalPositionInCanvas(neighbour_.gameObject);
        Vector2 aim_coord = this.CalcMyAimCoord(neighbour_coord, dir_);

        offset_ = aim_coord - moved_coord;
        float cur_dis = (offset_).sqrMagnitude;


        if (dir_ <= ENUM_RECT_DIR.E_RIGHT)
        {
            if (cur_dis > this.m_sqrt_straight_near_dis)
                return false;
        }
        else
        {
            if (cur_dis > this.m_sqrt_oblique_near_dis)
                return false;
        }

        offset_ = aim_coord - moved_coord;
        return true;

    }

    public Vector2 GetLocalPositionInCanvas(GameObject obj)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, obj.transform.position, null, out localPoint))
        {
            return localPoint;
        }

        return Vector2.zero;
    }

}
