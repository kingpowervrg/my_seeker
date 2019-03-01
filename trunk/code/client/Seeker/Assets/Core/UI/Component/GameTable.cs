using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EngineCore
{
    public class GameTable : GameUIContainer
    {
        private UnityEngine.UI.LayoutGroup _uiTile;
        Transform cachedTransform;
        private int nMaxLineNum = 100;      //最大行数
        private int nLineNum = 7;           //实体的行数
        private float nLineWidth = 20;        //行宽度
        private int startLine = 0;          //起始行
        private bool isHorizontal = false;
        private float curMovePos = 0;         //当前偏移值
        private float oldScrollPos = 0;     //当前偏移值
        private bool isNegative = false;        //正负方向
        private float scrollSpeed = 10;        //
        private float maxSize = 100;        //
        public Action<string, int> OnTableChange;
        private string eventName = "none";
        private bool noProcessFirst = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxNum">最多多少行</param>
        /// <param name="lineNum">会创建多少行实体</param>
        /// <param name="lineW">水平还是垂直的 0水平 1垂直</param>
        /// <param name="dir">往正方向拖还是负方向</param>
        /// <param name="speed">拖动影响的速度</param>
        /// <param name="name">数据变化时lua事件给出的名字</param>
        public void SetInfo(int maxNum, int lineNum, int lineW, bool dir, float speed, string name)
        {
            nMaxLineNum = maxNum;
            nLineNum = lineNum;
            GridLayoutGroup tempGroup = _uiTile as GridLayoutGroup;
            if (lineW == 0)
            {
                isHorizontal = true;
                nLineWidth = tempGroup.cellSize.x + tempGroup.spacing.x;
            }
            else
            {
                isHorizontal = false;
                nLineWidth = tempGroup.cellSize.y + tempGroup.spacing.y;
            }
            eventName = name;
            maxSize = nMaxLineNum * nLineWidth;
            scrollSpeed = speed;
            isNegative = dir;
            noProcessFirst = true;
        }

        public void SetMaxNum(int maxNum)
        {
            nMaxLineNum = maxNum;
            maxSize = nMaxLineNum * nLineWidth;
            noProcessFirst = true;
        }

        public void OnValueChanged(Vector2 data)
        {
            float delta = 0;
            float inputpos = 0;
            if (isHorizontal)
            {
                inputpos = scrollView.content.position.x;
            }
            else
            {
                inputpos = scrollView.content.position.y;
            }
            //    D.log("OnValueChanged data.x = "+ data.x+ " data.y = "+ data.y);
            //    D.log("OnValueChanged content.x = " + scrollView.content.position.x + " content.y = " + scrollView.content.position.y);

            delta = inputpos - oldScrollPos;
            oldScrollPos = inputpos;
            if (noProcessFirst)
            {
                noProcessFirst = false;
                delta = 0;
            }
            delta = delta * scrollSpeed + curMovePos;
            setPosTo(delta);
        }
        public UnityEngine.UI.ScrollRect scrollView;
        protected override void OnInit()
        {

            _uiTile = GetComponent<UnityEngine.UI.LayoutGroup>();
            cachedTransform = _uiTile.transform;
            if (Widget.parent != null && Widget.parent.parent != null)
                scrollView = Widget.parent.parent.gameObject.GetComponentInChildren<UnityEngine.UI.ScrollRect>(); //GetComponent<ScrollRect>();
            if (scrollView == null)
            {
                scrollView = Widget.parent.parent.gameObject.GetComponent<UnityEngine.UI.ScrollRect>();
            }
            if (scrollView != null)
            {
                UnityAction<Vector2> valueChange = new UnityAction<Vector2>(OnValueChanged);
                scrollView.onValueChanged.AddListener(valueChange);
            }
            GridLayoutGroup tempGroup = _uiTile as GridLayoutGroup;
            if (isHorizontal)
            {
                nLineWidth = tempGroup.cellSize.x + tempGroup.spacing.x;
            }
            else
            {
                nLineWidth = tempGroup.cellSize.y + tempGroup.spacing.y;
            }

            base.OnInit();
        }
        protected override Transform ContainerTransform
        {
            get
            {
                return cachedTransform;
            }
        }
        public override void OnShow(object param)
        {
            base.OnShow(param);
        }


        public void setPosTo(float pos)
        {
            float output = pos;
            if (isNegative)
            {
                output = setPosToNegative(pos);
            }
            else
            {
                output = setPosToPositive(pos);
            }

            curMovePos = pos;
            int onepos = (int)(curMovePos / nLineWidth);
            if (isNegative)
            {
                onepos = -onepos;
            }
            //        D.log(string.Format("========onepos ={0} nMaxLineNum = {1} nLineNum = {2}", onepos, nMaxLineNum, nLineNum));
            //   if (onepos >= nMaxLineNum - nLineNum)
            //   {
            //       onepos = nMaxLineNum - nLineNum - 1;
            //   }
            //    D.log(string.Format("========onepos2 ={0}", onepos));
            if (onepos < 0)
            {
                onepos = 0;
            }
            if (onepos != startLine)
            {
                startLine = onepos;
                // Common.SendEventToLua.SafeInvoke(EventID.Lua_Event_OnTableChange, string.Format("{0}:{1}", luaName, startLine));
                //           LuaClient.Instance.CallFunction((int)EventID.Lua_Event_OnTableChange, string.Format("{0}:{1}", luaName, startLine));
                OnTableChange.SafeInvoke(eventName, startLine);
            }
            if (isHorizontal)
            {
                X = output;
            }
            else
            {
                Y = output;
            }
            //       D.log(string.Format("========output pos={0}  curMovePos={1} startLine={2} , nLineWidth={3}", output, curMovePos, startLine, nLineWidth));
        }

        private float autoReturnTime = 0.3f;
        private float setPosToNegative(float pos)
        {
            float output = pos;
            if (pos > 0)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    setNeedReturn(0);
                }, autoReturnTime);
                return output;
            }
            while (output < -nLineWidth)
            {
                output = output + nLineWidth;
            }
            float big = -maxSize;
            if (pos < big)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    setNeedReturn(big + nLineWidth);
                }, autoReturnTime);
                return output;
            }
            return output;
        }

        private float setPosToPositive(float pos)
        {
            //   D.log(string.Format("========setPosToPositive pos={0}  maxSize={1}", pos, maxSize));
            float output = pos;
            if (pos < 0)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    setNeedReturn(0);
                }, autoReturnTime);
                return output;
            }
            while (output > nLineWidth)
            {
                output = output - nLineWidth;
            }

            float big = maxSize;
            if (pos > big)
            {
                TimeModule.Instance.SetTimeout(() =>
                {
                    setNeedReturn(big - nLineWidth - 5);
                }, autoReturnTime);
                return output;
            }
            return output;
        }

        private void setNeedReturn(float pos)
        {
            scrollView.StopMovement();
            //   D.log(string.Format("========setNeedReturn pos={0}  ", pos));
            setPosTo(pos);
        }
    }
}