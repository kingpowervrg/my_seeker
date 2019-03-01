using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TextFader : MonoBehaviour {

	public bool IgnoreWhiteSpaces = true;
	public bool StartOnEnable = true;
	public float CharFadeDuration = 0.1f;

	//CharLimiter CharLimiter;
	CharFader CharFader;
	Text text;

	int currentLetterIndex;
	float currentCharFadeTime;

    public Action OnComplete = null;

    void OnEnable() {

		//if( CharLimiter == null )
		//	CharLimiter = gameObject.AddComponent<CharLimiter>();

		if( CharFader == null )
			CharFader = gameObject.AddComponent<CharFader>();

		//CharLimiter.enabled = true;
		CharFader.enabled = true;

        if (text == null)
            text = GetComponent<Text>();

        if ( StartOnEnable )
		{
			PerformAnimation();
		}
	}

	void OnDisable() {
		//CharLimiter.enabled = false;
		CharFader.enabled = false;
        if (OnComplete != null)
        {
            OnComplete();
        }
    }

	public void PerformAnimation() {
		currentLetterIndex = 0;
		currentCharFadeTime = 0.0f;
	}

    private bool SkipRichText()
    {
        var str = text.text;
        if (!str[currentLetterIndex].Equals('<'))
        {
            return false;
        }
        for (int i = currentLetterIndex + 1; i < str.Length; i++)
        {
            if (str[i].Equals('>'))
            {
                currentLetterIndex = i;
                return true;
            }
        }
        return false;
    }

	void Update() {

		if( IgnoreWhiteSpaces )
		{
			var str = text.text;

            if (currentLetterIndex >= str.Length)
            {
                enabled = false;
                if (OnComplete != null)
                {
                    OnComplete();
                }
                return;
            }

            var currentChar = str[currentLetterIndex];
			if( currentChar == ' ' || SkipRichText())
			{
				currentLetterIndex++;
				Update();
				return;
			}
		}

		//CharLimiter.NumberOfLetters = currentLetterIndex + 1;

		currentCharFadeTime += Time.deltaTime;
		float progress = currentCharFadeTime / CharFadeDuration;
	
		if( progress >= 1.0f )
		{
			CharFader.SetCharAlpha( currentLetterIndex, 255 );

			currentLetterIndex++;
			currentCharFadeTime = 0.0f;

			if( currentLetterIndex >= text.text.Length )
			{
                enabled = false;
                
            }
		}
		else
		{
			byte alpha = (byte)(progress * 255);
			CharFader.SetCharAlpha( currentLetterIndex, alpha );

		}
	}
}
