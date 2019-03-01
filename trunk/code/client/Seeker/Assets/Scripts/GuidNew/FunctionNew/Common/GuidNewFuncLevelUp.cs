using EngineCore;
namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLevelUp : GuidNewFunctionBase
    {
        private int oldLevel;
        private int newLevel;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.oldLevel = int.Parse(param[0]);
            this.newLevel = int.Parse(param[1]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            SCPlayerUpLevel levelData = new SCPlayerUpLevel();
            levelData.OldLevel = this.oldLevel;
            levelData.NewLevel = this.newLevel;
            //MessageHandler.Call(MessageDefine.SCPlayerUpLevel, levelData);

            LevelUpData data = new LevelUpData()
            {
                msg = levelData,
                m_click_act = null,
            };

            FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_LEVEL_UP);
            param.Param = data;

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            GlobalInfo.MY_PLAYER_INFO.SetLevel(newLevel);

            OnDestory();
        }
    }
}
