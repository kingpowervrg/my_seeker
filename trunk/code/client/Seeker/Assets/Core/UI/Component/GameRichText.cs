using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

using GOGUI;

namespace EngineCore
{
    public class GameRichText : GameTextComponent
    {
        public bool autoResize = false;
        public int maxWidth = 100;

        protected override void OnInit()
        {
            base.OnInit();
            base.label = GetComponent<AdvancedText>();
        }
    }
}
