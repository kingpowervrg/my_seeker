using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class FriendDataManager : Singleton<FriendDataManager>
    {
        private Dictionary<long, FriendIcon> m_friend_icons_dict = new Dictionary<long, FriendIcon>();
        public System.Collections.Generic.Dictionary<long, FriendIcon> Friend_icons_dict
        {
            get { return m_friend_icons_dict; }
        }

        Dictionary<FriendReqType, Dictionary<long, PlayerFriendMsg>> m_datas_dict = new Dictionary<FriendReqType, Dictionary<long, PlayerFriendMsg>>();


        Dictionary<long, FriendGift> m_gifts_dict = new Dictionary<long, FriendGift>();

        List<FriendRecommendMsg> m_recommends = new List<FriendRecommendMsg>();
        public System.Collections.Generic.List<FriendRecommendMsg> Recommends
        {
            get { return m_recommends; }
        }

        DateTime m_recommend_expire_date = DateTime.MinValue;
        public System.DateTime Recommend_expire_date
        {
            get { return m_recommend_expire_date; }
            set { m_recommend_expire_date = value; }
        }
        private int m_max_friend_num;
        public int Max_friend_num
        {
            get { return m_max_friend_num; }
            set { m_max_friend_num = value; }
        }

        private bool m_last_apply;
        public bool Last_apply
        {
            get { return m_last_apply; }
            set { m_last_apply = value; }
        }
        private int m_send_gift_left_num;
        public int Send_gift_left_num
        {
            get { return m_send_gift_left_num; }
            set { m_send_gift_left_num = value; }
        }


        public FriendDataManager()
        {
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendGiftResponse, OnScResponse);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCFriendGiftDrawResponse, OnScResponse);
        }


        public void RefreshAllInfo()
        {
            RefreshFriendInfo(FriendReqType.Added);
            TimeModule.Instance.SetTimeout(() => RefreshFriendInfo(FriendReqType.Addinfo), 0.5f);
            TimeModule.Instance.SetTimeout(() => RefreshFriendInfo(FriendReqType.Agreeing), 1.0f);
            TimeModule.Instance.SetTimeout(() => RefreshFriendGift(), 1.5f);

        }


        void OnScResponse(object s)
        {

            if (s is SCFriendResponse)
            {
                var rsp = s as SCFriendResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;



                if (FriendReqType.Added == rsp.Type)
                {
                    FriendDataManager.Instance.Last_apply = rsp.LastAgree;
                    FriendDataManager.Instance.Is_receive_application = !rsp.AddSwitch;
                    FriendDataManager.Instance.Max_friend_num = rsp.Limit;
                    FriendDataManager.Instance.Send_gift_left_num = rsp.GiftCountLeft;
                }
                else if (FriendReqType.Addinfo == rsp.Type)
                {
                    FriendDataManager.Instance.Last_apply = false;
                }


                FriendDataManager.Instance.SetDatas(rsp.Friends, rsp.Type);


            }
            else if (s is SCFriendGiftResponse)
            {
                var rsp = s as SCFriendGiftResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;

                FriendDataManager.Instance.Receive_gift_max_num = rsp.Limit;
                FriendDataManager.Instance.Receive_gift_left_num = rsp.Limit - rsp.Count;
                FriendDataManager.Instance.SetGifts(rsp.FriendGiftLists);

            }
            else if (s is SCFriendGiftDrawResponse)
            {
                var rsp = s as SCFriendGiftDrawResponse;
                if (MsgStatusCodeUtil.OnError(rsp.Status))
                    return;


                foreach (var item in rsp.PlayerPropMsg)
                {
                    long item_id = item.PropId;
                    int item_count = item.Count;

                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(item_id, item_count);

                }
                GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();

            }

        }


        public int SendGiftLeftNumReduceOne()
        {
            if (m_send_gift_left_num <= 0)
                return -1;

            --m_send_gift_left_num;

            if (0 == m_send_gift_left_num)
            {
                this.DiableSendGift();
            }

            return m_send_gift_left_num;
        }

        private int m_receive_gift_max_num;
        public int Receive_gift_max_num
        {
            get { return m_receive_gift_max_num; }
            set { m_receive_gift_max_num = value; }
        }

        private int m_receive_gift_left_num;
        public int Receive_gift_left_num
        {
            get { return m_receive_gift_left_num; }
            set { m_receive_gift_left_num = value; }
        }

        public int ReceiveGiftLeftNumReduce(int delta_)
        {
            if (m_receive_gift_left_num <= 0)
                return -1;

            int total = m_receive_gift_left_num;

            m_receive_gift_left_num -= delta_;
            return m_receive_gift_left_num > 0 ? delta_ : total;
        }

        private bool m_is_receive_application;
        public bool Is_receive_application
        {
            get { return m_is_receive_application; }
            set { m_is_receive_application = value; }
        }
        //public void AddData(PlayerFriendMsg data_, FriendReqType type_)
        //{
        //    if (!m_datas_dict.ContainsKey(type_))
        //    {
        //        m_datas_dict[type_] = new Dictionary<long, PlayerFriendMsg>();
        //    }

        //    if (!m_datas_dict[type_].ContainsKey(data_.PlayerId))
        //    {
        //        m_datas_dict[type_].Add(data_.PlayerId, data_);
        //    }
        //    else
        //    {
        //        this.m_datas_dict[type_][data_.PlayerId] = data_;
        //    }

        //}

        public void SetDatas(IEnumerable<PlayerFriendMsg> datas_, FriendReqType type_)
        {
            if (!m_datas_dict.ContainsKey(type_))
            {
                m_datas_dict[type_] = new Dictionary<long, PlayerFriendMsg>();
            }
            else
            {
                m_datas_dict[type_].Clear();
            }

            foreach (var data in datas_)

            {
                if (!this.m_datas_dict[type_].ContainsKey(data.PlayerId))
                {
                    m_datas_dict[type_].Add(data.PlayerId, data);
                }
                else
                {
                    this.m_datas_dict[type_][data.PlayerId] = data;
                }
            }

        }

        public PlayerFriendMsg GetData(long id_, FriendReqType type_)
        {
            PlayerFriendMsg ret;

            if (this.m_datas_dict.ContainsKey(type_) && this.m_datas_dict[type_].TryGetValue(id_, out ret))
            {
                return ret;
            }

            return null;
        }

        public Dictionary<long, PlayerFriendMsg> GetDatas(FriendReqType type_)
        {
            if (this.m_datas_dict.ContainsKey(type_))
            {
                return this.m_datas_dict[type_];
            }

            return null;
        }


        public void RemoveData(long id_, FriendReqType type_)
        {
            if (this.m_datas_dict.ContainsKey(type_) && this.m_datas_dict[type_].ContainsKey(id_))
            {
                this.m_datas_dict[type_].Remove(id_);
            }
        }

        public void RemoveDatas(FriendReqType type_)
        {
            if (this.m_datas_dict.ContainsKey(type_))
            {
                this.m_datas_dict.Remove(type_);
            }
        }




        public void SetGifts(IEnumerable<FriendGift> datas_)
        {
            m_gifts_dict.Clear();

            foreach (var data in datas_)

            {
                if (!this.m_gifts_dict.ContainsKey(data.GiftId))
                {
                    m_gifts_dict.Add(data.GiftId, data);
                }
                else
                {
                    this.m_gifts_dict[data.GiftId] = data;
                }
            }

        }

        public Dictionary<long, FriendGift> GetGifts()
        {
            return m_gifts_dict;
        }

        public void RemoveGift(long gift_id_)
        {
            if (m_gifts_dict.ContainsKey(gift_id_))
            {
                m_gifts_dict.Remove(gift_id_);
            }
        }

        public void RemoveAllGifts()
        {
            m_gifts_dict.Clear();
        }

        /// <summary>
        /// 删除最近的几个礼物
        /// </summary>
        /// <param name="count_"></param>
        public void RemoveRecentGifts(int count_)
        {
            List<FriendGift> sorted_gifts = new List<FriendGift>(m_gifts_dict.Values);

            sorted_gifts.Sort((a, b) => { if (a.PlayerFriends.StatusTime > b.PlayerFriends.StatusTime) return -1; else return 1; });

            List<long> will_del_gifts = new List<long>();

            for (int i = 0; i < count_; ++i)
            {
                will_del_gifts.Add(sorted_gifts[i].GiftId);
            }

            foreach (var id in will_del_gifts)
            {
                m_gifts_dict.Remove(id);
            }
        }

        public void DiableSendGift()
        {
            foreach (var item in this.GetDatas(FriendReqType.Added).Values)
            {
                item.Gift = false;
            }
        }


        public void SetHeadIcon(GameImage head_icon, GameNetworkRawImage head_tex, string icon_url_, long player_id_)
        {
            if (CommonTools.IsNeedDownloadIcon(icon_url_))
            {
                head_icon.Visible = false;
                head_tex.Visible = true;

                if (FriendDataManager.Instance.Friend_icons_dict.ContainsKey(player_id_))
                {
                    FriendIcon f_icon = FriendDataManager.Instance.Friend_icons_dict[player_id_];
                    if (f_icon.Url != icon_url_)
                    {
                        head_tex.OnLoadFinish = (tex_) => { f_icon.Url = icon_url_; f_icon.m_tex = tex_; };
                        head_tex.TextureName = icon_url_;
                    }
                    else
                    {
                        head_tex.SetTexture(f_icon.m_tex);
                    }
                }
                else
                {
                    head_tex.OnLoadFinish = (tex_) =>
                    {
                        FriendIcon f_icon = new FriendIcon();
                        f_icon.Url = icon_url_;
                        f_icon.m_tex = tex_;
                        FriendDataManager.Instance.Friend_icons_dict.Add(player_id_, f_icon);
                    };
                    head_tex.TextureName = icon_url_;
                }

            }
            else
            {
                head_icon.Visible = true;
                head_tex.Visible = false;
                head_icon.Sprite = CommonData.GetSize3HEADByDefault(icon_url_);
            }
        }


        public void SetHeadIcon(GameNetworkRawImage head_tex, string icon_url_, long player_id_)
        {
            if (CommonTools.IsNeedDownloadIcon(icon_url_))
            {
                head_tex.Visible = true;

                if (FriendDataManager.Instance.Friend_icons_dict.ContainsKey(player_id_))
                {
                    FriendIcon f_icon = FriendDataManager.Instance.Friend_icons_dict[player_id_];
                    if (f_icon.Url != icon_url_)
                    {
                        head_tex.OnLoadFinish = (tex_) => { f_icon.Url = icon_url_; f_icon.m_tex = tex_; };
                        head_tex.TextureName = icon_url_;
                    }
                    else
                    {
                        head_tex.SetTexture(f_icon.m_tex);
                    }
                }
                else
                {
                    head_tex.OnLoadFinish = (tex_) =>
                    {
                        FriendIcon f_icon = new FriendIcon();
                        f_icon.Url = icon_url_;
                        f_icon.m_tex = tex_;
                        FriendDataManager.Instance.Friend_icons_dict.Add(player_id_, f_icon);
                    };
                    head_tex.TextureName = icon_url_;
                }

            }
            else
            {
                head_tex.Visible = true;
                head_tex.TextureName = CommonData.GetBigPortrait(icon_url_);
            }
        }

        public void RefreshFriendInfo(FriendReqType type_)
        {
            CSFriendRequest req = new CSFriendRequest();
            req.Type = type_;
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

        public void RefreshFriendGift()
        {
            CSFriendGiftRequest req = new CSFriendGiftRequest();
            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);
        }

    }
}
