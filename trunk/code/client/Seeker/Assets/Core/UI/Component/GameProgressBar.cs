using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameProgressBar : GameUIComponent
    {
        protected UnityEngine.UI.Slider pb;
        protected GameImage frontBar;
        protected GameLabel lbNum;

        private float m_moveSpeed;
        private float m_targetValue;
        private Vector3[] m_fillRectWorldConner = new Vector3[4];

        public event Action<float> OnValueChanged;
        protected override void OnInit()
        {
            base.OnInit();
            pb = GetComponent<UnityEngine.UI.Slider>();
            frontBar = Make<GameImage>("Fill Area:Fill");
            pb.onValueChanged.AddListener(ValueChangeHandler);
        }



        public virtual float Value
        {
            get { return pb.value; }
            set
            {
                if (float.IsNaN(value))
                {
                    value = 0;
                }
                pb.value = value;
            }
        }

        public void SetValueWithSpeed(float value, float speed)
        {
            this.m_moveSpeed = speed;
            this.m_targetValue = value;
            TimeModule.Instance.SetTimeInterval(ProgressMoveInterval, Time.deltaTime);
        }

        Action Callback = null;
        public void SetValueWithTime(float value, float time, Action callback = null)
        {
            this.Callback = callback;
            this.m_moveSpeed = Math.Abs((value - Value) / time);
            this.m_targetValue = value;
            TimeModule.Instance.SetTimeInterval(ProgressMoveInterval, Time.deltaTime);
        }


        public void StopTween()
        {
            TimeModule.Instance.RemoveTimeaction(ProgressMoveInterval);
        }

        private void ProgressMoveInterval()
        {
            if (Value == this.m_targetValue)
            {
                TimeModule.Instance.RemoveTimeaction(ProgressMoveInterval);
                if (this.Callback != null)
                    this.Callback();
            }
            else if (Value > m_targetValue)
            {
                Value -= m_moveSpeed * Time.deltaTime;
                if (Value < m_targetValue)
                {
                    Value = m_targetValue;
                }

            }
            else
            {
                Value += m_moveSpeed * Time.deltaTime;
                if (Value > m_targetValue)
                {
                    Value = m_targetValue;
                }
            }

            if (ShowPercent)
                ValueStr = (int)(100 * Value) + "%";
        }

        public string ValueStr
        {
            set
            {
                if (lbNum == null)
                    lbNum = Make<GameLabel>("Num");
                if (lbNum != null)
                    lbNum.Text = value;
            }
            get
            {
                return lbNum == null ? "" : lbNum.Text;
            }
        }
        public string StyleName
        {
            set { frontBar.Sprite = value; }
            get { return frontBar.Sprite; }
        }

        private bool showPercent = false;
        public bool ShowPercent
        {
            get
            {
                return showPercent;
            }

            set
            {
                if (lbNum == null)
                    lbNum = Make<GameLabel>("Num");
                if (lbNum != null)
                    lbNum.Visible = value;
                showPercent = value;
            }
        }

        void ValueChangeHandler(float val)
        {
            UpdateFillRectConners();

            if (OnValueChanged != null)
                OnValueChanged(val);
        }

        private void UpdateFillRectConners()
        {
            if (pb.fillRect)
                pb.fillRect.GetWorldCorners(this.m_fillRectWorldConner);
        }

        public Vector3[] FillRectangleWorldConners
        {
            get { return this.m_fillRectWorldConner; }
        }

        public UnityEngine.UI.Slider ProgressSlider
        {
            get { return this.pb; }
        }
    }

}
