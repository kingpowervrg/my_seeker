using EngineCore;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_ChapterMap)]
    public class ChapterMapUILogic : UILogicBase
    {
        private ChapterListNewComponent m_chapterListComponent = null;

        private ChapterInfo m_displayChapterInfo = null;

        protected override void OnInit()
        {
            base.OnInit();

            this.m_chapterListComponent = Make<ChapterListNewComponent>("Panel_caselist");
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (param is ChapterInfo)
            {
                m_displayChapterInfo = param as ChapterInfo;
                this.m_chapterListComponent.Parameter = m_displayChapterInfo.ChapterID;
            }

            if (param is int || param is long)
                this.m_chapterListComponent.Parameter = param;

            this.m_chapterListComponent.Visible = true;

            SetCloseBtnID("Panel_caselist:Panel_animation:Button_close");
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
    }
}
