using EngineCore;

namespace SeekerGame
{
    public class BagItem : GameUIContainer
    {
        private GameImage m_Icon_img;
        private GameLabel m_Num_lab;

        //private GameButton m_item_btn;
        private GameToggleButton m_item_btn;
        private TweenRotationEuler m_Rota_tween;
        private bool m_isFirst = false;
        private UIBagPropInfo m_bagInfo;
        private PropData m_propData;
        private PropData m_lastPropData = null;
        //private GameUIEffect m_effect = null;

        protected override void OnInit()
        {
            base.OnInit();
            m_Icon_img = Make<GameImage>("Image");
            m_Num_lab = Make<GameLabel>("Text");
            m_item_btn = Make<GameToggleButton>(gameObject);
            m_item_btn.AddChangeCallBack(btnClick);
            m_Rota_tween = gameObject.GetComponent<TweenRotationEuler>();
            //this.m_effect = Make<GameUIEffect>("UI_xuanzhong");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            //this.m_effect.EffectPrefabName = "UI_xuanzhong.prefab";
            //this.m_effect.Visible = false;
        }


        void btnClick(bool value)
        {
            if (value)
            {
                if (null != m_lastPropData)
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.table_change.ToString());
                //m_Rota_tween.PlayForward();
                if (m_bagInfo != null && m_propData != null)
                {
                    m_bagInfo.setInfoData(m_propData);
                }
                GameEvents.UIEvents.UI_Bag_Event.OnItemClick.SafeInvoke(gameObject.transform);
                //this.m_effect.Visible = true;
            }
            else
            {
                //m_Rota_tween.PlayBackward();
                //this.m_effect.Visible = false;
            }
        }

        private void PropCost()
        {
        }

        public void setData(PropData pd, UIBagPropInfo bagInfo, bool isFirst)
        {
            m_propData = pd;
            m_bagInfo = bagInfo;
            if (pd == null || bagInfo == null)
            {
                return;
            }
            m_Icon_img.Sprite = pd.prop.icon;
            m_Num_lab.Text = pd.num.ToString();
            this.m_isFirst = isFirst;
            if (isFirst)
            {
                if (m_item_btn.Checked)
                {
                    btnClick(true);
                }
                else
                {
                    m_item_btn.Checked = true;
                }

            }

            m_lastPropData = m_propData;
        }

        public override void OnHide()
        {
            base.OnHide();
            m_lastPropData = null;
        }

        public override void Dispose()
        {
            base.Dispose();
            m_lastPropData = null;
            m_item_btn.RemoveChangeCallBack(btnClick);
        }
    }
}

