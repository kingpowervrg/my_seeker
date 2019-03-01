using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncLoadMutilMask : GuidNewFunctionBase
    {
        private string[] frameName;
        private string[] itemName;
        private List<MaskEmptyType> emptyType = new List<MaskEmptyType>();
        private float[] lessPixel;

        private int clickNum = 1;
        public override void OnInit(long funcID, string[] param)
        {
            base.OnInit(funcID, param);
            int len = param.Length / 4;
            frameName = new string[len];
            itemName = new string[len];
            lessPixel = new float[len];
            //emptyType = new MaskEmptyType[len];
            for (int i = 0; i < len; i++)
            {
                frameName[i] = param[i * 4];
                itemName[i] = param[i * 4 + 1].Replace(":", "/");
                emptyType.Add((MaskEmptyType)int.Parse(param[i * 4 + 2]));
                lessPixel[i] = float.Parse(param[i * 4 + 3]);
            }
            if (param.Length % 4 == 1)
            {
                clickNum = int.Parse(param[param.Length - 1]);
            }
        }

        public override void OnExecute()
        {
            base.OnExecute();
            List<Vector2[]> maskPos = new List<Vector2[]>();
            for (int i = 0; i < frameName.Length; i++)
            {
                GUIFrame frame = GuidNewModule.Instance.GetFrameByResName(frameName[i]);
                if (frame == null)
                {
                    continue;
                }
                Transform tran = frame.FrameRootTransform.Find(itemName[i]);
                if (tran == null)
                {
                    Debug.LogError("guid mask itemName error  " + itemName[i]);
                    continue;
                }
                RectTransform srcRect = tran.GetComponent<RectTransform>();
                Vector2[] cornPos = GuidTools.getCornPos(srcRect, lessPixel[i]);
                maskPos.Add(cornPos);
            }
            GameEvents.UI_Guid_Event.OnShowMask.SafeInvoke(maskPos, emptyType, MaskAnimType.ToOut, "",false);
            if (clickNum > 1)
            {
                GameEvents.UI_Guid_Event.OnMaskEnableClick.SafeInvoke(false);
            }
            GameEvents.UI_Guid_Event.OnMaskClick += OnMaskClick;
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

        public override void ClearFunc()
        {
            base.ClearFunc();
            GameEvents.UI_Guid_Event.OnMaskClick -= OnMaskClick;
        }

        private void OnMaskClick(Vector2 worldPos, bool inner)
        {
            OnDestory();
        }
    }
}
