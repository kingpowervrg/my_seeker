//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;

namespace SeekerGame
{
    class RecommendUI : BaseViewComponet<FriendUILogic>
    {
        private GameLabel m_title;
        private GameLabel m_count_down_txt;
        private GameUIContainer m_recommend_grid;
        private GameImage m_add_all_btn;
        private GameLabel m_add_all_btn_txt;

        private SCFriendRecommendListResponse m_data;
        protected override void OnInit()
        {
            base.OnInit();
            m_title = Make<GameLabel>("Text");
            m_title.Text = LocalizeModule.Instance.GetString("friend_recommend");
            m_count_down_txt = Make<GameLabel>("Image:Text");
            m_recommend_grid = Make<GameUIContainer>("Image:Content");
            m_add_all_btn = Make<GameImage>("Button");
            m_add_all_btn_txt = m_add_all_btn.Make<GameLabel>("Text");
            m_add_all_btn_txt.Text = LocalizeModule.Instance.GetString("friend_addueser_all");
            this.SetCloseBtnID("Button_close");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_add_all_btn.AddClickCallBack(AddAllClicked);

            if (DateTime.Now.CompareTo(FriendDataManager.Instance.Recommend_expire_date) > 0)
            {
                CurViewLogic().RequestRecommends();
            }
            else
            {
                Refresh();
            }


            TimeModule.Instance.SetTimeInterval(UpdateCountDown, 1.0f);
        }

        public override void OnHide()
        {
            base.OnHide();
            m_add_all_btn.RemoveClickCallBack(AddAllClicked);
            TimeModule.Instance.RemoveTimeaction(UpdateCountDown);
        }


        public void Refresh()
        {
            UpdateCountDown();

            m_recommend_grid.EnsureSize<RecommendItemUI>(FriendDataManager.Instance.Recommends.Count);

            for (int i = 0; i < FriendDataManager.Instance.Recommends.Count; ++i)
            {
                m_recommend_grid.GetChild<RecommendItemUI>(i).Refresh(FriendDataManager.Instance.Recommends[i]);
                m_recommend_grid.GetChild<RecommendItemUI>(i).Visible = true;
            }

            var valids = FriendDataManager.Instance.Recommends.FindAll((item) => ENUM_RECOMMEND_STATUS.E_RECOMMEND == (ENUM_RECOMMEND_STATUS)item.Status);

            bool has_recommend = null != valids && valids.Count > 0;
            m_add_all_btn.SetGray(!has_recommend);
            m_add_all_btn.Enable = has_recommend;

            string btn_txt = has_recommend ? "friend_addueser_all" : "friend_addueser_end";
            m_add_all_btn_txt.Text = LocalizeModule.Instance.GetString(btn_txt);
        }

        private void AddAllClicked(GameObject obj_)
        {
            var all = FriendDataManager.Instance.Recommends.FindAll((item) => (int)ENUM_RECOMMEND_STATUS.E_RECOMMEND == item.Status);
            List<long> all_ids = new List<long>();
            all.ForEach((item) => all_ids.Add(item.RecommendId));
            GameEvents.UIEvents.UI_Friend_Event.Listen_add_recommend_friend.SafeInvoke(all_ids);
        }

        private void UpdateCountDown()
        {
            if (System.DateTime.Now.CompareTo(FriendDataManager.Instance.Recommend_expire_date) < 0)
            {
                TimeSpan ts1 = new TimeSpan(FriendDataManager.Instance.Recommend_expire_date.Ticks);
                TimeSpan ts2 = new TimeSpan(System.DateTime.Now.Ticks);
                TimeSpan ts3 = ts1.Subtract(ts2).Duration();
                m_count_down_txt.Text = CommonTools.SecondToStringDay2Second(ts3.TotalSeconds);
            }
            else
            {
                m_count_down_txt.Text = "00:00:00";
            }
        }



    }
}
