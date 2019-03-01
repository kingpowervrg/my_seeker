//using EngineCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using GOEngine;
//using GOGUI;
//using DG.Tweening;

//namespace SeekerGame
//{
//    public class EventGameTweenDialogUIView : GameUIComponent
//    {
//        private const float C_TWEEN_TIME = 2.5f;

//        #region 对话信息

//        private GameLabel m_feedback_text;
//        private GameLabel m_dialogue_text;
//        private GameTexture m_police_tex;

//        private GameButton m_ok_btn;
//        #endregion

//        protected override void OnInit()
//        {
//            base.OnInit();

//            m_feedback_text = this.Make<GameLabel>("Text");
//            m_dialogue_text = this.Make<GameLabel>("Image_Dig:Text (1)");
//            m_police_tex = this.Make<GameTexture>("RawImage (1)");

//            m_ok_btn = this.Make<GameButton>("btnSale");
//        }

//        long m_finished_phase_id;
//        int m_valuation;
//        long m_most_valuable_officer_id;

//        public void Refresh(long finished_phase_id, int valuation, long most_valuable_officer_id)
//        {
//            m_finished_phase_id = finished_phase_id;
//            m_valuation = valuation;
//            m_most_valuable_officer_id = most_valuable_officer_id;
//        }

//        public override void OnShow(object param)
//        {
//            base.OnShow(param);

//            if (0 == m_finished_phase_id)
//                return;


//            long finished_phase_id = m_finished_phase_id;
//            int valuation = m_valuation;
//            long most_valuable_officer_id = m_most_valuable_officer_id;

//            ConfEvent event_data = ConfEvent.Get(EventGameManager.Instance.Event_id);

//            string t_feedback, t_dialogue;
//            EventGameUIAssist.GetFeedBackAndDialogue(most_valuable_officer_id, finished_phase_id, valuation, out t_feedback, out t_dialogue);

//            m_feedback_text.Text = t_feedback;
//            m_dialogue_text.Text = t_dialogue;
//            if (0 != most_valuable_officer_id)
//            {
//                m_police_tex.TextureName = ConfOfficer.Get(most_valuable_officer_id).hollowPortrait;
//            }
//            else
//            {
//                m_police_tex.TextureName = CommonData.GetBigPortrait(GlobalInfo.MY_PLAYER_INFO.PlayerIcon);
//            }

//            this.AddClick();

//            EventGameManager.Instance.LastScore = EventGameManager.Instance.Score;
//        }

//        public override void OnHide()
//        {
//            base.OnHide();

//            this.RemoveClick();

//        }







//        void AddClick()
//        {
//            this.m_ok_btn.AddClickCallBack(Clicked);

//        }

//        void RemoveClick()
//        {
//            this.m_ok_btn.RemoveClickCallBack(Clicked);
//        }

//        public void Clicked(GameObject obj)
//        {
//            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
//            ((EventGamePlayUILogic)LogicHandler).NextPhaseOnClicked();
//        }


//    }
//}
