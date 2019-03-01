// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PostFX
{

    [Serializable]
    public class Glitch : PostEffectBase
    {
        public enum GlitchingMode
        {
            Interferences,
            Tearing,
            Complete
        }

        [Serializable]
        public class InterferenceSettings
        {
            public float Speed = 10f;
            public float Density = 8f;
            public float MaxDisplacement = 2f;
        }

        [Serializable]
        public class TearingSettings
        {
            public float Speed = 1f;

            [Range(0f, 1f)]
            public float Intensity = 0.25f;

            [Range(0f, 0.5f)]
            public float MaxDisplacement = 0.05f;

            public bool AllowFlipping = false;
            public bool YuvColorBleeding = true;

            [Range(-2f, 2f)]
            public float YuvOffset = 0.5f;
        }

        [Tooltip("Automatically activate/deactivate the effect randomly.")]
        public bool RandomActivation = false;

        public Vector2 RandomEvery = new Vector2(1f, 2f);
        public Vector2 RandomDuration = new Vector2(1f, 2f);

        [Tooltip("Glitch type.")]
        public GlitchingMode Mode = GlitchingMode.Interferences;

        public InterferenceSettings SettingsInterferences = new InterferenceSettings();
        public TearingSettings SettingsTearing = new TearingSettings();

        protected bool m_Activated = true;
        protected float m_EveryTimer = 0f;
        protected float m_EveryTimerEnd = 0f;
        protected float m_DurationTimer = 0f;
        protected float m_DurationTimerEnd = 0f;

        public Glitch()
        {
            et = EffectType.Glitch;
        }

        public bool IsActive
        {
            get { return m_Activated; }
        }

        public override void OnEnable()
        {
            m_DurationTimerEnd = Random.Range(RandomDuration.x, RandomDuration.y);
        }

        public override void Update()
        {
            if (!RandomActivation)
                return;

            if (m_Activated)
            {
                m_DurationTimer += Time.deltaTime;

                if (m_DurationTimer >= m_DurationTimerEnd)
                {
                    m_DurationTimer = 0f;
                    m_Activated = false;
                    m_EveryTimerEnd = Random.Range(RandomEvery.x, RandomEvery.y);
                }
            }
            else
            {
                m_EveryTimer += Time.deltaTime;

                if (m_EveryTimer >= m_EveryTimerEnd)
                {
                    m_EveryTimer = 0f;
                    m_Activated = true;
                    m_DurationTimerEnd = Random.Range(RandomDuration.x, RandomDuration.y);
                }
            }
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (!m_Activated)
            {
                Graphics.Blit(source, destination);
                return;
            }

            if (Mode == GlitchingMode.Interferences)
            {
                DoInterferences(source, destination, SettingsInterferences);
            }
            else if (Mode == GlitchingMode.Tearing)
            {
                DoTearing(source, destination, SettingsTearing);
            }
            else // Complete
            {
                RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
                DoTearing(source, temp, SettingsTearing);
                DoInterferences(temp, destination, SettingsInterferences);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

        protected virtual void DoInterferences(RenderTexture source, RenderTexture destination, InterferenceSettings settings)
        {
            material.SetVector("_Params", new Vector3(settings.Speed, settings.Density, settings.MaxDisplacement));
            Graphics.Blit(source, destination, material, 0);
        }

        protected virtual void DoTearing(RenderTexture source, RenderTexture destination, TearingSettings settings)
        {
            material.SetVector("_Params", new Vector4(settings.Speed, settings.Intensity, settings.MaxDisplacement, settings.YuvOffset));

            int pass = 1;
            if (settings.AllowFlipping && settings.YuvColorBleeding) pass = 4;
            else if (settings.AllowFlipping) pass = 2;
            else if (settings.YuvColorBleeding) pass = 3;

            Graphics.Blit(source, destination, material, pass);
        }


        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("Hidden/Colorful/Glitch");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }
    }
}
