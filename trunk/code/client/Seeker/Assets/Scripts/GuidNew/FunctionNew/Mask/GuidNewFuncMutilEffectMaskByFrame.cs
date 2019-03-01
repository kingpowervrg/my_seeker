using System;
using System.Collections.Generic;
using UnityEngine;
using EngineCore;
namespace SeekerGame.NewGuid
{
    /// <summary>
    /// 多个遮罩
    /// </summary>
    public class GuidNewFuncMutilEffectMaskByFrame : GuidNewFunctionBase
    {
        private long m_effectID;
        private string frameName;

        private int parentCount = 0;
        private string[] itemParent;

        private int effectCount = 0;
        private string[] effectRes;

        private int itemCount = 0;
        private string[] itemName;

        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            this.m_effectID = long.Parse(param[0]);
            this.frameName = param[1];
            this.parentCount = int.Parse(param[2]);
            this.effectCount = int.Parse(param[3]);

            itemCount = (param.Length - 4 - parentCount - effectCount);

            itemParent = new string[parentCount];
            for (int i = 0; i < parentCount; i++)
            {
                itemParent[i] = param[i + 4];
            }

            effectRes = new string[effectCount];
            for (int i = 0; i < effectCount; i++)
            {
                effectRes[i] = param[i + 4 + parentCount];
            }

            itemName = new string[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                itemName[i] = param[i + 4 + parentCount + effectCount];
            }
        }

        GameObject m_maskRoot = null;
        private Transform[] transParent = null;
        public override void OnExecute()
        {
            base.OnExecute();
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(false);
            GameEvents.UI_Guid_Event.OnEventClick += OnEventClick;
            GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName);
            transParent = new Transform[parentCount];
            for (int i = 0; i < parentCount; i++)
            {
                transParent[i] = frame.FrameRootTransform.Find(itemParent[i].Replace(":","/"));
            }

            GUIFrame frameGuid = GuidNewModule.Instance.GetFrameByResName(UIDefine.UI_GUID);
            m_maskRoot = frameGuid.FrameRootTransform.Find("guid/mask").gameObject;
            for (int i = 0; i < itemCount; i++)
            {
                string[] itemInfo = itemName[i].Split('|');
                int itemParentIndex = int.Parse(itemInfo[0]);
                int effectIndex = int.Parse(itemInfo[1]);
                RectTransform m_target = transParent[itemParentIndex].Find(itemInfo[2].Replace(":","/")).GetComponent<RectTransform>();
                Vector3 entityLocalPos = m_maskRoot.transform.InverseTransformPoint(m_target.position);

                string[] effectInfo = this.effectRes[effectIndex].Split('|');
                Vector2 m_effectScale = Vector2.one;
                m_effectScale.x = m_target.sizeDelta.x / float.Parse(effectInfo[1]);
                m_effectScale.y = m_target.sizeDelta.y / float.Parse(effectInfo[2]);
                GameEvents.UI_Guid_Event.OnLoadEffect.SafeInvoke(this.m_effectID + i, effectInfo[0], entityLocalPos, m_effectScale, 0f);
            }
        }


        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            OnClear();
        }

        public override void ForceFuncDestory()
        {
            base.ForceFuncDestory();
            OnClear();
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            OnClear();
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            OnClear();
        }

        private void OnClear()
        {
            GameEvents.UI_Guid_Event.OnEnableClick.SafeInvoke(true);
            GameEvents.UI_Guid_Event.OnEventClick -= OnEventClick;
            for (int i = 0; i < itemCount; i++)
                GameEvents.UI_Guid_Event.OnRemoveEffect.SafeInvoke(this.m_effectID + i, true);
        }

        private void OnEventClick(Vector2 worldPos)
        {
            OnDestory();
        }
    }
}
