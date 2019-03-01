

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncAddItemToUser : GuidNewFunctionBase
    {
        private int m_type = 0; //0表示道具
        private long m_itemId;
        private int m_count = 1;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_type = int.Parse(param[0]);
            this.m_itemId = long.Parse(param[1]);
            this.m_count = int.Parse(param[2]);
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (0 == m_type)
            {
                GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(m_itemId,m_count);

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();
            }
            OnDestory();
        }

    }
}
