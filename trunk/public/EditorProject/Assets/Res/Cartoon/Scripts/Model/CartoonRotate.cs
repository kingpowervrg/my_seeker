using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CartoonRotate : CartoonFixed
{

    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 顺时针
    /// </summary>
    public void ClockwiseRotate()
    {
        RightMoveDoorIndex(2);
    }

    /// <summary>
    /// 逆时针
    /// </summary>
    public void AntiClockwiseRotate()
    {
        LeftMoveDoorIndex(2);
    }

    private void RightMoveDoorIndex(int delta_)
    {
        for (int i = 0; i < delta_; ++i)
        {
            int last = this.m_doors_idx.Last<int>();
            this.m_doors_idx.RemoveAt(this.m_doors_idx.Count - 1);
            this.m_doors_idx.Insert(0, last);
        }

        string output_str = "";
        foreach (int door in m_doors_idx)
        {
            output_str += door.ToString();
            output_str += " ";
        }

        Debug.LogWarning("顺时针旋转 门的顺序变为" + output_str);
    }

    private void LeftMoveDoorIndex(int delta_)
    {
        for (int i = 0; i < delta_; ++i)
        {
            int first = this.m_doors_idx.First<int>();
            this.m_doors_idx.RemoveAt(0);
            this.m_doors_idx.Add(first);
        }

        string output_str = "";
        foreach (int door in m_doors_idx)
        {
            output_str += door.ToString();
            output_str += " ";
        }

        Debug.LogWarning("逆时针旋转 门的顺序变为" + output_str);
    }
}

