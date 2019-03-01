using UnityEngine;
using SeekerGame;
namespace EngineCore
{
    public class GameFilledImage : GameImage
    {
        private float m_FillTime;
        private float m_Speed;
        private bool m_isRun = false;
        private float m_startFill = 0;
        public float FillTime
        {
            set
            {
                m_FillTime = value;
                m_Speed = 1f / m_FillTime;
                FillAmmount = 1f;
                m_startFill = 0f;//Time.time;
                m_isRun = true;
                //allTime = 0f;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            IsForceUpdate = true;
        }

        private bool m_pause = false;
        public void SetGameStatus(GameSceneBase.GameStatus status)
        {
            if (status == SceneBase.GameStatus.GAMING)
            {
                this.m_pause = false;
            }
            else if (status == SceneBase.GameStatus.PAUSE)
            {
                this.m_pause = true;
            }
        }

        private float allTime = 0;

        public override void OnHide()
        {
            base.OnHide();
            this.m_isRun = false;
            this.FillAmmount = 0f;
            this.m_Speed = 0f;
        }

        public override void ForceUpdate()
        {
            if (!m_isRun || m_pause)
            {
                return;
            }
            this.m_startFill += Time.deltaTime;
            //float deltaTime = Time.deltaTime;//Time.time - this.m_startFill;
            if (FillAmmount <= 1f && FillAmmount > 0)
            {
                FillAmmount = 1 - m_Speed * m_startFill;

            }
            else
            {
                FillAmmount = 0f;
                m_isRun = false;
            }

        }
    }
}
