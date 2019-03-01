using System;
using System.Collections.Generic;
using UnityEngine;


    public class JigsawData
    {
        public List<JigsawDataJson> M_jon_datas { get; set; }
    }

    public class JigsawDataJson
    {
        public int M_template_id { get; set; }
        public int M_dimention { get; set; }
        public List<JigsawChipJson> M_chips { get; set; }

    }

    public class JigsawChipJson
    {
        public string M_chip_name { get; set; }
        public string M_tex_anme { get; set; }
        public RectJson M_tex_size { get; set; }
    }

    public class RectJson
    {
        public float M_x { get; set; }
        public float M_y { get; set; }
        public float M_w { get; set; }
        public float M_h { get; set; }

        public RectJson()
        { }

        public RectJson(float x_, float y_, float w_, float h_)
        {
            this.M_x = x_;
            this.M_y = y_;
            this.M_w = w_;
            this.M_h = h_;
        }
    }

