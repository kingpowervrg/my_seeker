using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
using HedgehogTeam.EasyTouch;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadMask : GuidNewFunctionBase
    {
        private GameObject m_frameObj;
        private string frameName;
        private string itemName;
        private MaskEmptyType emptyType;
        private MaskAnimType animType;
        private string eventName;
        private int isScene = 0;
        private float lessPixel = 0f;

        private bool m_tick = false;
        private float delayTimeTT = 0f;

        private float m_rotation = -1f;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID,param);
            try
            {
                this.frameName = param[0];
                this.itemName = param[1];
                this.itemName = itemName.Replace(":", "/");
                this.emptyType = (MaskEmptyType)int.Parse(param[2]);
                this.animType = (MaskAnimType)int.Parse(param[3]);
                this.eventName = param[4];
                this.isScene = int.Parse(param[5]);
                this.m_tick = false;
                if (param.Length >= 7)
                {
                    this.lessPixel = float.Parse(param[6]);
                }
                if (param.Length >= 8)
                {
                    this.delayTimeTT = float.Parse(param[7]);
                }
                if (param.Length == 9)
                {
                    this.m_rotation = float.Parse(param[8]);
                }
            }
            catch (FormatException exp)
            {
                Debug.LogError("error ==");
            }
        }

        public override void OnLoadRes()
        {
            base.OnLoadRes();
            
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName);
            if (frame == null)
            {
                return;
            }
            Transform tran = frame.FrameRootTransform.Find(itemName);
            if (tran == null)
            {
                return;
            }
            if (this.m_tick)
            {
                GuidNewModule.Instance.RemoveFunction(this);
            }
            //Vector4 m_tempVec = new Vector4(Screen);
            //GameEvents.UI_Guid_Event.OnReflashCircleMask.SafeInvoke(tempVec, 1);
            GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
            RectTransform srcRect = tran.GetComponent<RectTransform>();
            Action act = () => {
                if (this.m_isClearing)
                {
                    return;
                }
                Vector2[] cornPos = GuidTools.getCornPos(srcRect,this.lessPixel);
                GameEvents.UI_Guid_Event.OnShowMask.SafeInvoke(new List<Vector2[]> { cornPos }, new List<MaskEmptyType> { emptyType }, animType, eventName,isScene == 2);
                if (this.m_rotation >= 0)
                {
                    GUIFrame guidFrame = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
                    if (guidFrame != null)
                    {
                        Vector2[] emptyPos = GuidNewTools.GetEmptyPos(cornPos, guidFrame.FrameRootTransform);
                        //Vector2 center = new Vector2((emptyPos[2].x + cornPos[0].x) / 2f, (cornPos[1].y + cornPos[0].y) / 2f);
                        GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(20000, "UI_xinshouyindao_shou.prefab", emptyPos[0], Vector2.one, m_rotation);
                    }
                }
            };
            if (srcRect.sizeDelta.x > 0)
            {
                if (this.delayTimeTT > 0)
                {
                    TimeModule.Instance.SetTimeout(act, this.delayTimeTT);
                }
                else
                {
                    act();
                }
            }
            else
            {
                TimeModule.Instance.SetTimeout(act,0.8f);
                Debug.Log("mask time out === ");
            }
            
        }

        private Transform GetItemName(Transform root,string itemName)
        {
            Transform tran = root.Find(itemName);
            if (tran == null)
            {
                TimeModule.Instance.SetTimeout(()=> { },0.2f);
            }
            return tran;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName);
            if (frame == null)
            {
                GuidNewModule.Instance.PushFunction(this);
                this.m_tick = true;
            }
            else
            {
                OnLoadRes();
            }
        }

        public override void Tick(float time)
        {
            base.Tick(time);
            OnLoadRes();
            //OnRun();
            //GuidNewModule.Instance.RemoveFunction(this);
        }

        private void OnMaskClick(Vector2 worldPos,bool inner)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                OnDestory();
                return;
            }
            else if(inner && !string.IsNullOrEmpty(eventName))
            {
                OnDestory();
                return;
            }
        }

        //private void OnMaskLongClick(bool click)
        //{
        //    if (click)
        //    {
        //        OnDestory();
        //    }
        //}

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);

            if (isScene == 1)
            {
                Gesture gesture = new Gesture();
                GameObject obj = gesture.GetCurrentPickedObject();
                if (obj != null)
                {
                    gesture.pickedObject = obj;
                }
                EngineCoreEvents.InputEvent.OnOneFingerTouchup.SafeInvoke(gesture);
            }
            //if (this.m_rotation >= 0f)
            //{
            //    GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(20000, true);
            //}
            Debug.Log("end mask =======" + m_funcID);
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;

        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

    }
}
