using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class GroupToolTipsDatas
    {
        public List<GroupToolTipsData> Datas {get; set;}

        public Vector3 WorldPos { get; set; }

        public Vector2 ScreenPos { get; set; }
    }


    public class GroupToolTipsData
    {
        public long ItemID { get; set; }
        public int CurCount { get; set; }

    }
}
