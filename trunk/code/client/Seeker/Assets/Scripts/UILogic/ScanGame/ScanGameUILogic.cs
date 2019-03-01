
//#define TEST
using EngineCore;
using Google.Protobuf;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_SCAN_GAME)]
    public class ScanGameUILogic : BaseViewComponetLogic
    {
        ScanGameView m_game_view;
        ScanResultView m_result_view;
        ScanRewardView m_reward_view;

        GameImage m_fly_vit_icon;
        TweenPosition m_fly_vit_ts;
        GameUIComponent m_fly_vit_aim;

        long m_scan_id;
        public long Scan_id
        {
            get { return m_scan_id; }
        }

        long m_task_id;
        public float m_total_time;
        public float m_cur_time;
        public bool m_is_update_time = false;

        HashSet<long> m_need_find_clue_ids = new HashSet<long>();
        int m_total_clue_num;
        int m_cur_clue_num;
        public int Cur_clue_num
        {
            get { return m_cur_clue_num; }
        }
        bool m_win;
        public bool Win
        {
            get { return m_win; }
        }

        SortedDictionary<int, HashSet<long>> cur_game_finded_clues = new SortedDictionary<int, HashSet<long>>();
        public System.Collections.Generic.SortedDictionary<int, System.Collections.Generic.HashSet<long>> Cur_game_finded_clues
        {
            get { return cur_game_finded_clues; }
        }

        int m_reward_vit_num;
        protected override void OnInit()
        {
            base.OnInit();

            NeedUpdateByFrame = true;
            //IsFullScreen = true;

            m_game_view = Make<ScanGameView>("InGame");
            m_result_view = Make<ScanResultView>("Result");
            m_reward_view = Make<ScanRewardView>("Panel_energy");
            m_fly_vit_icon = Make<GameImage>("Image_Fly_Vit");
            m_fly_vit_ts = m_fly_vit_icon.GetComponent<TweenPosition>();
            m_fly_vit_aim = Make<GameUIComponent>("Panel_top:Image_energy:Image_icon");

        }

        public override void OnShow(object param)
        {

            base.OnShow(param);

            GameEvents.UIEvents.UI_Pause_Event.OnQuit += Quit;
            GameEvents.UIEvents.UI_Scan_Event.Listen_FindClue += FindClue;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RemoveClueAnchor += RemoveClue;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleDetailItemView += RecycleDetailItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyIconItemView += RecycleFlyIconItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowFlyIconItemView += ShowFlyIconItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_AddClueProgress += AddClueProgress;
            GameEvents.UIEvents.UI_Scan_Event.Listen_VitEffectFinishFly += RecycleFlyVitEffectAndPlayVitNum;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyVitNumItemView += RecycleFlyVitNum;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowReward += ShowReward;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShakeFinished += ShowVitIcon;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ResumeGame += Resume;

            m_fly_vit_ts.AddTweenCompletedCallback(FlyVitFinished);

            MessageHandler.RegisterMessageHandler(MessageDefine.SCFindRewardResp, OnScResponse);

            if (null != param)
            {
                List<long> my_param = param as List<long>;
                this.m_scan_id = my_param[0];
                this.m_task_id = my_param[1];
            }


            ConfFind scan_data = ConfFind.Get(m_scan_id);
            m_cur_time = m_total_time = scan_data.time;
            m_total_clue_num = GetTotalClueNumAndContent(m_scan_id);
            m_cur_clue_num = 0;
            cur_game_finded_clues.Clear();
            m_reward_vit_num = 0;
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.WINDOWED;


        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            GameEvents.UIEvents.UI_Pause_Event.OnQuit -= Quit;
            GameEvents.UIEvents.UI_Scan_Event.Listen_FindClue -= FindClue;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RemoveClueAnchor -= RemoveClue;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleDetailItemView -= RecycleDetailItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyIconItemView -= RecycleFlyIconItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowFlyIconItemView -= ShowFlyIconItemView;
            GameEvents.UIEvents.UI_Scan_Event.Listen_AddClueProgress -= AddClueProgress;
            GameEvents.UIEvents.UI_Scan_Event.Listen_VitEffectFinishFly -= RecycleFlyVitEffectAndPlayVitNum;
            GameEvents.UIEvents.UI_Scan_Event.Listen_RecycleFlyVitNumItemView -= RecycleFlyVitNum;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShowReward -= ShowReward;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ShakeFinished -= ShowVitIcon;
            GameEvents.UIEvents.UI_Scan_Event.Listen_ResumeGame -= Resume;

            m_fly_vit_ts.RemoveTweenCompletedCallback(FlyVitFinished);

            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCFindRewardResp, OnScResponse);
            
        }

        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {

            base.OnPackageRequest(imsg, msg_params);


            if (imsg is CSFindRewardReq)
            {
                var req = imsg as CSFindRewardReq;

                long findId = (long)msg_params[0];//尸检id
                int result = (int)msg_params[1];//1成功 2失败
                int foundNum = (int)msg_params[2];//找到的线索num
                int totalNum = (int)msg_params[3];//总共的线索num

                req.FindId = findId;
                req.Result = result;
                req.FoundNum = foundNum;
                req.TotalNum = totalNum;
            }



        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);

            if (s is SCFindRewardResp)
            {
                var rsp = s as SCFindRewardResp;

                if (MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                    return;


                m_reward_vit_num = rsp.Vit;


            }
        }


        public override void Update()
        {
            base.Update();

            UpdateTime();

        }

        void UpdateTime()
        {
            if (!m_is_update_time)
                return;

            m_cur_time -= Time.deltaTime;
            m_game_view.UpdateTime((double)m_cur_time);

            if (m_cur_time < 0)
            {
                //失败
                m_is_update_time = false;
                m_game_view.DisableFind();
                FinishGame(false);
            }
        }

        public void AddFindedClue(long clue_id_)
        {
            int clue_type = 0;

            foreach (var kvp in ScanDataManager.Instance.Examin_clue_datas(m_scan_id))
            {
                if (kvp.Value.Contains(clue_id_))
                {
                    clue_type = kvp.Key;
                    break;
                }
            }


            if (cur_game_finded_clues.ContainsKey(clue_type))
            {
                cur_game_finded_clues[clue_type].Add(clue_id_);
            }
            else
            {
                cur_game_finded_clues.Add(clue_type, new HashSet<long>() { clue_id_ });
            }
        }

        public Vector3 GetClueProgressItemViewWorldPos(int scan_type_)
        {
            return m_game_view.GetClueProgressItemViewWorldPos(scan_type_);
        }

        int GetTotalClueNumAndContent(long scan_id_)
        {
            int ret = 0;
            m_need_find_clue_ids.Clear();

            foreach (var kvp in ScanDataManager.Instance.Examin_clue_datas(scan_id_))
            {
                ret += kvp.Value.Count;
                m_need_find_clue_ids.UnionWith(kvp.Value);
            }

            return ret;
        }



        public void Quit()
        {
            this.CloseFrame(true);
        }



        public void Pause(bool v_)
        {
            if (!v_)
            {
                m_is_update_time = true;
            }
            else
            {
                m_is_update_time = false;

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(
                new FrameMgr.OpenUIParams(UIDefine.UI_GAME_MAIN_SETTING)
                {
                    Param = new PauseData()
                    {
                        m_mode = ENUM_SEARCH_MODE.E_SCAN,
                        m_id = m_scan_id,
                    }
                });
            }

        }

        public void FlyVitIcon()
        {
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            m_fly_vit_ts.From = m_fly_vit_icon.Position;
            m_fly_vit_ts.To = m_fly_vit_aim.Position;
            m_fly_vit_icon.Visible = true;
            m_game_view.Visible = false;
        }

        void FlyVitFinished()
        {


            int add_vit = 0;

            if (0 != m_reward_vit_num)
            {
                add_vit = m_reward_vit_num;
            }
            else
            {
                add_vit += Cur_clue_num;

                add_vit += Win ? 5 : 0;
            }



            GameEvents.UIEvents.UI_Scan_Event.Tell_AddVit.SafeInvoke(add_vit);

            this.CloseFrame(true);
        }

        void FindClue(long clue_id_)
        {
            if (!m_is_update_time)
                return;

            if (!m_need_find_clue_ids.Contains(clue_id_))
                return;

            m_need_find_clue_ids.Remove(clue_id_);

            AddFindedClue(clue_id_);

            m_game_view.ShowDetail(clue_id_);

            //是否找完
            ++m_cur_clue_num;

            if (m_cur_clue_num == m_total_clue_num)
            {
                m_is_update_time = false;

                //胜利
                m_game_view.DisableFind();

                TimeModule.Instance.SetTimeout(() => FinishGame(true), 1.0f);
            }

        }

        void AddClueProgress(long clue_id_)
        {
            m_game_view.AddClueProgress(clue_id_);
        }

        void RemoveClue(long clue_anchor_view_id_)
        {
            //更新ui
            m_game_view.RemoveSpecailItem(clue_anchor_view_id_);
        }

        void RecycleDetailItemView(ClueDetailView view_)
        {
            m_game_view.RecycleDetailItemView(view_);
        }


        void ShowFlyIconItemView(long clue_id_)
        {
            m_game_view.ShowFlyIcon(clue_id_);
        }

        void RecycleFlyIconItemView(FlyIconItemView view_)
        {
            m_game_view.RecycleFlyIconItemView(view_);
        }

        void RecycleFlyVitEffectAndPlayVitNum(FlyVitEffectItemView view_)
        {
            int vit_add_num = view_.Vit_add;

            m_reward_view.RecycleFlyVitEffect(view_);
            m_reward_view.ShakeVit(view_);
            m_reward_view.FlyVitNum(vit_add_num);

        }

        void RecycleFlyVitNum(FlyNumItemView view_)
        {
            m_reward_view.RecycleFlyNum(view_);
        }



        void FinishGame(bool win_)
        {
            m_win = win_;


            CSFindRewardReq req = new CSFindRewardReq();

            //int64 findId = 1;//尸检id
            //int32 result = 2;//1成功 2失败
            //int32 foundNum = 3;//找到的线索num
            //int32 totalNum = 4;//总共的线索num

            this.OnScAsyncRequest(req, m_scan_id, m_win ? 1 : 2, m_cur_clue_num, m_total_clue_num);


            TimeModule.Instance.SetTimeout(ShowResult, 1.5f);

            TimeModule.Instance.SetTimeout(HideResult, 3.5f);
        }


        void ShowResult()
        {
            float progress_ = (float)m_cur_clue_num / (float)m_total_clue_num;
            m_result_view.Refresh(progress_);
            m_result_view.Visible = true;
        }



        void HideResult()
        {
            m_result_view.Visible = false;

#if TEST

#endif

        }

        void ShowReward()
        {
            m_reward_view.Visible = true;
        }

        void ShowVitIcon()
        {
            m_reward_view.ShowVitIcon();
        }

        void Resume()
        {
            Pause(false);
        }

    }
}



