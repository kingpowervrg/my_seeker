using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class ToolTipsData
    {
        public long ItemID { get; set; }
        public int CurCount { get; set; }
        public int MaxCount { get; set; }

        public Vector3 WorldPos { get; set; }

        public Vector2 ScreenPos { get; set; }
    }
}
