using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace EngineCore
{
    public class GameLabel : GameTextComponent
    {
        public override void OnHide()
        {
            base.OnHide();
            FinishWriter();
        }

        private bool isPlay = false;
        private int indexPos = -1;
        private string cacheText;
        bool sound = false;
        const string WriteSound = "Keyboard1.wav";
        AudioSource audio = null;
        List<KeyValuePair<int, int>> colorPos = new List<KeyValuePair<int, int>>();
        public virtual void StartWriter(string text, float speed = 0.4f, bool sound = true)
        {
            if (string.IsNullOrEmpty(text)) { Text = text; return; }
            isPlay = true;
            this.sound = sound;
            indexPos = 0;
            cacheText = text;
            FilterColorSymbol(text);
            FilterSizeSymbol(text);
            UpdateWriter();
            TimeModule.Instance.RemoveTimeaction(UpdateWriter);
            TimeModule.Instance.SetTimeInterval(UpdateWriter, speed);
            //if (sound && audio ==null)
            //    audio = AddSound(WriteSound, Main.Inst.gameObject, true, true, GOEngine.AudioType.AUDIO_TYPE_EFFECT, true);
        }

        public virtual void UpdateWriter()
        {
            if (isPlayAction() == false)
                return;
            CheckColorSymbol();
            CheckSizeSymbol();
            CheckColorSymbol();
            CheckSizeSymbol();
            ++indexPos;
            string t = cacheText.Substring(0, indexPos);
            if(StartColor && StartSize)
            {

            }
            FillSymbol(ref t);
            Text = t;
            if (indexPos >= cacheText.Length)
            {
                FinishWriter();
            }
        }
        public virtual void FinishWriter()
        {
            if (isPlay)
            {
                ClearSizeSymbol();
                ClearColorSymbol();
                Text = cacheText;
                isPlay = false;
                indexPos = -1;
                TimeModule.Instance.RemoveTimeaction(UpdateWriter);
            }
          /*  if (audio != null)
            {
                GOEngine.GOERoot.GOEAudioMgr.RemoveSound(audio);
                audio = null;
            }*/
        }
        public bool isPlayAction()
        {
            return isPlay;
        }

        MatchCollection ColorMatches;
        MatchCollection ColorMatchesEnd;
        bool StartColor = false;    //标记是否开启
        int ColorMatcheIdx = 0;
        private void FilterColorSymbol(string text)
        {
            ColorMatches = Regex.Matches(text,
                                            @"<color=#[A-Za-z]*[0-9]*>",
                                            RegexOptions.IgnoreCase | 
                                            RegexOptions.ExplicitCapture 
                                            );

            ColorMatchesEnd = Regex.Matches(text,
                                            @"</color>",
                                            RegexOptions.IgnoreCase |
                                            RegexOptions.ExplicitCapture
                                            );
            ColorMatcheIdx = 0;
        }

        private void CheckColorSymbol()
        {
            if(!StartColor && ColorMatches != null && ColorMatcheIdx < ColorMatches.Count)
            {
                if (indexPos == ColorMatches[ColorMatcheIdx].Index)
                {
                    StartColor = true;
                    indexPos += ColorMatches[ColorMatcheIdx].Length;
                }
            }
            if (StartColor && ColorMatchesEnd != null && ColorMatcheIdx < ColorMatchesEnd.Count)
            {
                if (indexPos == ColorMatchesEnd[ColorMatcheIdx].Index)
                {
                    StartColor = false;
                    indexPos += ColorMatchesEnd[ColorMatcheIdx].Length;
                    ColorMatcheIdx++;
                }
            }
        }
        private void ClearColorSymbol()
        {
            ColorMatcheIdx = 0;
            StartColor = false;
            ColorMatches = null;
            ColorMatchesEnd = null;
        }

        MatchCollection SizeMatches;
        MatchCollection SizeMatchesEnd;
        bool StartSize = false;
        int SizeMatcheIdx = 0;
        private void FilterSizeSymbol(string text)
        {
            SizeMatches = Regex.Matches(text,
                                            @"<size=[0-9]*>",
                                            RegexOptions.IgnoreCase |
                                            RegexOptions.ExplicitCapture
                                            );

            SizeMatchesEnd = Regex.Matches(text,
                                            @"</size>",
                                            RegexOptions.IgnoreCase |
                                            RegexOptions.ExplicitCapture
                                            );
            SizeMatcheIdx = 0;
        }
        private void CheckSizeSymbol()
        {
            if (!StartSize && SizeMatches != null && SizeMatcheIdx < SizeMatches.Count)
            {
                if (indexPos == SizeMatches[SizeMatcheIdx].Index)
                {
                    StartSize = true;
                    indexPos += SizeMatches[SizeMatcheIdx].Length;
                }
            }
            if (StartSize && SizeMatchesEnd != null && SizeMatcheIdx < SizeMatchesEnd.Count)
            {
                if (indexPos == SizeMatchesEnd[SizeMatcheIdx].Index)
                {
                    StartSize = false;
                    indexPos += SizeMatchesEnd[SizeMatcheIdx].Length;
                    SizeMatcheIdx++;
                }
            }
        }

        private void ClearSizeSymbol()
        {
            SizeMatcheIdx = 0;
            StartSize = false;
            SizeMatches = null;
            SizeMatchesEnd = null;
        }

        private void FillSymbol(ref string text)
        {
            //补齐编码 需要判断color 和size 位置前后对应
            if (StartSize && StartColor)
            {
                if(SizeMatchesEnd[SizeMatcheIdx].Index > ColorMatchesEnd[ColorMatcheIdx].Index)
                    text += "</color></size>";
                else
                    text += "</size></color>";
            }
            else if (StartSize)
            {
                text += "</size>";
            }
            else if (StartColor)
            {
                text += "</color>";
            }
        }
    }

}
