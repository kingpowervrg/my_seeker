/********************************************************************
	created:  2019-2-15 16:33:37
	filename: UIComponentExtension.cs
	author:	  songguangze@outlook.com
	
	purpose:  Unity UI组件的扩展方法
*********************************************************************/

using UnityEngine.UI;

namespace EngineCore
{
    public static class UIComponentExtension
    {
        static Slider.SliderEvent EmptySliderEvent = new Slider.SliderEvent();
        public static void SetValue(this Slider instance, float value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptySliderEvent;
            instance.value = value;
            instance.onValueChanged = originalEvent;
        }

        static Toggle.ToggleEvent EmptyToggleEvent = new Toggle.ToggleEvent();
        public static void SetValue(this Toggle instance, bool value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptyToggleEvent;
            instance.isOn = value;
            instance.onValueChanged = originalEvent;
        }

        static InputField.OnChangeEvent EmptyInputFieldEvent = new InputField.OnChangeEvent();
        public static void SetValue(this InputField instance, string value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptyInputFieldEvent;
            instance.text = value;
            instance.onValueChanged = originalEvent;
        }
    }
}