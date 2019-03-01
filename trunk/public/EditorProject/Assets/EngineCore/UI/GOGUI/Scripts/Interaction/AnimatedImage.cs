using UnityEngine;
using UnityEngine.UI;

namespace GOGUI
{

    /// <summary>
    /// 序列帧组建
    /// </summary>
    [AddComponentMenu("UI/Animated Image", 11)]
    [ExecuteInEditMode]
    public class AnimatedImage : Image
    {
        [SerializeField]
        int fps = 25;
        [SerializeField]
        Sprite[] sprites;
        [SerializeField]
        float delay;

        [SerializeField]
        private bool m_isPlayOnce = false;

        float tpf;
        int idx;
        float accumulatedTime;
        bool delayStarted;
        public int FPS { get { return fps; } set { fps = value; } }

        public float Delay { get { return delay; } set { delay = value; } }

        public Sprite[] Sprites { get { return sprites; } set { sprites = value; } }

        public string[] animationNames;
        private bool m_isAnimationSpriteLoadFinish = false;


        private bool m_playedOnce = false;


        void OnEnable()
        {
            tpf = 1f / fps;
            Reset();
        }

        void Update()
        {
            if (IsSpriteResValid())
            {
                if (m_playedOnce && m_isPlayOnce)
                    enabled = false;

                accumulatedTime += Time.deltaTime;
                while (!delayStarted && accumulatedTime >= tpf)
                {
                    idx++;
                    accumulatedTime -= tpf;

                    if (idx >= sprites.Length)
                        m_playedOnce = true;


                    if (delay > 0 && idx >= sprites.Length)
                    {
                        idx = 0;
                        delayStarted = true;
                    }
                }
                if (delayStarted)
                {
                    if (accumulatedTime < delay)
                    {
                        return;
                    }
                    accumulatedTime -= delay;
                    delayStarted = false;
                }


                idx = idx % sprites.Length;
                if (sprites == null || sprites.Length <= 0)
                    return;

                sprite = sprites[idx];
                //UIAtlasManager.GetInstance().SetAtlasMaterial(this, animationNames[idx]);
                //}
            }
        }

        private bool IsSpriteResValid()
        {
            if (m_isAnimationSpriteLoadFinish)
                return true;
            else
            {
                for (int i = 0; i < animationNames.Length; ++i)
                {
                    if (Sprites[i] == null)
                    {
                        m_isAnimationSpriteLoadFinish = false;
                        return false;
                    }
                }
                m_isAnimationSpriteLoadFinish = true;
                return m_isAnimationSpriteLoadFinish;
            }
        }

        public void Reset()
        {
            idx = 0;
            accumulatedTime = 0;
            m_playedOnce = false;
        }

        public void SetFPS(int _fps)
        {
            fps = _fps;
            tpf = 1f / fps;
            Reset();
        }
    }
}