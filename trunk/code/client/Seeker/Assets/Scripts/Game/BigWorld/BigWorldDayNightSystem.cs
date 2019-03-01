using System;
using System.Collections.Generic;
using UnityEngine;
using GOGUI;
using EngineCore;

namespace SeekerGame
{
    public class BigWorldDayNightSystem
    {
        private string[] m_LightTexPath = new string[] { "Lightmap-0_comp_light_dusk.exr" }; //傍晚 晚上 , "Lightmap-0_comp_light_night.exr"
        private float m_dayTotalSecond = 14400f; //一天多少秒 4*60*60  14400f
        private float m_daySpeed = 0f; //速度
        private float m_dayHour = 0f;
        private Texture[] m_LightTex;
        private float m_curSecond = 0f;

        private int currentLightTexTotal = 0;
        private Action m_LoadLightTexOver = null;
        private int m_lastIndex = -1;
        private Dictionary<string, Material> m_allMat = new Dictionary<string, Material>();
        private GameObject m_yeWanParticle;
        private GameObject yeWanParticle
        {
            get
            {
                if (m_yeWanParticle ==null)
                {
                    this.m_yeWanParticle = GameObject.FindGameObjectWithTag("sceneYeWan");
                    if (this.m_yeWanParticle != null)
                    {
                        this.m_yeWanParticle.SetActive(false);
                    }
                }
                return m_yeWanParticle;
            }
        }
        public BigWorldDayNightSystem(Dictionary<string, Material> allMat)
        {
            //this.m_yeWanParticle = GameObject.FindGameObjectWithTag("sceneYeWan");
            if (yeWanParticle != null)
            {
                yeWanParticle.SetActive(false);
            }
            m_LightTex = new Texture[m_LightTexPath.Length + 1];
            long second = CommonTools.GetCurrentTimeSecond();
            m_daySpeed = 86400f / m_dayTotalSecond;

            m_curSecond = second / m_daySpeed;
            //m_curSecond = second % m_dayTotalSecond;
            this.m_dayHour = this.m_dayTotalSecond / m_LightTex.Length;
            this.m_allMat = allMat;
        }

        private Texture getSceneLightMap()
        {
            LightmapData[] lightmapData = LightmapSettings.lightmaps;
            if (lightmapData == null)
            {
                Debug.LogError("lightmap error");
                return null;
            }
            return lightmapData[0].lightmapColor;
        }

        public void LoadLightTex(Action loadOver)
        {
            currentLightTexTotal = 0;
            this.m_LoadLightTexOver = loadOver;
            m_LightTex[0] = getSceneLightMap();
            for (int i = 0; i < m_LightTexPath.Length; i++)
            {
                LoadLightTex(i);
            }

        }

        private void LoadLightTex(int i)
        {
            GOGUITools.GetAssetAction.SafeInvoke(m_LightTexPath[i], (prefabName, obj) =>
            {
                if (obj != null)
                {
                    Texture tex = obj as Texture;
                    //if (i == 0)
                    //{
                    //    m_LightTex[0] = tex;
                    //}
                    m_LightTex[i + 1] = tex;
                }
                currentLightTexTotal++;
                if (currentLightTexTotal == m_LightTexPath.Length && m_LoadLightTexOver != null)
                {
                    m_LoadLightTexOver();
                    StartDayNight();
                }
            }, LoadPriority.Default);
        }

        public void StartDayNight()
        {
            TimeModule.Instance.SetTimeout(RunDayNight, 1f, true, true);
        }

        private void RunDayNight()
        {
            m_curSecond += 1f;
            //m_curSecond += m_daySpeed;
            m_curSecond = m_curSecond % m_dayTotalSecond;
            int lightIndex = Mathf.FloorToInt(m_curSecond / m_dayHour);
            //Debug.Log("lightIndex = " + lightIndex);
            if (m_lastIndex != lightIndex && m_LightTex[lightIndex] != null)
            {
                LightmapData[] lightmapdata = new LightmapData[] { new LightmapData() };
                lightmapdata[0].lightmapColor = (Texture2D)m_LightTex[lightIndex];
                LightmapSettings.lightmaps = lightmapdata;
                m_lastIndex = lightIndex;
                foreach (var kv in m_allMat)
                {
                    kv.Value.SetTexture("_NightTex", m_LightTex[(lightIndex + 1) % m_LightTex.Length]);
                }
            }
            //float lerp = m_curSecond / m_dayHour;
            float lerp = m_curSecond % m_dayHour / (m_dayHour / 2f);
            //if (yeWanParticle != null)
            //{
            //    if (lerp >= 1f && lightIndex == 1)
            //    {
            //        yeWanParticle.SetActive(true);
            //    }
            //    else
            //    {
            //        yeWanParticle.SetActive(false);
            //    }
            //}
            
            lerp = Mathf.Clamp01(lerp);
            GameEvents.BigWorld_Event.OnReflashTime.SafeInvoke(lightIndex, lerp);
            foreach (var kv in m_allMat)
            {
                kv.Value.SetFloat("_lerp", lerp);
            }
        }

        public void OnDestory()

        {
            this.currentLightTexTotal = 0;
            this.m_LoadLightTexOver = null;
            TimeModule.Instance.RemoveTimeaction(RunDayNight);
            for (int i = 0; i < m_LightTexPath.Length; ++i)
                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(m_LightTexPath[i], m_LightTex[i + 1]);

            LightmapSettings.lightmaps = null;
        }
    }
}
