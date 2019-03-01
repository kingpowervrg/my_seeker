using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    public class DropOutJsonData
    {
        public int count { get; set; }
        public int rate { get; set; }
        public long value { get; set; }

        public List<DropOutJsonData> Datas { get; set; }
    }


}

