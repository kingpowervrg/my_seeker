using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineCore
{
    /// <summary>
    /// check状态改变时，切换图片
    /// </summary>
    public class GameSpecialCheckBox : GameIndicateCheckBox
    {
        private GameImage normalSprite;
        private GameImage selectSprite;
        protected override void OnInit()
        {
            base.OnInit();
            normalSprite = Make<GameImage>("Background");
            selectSprite = Make<GameImage>("Checkmark");
            AddClickCallBack(onClick);
        }

        public override bool Checked
        {
            get
            {
                return base.Checked;
            }
            set
            {
                base.Checked = value;
                selectSprite.Visible = value;
                normalSprite.Visible = !value;

            }
        }

        private void onClick(GameObject obj)
        {
            tog.isOn = !tog.isOn;
        }
    }
}
