using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace EngineCore
{
    public class GameCountDown : GameLabel
    {
        //lua set
        public float countDownSecond = 0;
        public string showFarmat = string.Empty; //"{0:00}小时{1:00}分钟{2:00}秒"
        public bool IsAddMode = false;
        public bool IsZeroNoShow = false;//小时和分钟是0的情况 不显示
        public bool IsHourNoShow = false;//小时和分钟是0的情况 不显示
        public static int FrameLimit = 5;

        public static SafeAction<GameUIComponent> AddToUpdateList;
        public static SafeAction<GameUIComponent> DeleteToUpdateList;

        public System.Action EndCall;

        private float changeTime = 0.0f;
        private int changeFrame = 0;
        private bool isStart = false;
        private bool isInList = false;

        public int mShowValue = 0;

        public bool isRemove = false;


        public bool ChangeUpdate(float ft)
        {
            if (!isStart)
                return isRemove;


            changeFrame++;
            changeTime += ft;
            if (changeFrame > FrameLimit && countDownSecond > 0)
            {
                if (IsAddMode)
                {
                    countDownSecond += changeTime;
                }
                else
                {
                    countDownSecond -= changeTime;
                }

                if (!IsAddMode)
                {
                    if (mShowValue - countDownSecond > 1)
                    {

                    }
                    else
                    {
                        changeFrame = 0;
                        changeTime = 0.0f;
                        return isRemove;
                    }
                }
                else
                {
                    if (countDownSecond - mShowValue > 1)
                    {

                    }
                    else
                    {

                        changeFrame = 0;
                        changeTime = 0.0f;
                        return isRemove;
                    }
                }



                changeFrame = 0;
                changeTime = 0.0f;
                if (countDownSecond < 0)
                {
                    countDownSecond = 0;
                    isStart = false;

                    if (isInList == true)
                    {
                        isInList = false;
                        isRemove = true;
                    }

                    EndTime();
                }

                ChangeShowTime();
            }
            return isRemove;
        }

        StringBuilder stringBuilder = new StringBuilder();
        private void ChangeShowTime()
        {
            mShowValue = (int)countDownSecond + 1;
            int hour = (int)countDownSecond / 3600;
            int minu = ((int)countDownSecond % 3600) / 60;
            int second = (int)countDownSecond % 60;

            string show = string.Empty;
            if (!string.IsNullOrEmpty(showFarmat))
            {
                if (IsHourNoShow)
                {
                    show = string.Format(showFarmat, minu, second);
                }
                else
                {
                    show = string.Format(showFarmat, hour, minu, second);
                }
            }
            else if (IsZeroNoShow == true)
            {
                {
                    stringBuilder.Length = 0;
                    if (hour > 0)
                    {
                        stringBuilder.AppendFormat("{0:D2}:", hour);
                    }
                    if (minu > 0 || hour > 0)
                    {
                        stringBuilder.AppendFormat("{0:D2}:", minu);
                    }
                    //if (second > 0 || minu > 0 || hour > 0)
                    {
                        stringBuilder.AppendFormat("{0:D2}", second);
                    }

                    show = stringBuilder.ToString();
                }
            }
            else if (IsHourNoShow)
            {
                stringBuilder.Length = 0;

                {
                    stringBuilder.AppendFormat("{0:D2}:", minu);
                }
                {
                    stringBuilder.AppendFormat("{0:D2}", second);
                }

                show = stringBuilder.ToString();
            }
            else
                show = string.Format("{0:D2}", hour) + ":" + string.Format("{0:D2}", minu) + ":" + string.Format("{0:D2}", second);

            this.Text = show;
        }


        public void SetTimeText(string _str)
        {
            this.Text = _str;
        }
        public void EndTime()
        {
            if (EndCall != null)
            {
                EndCall.Invoke();
            }
        }

        public void StartRun()
        {
            isStart = true;
            isRemove = false;
            if (isInList == false)
            {
                isInList = true;
                if (AddToUpdateList.IsNull == false)
                {
                    AddToUpdateList.SafeInvoke(this);
                }
            }

            ChangeShowTime();
        }

        public void EndRun()
        {
            isInList = false;
            isStart = false;
            isRemove = true;
            if (DeleteToUpdateList.IsNull == false)
            {
                DeleteToUpdateList.SafeInvoke(this);
            }
        }
        public void SetEndCallBack(System.Action action)
        {
            EndCall = action;
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
        }

        public override void OnHide()
        {
            EndRun();
        }
        public override void Dispose()
        {
            EndRun();
        }
    }
}
