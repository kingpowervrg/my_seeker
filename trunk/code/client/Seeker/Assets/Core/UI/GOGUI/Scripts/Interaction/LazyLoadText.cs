using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GOGUI
{

    /// <summary>
    /// 后期加载图片
    /// </summary>
    public class LazyLoadText : Text
    {
        [SerializeField]
        string fontName;
        bool fontLoading;
        bool visible;
        public string FontName
        {
            get { return fontName; }
            set
            {
                if (value != fontName)
                    ReleaseFont();
                fontName = value;
            }
        }

        static bool delegateInitialized = false;
        static HashSet<LazyLoadText> visibleLoader = new HashSet<LazyLoadText>();

        protected override void OnDisable()
        {
            base.OnDisable();
            visibleLoader.Remove(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckResource();
            visibleLoader.Add(this);
            if (!delegateInitialized)
            {
                delegateInitialized = true;
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            }
        }

        static void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            foreach (var i in visibleLoader)
            {
                i.fontLoading = false;
                i.CheckResource();
            }
        }

        void CheckResource()
        {
            if (GOGUITools.GetAssetAction != null)
            {
                if (!string.IsNullOrEmpty(fontName) && !fontLoading && !this.font)
                {
                    fontLoading = true;
                    GOGUITools.GetAssetAction.SafeInvoke(fontName, OnGetFont, LoadPriority.Prior);
                }
            }
        }

        void OnGetFont(string name, Object obj)
        {
            fontLoading = false;
            if (name != fontName)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                return;
            }
            Font font = obj as Font;

            if (font)
            {
                this.font = font;
            }
        }
        void ReleaseFont()
        {
            if (!string.IsNullOrEmpty(fontName) && this.font)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(fontName, this.font);
                this.font = null;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            visibleLoader.Remove(this);
            ReleaseFont();
        }
    }
}