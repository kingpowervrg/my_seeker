using EngineCore;

namespace SeekerGame
{
    public class LoadingData
    {
        public float[] m_time;
        public float[] m_progress;
        public bool[] m_state;
        public bool isBigWorld;

        public LoadingData(float[] time, float[] progress, bool[] state,bool isbigworld)
        {
            this.m_time = time;
            this.m_progress = progress;
            this.m_state = state;
            this.isBigWorld = isbigworld;
        }
    }

    public class LoadingProgressManager : Singleton<LoadingProgressManager>
    {
        private GUIFrame m_loadingFrame = null;

        public void LoadProgress(float[] time, float[] progress, bool[] state,bool isBigworld)
        {
            LoadingData loadData = new LoadingData(time, progress, state, isBigworld);

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_Loading);
            param.Param = loadData;
            m_loadingFrame = EngineCoreEvents.UIEvent.ShowUIAndGetFrameWithParam.SafeInvoke(param);
        }

        public void LoadProgressScene()
        {
            float[] time = new float[] { 0.8f, 1.5f, 0.3f };
            float[] progress = new float[] { 0.4f, 0.9f, 1f };
            bool[] state = new bool[] { true, false, false };
            LoadProgress(time, progress, state,false);
        }

        public void LoadProgressTalkScene()
        {
            float[] time = new float[] { 0.5f, 1f, 0.2f };
            float[] progress = new float[] { 0.5f, 0.9f, 1f };
            bool[] state = new bool[] { true, false, false };
            LoadProgress(time, progress, state,false);
        }

        //加载主场景
        public void LoadBigWorldScene(System.Action cb)
        {
            float[] time = new float[] { 2f, 0.8f, 0.3f };
            float[] progress = new float[] { 0.7f, 0.9f, 1f };
            bool[] state = new bool[] { true, false, false };
            GameEvents.UIEvents.UI_Loading_Event.OnStartLoading = cb;
            LoadProgress(time, progress, state,true);
        }

        public void LoadOneStage()
        {
            float[] time = new float[] { 100f };
            float[] progress = new float[] { 1f };
            bool[] state = new bool[] { true };
            LoadProgress(time, progress, state,false);
        }

        public void LoadFacebook()
        {
            if (!GameRoot.instance.GameFSM.CurrentState.StateFlag.Equals((int)ClientFSM.ClientState.LOGIN))
            {
                float[] time = new float[] { 6.5f };
                float[] progress = new float[] { 6.5f };
                bool[] state = new bool[] { true };

                LoadingData loadData = new LoadingData(time, progress, state,false);

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_FB_Loading);
                param.Param = loadData;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
        }

        public bool IsLoading
        {
            get
            {
                return this.m_loadingFrame != null && this.m_loadingFrame.Visible;
            }
        }
    }
}
