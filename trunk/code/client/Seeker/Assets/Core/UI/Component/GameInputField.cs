using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameInputField : GameTextComponent
    {
        public UnityEngine.UI.InputField input;
        public int MaxCharNum
        {
            set { input.characterLimit = value; }
            get { return input.characterLimit; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            input = GetComponent<UnityEngine.UI.InputField>();
        }

        private void onValueChange()
        {
            if (MaxCharNum <= 0)
                return;
            string v = input.text;
            if(v.Length > MaxCharNum)
            {
                input.text = v.Substring(0, MaxCharNum);
            }
        }

        public bool Editable
        {
            set 
            { 
                input.interactable = value;
            }
            get { return input.enabled; }
        }

        public override string Text
        {
            set { input.text = value; }
            get { return input.text; }
        }

        public bool MultipleLine
        {
            set { input.lineType = value ? UnityEngine.UI.InputField.LineType.MultiLineNewline : UnityEngine.UI.InputField.LineType.SingleLine; }
            get { return input.lineType == UnityEngine.UI.InputField.LineType.MultiLineNewline; }
        }

        public void AddChangeCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            RemoveChangeCallBack(call);
            input.onValueChange.AddListener(call);
        }
        public void RemoveChangeCallBack(UnityEngine.Events.UnityAction<string> call)
        {
            input.onValueChange.RemoveListener(call);
        }
    }
}
