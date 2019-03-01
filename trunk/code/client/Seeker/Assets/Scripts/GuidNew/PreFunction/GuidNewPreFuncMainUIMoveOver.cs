using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewPreFuncMainUIMoveOver : GuidNewPreFunctionBase
    {
        private float m_delayTime = 0f;
        public override void OnInit(string[] param)
        {
            base.OnInit(param);
            if (param.Length == 1)
            {
                m_delayTime = float.Parse(param[0]);
            }
        }

        public override void OnCheck(Action action)
        {
            base.OnCheck(action);
            if (GameEntryCommonManager.Instance.TweenerOver())
            {
                if (action != null)
                {
                    if (this.m_delayTime > 0)
                    {
                        Action cb = () =>
                        {
                            action();
                        };
                        TimeModule.Instance.RemoveTimeaction(cb);
                        TimeModule.Instance.SetTimeout(cb, this.m_delayTime);
                    }
                    else
                    {
                        action();
                    }
                }
            }
        }

        private void ActionMoveOver()
        {

        }
    }
}
