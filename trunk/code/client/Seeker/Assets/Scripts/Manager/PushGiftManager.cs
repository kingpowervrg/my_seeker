//#define PLATFORM_ID
using EngineCore;
using GOEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class PushGiftManager : Singleton<PushGiftManager>
    {

        private Stack<ENUM_PUSH_GIFT_BLOCK_TYPE> m_turn_on_types = new Stack<ENUM_PUSH_GIFT_BLOCK_TYPE>();

        private List<Push_Info> m_login_push_infos;

        private List<Push_Info> m_block_push_infos;

        private List<Push_Info> m_level_up_push_infos;


        public PushGiftManager()
        {
            m_turn_on_types = new Stack<ENUM_PUSH_GIFT_BLOCK_TYPE>();

            MessageHandler.RegisterMessageHandler(MessageDefine.SCGetPushResponse, OnPushGiftRsp);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCBuyPushResponse, OnPushGiftRsp);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCAutoOpenGiftDropResp, CommonHelper.OnOpenAutoGiftCallback);
        }

        public void Sync()
        {
            List<Push_Info> infos = new List<Push_Info>();

            ConfPush.array.ForEach(
                (item) =>
                {
                    int cur_platform = 0;
#if UNITY_IOS
                    cur_platform = 2;
#else
                    cur_platform = 1;
#endif

                    if (item.ostype.Contains(cur_platform) && 0 != item.bolckType)
                    {
                        Push_Info info = new Push_Info()
                        {
                            PushId = item.id,
                            EndTime = 0L,
                            Buyed = false,
                            Type = 2
                        };
                        infos.Add(info);
                    }
                });

            this.RefreshBlockPush(infos);
        }

        public void RefreshLoginPush(List<Push_Info> infos_)
        {
            m_login_push_infos = infos_;
        }

        public void RefreshBlockPush(List<Push_Info> infos_)
        {
            m_block_push_infos = infos_;
        }
        public void RefreshLvlPush(List<Push_Info> infos_)
        {
            m_level_up_push_infos = infos_;
        }


        public ENUM_PUSH_GIFT_BLOCK_TYPE GetTurnOnType()
        {
            if (m_turn_on_types.Count > 0)
                return m_turn_on_types.Pop();

            return ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE;
        }

        public ENUM_PUSH_GIFT_BLOCK_TYPE CheckTurnOnType()
        {
            if (m_turn_on_types.Count > 0)
                return m_turn_on_types.Peek();

            return ENUM_PUSH_GIFT_BLOCK_TYPE.E_NONE;
        }


        public void Cache(ENUM_PUSH_GIFT_BLOCK_TYPE type_)
        {
            m_turn_on_types.Push(type_);
        }
        public bool TurnOn(ENUM_PUSH_GIFT_BLOCK_TYPE type_)
        {

            if (null == this.GetPushInfosByTurnOnType(type_))
            {
                return false;
            }

            if (type_ == ENUM_PUSH_GIFT_BLOCK_TYPE.E_COIN || type_ == ENUM_PUSH_GIFT_BLOCK_TYPE.E_VIT)
            {
                GiftBagUILogic.OpenGiftBag(type_);
                return true;
            }

            if (!GameEvents.UIEvents.UI_PushGift_Event.OnGo.SafeInvoke(type_))
            {
                Cache(type_);
            }

            return true;

        }

        public void BoughtLoginGift(long charge_id_)
        {
            if (null != this.m_login_push_infos)
            {

                foreach (var info in m_login_push_infos)
                {
                    if (info.Buyed)
                        continue;

                    ConfPush push = ConfPush.Get(info.PushId);

                    if (push.chargeid == charge_id_)
                    {
                        info.Buyed = true;
                    }
                }
            }
        }

        public List<Push_Info> GetPushInfosByTurnOnType(ENUM_PUSH_GIFT_BLOCK_TYPE type_, bool bought = false)
        {

            if (ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN == type_)
            {
                if (null != this.m_login_push_infos)
                {
                    var boughts = this.m_login_push_infos.FindAll((item) => bought == item.Buyed);

                    if (null == boughts || 0 == boughts.Count)
                        return null;
                }

                return this.m_login_push_infos;
            }
            else if (ENUM_PUSH_GIFT_BLOCK_TYPE.E_LVL == type_)
            {
                return this.m_level_up_push_infos;
            }
            else
            {
                List<Push_Info> cur_infos = new List<Push_Info>();

                this.m_block_push_infos.ForEach((item) => { if ((byte)type_ == ConfPush.Get(item.PushId).bolckType) cur_infos.Add(item); });

                return cur_infos.Count > 0 ? cur_infos : null;

            }

        }

        private void OnPushGiftRsp(object s)
        {
            if (s is SCGetPushResponse)
            {
                var rsp = s as SCGetPushResponse;

                List<Push_Info> infos = new List<Push_Info>();

                if (null == rsp.Infos || 0 == rsp.Infos.Count)
                {
                    m_login_push_infos = null;
                    return;
                }

                infos.AddRange(rsp.Infos);

                PushGiftManager.Instance.RefreshLoginPush(infos);
            }
            else if (s is SCBuyPushResponse)
            {
                var rsp = s as SCBuyPushResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.ReponseStatus))
                {
#if UNITY_IOS && PLATFORM_ID
                    GameEvents.IAPEvents.Sys_BuyProductIOSEvent.SafeInvoke(ConfCharge.Get(ConfPush.Get(rsp.PushId).chargeid).chargeSouceId);
#else
                    GameEvents.IAPEvents.Sys_BuyProductEvent.SafeInvoke(ConfCharge.Get(ConfPush.Get(rsp.PushId).chargeid).id);
#endif
                }
            }

        }

        public int GetLoginTypeCount()
        {

            if (GlobalInfo.GAME_NETMODE == GameNetworkMode.Standalone)
            {
                return 0;
            }
            return m_login_push_infos.FindAll((item) => false == item.Buyed).Count;
        }


        public long GetLoginTypeLeftTime()
        {
            long min_end_time = 0L;

            foreach (var item in m_login_push_infos)
            {
                if (item.Buyed)
                    continue;

                if (0L == min_end_time || item.EndTime < min_end_time)
                    min_end_time = item.EndTime;
            }

            //Debug.Log("min end time = " + CommonTools.SecondToTitleString(min_end_time));
            long ret = min_end_time - CommonTools.DateTimeToTimeStamp(CommonTools.GetCurrentTime()) / 10000000;
            return Math.Max(ret, 0);
        }
    }
}
