using EngineCore;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    public class SignInManager
    {
        static Queue<SCPlayerCheckInInfoResp> m_cache;

        public static void Clear()
        {
            if (m_cache != null)
            {
                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow -= OnShowBonusPopView;
                m_cache.Clear();
            }
        }

        public SignInManager()
        {
            m_cache = new Queue<SCPlayerCheckInInfoResp>();
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow -= OnShowBonusPopView;
            GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow += OnShowBonusPopView;

            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Network)
            {
                MessageHandler.RegisterMessageHandler(MessageDefine.SCPlayerCheckInInfoResp, OnResponse);
                CSPlayerCheckInInfoReq req = new CSPlayerCheckInInfoReq();

#if !NETWORK_SYNC || UNITY_EDITOR
                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
#else
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
            }

        }



        public void OnResponse(object obj)
        {
            if (obj is SCPlayerCheckInInfoResp)
            {
                
                SCPlayerCheckInInfoResp res = (SCPlayerCheckInInfoResp)obj;
                if (res == null || res.Status == 1)
                {
                    GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnShow.SafeInvoke();
                    return;
                }

                m_cache.Enqueue(res);

                GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Tell_OnCache.SafeInvoke(EUNM_BONUS_POP_VIEW_TYPE.E_DAILY_SIGN);
                OnDispose();
                //FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SIGNIN);
                //param.Param = res;
                //EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
        }

        public void OnDispose()
        {
            //GameEvents.UIEvents.UI_Bonus_Pop_View_Event.Listen_OnShow -= OnShowBonusPopView;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCPlayerCheckInInfoResp, OnResponse);
        }

        private static void OnShowBonusPopView(EUNM_BONUS_POP_VIEW_TYPE t_)
        {
            if (EUNM_BONUS_POP_VIEW_TYPE.E_DAILY_SIGN != t_)
                return;

            if (0 == m_cache.Count)
            {
                return;
            }

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_SIGNIN);
            param.Param = m_cache.Dequeue();
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }
    }
}
