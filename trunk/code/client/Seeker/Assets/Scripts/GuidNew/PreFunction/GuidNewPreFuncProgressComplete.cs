using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncProgressComplete : GuidNewPreFunctionBase
    {
        private bool loadCompltete = false;
        private float timeDelay = 0f;
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            this.timeDelay = float.Parse(param[0]);
            this.loadCompltete = false;
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver += OnLoadingOver;
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (this.loadCompltete)
            {
                this.loadCompltete = false;
                if (this.timeDelay > 0)
                {
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        action();
                    }, timeDelay);
                }
                else
                {
                    action();
                }
            }
        }

        private void OnLoadingOver()
        {
            this.loadCompltete = true;
            //if (this.timeDelay > 0)
            //{
            //    TimeModule.Instance.SetTimeout(() =>
            //    {
                    
            //        //GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver -= OnLoadingOver;
            //    }, timeDelay);
            //}
            //else
            //{
            //    this.loadCompltete = true;
            //    //GameEvents.UIEvents.UI_Loading_Event.OnLoadingOver -= OnLoadingOver;
            //}
           
        }

    }
}
