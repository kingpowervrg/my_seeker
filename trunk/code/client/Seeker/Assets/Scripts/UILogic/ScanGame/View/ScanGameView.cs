//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using UnityEngine;
using GOEngine;
using Google.Protobuf;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace SeekerGame
{
    public class ScanGameView : BaseViewComponet<ScanGameUILogic>
    {
        #region 游戏中
        class touch_item_view_type
        {
            public long m_id;
            public SCAN_GAME_ITEM_TYPE m_type;
        }

        GameLabel m_time_txt;
        GameTexture m_input_tex;
        GameButton m_pause_btn;
        GameUIContainer m_clue_progress_grid;
        GameRecycleContainer m_normal_grid;
        //GameRecycleContainer m_special_grid;
        GameUIContainer m_special_grid;
        //GameUIContainer m_anchors_grid;
        GameRecycleContainer m_details_grid;
        GameRecycleContainer m_fly_icons_grid;

        Queue<touch_item_view_type> m_active_item_view_queue = new Queue<touch_item_view_type>();
        #endregion

        #region 开始界面
        TweenPosition m_fly_time_tween_pos;
        GameLabel m_fly_time_txt;
        TweenPosition m_start_view_tween_pos;
        ScanStartView m_start_view;

        bool m_start_view_visible = false;
        #endregion

        long m_scan_id;



        protected override void OnInit()
        {
            base.OnInit();
            m_time_txt = Make<GameLabel>("Text");
            m_input_tex = Make<GameTexture>("RawImage_bg");
            m_pause_btn = Make<GameButton>("Button_pause");
            m_clue_progress_grid = Make<GameUIContainer>("Canvas:ClueProgressGrid");
            m_clue_progress_grid.Visible = false;
            m_normal_grid = Make<GameRecycleContainer>("Normal_Grid");
            m_normal_grid.EnsureSize<NormalItemView>(5);
            for (int i = 0; i < m_normal_grid.ChildCount; ++i)
            {
                m_normal_grid.GetChild<NormalItemView>(i).Refresh(i);
            }

            m_special_grid = Make<GameRecycleContainer>("Special_Grid");
            //m_special_grid.EnsureSize<SpecialItemView>(5);
            //for (int i = 0; i < m_special_grid.ChildCount; ++i)
            //{
            //    m_special_grid.GetChild<SpecialItemView>(i).Refresh(i);
            //}

            //m_anchors_grid = Make<GameUIContainer>("Anchors_Grid");

            m_details_grid = Make<GameRecycleContainer>("Detail_Grid");
            m_details_grid.EnsureSize<ClueDetailView>(3);

            m_fly_icons_grid = Make<GameRecycleContainer>("FlyIcon_Grid");
            m_fly_icons_grid.EnsureSize<FlyIconItemView>(3);


            m_fly_time_txt = Make<GameLabel>("Text (2)");
            m_fly_time_tween_pos = m_fly_time_txt.GetComponent<TweenPosition>();
            m_start_view = Make<ScanStartView>("Panel_start");
            m_start_view_tween_pos = m_start_view.GetComponent<TweenPosition>();
        }



        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_pause_btn.AddClickCallBack(OnBtnGamePauseClick);
            m_input_tex.AddPressUpCallBack(OnTexPointerUp);
            m_input_tex.AddPressDownCallBack(OnTexPointerDown);
            m_input_tex.AddDragCallBack(OnTexDrag);
            m_input_tex.AddDragStartCallBack(OnTexDragStart);
            m_input_tex.AddDragEndCallBack(OnTexDragEnd);
            m_start_view.AddClickCallBack(OnStartViewClicked);
            m_clue_progress_grid.AddClickCallBack(OnProgressClicked);

            m_fly_time_tween_pos.AddTweenCompletedCallback(OnFlyTimeTweenFinished);
            m_start_view_tween_pos.AddTweenCompletedCallback(OnStartViewTweenFinished);

            m_active_item_view_queue.Clear();

            if (null != param)
            {
                List<long> my_param = param as List<long>;
                this.m_scan_id = my_param[0];
            }
            else
            {
                m_scan_id = CurViewLogic().Scan_id;
            }
            Refresh(m_scan_id);

            TimeModule.Instance.SetTimeout(() => { HideStartView(); m_start_view.SetTimeVisible(false); m_fly_time_txt.Visible = true; }, 1.0f);
        }

        public override void OnHide()
        {
            base.OnHide();

            m_pause_btn.RemoveClickCallBack(OnBtnGamePauseClick);
            m_input_tex.RemovePressUpCallBack(OnTexPointerUp);
            m_input_tex.RemovePressDownCallBack(OnTexPointerDown);
            m_input_tex.RemoveDragCallBack(OnTexDrag);
            m_input_tex.RemoveDragStartCallBack(OnTexDragStart);
            m_input_tex.RemoveDragEndCallBack(OnTexDragEnd);
            m_start_view.RemoveClickCallBack(OnStartViewClicked);

            m_fly_time_tween_pos.RemoveTweenCompletedCallback(OnFlyTimeTweenFinished);
            m_start_view_tween_pos.RemoveTweenCompletedCallback(OnStartViewTweenFinished);


        }

        public void RemoveSpecailItem(long id_)
        {
            for (int i = 0; i < m_special_grid.ChildCount; ++i)
            {
                var special = m_special_grid.GetChild<SpecialItemView>(i);

                if (id_ == special.Unique_id)
                {
                    m_special_grid.RemoveChild<SpecialItemView>(i);
                    break;
                }
            }
        }

        public void RecycleDetailItemView(ClueDetailView view_)
        {
            m_details_grid.RecycleElement<ClueDetailView>(view_);
        }


        public void RecycleFlyIconItemView(FlyIconItemView view_)
        {
            m_fly_icons_grid.RecycleElement<FlyIconItemView>(view_);
        }


        public void RecycleItem(SCAN_GAME_ITEM_TYPE type_, long id_)
        {

            if (SCAN_GAME_ITEM_TYPE.NORMAL == type_)
            {
                for (int i = 0; i < m_normal_grid.ChildCount; ++i)
                {
                    var item = m_normal_grid.GetChild<NormalItemView>(i);

                    if (item.Unique_id == id_)
                    {
                        Debug.Log("Die " + item);
                        m_normal_grid.RecycleElement<NormalItemView>(item);
                        break;
                    }
                }
            }
            else if (SCAN_GAME_ITEM_TYPE.SPECIAL == type_)
            {
                //for (int i = 0; i < m_special_grid.ChildCount; ++i)
                //{
                //    var item = m_special_grid.GetChild<SpecialItemView>(i);

                //    if (item.Unique_id == id_)
                //    {
                //        m_special_grid.RecycleElement<SpecialItemView>(item);
                //        break;
                //    }
                //}
            }

        }

        public void ShowDetail(long clue_id_)
        {
            Dictionary<int, HashSet<long>> scan_datas = ScanDataManager.Instance.Examin_clue_datas(m_scan_id);

            int scan_type = 0;

            foreach (var kvp in scan_datas)
            {
                if (kvp.Value.Contains(clue_id_))
                {
                    scan_type = kvp.Key;
                    break;
                }
            }

            var detail = m_details_grid.GetAvaliableContainerElement<ClueDetailView>();
            detail.Refresh(clue_id_, ConfFindClue.Get(clue_id_).icon, ConfFindClue.Get(clue_id_).detail);
            detail.Visible = true;

        }


        public void ShowFlyIcon(long clue_id_)
        {
            var fly = m_fly_icons_grid.GetAvaliableContainerElement<FlyIconItemView>();

            Dictionary<int, HashSet<long>> scan_datas = ScanDataManager.Instance.Examin_clue_datas(m_scan_id);

            int scan_type = 0;
            foreach (var kvp in scan_datas)
            {
                if (kvp.Value.Contains(clue_id_))
                {
                    scan_type = kvp.Key;
                    break;
                }
            }

            for (int i = 0; i < m_clue_progress_grid.ChildCount; ++i)
            {
                var progress = m_clue_progress_grid.GetChild<ClueProgressItemView>(i);

                if (scan_type == m_clue_progress_grid.GetChild<ClueProgressItemView>(i).Scan_type)
                {
                    fly.Refresh(clue_id_, ConfFindClue.Get(clue_id_).icon, m_fly_icons_grid.Position, progress.Position);
                    fly.Visible = true;
                    break;
                }
            }
        }

        public void DisableFind()
        {
            m_input_tex.RemovePressUpCallBack(OnTexPointerUp);
            m_input_tex.RemovePressDownCallBack(OnTexPointerDown);
            m_input_tex.RemoveDragCallBack(OnTexDrag);

            m_pause_btn.RemoveClickCallBack(OnBtnGamePauseClick);
            m_clue_progress_grid.RemoveClickCallBack(OnProgressClicked);

            m_start_view.Visible = false;
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GAME_MAIN_SETTING);
        }

        public void AddClueProgress(long clue_id_)
        {
            Dictionary<int, HashSet<long>> scan_datas = ScanDataManager.Instance.Examin_clue_datas(m_scan_id);

            foreach (var kvp in scan_datas)
            {
                if (kvp.Value.Contains(clue_id_))
                {

                    for (int i = 0; i < m_clue_progress_grid.ChildCount; ++i)
                    {
                        var progress = m_clue_progress_grid.GetChild<ClueProgressItemView>(i);

                        if (progress.Scan_type != kvp.Key)
                            continue;

                        progress.AddProgress(kvp.Key, 1);
                        break;
                    }

                    break;
                }
            }
        }

        public Vector3 GetClueProgressItemViewWorldPos(int scan_type_)
        {
            for (int i = 0; i < m_clue_progress_grid.ChildCount; ++i)
            {
                var clue_progress = m_clue_progress_grid.GetChild<ClueProgressItemView>(i);

                if (clue_progress.Scan_type == scan_type_)
                {
                    return clue_progress.Position;
                }
            }

            return m_clue_progress_grid.Position;
        }
        NormalItemView ShowItem(SCAN_GAME_ITEM_TYPE type_, Vector2 local_pos_)
        {
            if (SCAN_GAME_ITEM_TYPE.NORMAL == type_)
            {
                var normal = m_normal_grid.GetAvaliableContainerElement<NormalItemView>();
                Debug.Log("Born " + normal);
                normal.SetPos(local_pos_);
                normal.Visible = true;

                m_active_item_view_queue.Enqueue(new touch_item_view_type() { m_id = normal.Unique_id, m_type = SCAN_GAME_ITEM_TYPE.NORMAL });

                return normal;
            }
            else if (SCAN_GAME_ITEM_TYPE.SPECIAL == type_)
            {
                //var special = m_special_grid.GetAvaliableContainerElement<SpecialItemView>();
                //special.Visible = true;
                //m_active_item_view_queue.Enqueue(new touch_item_view_type() { m_id = special.Unique_id, m_type = SCAN_GAME_ITEM_TYPE.SPECIAL });
                return null;
            }

            return null;
        }

        void HideItem()
        {
            var active_item = m_active_item_view_queue.Dequeue();

            if (null == active_item)
                return;



            RecycleItem(active_item.m_type, active_item.m_id);
        }

        void Refresh(long scan_id_)
        {
            ConfFind scan_data = ConfFind.Get(scan_id_);

            UpdateTime((double)scan_data.time);
            m_time_txt.Visible = false;
            m_fly_time_txt.Text = m_time_txt.Text;
            m_fly_time_txt.Visible = false;

            RefreshAnchors(scan_data);

            RefreshClueProgress(scan_data);


        }

        void RefreshClueProgress(ConfFind scan_data)
        {

            Dictionary<int, HashSet<long>> scan_datas = ScanDataManager.Instance.Examin_clue_datas(scan_data.id);

            m_clue_progress_grid.EnsureSize<ClueProgressItemView>(scan_datas.Keys.Count);

            int i = 0;
            foreach (var kvp in scan_datas)
            {
                int scan_type = kvp.Key;
                var item = m_clue_progress_grid.GetChild<ClueProgressItemView>(i);
                item.Refresh(scan_type, ConfFindTypeIcon.Get(scan_type).icon, kvp.Value.Count);
                item.Visible = true;
                ++i;
            }

            m_clue_progress_grid.Visible = false;
        }

        public void UpdateTime(double senconds_)
        {
            m_time_txt.Text = CommonTools.SecondToStringMMSS(senconds_);
        }


        void RefreshAnchors(ConfFind scan_data_)
        {

            ScanDataManager.Instance.GetScanData(scan_data_.id, CreateClueAnchorsAndBGTex);
        }

        //void CreateClueAnchorsAndBGTex(ScanJsonData data_)
        //{
        //    if (!this.Visible)
        //        return;

        //    m_anchors_grid.EnsureSize<ClueAnchorItemView>(data_.M_anchors.Count);

        //    for (int i = 0; i < m_anchors_grid.ChildCount; ++i)
        //    {
        //        var clue_anchor_item_view = m_anchors_grid.GetChild<ClueAnchorItemView>(i);

        //        var clue_anchor_data = data_.M_anchors[i];

        //        clue_anchor_item_view.Refresh(clue_anchor_data.M_clue_id, clue_anchor_data.M_x, clue_anchor_data.M_y);
        //        clue_anchor_item_view.Visible = true;
        //    }
        //}



        void CreateClueAnchorsAndBGTex(ScanJsonData data_)
        {
            if (!this.Visible)
                return;
            m_input_tex.TextureName = data_.M_tex_name;
            m_input_tex.Widget.anchoredPosition = new Vector2(data_.M_tex_x, data_.M_tex_y);
            m_input_tex.Widget.sizeDelta = new Vector2(data_.M_tex_w, data_.M_tex_h);

            m_special_grid.EnsureSize<SpecialItemView>(data_.M_anchors.Count);

            for (int i = 0; i < m_special_grid.ChildCount; ++i)
            {
                var clue_anchor_item_view = m_special_grid.GetChild<SpecialItemView>(i);

                var clue_anchor_data = data_.M_anchors[i];

                clue_anchor_item_view.RefreshClueID(clue_anchor_data.M_clue_id, clue_anchor_data.M_w, clue_anchor_data.M_h);
                clue_anchor_item_view.Refresh(i);
                clue_anchor_item_view.Widget.anchoredPosition = new Vector2(clue_anchor_data.M_x, clue_anchor_data.M_y);
                //clue_anchor_item_view.Widget.sizeDelta = new Vector2(clue_anchor_data.M_w, clue_anchor_data.M_h);
                clue_anchor_item_view.Visible = true;
            }
        }




        void OnBtnGamePauseClick(GameObject go)
        {

            CurViewLogic().Pause(true);

        }

        NormalItemView m_cur_normal_item_view;

        void OnTexPointerDown(GameObject obj)
        {
            Debug.Log("OnTexPointerDown !!!!!!!!!!!!!!!");
            Vector2 pointer_pos = m_input_tex.BaseEventTriggerListener.PointerEventData.position;
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(CurViewLogic().Transform, pointer_pos, FrameMgr.Instance.UICamera, out localPos);
            m_cur_normal_item_view = ShowItem(SCAN_GAME_ITEM_TYPE.NORMAL, localPos);
        }

        void OnTexPointerUp(GameObject obj)
        {
            Debug.Log("OnTexPointerUp !!!!!!!!!!!!!!!");
            m_cur_normal_item_view = null;
            HideItem();
        }
        void OnTexDragStart(GameObject go, Vector2 delta)
        {
            Debug.Log("OnTexDragStart !!!!!!!!!!!!!!!");
        }

        void OnTexDragEnd(GameObject go, Vector2 delta)
        {
            Debug.Log("OnTexDragEnd !!!!!!!!!!!!!!!");
        }

        void OnTexDrag(GameObject go, Vector2 delta, Vector2 pointer_pos)
        {
            if (null == m_cur_normal_item_view)
                return;

            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(CurViewLogic().Transform, pointer_pos, FrameMgr.Instance.UICamera, out localPos))
                m_cur_normal_item_view.SetPos(localPos);
        }

        void ShowStartView()
        {
            m_start_view_tween_pos.ResetAndPlay(false);
            m_start_view_visible = true;
        }
        void HideStartView()
        {
            if (!m_start_view.Visible)
                return;

            m_start_view_tween_pos.ResetAndPlay(true);
            m_start_view_visible = false;
        }

        void OnProgressClicked(GameObject obj)
        {
            m_clue_progress_grid.Visible = false;
            ShowStartView();
        }

        void OnStartViewClicked(GameObject obj)
        {
            HideStartView();
        }


        void OnFlyTimeTweenFinished()
        {
            m_time_txt.Visible = true;
        }

        void OnStartViewTweenFinished()
        {
            if (m_start_view_visible)
            {
                return;
            }
            else
            {
                m_clue_progress_grid.Visible = true;
                CurViewLogic().Pause(false);
            }


        }




    }


}