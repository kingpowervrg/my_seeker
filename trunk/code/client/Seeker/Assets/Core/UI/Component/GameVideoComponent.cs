/********************************************************************
	created:  2018-11-26 10:53:6
	filename: GameVideoComponent.cs
	author:	  songguangze@outlook.com
	
	purpose:  Video Player组件
*********************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EngineCore
{
    public class GameVideoComponent : GameUIComponent
    {
        private VideoPlayer m_sourceVideoComponent = null;
        private RawImage m_videoRenderDst = null;
        private AudioSource m_videoAudioSource = null;

        private string m_videoName = null;
        private RenderTexture m_videoRt = null;

        public System.Action m_playComplete = null;
        protected override void OnInit()
        {
            m_sourceVideoComponent = gameObject.GetOrAddComponent<VideoPlayer>();
            m_sourceVideoComponent.playOnAwake = false;
            m_sourceVideoComponent.source = VideoSource.Url;
            m_sourceVideoComponent.renderMode = VideoRenderMode.RenderTexture;

            m_videoAudioSource = gameObject.GetOrAddComponent<AudioSource>();
            m_videoAudioSource.playOnAwake = false;
            m_videoRenderDst = GetComponent<RawImage>();

            m_videoRt = new RenderTexture((int)Widget.rect.width, (int)Widget.rect.height, 0);
            m_videoRenderDst.texture = this.m_videoRt;

            m_sourceVideoComponent.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_sourceVideoComponent.controlledAudioTrackCount = 1;
            m_sourceVideoComponent.EnableAudioTrack(0, true);
            m_sourceVideoComponent.SetTargetAudioSource(0, m_videoAudioSource);

            m_sourceVideoComponent.targetTexture = m_videoRt;
        }

        /// <summary>
        /// 播放Video
        /// </summary>
        /// <param name="isPlayFromBegining"></param>
        public void PlayVideo(bool isPlayFromBegining = true)
        {
            if (isPlayFromBegining && m_sourceVideoComponent.isPrepared)
                this.m_sourceVideoComponent.time = 0;

            EngineCoreEvents.BridgeEvent.StartCoroutine(PlayVideoInternal());
        }

        private void loopPointReached(UnityEngine.Video.VideoPlayer vp)
        {
            if (m_playComplete != null)
            {
                m_playComplete();
            }
        }

        public override void OnHide()
        {
            base.OnHide();

            this.m_videoAudioSource.Stop();
            this.m_sourceVideoComponent.Stop();
        }

        public string VideoName
        {
            get
            {
                return m_videoName;
            }
            set
            {
                if (!value.EndsWithFast(".mp4"))
                    value = value + ".mp4";

                if (m_videoName != value)
                {
                    this.m_sourceVideoComponent.source = VideoSource.Url;
                    if (value.StartsWithFast("http://"))
                        m_sourceVideoComponent.url = value;
                    else
                        m_sourceVideoComponent.url = WWWUtil.GetVideoUrl(value);

                    EngineCoreEvents.BridgeEvent.StartCoroutine(PlayVideoInternal());

                    this.m_videoName = value;
                }
            }
        }


        public bool IsVideoClipAvalid
        {
            get
            {
                return this.m_sourceVideoComponent != null && this.m_sourceVideoComponent.clip;
            }
        }


        private IEnumerator PlayVideoInternal()
        {
            if (!this.m_sourceVideoComponent.isPrepared)
                this.m_sourceVideoComponent.Prepare();

            while (!this.m_sourceVideoComponent.isPrepared)
                yield return null;

            if (Visible)
            {
                this.m_sourceVideoComponent.loopPointReached -= loopPointReached;
                this.m_sourceVideoComponent.loopPointReached += loopPointReached;
                this.m_sourceVideoComponent.Play();
                this.m_videoAudioSource.Play();
            }
        }


        private void ReleaseVideoClip()
        {
            if (this.m_sourceVideoComponent.isPlaying)
                this.m_sourceVideoComponent.Stop();

            m_videoName = string.Empty;
        }

        public VideoPlayer VideoPlayerComponent
        {
            get { return this.m_sourceVideoComponent; }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.m_sourceVideoComponent.loopPointReached -= loopPointReached;
            ReleaseVideoClip();
            this.m_videoRt.Release();
        }
    }
}