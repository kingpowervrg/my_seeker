using UnityEngine;
using EngineCore;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 流光提示
    /// </summary>
    public class GuidNewFuncSceneExhibitEffectTips : GuidNewFunctionBase
    {
        private long m_exhibitID;
        private bool m_isTips;
        private float m_delayTime;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            if (param.Length >= 2)
            {
                this.m_exhibitID = long.Parse(param[0]);
                this.m_isTips = bool.Parse(param[1]);
            }
            if (param.Length >= 3)
            {
                this.m_delayTime = float.Parse(param[2]);
            }
        }

        //private Texture effectTex = null;
        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.MainGameEvents.OnGameOver += OnGameOver;
            if (this.m_delayTime > 0)
            {
                TimeModule.Instance.SetTimeout(TimeDelay, this.m_delayTime);
            }
            else
            {
                TimeDelay();
            }
        }

        private void TimeDelay()
        {
            GameEvents.MainGameEvents.OnStreamerHint(m_isTips, m_exhibitID);
            GameEvents.MainGameEvents.OnGameOver -= OnGameOver;
            OnDestory();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            TimeModule.Instance.RemoveTimeaction(TimeDelay);
        }

        private void OnGameOver(SceneBase.GameResult result)
        {
            if (result == SceneBase.GameResult.ALL_ITEM_FOUND)
            {
                TimeDelay();
            }
        }

    }
}
