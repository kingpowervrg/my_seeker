using System;
using System.Collections.Generic;
using UnityEngine;


public class ScanJsonData
{
    public int M_id { get; set; }

    public string M_tex_name { get; set; }

    public float M_tex_x { get; set; }
    public float M_tex_y { get; set; }
    public float M_tex_w { get; set; }
    public float M_tex_h { get; set; }

    public List<ScanAnchorJsonData> M_anchors { get; set; }

}

public class ScanAnchorJsonData
{
    public int M_clue_id { get; set; }
    public float M_x { get; set; }
    public float M_y { get; set; }

    public float M_w { get; set; }
    public float M_h { get; set; }
}



