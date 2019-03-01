using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GOGUI
{

    /// <summary>
    /// 后期加载图片
    /// </summary>
    public class LazyLoadImage : Image
    {
        [SerializeField]
        string spriteName;

        [System.NonSerialized]
        Mask maskComponent;
        [System.NonSerialized]
        bool maskGot;
        [System.NonSerialized]
        bool imgLoading;
        [System.NonSerialized]
        bool visible;
        static Sprite emptySprite;

        public bool neddSetNativeSize = false;
        public float needSetFillAmmount = 1f;

        public System.Action<string> OnLoaded { get; set; }

        const string CreatedSpriteName = "&Created%";
        public string SpriteName
        {
            get { return spriteName; }
            set
            {
                if (spriteName != value)
                {
                    ReleaseSprite();
                }
                spriteName = value;
            }
        }

        static bool delegateInitialized = false;
        static HashSet<LazyLoadImage> visibleLoader = new HashSet<LazyLoadImage>();

        protected override void OnDisable()
        {
            base.OnDisable();
            visibleLoader.Remove(this);
            ReleaseSprite();
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
                i.imgLoading = false;
                i.CheckResource();
            }
        }

        void CheckResource()
        {
            if (!emptySprite)
            {
                emptySprite = Resources.Load<Sprite>("EmptySprite");
            }
            if (!maskGot)
            {
                maskGot = true;
                maskComponent = gameObject.GetComponent<Mask>();
            }
            if (GOGUITools.GetAssetAction != null)
            {
                if (!string.IsNullOrEmpty(spriteName) && (!sprite || !sprite.texture || sprite == emptySprite))
                {
                    LoadSprite();
                }
            }
        }

        public void LoadSprite()
        {
            if (!imgLoading && !string.IsNullOrEmpty(spriteName) && this)
            {
                imgLoading = true;
                sprite = emptySprite;

                GOGUITools.GetAssetAction.SafeInvoke(spriteName, OnGetSprite, LoadPriority.Prior);
            }
        }

        void OnGetSprite(string name, Object obj)
        {
            if (!this)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                return;
            }
            if (name != spriteName)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                GOGUITools.GetAssetAction.SafeInvoke(spriteName, OnGetSprite, LoadPriority.Prior);
                return;
            }
            imgLoading = false;

            Sprite sprite = null;
            if (obj is Texture2D)
            {
                Texture2D tex = (Texture2D)obj;
                sprite = Sprite.Create((Texture2D)obj, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sprite.name = CreatedSpriteName;
            }
            else
                sprite = obj as Sprite;
            if (sprite)
            {
                if (this.sprite && this.sprite != emptySprite)
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(name, this.sprite);
                }
                this.sprite = sprite;
                if (neddSetNativeSize == true)
                {
                    SetNativeSize();
                }
                if (needSetFillAmmount != 1.0)
                {
                    Debug.Log("needSetFillAmmount:" + needSetFillAmmount);
                    fillAmount = needSetFillAmmount;
                }
            }
            ActivateMask();
            //OnLoaded.SafeInvoke(SpriteName);
        }

        void ActivateMask()
        {
            if (maskComponent)
            {
                maskComponent.enabled = false;
                maskComponent.enabled = true;
            }
        }

        void ReleaseSprite()
        {
            if (!string.IsNullOrEmpty(spriteName) && this.sprite)
            {
                if (this.sprite != emptySprite)
                {
                    if (this.sprite.name == CreatedSpriteName)
                    {
                        if (this.sprite.texture)
                            GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, this.sprite.texture);
                        else
                            Object.Destroy(this.sprite);
                    }
                    else
                        GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, this.sprite);

                    this.sprite = null;
                }
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            visibleLoader.Remove(this);
            ReleaseSprite();
        }
    }
}