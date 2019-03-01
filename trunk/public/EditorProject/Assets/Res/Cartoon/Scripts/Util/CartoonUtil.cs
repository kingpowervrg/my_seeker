using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class CartoonUtil
{
    public static int GetUpIndex(int cur_index_, int model_width_, int panel_w_size_, int panel_h_size_)
    {
        int row = cur_index_ / panel_w_size_;
        row -= model_width_ - 1;

        if (row >= 0)
        {
            return cur_index_ - panel_w_size_;// * (model_width_ - 1);
        }

        return -1;
    }

    public static int GetDownIndex(int cur_index_, int model_width_, int panel_w_size_, int panel_h_size_)
    {
        int row = cur_index_ / panel_w_size_;
        row += model_width_ - 1;

        if (row < panel_h_size_)
        {
            return cur_index_ + panel_w_size_;// * (model_width_ - 1);
        }

        return -1;
    }

    public static int GetLeftIndex(int cur_index_, int model_width_, int panel_w_size_, int panel_h_size_)
    {
        int col = cur_index_ % panel_w_size_;
        col -= model_width_ - 1;

        if (col >= 0)
        {
            return cur_index_ - 1;// - (model_width_ - 1);
        }

        return -1;
    }


    public static int GetRightIndex(int cur_index_, int model_width_, int panel_w_size_, int panel_h_size_)
    {
        int col = cur_index_ % panel_w_size_;
        col += model_width_ - 1;

        if (col < panel_w_size_)
        {
            return cur_index_ + 1;// (model_width_ - 1);
        }

        return -1;
    }

    public static int GetDirDelta(ENUM_ITEM_DIRECTION from, ENUM_ITEM_DIRECTION to)
    {
        return 0;
    }

    public static void ClockwiseRotate(List<int> doors_idx)
    {
        RightMoveDoorIndex(doors_idx, 2);
    }

    /// <summary>
    /// 逆时针
    /// </summary>
    public static void AntiClockwiseRotate(List<int> doors_idx)
    {
        LeftMoveDoorIndex(doors_idx, 2);
    }

    public static void RightMoveDoorIndex(List<int> doors_idx, int delta_)
    {
        for (int i = 0; i < delta_; ++i)
        {
            int last = doors_idx.Last<int>();
            doors_idx.RemoveAt(doors_idx.Count - 1);
            doors_idx.Insert(0, last);
        }
    }

    public static void LeftMoveDoorIndex(List<int> doors_idx, int delta_)
    {
        for (int i = 0; i < delta_; ++i)
        {
            int first = doors_idx.First<int>();
            doors_idx.RemoveAt(0);
            doors_idx.Add(first);
        }
    }

}

