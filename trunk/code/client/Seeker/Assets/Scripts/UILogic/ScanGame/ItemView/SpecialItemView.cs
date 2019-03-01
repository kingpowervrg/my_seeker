using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class SpecialItemView : NormalItemView
    {

        TweenFillAmount m_fill_tween;
        GameUIComponent m_special_view;
        GameUIComponent m_input;
        GameImage m_blood;

        long m_clue_id;
        bool m_is_finded;

        protected override void OnInit()
        {
            m_special_view = Make<GameUIComponent>("Panel_special");
            m_input = Make<GameUIComponent>("Input");
            m_blood = Make<GameImage>("Image_Blood_Root:Image_Blood");
            m_fill_tween = this.gameObject.transform.Find("Panel_special/Image (3)").GetComponent<TweenFillAmount>();

            m_special_view.Visible = false;
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_input.AddPressDownCallBack(PressDown);
            m_input.AddPressUpCallBack(PressUp);
            m_input.AddOverCallBack(OnEnterView);
            m_input.AddExitCallBack(OnExitView);


            m_fill_tween.AddTweenCompletedCallback(TweenFillFinished);

            m_is_finded = false;
        }

        public override void OnHide()
        {
            base.OnHide();

            m_input.RemovePressDownCallBack(PressDown);
            m_input.RemovePressUpCallBack(PressUp);
            m_input.RemoveOverCallBack(OnEnterView);
            m_input.RemoveExitCallBack(OnExitView);

            m_fill_tween.RemoveTweenCompletedCallback(TweenFillFinished);
        }

        void Unregister()
        {
            m_input.RemovePressDownCallBack(PressDown);
            m_input.RemovePressUpCallBack(PressUp);
            m_input.RemoveOverCallBack(OnEnterView);
            m_input.RemoveExitCallBack(OnExitView);

            m_fill_tween.RemoveTweenCompletedCallback(TweenFillFinished);
        }

        public void RefreshClueID(long clue_id_, float w, float h)
        {
            m_clue_id = clue_id_;
            m_input.Widget.sizeDelta = new Vector2(w, h);
            m_blood.Widget.sizeDelta = new Vector2(w, h);
        }

        void PauseFillAmount()
        {
            m_fill_tween.Stop();
        }

        void PressDown(GameObject obj)
        {
            if (m_is_finded)
                return;

            if (m_special_view.CachedVisible)
                return;

            Debug.Log("显示线索!!!!!!!!!!");

            m_special_view.Visible = true;
        }

        void PressUp(GameObject obj)
        {


            //if (!m_special_view.CachedVisible)
            //    return;


            if (m_special_view.CachedVisible)
            {
                Debug.Log("暂停线索!!!!!!!!!");
                PauseFillAmount();
            }

            if (m_special_view.CachedVisible)
            {
                Debug.Log("隐藏线索!!!!!!!!!");
                m_special_view.Visible = false;
            }

            if (m_is_finded)
            {
                if (this.CachedVisible)
                    this.Visible = false;
            }

        }

        void OnEnterView(GameObject obj)
        {
            PressDown(obj);
        }


        void OnExitView(GameObject obj)
        {
            PressUp(obj);
        }

        void TweenFillFinished()
        {
            Debug.Log("找到线索!!!!!!!!!");
            GameEvents.UIEvents.UI_Scan_Event.Listen_FindClue.SafeInvoke(m_clue_id);
            Unregister();
            m_is_finded = true;
            m_special_view.Visible = false;
            this.Visible = false;
        }


    }

}
