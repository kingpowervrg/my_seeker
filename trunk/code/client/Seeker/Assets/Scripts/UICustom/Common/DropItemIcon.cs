using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using EngineCore;
using UnityEngine;
namespace SeekerGame
{
    class DropItemIcon : GameUIComponent
    {

        protected GameImage m_icon;

        protected GameUIComponent m_iconRoot = null;


        protected long m_item_id = 0;
        protected int m_num;
        public int Num
        {
            get { return m_num; }
            set { m_num = value; }
        }

        protected int m_tween_num;
        public EngineCore.GameImage Icon
        {
            get { return m_icon; }
        }
        //private GameButton m_action_btn;

        protected GameImage m_num_root;
        protected GameLabel m_num_txt;

        protected Transform m_target = null;
        protected Transform m_bagTran = null;
        protected TweenScale m_tweenPos = null;
        protected EUNM_BASE_REWARD m_dropType = EUNM_BASE_REWARD.E_INVALID;

        protected Vector2 m_offset = Vector2.zero;

        protected bool m_tips = true;
        protected override void OnInit()
        {
            base.OnInit();
            this.FindUIComponent();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_icon.AddLongPressCallBack(PressDown);
            m_icon.AddPressUpCallBack(PressUp);
        }


        public override void OnHide()
        {
            base.OnHide();

            m_icon.RemoveLongPressCallBack(PressDown);
            m_icon.RemovePressUpCallBack(PressUp);
        }

        public virtual void SetNum(int num_, float duration_)
        {
            if (null != m_num_root)
                m_num_root.Visible = true;
            DOTween.To(() => { return m_tween_num; }, (val) => { m_tween_num = val; m_num_txt.Text = string.Format("x{0}", m_tween_num); }, num_, duration_);
            //TweenText.Begin(m_num_txt.Label, duration_, 0.0f, Num, num_);
            Num = num_;
        }

        public void InitSprite(string tex_name_, string num_str = "0", long item_id = 0)
        {
            m_icon.Sprite = tex_name_;
            m_item_id = item_id;

            int num_;

            if (int.TryParse(num_str, out num_))
            {
                this.InitSprite(tex_name_, num_, item_id);
            }
            else
            {
                if (null != m_num_txt)
                    m_num_txt.Text = num_str;
            }


        }

        public void EnableTips(bool v_)
        {
            m_tips = v_;
        }
        public void InitSprite(string tex_name_, int num_ = 0, long item_id = 0)
        {
            m_icon.Sprite = tex_name_;
            m_item_id = item_id;

            Num = num_;

            if (0 == num_)
            {
                if (null != m_num_root)
                    m_num_root.Visible = false;

                if (null != m_num_txt)
                    m_num_root.Visible = false;
            }
            else
            {
                if (null != m_num_root)
                {
                    m_num_root.Visible = true;
                }

                if (null != m_num_txt)
                {
                    m_num_txt.Text = num_.ToString();
                }
            }



        }

        public void InitToolTipOffset(Vector2 offset_)
        {
            m_offset = offset_;
        }

        public void SetTargetGameObj(EUNM_BASE_REWARD enumType)
        {
            this.m_dropType = enumType;
            if (GameEvents.UIEvents.UI_Drop_Event.GetPlayerInfoGameObject == null)
            {
                return;
            }
            Transform targetTrans = GameEvents.UIEvents.UI_Drop_Event.GetPlayerInfoGameObject(enumType);
            if (targetTrans == null)
            {
                this.m_target = this.m_bagTran;
                return;
            }
            this.m_target = targetTrans;
        }

        //背包位置
        public void SetTargetBagTransform(Transform bagTran)
        {
            this.m_bagTran = bagTran;
        }

        public void SetTargetGameObjByID(long itemId)
        {
            //EUNM_BASE_REWARD typeenum = EUNM_BASE_REWARD.E_VIT;
            ConfProp confprop = ConfProp.Get(itemId);
            if (confprop == null)
            {
                return;
            }
            this.m_dropType = EUNM_BASE_REWARD.E_INVALID;
            if (confprop.type == 8)
            {
                this.m_dropType = EUNM_BASE_REWARD.E_VIT;
            }
            else if (confprop.type == 5)
            {
                this.m_dropType = EUNM_BASE_REWARD.E_COIN;
            }
            else if (confprop.type == 6)
            {
                this.m_dropType = EUNM_BASE_REWARD.E_CASH;
            }
            else if (confprop.type == 7)
            {
                this.m_dropType = EUNM_BASE_REWARD.E_EXP;
            }
            if (GameEvents.UIEvents.UI_Drop_Event.GetPlayerInfoGameObject == null)
            {
                return;
            }
            m_target = GameEvents.UIEvents.UI_Drop_Event.GetPlayerInfoGameObject(this.m_dropType);
            if (m_target == null)
            {
                m_target = m_bagTran;
            }
        }

