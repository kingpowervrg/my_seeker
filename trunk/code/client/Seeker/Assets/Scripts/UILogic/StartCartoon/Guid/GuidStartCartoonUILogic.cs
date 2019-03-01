using EngineCore;
using GOGUI;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_COMICS_GUID)]
    public class GuidStartCartoonUILogic : UILogicBase
    {
        private GuidStartCartoonCapter m_Capter;
        private ConfGuidNew m_ConfGuid;
        private int curIndex = 0;
        private int nextIndex = 0;
        private float screenX = 0f;
        private GameButton m_nextBtn = null;
        private GameLabel m_pageLabel = null;
        private GameButton m_skipBtn = null;
        private int maxPage = 0;
        private int curPage = 1;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_nextBtn = Make<GameButton>("Image_bg_2");
            this.m_pageLabel = Make<GameLabel>("Text_pagenumber");
            this.m_skipBtn = Make<GameButton>("BtnSkip");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            this.m_nextBtn.AddClickCallBack(BtnNext);
            this.m_skipBtn.AddClickCallBack(BtnSkip);
           // this.maxPage = 
            GameEvents.UI_Guid_Event.OnGuidNewNext += MoveNextIndex;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext += OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible += OnNextBtnVisible;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnPageChange += OnPageChange;
            if (param !=null)
            {
                m_ConfGuid = (ConfGuidNew)param;
                GetIndex();
            }
            screenX = Transform.sizeDelta.x;
            m_Capter = Make<GuidStartCartoonCapter>("guid");
            this.maxPage = m_Capter.Widget.childCount;
            this.curPage = curIndex + 1;
            m_Capter.SetIndex(curIndex, nextIndex,screenX,m_ConfGuid.id);
            m_Capter.Visible = true;
            this.m_pageLabel.Text = string.Format("{0}/{1}", curPage, this.maxPage);
            
        }
        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
        private void BtnNext(GameObject obj)
        {
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextCapter.SafeInvoke();
        }

        private void BtnSkip(GameObject obj)
        {
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextKeyCapter.SafeInvoke();
        }

        private void OnNextBtnVisible(bool visible,int type)
        {
            if (type == 0)
            {
                this.m_nextBtn.Enable = visible;
            }
            else
            {
                this.m_skipBtn.Visible = visible;
            }
        }

        private void GetIndex()
        {
            int.TryParse(m_ConfGuid.typeValue, out curIndex);
            if (m_ConfGuid.nextId > 0)
            {
                ConfGuidNew nextGuid = ConfGuidNew.Get(m_ConfGuid.nextId);
                if (nextGuid != null)
                {
                    int.TryParse(nextGuid.typeValue, out nextIndex);
                    return;
                }
            }
            nextIndex = -1;
        }

        private void MoveNextIndex(long guidID)
        {
            m_ConfGuid = ConfGuidNew.Get(guidID);
            GetIndex();
            m_Capter.SetIndex(curIndex, nextIndex, screenX, m_ConfGuid.id);
        }

        private void OnNext()
        {
            this.curPage++;
            this.m_pageLabel.Text = string.Format("{0}/{1}", this.curPage, this.maxPage);
        }

        private void OnPageChange(int curPage)
        {
            this.curPage = curPage;
            this.m_pageLabel.Text = string.Format("{0}/{1}", this.curPage, this.maxPage);
        }

        public override void OnHide()
        {
            base.OnHide();
            curIndex = 0;
            nextIndex = 0;
            maxPage = 0;
            curPage = 1;
            GameEvents.UI_Guid_Event.OnGuidNewNext -= MoveNextIndex;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNext -= OnNext;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnNextBtnVisible -= OnNextBtnVisible;
            GameEvents.UIEvents.UI_StartCartoon_Event.OnPageChange -= OnPageChange;
            this.m_nextBtn.RemoveClickCallBack(BtnNext);
            this.m_skipBtn.RemoveClickCallBack(BtnSkip);
        }
    }
}
