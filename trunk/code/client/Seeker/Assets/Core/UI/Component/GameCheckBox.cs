using UnityEngine;

namespace EngineCore
{
    public class GameCheckBox : GameTextComponent
    {
        protected UnityEngine.UI.Toggle tog;
        GOGUI.EventTriggerListener.VoidDelegate clickAction;

        public virtual bool Checked
        {
            get { return tog.isOn; }
            set { tog.isOn = value; }
        }

        public bool Enabled
        {
            get { return tog.interactable; }
            set
            {
                tog.interactable = value;
            }
        }

        public bool Activated
        {
            get
            {
                return tog.enabled;
            }
            set
            {
                tog.enabled = value;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            tog = GetComponent<UnityEngine.UI.Toggle>();
            AddClickCallBack(OnClick);
            AddLongPressEndCallBack(OnPressEndClick);
        }

        public void AddChangeCallBack(UnityEngine.Events.UnityAction<bool> call)
        {
            RemoveChangeCallBack(call);
            tog.onValueChanged.AddListener(call);
        }

        public void RemoveChangeCallBack(UnityEngine.Events.UnityAction<bool> call)
        {
            tog.onValueChanged.RemoveListener(call);
        }
        public override void AddClickCallBack(GOGUI.EventTriggerListener.VoidDelegate func)
        {
            GOGUI.EventTriggerListener lis = GOGUI.EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onClick += func;

            clickAction -= func;
            clickAction += func;
        }

        public override void RemoveClickCallBack(GOGUI.EventTriggerListener.VoidDelegate func)
        {
            GOGUI.EventTriggerListener triggerListener = GOGUI.EventTriggerListener.Get(gameObject);
            triggerListener.parameter = this;
            triggerListener.onClick -= func;

            clickAction -= func;
        }

        void OnClick(GameObject go)
        {

        }

        void OnPressEndClick(GameObject go)
        {
        }

        public void SetValueWithoutOnChangedNotify(bool isOn)
        {
            tog.SetValue(isOn);
        }
    }
}
