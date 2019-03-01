using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EngineCore;
using SeekerGame;
public class TextMutilLanguage : MonoBehaviour
{
    void Awake()
    {
        Text tex = GetComponent<Text>();
        if (tex != null)
        {
            tex.text = tex.text.Replace("\r","");
            tex.text = LocalizeModule.Instance.GetString(tex.text);
        }
    }
}
