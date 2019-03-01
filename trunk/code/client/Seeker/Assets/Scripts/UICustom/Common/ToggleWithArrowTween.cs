using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using System;
using UnityEngine.UI;

namespace SeekerGame
{
    public class ToggleWithArrowTween : GameUIComponent
    {
        private SafeAction<bool, int> m_onItemIsClicked;

        private Text[] m_info_text;

        private Image m_icon_img;

        private UITweenerBase m_tween;

        private GameToggleButton m_item_btn;

        private int m_item_id;
        public int Item_id
        {
            get { return m_item_id; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_info_text = this.gameObject.transform.GetComponentsInChildren<Text>();
            m_icon_img = GetChildByNameRecursive(this.gameObject.transform, "Arrow").GetComponent<Image>();
            m_item_btn = Make<GameToggleButton>(gameObject);
            m_tween = this.GetComponent<UITweenerBase>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_item_btn.AddChangeCallBack(BtnClick);

        }

        public override void OnHide()
        {
            base.OnHide();
            m_item_btn.RemoveChangeCallBack(BtnClick);
        }

        public bool Checked
        {
            set
            {
                DoCheck(value);
            }

            get
            {
                return m_item_btn.Checked;
            }
        }


        public void Refresh(int id_, string info_, bool checked_, Action<bool, int> clicked_)
        {
            this.m_item_id = id_;

            if (null != this.m_info_text)
            {
                for (int i = 0; i < this.m_info_text.Length; ++i)
                {
                    this.m_info_text[i].text = info_;
                    if (i > 0)
                    {
                        this.m_info_text[i].gameObject.SetActive(false);
                    }
                }

                ShowLable(checked_);
            }

            this.m_onItemIsClicked = clicked_;

            m_item_btn.Checked = checked_;

            if (null != m_icon_img)
                m_icon_img.gameObject.SetActive(checked_);

        }

        private void DoCheck(bool checked_)
        {

            ShowLable(checked_);

            m_item_btn.Checked = checked_;

            if (null != m_icon_img)
                m_icon_img.gameObject.SetActive(checked_);
        }



        private void ShowLable(bool selected_)
        {
            if (null != this.m_info_text)
            {
                if (this.m_info_text.Length > 1)
                {
                    this.m_info_text[0].gameObject.SetActive(!selected_);
                    this.m_info_text[1].gameObject.SetActive(selected_);
                }
            }
        }

        void BtnClick(bool value)
        {
            if (value)
            {
                if (null != m_tween)
                    this.m_tween.PlayForward();
            }
            else
            {
                if (null != m_tween)
                    this.m_tween.PlayBackward();
            }

            if (null != m_icon_img)
                this.m_icon_img.gameObject.SetActive(value);

            ShowLable(value);

            if (!m_onItemIsClicked.IsNull)
            {
                this.m_onItemIsClicked.SafeInvoke(value, this.m_item_id);
            }
        }


        private Transform GetChildByNameRecursive(Transform root_, string name_)
        {
            foreach (Transform child in root_)
            {
                if (child.name == name_)
                    return child;

                Transform bottom_child = GetChildByNameRecursive(child, name_);

                if (null != bottom_child)
                    return bottom_child;

            }

            return null;
        }

    }
}