        private Vector3 m_startIconLocalPos = Vector3.zero;
        private Vector3 m_startIconLocalScale = Vector3.one;
        public bool PlayFlyToTarget()
        {
            if (m_target == null)
            {
                Debug.Log("error target ===");
                return false;
            }
            this.m_startIconLocalPos = this.m_icon.Widget.anchoredPosition;
            this.m_startIconLocalScale = this.m_icon.Widget.localScale;
            this.m_icon.Widget.SetParent(LogicHandler.Transform);
            this.Visible = false;
            //this.m_iconRoot.Visible = false;
            Vector3[] wayPoint = new Vector3[2];
            wayPoint[0] = this.m_icon.Position;
            wayPoint[1] = this.m_target.position;
            this.m_tweenPos = this.m_icon.gameObject.GetOrAddComponent<TweenScale>();
            this.m_tweenPos.From = this.m_icon.Widget.localScale;
            this.m_tweenPos.To = Vector2.one * 0.3f;
            this.m_icon.Widget.DOPath(wayPoint, 1f, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                PlayAddition();
                this.m_icon.Visible = false;
                this.m_icon.Widget.SetParent(m_iconRoot.Widget);
                GameObject.DestroyImmediate(this.m_tweenPos);
                this.m_icon.Widget.anchoredPosition = this.m_startIconLocalPos;
                this.m_icon.Widget.localScale = this.m_startIconLocalScale;
                this.m_icon.Widget.SetAsFirstSibling();
                this.m_icon.Visible = true;
            });
            this.m_tweenPos.ResetAndPlay();
            return true;
        }

        private void PlayAddition()
        {
            if (this.m_dropType == EUNM_BASE_REWARD.E_CASH)
            {
                GlobalInfo.MY_PLAYER_INFO.ChangeCash(Num);
            }
            else if (this.m_dropType == EUNM_BASE_REWARD.E_COIN)
            {
                GlobalInfo.MY_PLAYER_INFO.ChangeCoin(Num);
            }
            else if (this.m_dropType == EUNM_BASE_REWARD.E_VIT)
            {
                GlobalInfo.MY_PLAYER_INFO.ChangeVit(Num);
            }
        }

        private void FindUIComponent()
        {
            this.m_iconRoot = TryMake<GameUIComponent>("Background");

            if (null != this.m_iconRoot)
            {
                m_icon = m_iconRoot.Make<GameImage>("Image");
            }
            else
            {
                m_icon = Make<GameImage>("Image");
            }

            //m_tweenPos = m_icon.GetComponent<TweenScale>();
            m_num_root = this.TryMake<GameImage>("Background/Image (1)");
            if (null != m_num_root)
            {
                m_num_txt = m_num_root.Make<GameLabel>("Text");
            }
            else
            {
                m_num_txt = m_icon.TryMake<GameLabel>("Text");
            }
        }


        private void PressDown(GameObject go, Vector2 delta)
        {
            if (!m_tips)
                return;

            if (0 != m_item_id)
            {

                Vector2 screenPoint2 = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, this.Widget.position);

                int address_count = ConfProp.Get(m_item_id).address.Length;

                ToolTipsData data = new ToolTipsData()
                {
                    ItemID = m_item_id,
                    CurCount = Num,
                    MaxCount = 0,

                    ScreenPos = screenPoint2 - new Vector2(ToolTipsView.C_WIDTH * 0.5f + m_offset.x/* - this.Widget.sizeDelta.x * 0.5f*/, -10.0f * address_count + m_offset.y),
                };


                FrameMgr.OpenUIParams ui_data = new FrameMgr.OpenUIParams(UIDefine.UI_TOOL_TIPS)
                {
                    Param = data,
                };

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_data);
            }
        }

        private void PressUp(GameObject go)
        {
            if (!m_tips)
                return;

            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_TOOL_TIPS);
        }
    }

}
