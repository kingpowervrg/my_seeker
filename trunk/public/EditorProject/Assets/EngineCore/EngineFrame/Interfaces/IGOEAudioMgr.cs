using System;
using GOEngine.Implement;
using UnityEngine;

namespace EngineCore
{
    public interface IGOEAudioMgr
    {
        /// <summary>
        /// 添加音乐
        /// </summary>
        /// <param name="name">音乐名</param>
        /// <param name="obj">添加到的对象</param>
        /// <param name="type"></param>
        /// <param name="removeAtEnd"></param>
        /// <param name="notFade"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        global::UnityEngine.AudioSource AddSound(string name, GameObject obj, bool loop = false, bool removeAtEnd = true,
           AudioType type = AudioType.AUDIO_TYPE_EFFECT, bool notFade = false);
        /// <summary>
        /// 添加音效
        /// </summary>
        /// <param name="info">音效信息</param>
        /// <returns></returns>
        AudioSource AddSound(ref PlayAudioInfo info);
        /// <summary>
        /// 删除某个音乐
        /// </summary>
        /// <param name="aus"></param>
        void RemoveSound(global::UnityEngine.AudioSource aus, bool needFade = true);
        /// <summary>
        /// 播放音乐
        /// </summary>
        void PlayAudio();
        /// <summary>
        /// 停止音乐
        /// </summary>
        void StopAudio(bool fade = true);
    }
}
