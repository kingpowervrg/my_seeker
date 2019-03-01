using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GOGUI
{

    /// <summary>
    /// 后期加载图片
    /// </summary>
    public class LazyImageLoader : MonoBehaviour
    {
        [SerializeField]
        string spriteName;
        [SerializeField]
        string rawImgName;
        [SerializeField]
        Image image;
        [SerializeField]
        SpriteRenderer sr;
        [SerializeField]
        RawImage rawImage;
        [SerializeField]
        AdvancedText advTxt;
        [SerializeField]
        Button btn;
        [SerializeField]
        string imgFontName;
        [SerializeField]
        Text text;
        [SerializeField]
        string fontName;
        [SerializeField]
        AnimatedImage animImg;
        [SerializeField]
        string[] animImgNames;
        [SerializeField]
        string btnPressedName;

        [System.NonSerialized]
        Mask maskComponent;
        [System.NonSerialized]
        bool maskGot;
        bool imgLoading, rawLoading, fontLoading, imgFontLoading, animImgLoading;
        bool buttonLoading;
        bool visible;
        static Sprite emptySprite;
        static Texture emptyTexture;

        public bool neddSetNativeSize = false;
        public float needSetFillAmmount = 1f;
        Dictionary<string, int> spriteMapping;

        const string CreatedSpriteName = "&Created%";
        public System.Action<string> OnGetRawImageAction = null;
        public string SpriteName
        {
            get { return spriteName; }
            set
            {
                if (spriteName != value)
                {
                    ReleaseSprite();
                    ReleaseSpriteRenderer();
                }
                spriteName = value;
            }
        }

        public Image Image { get { return image; } set { image = value; } }

        public SpriteRenderer SpriteRenderer { get { return sr; } set { sr = value; } }

        public string RawImageName
        {
            get { return rawImgName; }
            set
            {
                if (value != rawImgName)
                    ReleaseRawImage();
                rawImgName = value;
            }
        }

        public RawImage RawImage { get { return rawImage; } set { rawImage = value; } }

        public string ImageFontName
        {
            get { return imgFontName; }
            set
            {
                if (imgFontName != value)
                    ReleaseImageFont();
                imgFontName = value;
            }
        }

        public AdvancedText AdvancedText { get { return advTxt; } set { advTxt = value; } }

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
        public Text Text { get { return text; } set { text = value; } }

        public string[] AnimatedImageNames
        {
            get { return animImgNames; }
            set
            {
                animImgNames = value;
            }
        }

        public AnimatedImage AnimatedImage { get { return animImg; } set { animImg = value; } }

        public string ButtonPressedName { get { return btnPressedName; } set { btnPressedName = value; } }

        public Button Button { get { return btn; } set { btn = value; } }

        static bool delegateInitialized = false;
        static HashSet<LazyImageLoader> visibleLoader = new HashSet<LazyImageLoader>();

        void OnDisable()
        {
            visibleLoader.Remove(this);
            ReleaseRawImage();
            ReleaseSprite();
        }

        void OnEnable()
        {
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
                i.rawLoading = false;
                i.imgFontLoading = false;
                i.fontLoading = false;
                i.buttonLoading = false;
                i.animImgLoading = false;
                i.CheckResource();
            }
        }

        void CheckResource()
        {
            if (!emptySprite)
            {
                emptySprite = Resources.Load<Sprite>("EmptySprite");
                emptyTexture = emptySprite.texture;
            }
            if (!maskGot)
            {
                maskGot = true;
                maskComponent = gameObject.GetComponent<Mask>();
            }
            if (GOGUITools.GetAssetAction != null)
            {
                if (!string.IsNullOrEmpty(spriteName) && image && (!image.sprite || !image.sprite.texture))
                {
                    LoadSprite();
                }
                if (!string.IsNullOrEmpty(spriteName) && !imgLoading && sr && (!sr.sprite || !sr.sprite.texture))
                {
                    imgLoading = true;
                    if (sr.sprite && sr.sprite != emptySprite)
                    {
                        GameObject.Destroy(sr.sprite);
                        sr.sprite = null;
                    }
                    sr.sprite = emptySprite;
                    GOGUITools.GetAssetAction.SafeInvoke(spriteName, OnGetSprite, LoadPriority.Prior);
                }
                if (!string.IsNullOrEmpty(rawImgName) && !rawLoading && rawImage && !rawImage.texture)
                {
                    LoadRawImage();
                }
                if (!string.IsNullOrEmpty(ImageFontName) && !imgFontLoading && advTxt && !advTxt.ImageFont)
                {
                    imgFontLoading = true;
                    GOGUITools.GetAssetAction.SafeInvoke(ImageFontName, OnGetImageFont, LoadPriority.Prior);
                }
                if (!string.IsNullOrEmpty(fontName) && !fontLoading && text && !text.font)
                {
                    fontLoading = true;
                    GOGUITools.GetAssetAction.SafeInvoke(fontName, OnGetFont, LoadPriority.Prior);
                }
                if (animImgNames != null && !animImgLoading && animImg)
                {
                    if (spriteMapping == null)
                    {
                        spriteMapping = new Dictionary<string, int>();
                    }
                    spriteMapping.Clear();
                    animImg.sprite = emptySprite;
                    for (int i = 0; i < animImgNames.Length; i++)
                    {
                        if (!animImg.Sprites[i])
                        {
                            if (string.IsNullOrEmpty(animImgNames[i]))
                                continue;
                            animImg.Sprites[i] = emptySprite;
                            animImgLoading = true;
                            spriteMapping[animImgNames[i]] = i;
                            GOGUITools.GetAssetAction.SafeInvoke(animImgNames[i], OnGetAnimSprite, LoadPriority.Prior);
                        }
                    }
                    animImg.animationNames = animImgNames;
                }
                if (!string.IsNullOrEmpty(btnPressedName) && !buttonLoading && btn && !btn.spriteState.pressedSprite)
                {
                    buttonLoading = true;
                    GOGUITools.GetAssetAction.SafeInvoke(btnPressedName, OnGetPressSprite, LoadPriority.Prior);
                }
            }
        }

        public void LoadSprite()
        {
            if (!imgLoading && !string.IsNullOrEmpty(spriteName))
            {
                imgLoading = true;
                image.sprite = emptySprite;

                GOGUITools.GetAssetAction.SafeInvoke(spriteName, OnGetSprite, LoadPriority.Prior);
            }
        }

        public void LoadSpriteRenderer()
        {
            if (!imgLoading && !string.IsNullOrEmpty(spriteName))
            {
                imgLoading = true;
                sr.sprite = emptySprite;

                GOGUITools.GetAssetAction.SafeInvoke(spriteName, OnGetSprite, LoadPriority.Prior);
            }
        }

        public void LoadRawImage()
        {
            if (!rawLoading && !string.IsNullOrEmpty(rawImgName))
            {
                rawLoading = true;
                rawImage.texture = emptyTexture;
                GOGUITools.GetAssetAction.SafeInvoke(rawImgName, OnGetRawImage, LoadPriority.Prior);
            }
        }
        void OnGetPressSprite(string name, Object obj)
        {
            buttonLoading = false;
            if (name != btnPressedName)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                return;
            }

            Sprite sprite = null;
            sprite = obj as Sprite;
            if (sprite && btn)
            {
                var state = btn.spriteState;
                state.pressedSprite = sprite;
                btn.spriteState = state;
            }
            ActivateMask();
        }
        void OnGetAnimSprite(string name, Object obj)
        {
            int idx;
            if (spriteMapping.TryGetValue(name, out idx))
            {
                if (animImg)
                {
                    animImg.Sprites[idx] = obj as Sprite;
                }

                spriteMapping.Remove(name);
            }

            if (spriteMapping.Count == 0)
            {
                animImgLoading = false;
                animImg.enabled = true;
                animImg.Reset();
            }
        }

        void OnGetSprite(string name, Object obj)
        {
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
            if (sprite && image)
            {
                if (image.sprite && image.sprite != emptySprite)
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(name, image.sprite);
                }
                image.sprite = sprite;
                if (neddSetNativeSize == true)
                {
                    image.SetNativeSize();
                }
                if (needSetFillAmmount != 1.0)
                {
                    Debug.Log("needSetFillAmmount:" + needSetFillAmmount);
                    image.fillAmount = needSetFillAmmount;
                }
            }
            if (sprite && sr)
            {
                if (sr.sprite && sr.sprite != emptySprite)
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(name, sr.sprite);
                }
                sr.sprite = sprite;
            }
            ActivateMask();
        }

        void OnGetRawImage(string name, Object obj)
        {
            if (!enabled)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                return;
            }
            if (name != rawImgName)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                GOGUITools.GetAssetAction.SafeInvoke(rawImgName, OnGetRawImage, LoadPriority.Prior);
                return;
            }
            rawLoading = false;
            Texture tex = obj as Texture;
            if (tex && rawImage)
            {
                if (rawImage.texture && rawImage.texture != emptyTexture)
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(name, rawImage.texture);
                }
                rawImage.texture = tex;
            }
            ActivateMask();

            if (OnGetRawImageAction != null)
            {
                OnGetRawImageAction(name);
            }
        }

        void OnGetImageFont(string name, Object obj)
        {
            imgFontLoading = false;
            if (name != imgFontName)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(name, obj);
                return;
            }
            ImageFont font = ((GameObject)obj).GetComponent<ImageFont>();

            if (font && advTxt)
            {
                advTxt.ImageFont = font;
            }
            ActivateMask();
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

            if (font && text)
            {
                text.font = font;
            }
            ActivateMask();
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
            if (!string.IsNullOrEmpty(spriteName) && image && (image.sprite))
            {
                if (image.sprite != emptySprite)
                {
                    if (image.sprite.name == CreatedSpriteName)
                    {
                        if (image.sprite.texture)
                            GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, image.sprite.texture);
                        else
                            Object.Destroy(image.sprite);
                    }
                    else
                        GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, image.sprite);

                    image.sprite = null;
                }
            }

        }

        void ReleaseSpriteRenderer()
        {
            if (!string.IsNullOrEmpty(spriteName) && sr && (sr.sprite))
            {
                if (sr.sprite != emptySprite)
                {
                    if (sr.sprite.name == CreatedSpriteName && sr.sprite.texture)
                        GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, sr.sprite.texture);
                    else
                        GOGUITools.ReleaseAssetAction.SafeInvoke(spriteName, sr.sprite);
                    sr.sprite = null;
                }
            }
        }

        void ReleaseRawImage()
        {
            if (!string.IsNullOrEmpty(rawImgName) && rawImage && rawImage.texture)
            {
                if (rawImage.texture != emptyTexture)
                {
                    GOGUITools.ReleaseAssetAction.SafeInvoke(rawImgName, rawImage.texture);
                    rawImage.texture = null;
                }
            }
        }

        void ReleaseImageFont()
        {
            if (!string.IsNullOrEmpty(ImageFontName) && advTxt && advTxt.ImageFont)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(ImageFontName, advTxt.ImageFont.gameObject);
                advTxt.ImageFont = null;
            }
        }

        void ReleaseFont()
        {
            if (!string.IsNullOrEmpty(fontName) && text && text.font)
            {
                GOGUITools.ReleaseAssetAction.SafeInvoke(fontName, text.font);
                text.font = null;
            }
        }

        void ReleaseAnimatedImage()
        {
            if (animImgNames != null && animImg)
            {
                for (int i = 0; i < animImgNames.Length; i++)
                {
                    if (animImg.Sprites[i])
                    {
                        if (animImg.Sprites[i] == emptySprite)
                            continue;
                        if (string.IsNullOrEmpty(animImgNames[i]))
                            continue;
                        GOGUITools.ReleaseAssetAction.SafeInvoke(animImgNames[i], animImg.Sprites[i]);
                        animImg.Sprites[i] = null;
                    }
                }
            }
        }

        void ReleaseButtonPress()
        {
            if (!string.IsNullOrEmpty(btnPressedName) && btn && btn.spriteState.pressedSprite)
            {
                if (btn.spriteState.pressedSprite != emptySprite)
                    GOGUITools.ReleaseAssetAction.SafeInvoke(btnPressedName, btn.spriteState.pressedSprite);
                var state = btn.spriteState;
                state.pressedSprite = null;
                btn.spriteState = state;
            }
        }
        void OnDestroy()
        {
            visibleLoader.Remove(this);
            ReleaseSprite();
            ReleaseSpriteRenderer();
            ReleaseRawImage();
            ReleaseImageFont();
            ReleaseFont();
            ReleaseAnimatedImage();
            ReleaseButtonPress();
        }
    }
}