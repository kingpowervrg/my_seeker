using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GOEngine;
using EngineCore;
namespace SeekerGame
{
    public class LoseView : BaseViewComponet<ResultUILogic>
    {

        private GameButton m_close_btn;
        private GameUIContainer m_gifts_grid;
        private GameLabel m_fail_info_text;

        private ENUM_SEARCH_MODE m_mode = ENUM_SEARCH_MODE.E_INVALID;
        WinFailData m_data;

        protected override void OnInit()
        {
            base.OnInit();

            m_close_btn = Make<GameButton>("Button_close");
            m_gifts_grid = Make<GameUIContainer>("Panel_levelupreward:Grid");
            m_fail_info_text = Make<GameLabel>("Panel_levelupreward:Text (1)");
        }


        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                var ret = param as WinFailData;

                this.m_mode = ret.m_mode;
                m_data = ret;
            }

            m_close_btn.AddClickCallBack(OnCloseClicked);
        }
        public override void OnHide()
        {
            base.OnHide();


            m_close_btn.RemoveClickCallBack(OnCloseClicked);
        }

        public void Refresh(WinFailData data_)
        {
            m_mode = data_.m_mode;
            m_data = data_;

            RefreshLoseContent();
        }

        private void RefreshLoseContent()
        {

            if (ENUM_SEARCH_MODE.E_JIGSAW == this.m_mode)
            {
                string txt = "pintu_fail";
                m_fail_info_text.Text = LocalizeModule.Instance.GetString(txt);
            }
            else if (ENUM_SEARCH_MODE.E_SEARCH_ROOM == this.m_mode)
            {
                string txt = "over_lucky_1";
                m_fail_info_text.Text = LocalizeModule.Instance.GetString(txt);

                ShowGifts(ENUM_PUSH_GIFT_BLOCK_TYPE.E_SEEK);
            }
        }

        private void ShowGifts(ENUM_PUSH_GIFT_BLOCK_TYPE gift_type_)
        {
            List<Push_Info> gifts = PushGiftManager.Instance.GetPushInfosByTurnOnType(gift_type_);

            if (null != gifts)
            {
                m_gifts_grid.EnsureSize<LoseGiftItemView>(gifts.Count);

                for (int i = 0; i < gifts.Count; ++i)
                {

                    LoseGiftItemView item = m_gifts_grid.GetChild<LoseGiftItemView>(i);
                    item.Refresh(gifts[i]);
                    item.Visible = false;
                    item.Visible = true;
                }
            }
            else
            {
                m_gifts_grid.Clear();
            }
        }
        void OnCloseClicked(GameObject obj)
        {
            CurViewLogic().Quit();
        }


    }
}
