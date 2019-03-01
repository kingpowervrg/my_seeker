using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore
{
    public class GameIndicateCheckBox : GameCheckBox
    {
        private GameImage imgSign;
        private GameLabel lbNum;
        protected override void OnInit()
        {
            base.OnInit();

            imgSign = Make<GameImage>("numkuang");
            lbNum = Make<GameLabel>("num");

            if (imgSign != null)
                imgSign.Visible = false;
            if (lbNum != null)
                lbNum.Visible = false;
        }
        public void ShowSign(bool show = true, int num = 0)
        {
            if (imgSign != null)
                imgSign.Visible = show;
            if (lbNum != null)
            {
                lbNum.Visible = num > 0;
                lbNum.Text = num.ToString();
            }
        }
    }
}
