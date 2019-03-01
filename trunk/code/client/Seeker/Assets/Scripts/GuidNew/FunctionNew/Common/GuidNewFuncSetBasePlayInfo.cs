using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSetBasePlayInfo :GuidNewFunctionBase
    {
        private int coin = -1;
        private int cash = -1;
        private int vit = -1;
        private int level = -1;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 1)
            {
                this.coin = int.Parse(param[0]);
            }
            if (param.Length >= 2)
            {
                this.cash = int.Parse(param[1]);
            }
            if (param.Length >= 3)
            {
                this.vit = int.Parse(param[2]);
            }
            if (param.Length >= 4)
            {
                this.level = int.Parse(param[3]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            if (this.cash >= 0)
            {
                GlobalInfo.MY_PLAYER_INFO.SetCash(this.cash);
            }

            if (this.coin >= 0)
            {
                GlobalInfo.MY_PLAYER_INFO.SetCoin(this.coin);
            }

            if (this.vit >= 0)
            {
                GlobalInfo.MY_PLAYER_INFO.SetVit(this.vit);
            }

            if (this.level >= 0)
            {
                GlobalInfo.MY_PLAYER_INFO.SetLevel(this.level);
            }
            OnDestory();
        }
    }
}
