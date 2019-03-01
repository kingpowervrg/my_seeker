using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameHorizontalSlider : GameUIComponent
    {
        protected UnityEngine.UI.Slider slider;

        private GameLabel lbNum;

        public int MaxNum = 1;
        public int MinNum = 0;

        protected override void OnInit()
        {
            base.OnInit();
            slider = GetComponent<UnityEngine.UI.Slider>();
            slider.onValueChanged.AddListener(ValueChangeHandler);
            lbNum = Make<GameLabel>("Text");

        }

        public float Value
        {
            get { return slider.value; }
            set
            {
                if (Convert.ToInt32(value * MaxNum) < MinNum)
                    value = (float)MinNum / (float)MaxNum;
                else if (Convert.ToInt32(value * MaxNum) > MaxNum)
                    value = (float)MaxNum / (float)MaxNum;

                slider.value = value;
                ValueStr = Convert.ToInt32(value * MaxNum).ToString();
            }
        }
        public string ValueStr
        {
            set
            {
                if (lbNum != null)
                    lbNum.Text = value;
            }
            get
            {
                return lbNum == null ? "" : lbNum.Text;
            }
        }

   
        void ValueChangeHandler(float val)
        {
            Value = val;
        }
    }

}
