using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameUIWithBlackBack : GameUIComponent
    {
        public string CloseButtonId
        {
            set { setCloseBtnID(value); }
        }
        protected override void OnInit()
        {
            base.OnInit();
            /*
            GameImage btnBlack = Make<GameImage>("Backblackimage");
            if (btnBlack != null)
                btnBlack.AddClickCallBack(onClickBack);*/
        }

        private void setCloseBtnID(string str)
        {
            GameButton close = Make<GameButton>(str);
            if (close != null)
                close.AddClickCallBack(onClickClose);
        }

        protected virtual void onClickClose(GameObject obj)
        {
            Visible = false;
        }

        protected virtual void onClickBack(GameObject go)
        {
            Visible = false;
        }
    }

}
