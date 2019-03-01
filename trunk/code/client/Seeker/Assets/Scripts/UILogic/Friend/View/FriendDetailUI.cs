using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
namespace SeekerGame
{
    class FriendDetailUI : BaseViewComponet<FriendUILogic>
    {
        //GameLabel m_title_text;
        GameLabel m_name_text;
        GameNetworkRawImage m_head_tex;
        GameLabel m_lvl_text;
        GameLabel m_rank_text;
        GameLabel m_player_id_text;
        GameUIContainer m_achievement_grid;

        private string title_str;
        private string id_str;

        protected override void OnInit()
        {
            this.SetCloseBtnID("Panel_tipsanimate:Panel_lift:Image (2):Button_close");
            //m_title_text = this.Make<GameLabel>("Panel_tipsanimate:Panel_lift:Image_title:Text");
            //m_title_text.Text = LocalizeModule.Instance.GetString("UI_Police.detail");

            m_name_text = this.Make<GameLabel>("Panel_tipsanimate:Panel_lift:Image (1):Text (1)");
            m_head_tex = this.Make<GameNetworkRawImage>("Panel_tipsanimate:Panel_lift:Image (1):RawImage_head");
            m_lvl_text = this.Make<GameLabel>("Panel_tipsanimate:Panel_lift:Slider:Text");
            m_rank_text = this.Make<GameLabel>("Panel_tipsanimate:Panel_lift:Image (2):title");
            m_player_id_text = this.Make<GameLabel>("Panel_tipsanimate:Panel_lift:Image (2):ID");
            m_achievement_grid = this.Make<GameUIContainer>("Panel_tipsanimate:Panel_lift:Image (2):Scroll View:Viewport");

            title_str = LocalizeModule.Instance.GetString("title_name");
            id_str = LocalizeModule.Instance.GetString("friend_self_ID");
        }

        public override void OnShow(object param)
        {
        }

        public override void OnHide()
        {
        }


        public void Refresh(PlayerFriendMsg info_, IEnumerable<AchievementFriendMsg> achievements_)
        {

            ConfTitle my_title = ConfTitle.Get(info_.TitleId);
            string temp_str = null != my_title ? LocalizeModule.Instance.GetString(my_title.name) : "";
            m_rank_text.Text = $"{title_str}: {temp_str}";

            m_name_text.Text = info_.Name;
            m_lvl_text.Text = LocalizeModule.Instance.GetString("UI_start_1.lvl", info_.Level);

            if (CommonTools.IsNeedDownloadIcon(info_.Icon))
            {
                if (FriendDataManager.Instance.Friend_icons_dict.ContainsKey(info_.PlayerId))
                {
                    FriendIcon f_icon = FriendDataManager.Instance.Friend_icons_dict[info_.PlayerId];
                    if (f_icon.Url != info_.Icon)
                    {
                        m_head_tex.OnLoadFinish = (tex_) => { f_icon.Url = info_.Icon; f_icon.m_tex = tex_; };
                        m_head_tex.TextureName = info_.Icon;
                    }
                    else
                    {
                        m_head_tex.SetTexture(f_icon.m_tex);
                    }
                }
                else
                {
                    m_head_tex.OnLoadFinish = (tex_) =>
                    {
                        FriendIcon f_icon = new FriendIcon();
                        f_icon.Url = info_.Icon;
                        f_icon.m_tex = tex_;
                        FriendDataManager.Instance.Friend_icons_dict.Add(info_.PlayerId, f_icon);
                    };
                    m_head_tex.TextureName = info_.Icon;
                }

            }
            else
            {
                m_head_tex.TextureName = CommonData.GetBigPortrait(info_.Icon);
            }

            m_player_id_text.Text = $"{id_str}: {info_.PlayerId.ToString()}";

            List<AchievementFriendMsg> msgs = new List<AchievementFriendMsg>(achievements_);

            m_achievement_grid.Clear();

            if (msgs.Count > 0)
            {
                List<AchievementFriendMsg> filtered_msgs = msgs.FindAll((item) =>
              {
                  return (item.SubmitStatus & (1 << 1)) > 0 || (item.SubmitStatus & (1 << 2)) > 0 || (item.SubmitStatus & (1 << 3)) > 0;
              });

                if (filtered_msgs.Count > 4)
                {
                    filtered_msgs = filtered_msgs.GetRange(0, 4);
                }

                if (filtered_msgs.Count > 0)
                {
                    m_achievement_grid.EnsureSize<PlayerAchievementComponent>(filtered_msgs.Count);

                    for (int i = 0; i < m_achievement_grid.ChildCount; ++i)
                    {
                        ConfAchievement confAchieve = ConfAchievement.Get(filtered_msgs[i].Id);
                        PlayerAchievementComponent item = m_achievement_grid.GetChild<PlayerAchievementComponent>(i);

                        if (confAchieve != null)
                        {
                            string reward_icon = string.Empty;
                            long finish_time = 0L;
                            if ((filtered_msgs[i].SubmitStatus & (1 << 3)) > 0)
                            {
                                reward_icon = confAchieve.rewardicon3;
                                finish_time = filtered_msgs[i].FinishTime;
                            }
                            else if ((filtered_msgs[i].SubmitStatus & (1 << 2)) > 0)
                            {
                                reward_icon = confAchieve.rewardicon2;
                                finish_time = filtered_msgs[i].FinishTime2;
                            }
                            else if ((filtered_msgs[i].SubmitStatus & (1 << 1)) > 0)
                            {
                                reward_icon = confAchieve.rewardicon1;
                                finish_time = filtered_msgs[i].FinishTime1;
                            }

                            item.SetData(reward_icon, confAchieve.name, finish_time);
                        }

                        item.Visible = true;
                    }
                }
            }
        }
    }
}
