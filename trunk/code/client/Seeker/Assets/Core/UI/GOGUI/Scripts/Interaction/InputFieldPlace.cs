using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldPlace : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    InputField _inputField;


    public void OnSelect(BaseEventData eventData)
    {
        if (_inputField == null)
        {
            _inputField = gameObject.GetComponent<InputField>();
        }
        bool flag = string.IsNullOrEmpty(_inputField.text);
        if (!flag)
        {
            return;
        }

        _inputField.placeholder.gameObject.SetActive(false);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_inputField == null)
        {
            return;
        }
        bool flag = string.IsNullOrEmpty(_inputField.text);
        if (!flag)
        {
            return;
        }

        _inputField.placeholder.gameObject.SetActive(true);
    }
}