using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

namespace EngineCore
{
    public class GameAdvancedCountDown : GameUIComponent
    {
        //lua set
        public float countDownSecond = 0; 
        public string showFarmat = string.Empty; //"{0:00}小时{1:00}分钟{2:00}秒"
        public bool IsAddMode = false;
        public bool IsZeroNoShow = false;//小时和分钟是0的情况 不显示

        public static int FrameLimit = 10;

        public System.Action EndCall;

        private float changeTime = 0.0f;
        private int changeFrame = 0;
        private bool isStart = false;
        private bool isInList = false;


        public GOGUI.AdvancedText AdvancedText = null;
        public string RepalceStr = "";

        public bool isRemove = false;
        public int mShowValue = 0;

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
        List<int> tempList = new List<int>();
        private void ChangeShowTime()
        {
            mShowValue = (int)countDownSecond + 1;
            stringBuilder.Length = 0;
            tempList.Clear();
            int tempNumber = 0;
            int number = (int)countDownSecond;
            while (number > 0)
            {
                tempNumber = number % 10;
                number = number / 10;
                tempList.Add(tempNumber);
            }

            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                stringBuilder.Append(RepalceStr);
                stringBuilder.Append(tempList[i]);
            }
            
            AdvancedText.text = stringBuilder.ToString();
        }

        public void EndTime()
        {
            if(EndCall != null)
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
                if (GameCountDown.AddToUpdateList.IsNull == false)
                {
                    GameCountDown.AddToUpdateList.SafeInvoke(this);
                }
            }
            
            ChangeShowTime();
        }

        public void EndRun()
        {
            isInList = false;
            isStart = false;
            isRemove = true;
            if (GameCountDown.DeleteToUpdateList.IsNull == false)
            {
                GameCountDown.DeleteToUpdateList.SafeInvoke(this);
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
