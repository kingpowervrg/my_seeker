using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
class CartoonPlayer : MonoBehaviour
{
    VideoPlayer videoPlayer;
    RawImage m_tex;
    RenderTexture m_render;

    Action OnFinished;

    public void Init(VideoClip first_clip_, int w_, int h_, Action on_finished_)
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        m_tex = this.GetComponent<RawImage>();
        if (null == m_render)
        {
            m_render = new RenderTexture(w_, h_, 24);
        }

        m_tex.texture = m_render;

        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = true;
        videoPlayer.waitForFirstFrame = true;

        // Let's target the near plane instead.
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;

        videoPlayer.targetTexture = m_render;
        //// This will cause our scene to be visible through the video being played.
        //videoPlayer.targetCameraAlpha = 0.5F;
        //// Here, using absolute.
        //videoPlayer.url = "/Users/graham/movie.mov";
        // Skip the first 100 frames.
        videoPlayer.clip = first_clip_;
        videoPlayer.frame = 0;
        // Restart From beginning when done.
        videoPlayer.isLooping = false;
        // Each time we reach the end, we slow down the playback by a factor of 10.
        videoPlayer.loopPointReached += EndReached;
        // its prepareCompleted event.

        OnFinished = on_finished_;

        videoPlayer.Pause();
    }

    public void Play(VideoClip clip_)
    {
        videoPlayer.clip = clip_;
        videoPlayer.Play();
    }

    public void Reset()
    {
        videoPlayer.frame = 0;
        videoPlayer.Pause();
    }


    public void EndReached(VideoPlayer vp)
    {
        //vp.playbackSpeed = vp.playbackSpeed / 10.0F;

        if(null != OnFinished)
        {
            OnFinished();
        }
    }
}

