using EngineCore;
using System.Collections.Generic;

namespace SeekerGame
{
    //public class GuidStartCartoonData
    //{
    //    public int curIndex = 0;
    //    public int nextIndex = 0;
    //}

    public class StartGuidCartoon : AbsGuid
    {
        public override void StartGuid()
        {
            base.StartGuid();

            GameEvents.UI_Guid_Event.OnStartGuidCartoonOver += StartCartoonOver;
            GameEvents.UI_Guid_Event.OnStartGuidCartoonNext += StartCartoonNext;
            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_COMICS_GUID);
            param.Param = m_CurConf;
            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
        }

        protected override void EndGuid()
        {
            base.EndGuid();
        }

        protected override void Destory()
        {
            base.Destory();


            GameEvents.UI_Guid_Event.OnStartGuidCartoonOver -= StartCartoonOver;
            GameEvents.UI_Guid_Event.OnStartGuidCartoonNext -= StartCartoonNext;
            FrameMgr.HideUIParams hideParam = new FrameMgr.HideUIParams(UIDefine.UI_COMICS_GUID);
            hideParam.DestroyFrameImmediately = true;
            hideParam.DestoryFrameDelayTime = 2f;
            EngineCoreEvents.UIEvent.HideUIWithParamEvent.SafeInvoke(hideParam);
            BigWorldManager.Instance.LoadBigWorld();
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_GUEST_LOGIN);
        }

        private void StartCartoonNext()
        {
            GuidManager.Instance.SetProgressByIndex(m_keyID);
            if (m_CurConf.nextId <= 0)
            {
                EndGuid();
            }
            AbsGuid absGuid = GuidFactory.Instance.GetRootGuidByID(m_CurConf.nextId);
            if (absGuid == null)
            {
                EndGuid();
            }
            InitGuidInfo(absGuid.GetKeyID(), absGuid.GetCurID(), absGuid.GetRootID());
        }

        private void StartCartoonOver()
        {
            EndGuid();
        }
    }
}
