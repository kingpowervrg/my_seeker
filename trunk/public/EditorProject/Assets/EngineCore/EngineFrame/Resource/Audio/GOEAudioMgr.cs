using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace EngineCore
{
    /// <summary>
    /// 音频类型
    /// </summary>
    public enum AudioType
    {
        /// <summary>
        /// 音效
        /// </summary>
        AUDIO_TYPE_EFFECT = 10,
        /// <summary>
        /// 背景音乐
        /// </summary>
        AUDIO_TYPE_BGM = 20,
    }

    /// <summary>
    /// 音效播放设置
    /// </summary>
    public struct PlayAudioInfo
    {
        /// <summary>
        /// 需要把音频绑定到哪个GO
        /// </summary>
        public GameObject GameObject;
        /// <summary>
        /// 声音类型
        /// </summary>
        public AudioType Type;
        /// <summary>
        /// 资源文件
        /// </summary>
        public string Name;
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool Loop;
        /// <summary>
        /// 播放完毕自动销毁
        /// </summary>
        public bool RemoveAtEnd;
        /// <summary>
        /// 是否不淡入淡出
        /// </summary>
        public bool NotFade;
        /// <summary>
        /// 混音组
        /// </summary>
        public AudioMixerGroup MixerGroup;
        /// <summary>
        /// 0-1， 0为纯2D音效，1为纯3D音效
        /// </summary>
        public float SpatialBlend;
        /// <summary>
        /// 3D音效最大音量距离
        /// </summary>
        public float MinDistance;
        /// <summary>
        /// 3D音效最小音量距离
        /// </summary>
        public float MaxDistance;
        /// <summary>
        /// 混响区音量
        /// </summary>
        public float ReverbZoneMix;
    }
}
namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
 class GOEAudioMgr : IGOEAudioMgr
    {
        private List<AudioInfo> autoDestoryAudios = new List<AudioInfo>();
        private List<AudioInfo> needFadeAudios = new List<AudioInfo>();
        public AudioSource AddSound(string name, GameObject obj, bool loop = false, bool removeAtEnd = true,
            AudioType type = AudioType.AUDIO_TYPE_EFFECT, bool notFade = false)
        {
            PlayAudioInfo info = new PlayAudioInfo();
            info.Name = name;
            info.GameObject = obj;
            info.Loop = loop;
            info.RemoveAtEnd = removeAtEnd;
            info.Type = type;
            info.NotFade = notFade;
            if (type == AudioType.AUDIO_TYPE_BGM)
            {
                info.MixerGroup = EngineDelegateCore.DefaultBGMMixerGroup;
                info.SpatialBlend = 0;
                info.ReverbZoneMix = 0;
            }
            else
            {
                info.MixerGroup = EngineDelegateCore.DefaultEffectMixerGroup;
                info.SpatialBlend = EngineDelegateCore.DefaultEffectSpatialBlend;
                info.MinDistance = EngineDelegateCore.Default3DAudioMinDistance;
                info.MaxDistance = EngineDelegateCore.Default3DAudioMaxDistance;
                info.ReverbZoneMix = EngineDelegateCore.DefaultReverbZoneMix;
            }
            return AddSound(ref info);
        }

        public AudioSource AddSound(ref PlayAudioInfo info)
        {
            AudioClipLoader loader = info.GameObject.AddComponent<AudioClipLoader>();
            loader.ResName = info.Name;
            if (loader.AudioSource == null)
            {
                GameObject.DestroyImmediate(loader);
                return null;
            }
            loader.AudioSource.playOnAwake = !IsMute;
            loader.AudioSource.loop = info.Loop;
            loader.AudioSource.volume = GetVolumeByType(info.Type);
            loader.AudioSource.mute = loader.AudioSource.volume <= 0;
            loader.AudioSource.outputAudioMixerGroup = info.MixerGroup;
            loader.AudioSource.spatialBlend = info.SpatialBlend;
            loader.AudioSource.minDistance = info.MinDistance;
            loader.AudioSource.maxDistance = info.MaxDistance;
            loader.AudioSource.rolloffMode = AudioRolloffMode.Linear;
            loader.AudioSource.dopplerLevel = 0;
            loader.AudioSource.bypassReverbZones = info.ReverbZoneMix == 0;
            loader.AudioSource.reverbZoneMix = info.ReverbZoneMix;
            loader.LoadSound();
            if (!info.Loop && info.RemoveAtEnd)
            {
                autoDestoryAudios.Add(new AudioInfo(loader.AudioSource));
            }
            else if (info.Loop && !IsMute && !info.NotFade)
            {
                AudioInfo ai = new AudioInfo(loader.AudioSource);
                ai.SetFadeIn();
                ai.type = info.Type;
                loader.AudioSource.volume = 0;
                addToFadePool(ai);
            }
            return loader.AudioSource;
        }

        public static float GetVolumeByType(AudioType type)
        {
            switch (type)
            {
                case AudioType.AUDIO_TYPE_EFFECT:
                default:
                    return EngineDelegateCore.MaxSoundVolume;
                case AudioType.AUDIO_TYPE_BGM:
                    return EngineDelegateCore.MaxBGMVolume;
            }
        }
        public void Update()
        {
            for (int i = 0; i < autoDestoryAudios.Count; i++)
            {
                AudioInfo info = autoDestoryAudios[i];
                if (!info.audioSource)
                {
                    autoDestoryAudios.RemoveAt(i);
                    i--;
                    continue;
                }
                if (info.audioSource.clip == null)
                    continue;
                if (info.timeLeft == -1)
                    info.timeLeft = info.audioSource.clip.length;
                else
                {
                    info.timeLeft -= TimeModule.GameRealTime.DeltaTime;
                    if (info.timeLeft < 0)
                    {
                        RemoveSound(info.audioSource);
                        i--;
                    }
                }
            }
            for (int i = 0; i < needFadeAudios.Count; i++)
            {
                AudioInfo info = needFadeAudios[i];
                if (!info.audioSource)
                {
                    needFadeAudios.RemoveAt(i);
                    i--;
                    continue;
                }
                if (!info.isFadeIn && info.audioSource.clip == null)
                {
                    RemoveSound(info.audioSource, false);
                    i--;
                    continue;
                }
                if (info.audioSource.clip == null || !info.audioSource.isPlaying)
                    continue;
                info.timeLeft -= TimeModule.GameRealTime.DeltaTime;
                if (info.timeLeft > 0)
                    info.SetVolume();
                else
                {
                    if (info.removeAtEnd)
                        RemoveSound(info.audioSource, false);
                    else
                    {
                        if (!info.isFadeIn)
                            info.audioSource.Stop();
                        needFadeAudios.RemoveAt(i);
                    }
                    i--;
                }
            }
        }

        private void addToFadePool(AudioInfo info)
        {
            foreach (AudioInfo ai in needFadeAudios)
            {
                if (ai.audioSource == info.audioSource)
                {
                    needFadeAudios.Remove(ai);
                    break;
                }
            }
            needFadeAudios.Add(info);
        }

        public void RemoveSound(AudioSource aus, bool needFade = true)
        {
            if (needFade && aus.loop && aus.isPlaying)
            {
                AudioInfo info = new AudioInfo(aus);
                info.type = AudioType.AUDIO_TYPE_BGM;
                info.SetFadeOut(true);
                addToFadePool(info);
                return;
            }
            AudioClipLoader loader = GetReleatedLoader(aus);
            if (loader != null)
                GameObject.Destroy(loader);
            GameObject.Destroy(aus);
            for (int i = 0; i < autoDestoryAudios.Count; i++)
            {
                AudioInfo info = autoDestoryAudios[i];
                if (info.audioSource == aus)
                {
                    autoDestoryAudios.RemoveAt(i);
                    return;
                }
            }
            for (int i = 0; i < needFadeAudios.Count; i++)
            {
                AudioInfo info = needFadeAudios[i];
                if (info.audioSource == aus)
                {
                    needFadeAudios.RemoveAt(i);
                    return;
                }
            }
        }


        public AudioClipLoader GetReleatedLoader(AudioSource audioScource)
        {
            if (audioScource.gameObject == null)
                return null;
            AudioClipLoader[] loaders = audioScource.gameObject.GetComponents<AudioClipLoader>();
            foreach (AudioClipLoader loader in loaders)
            {
                if (loader.AudioSource == audioScource)
                    return loader;
            }
            return null;
        }

        public bool IsMute = false;
        public void StopAudio(bool fade = true)
        {
            IsMute = true;
            AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audios)
            {
                if (audioSource != null && audioSource.clip != null)
                {
                    if (audioSource.isPlaying)
                    {
                        if (audioSource.loop && fade)
                        {
                            AudioInfo info = new AudioInfo(audioSource);
                            info.SetFadeOut(false);
                            addToFadePool(info);
                        }
                        else
                            audioSource.Stop();
                    }
                }
                else
                    audioSource.playOnAwake = false;
            }
        }

        public void PlayAudio()
        {
            IsMute = false;
            AudioSource[] audios = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audios)
            {
                if (audioSource.enabled)
                {
                    if (audioSource != null && audioSource.loop && audioSource.clip != null)
                    {
                        if (!audioSource.isPlaying)
                        {
                            AudioInfo info = new AudioInfo(audioSource);
                            info.SetFadeIn();
                            addToFadePool(info);
                            audioSource.Play();
                        }
                    }
                    else
                        audioSource.playOnAwake = true;
                }
            }
        }
    }

    class AudioInfo
    {
        public float timeLeft = -1;
        public bool isFadeIn = true;
        public bool removeAtEnd = false;
        public AudioType type;
        public AudioSource audioSource;
        public AudioInfo(AudioSource source)
        {
            audioSource = source;
        }

        public void SetFadeIn()
        {
            timeLeft = EngineDelegateCore.VolumeDuration;
            isFadeIn = true;
            removeAtEnd = false;
            audioSource.volume = 0;
        }

        public void SetFadeOut(bool remove = false)
        {
            timeLeft = EngineDelegateCore.VolumeDuration;
            isFadeIn = false;
            audioSource.volume = GOEAudioMgr.GetVolumeByType(type);
            removeAtEnd = remove;
        }

        public void SetVolume()
        {
            if (isFadeIn)
                audioSource.volume = ((EngineDelegateCore.VolumeDuration - timeLeft) * GOEAudioMgr.GetVolumeByType(type) / EngineDelegateCore.VolumeDuration);
            else
                audioSource.volume = timeLeft * GOEAudioMgr.GetVolumeByType(type) / EngineDelegateCore.VolumeDuration;
        }
    }
}
