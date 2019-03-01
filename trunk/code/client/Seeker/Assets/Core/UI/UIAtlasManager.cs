using EngineCore;
using GOGUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAtlasManager
{
    class ImageItem
    {
        public Image image;
        public UIAtlasInfo.AlphaChannel channel = UIAtlasInfo.AlphaChannel.Channel_R;

        public ImageItem(Image _image, UIAtlasInfo.AlphaChannel _channel)
        {
            this.image = _image;
            this.channel = _channel;
        }

        private ImageItem()
        {

        }
    }
    class AtlasItem
    {
        public Texture2D Alphatexture;
        Material _alphaMaterialR;
        Material _alphaMaterialG;
        Material _alphaMaterialB;
        Material _alphaMaterialRGray;
        Material _alphaMaterialGGray;
        Material _alphaMaterialBGray;
        Material AlphaMaterialR
        {
            get
            {
                if (_alphaMaterialR == null)
                {

                    Shader RShader = Shader.Find("UI/Default-R");
                    _alphaMaterialR = new Material(RShader);

                    _alphaMaterialR.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialR;
            }
        }
        Material AlphaMaterialG
        {
            get
            {
                if (_alphaMaterialG == null)
                {
                    Shader GShader = Shader.Find("UI/Default-G");
                    _alphaMaterialG = new Material(GShader);
                    _alphaMaterialG.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialG;
            }
        }
        Material AlphaMaterialB
        {
            get
            {
                if (_alphaMaterialB == null)
                {
                    Shader BShader = Shader.Find("UI/Default-B");
                    _alphaMaterialB = new Material(BShader);
                    _alphaMaterialB.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialB;
            }
        }

        Material AlphaMaterialRGray
        {
            get
            {
                if (_alphaMaterialRGray == null)
                {

                    Shader RShader = Shader.Find("UI/Default-R (Gray)");
                    _alphaMaterialRGray = new Material(RShader);

                    _alphaMaterialRGray.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialRGray;
            }
        }

        Material AlphaMaterialGGray
        {
            get
            {
                if (_alphaMaterialGGray == null)
                {
                    Shader GShader = Shader.Find("UI/Default-G");
                    _alphaMaterialGGray = new Material(GShader);
                    _alphaMaterialGGray.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialGGray;
            }
        }

        Material AlphaMaterialBGray
        {
            get
            {
                if (_alphaMaterialBGray == null)
                {
                    Shader BShader = Shader.Find("UI/Default-B");
                    _alphaMaterialBGray = new Material(BShader);
                    _alphaMaterialBGray.SetTexture("_AlphaTex", Alphatexture);
                }
                return _alphaMaterialBGray;
            }
        }

        int refCount = 0;

        List<ImageItem> waitToSetList = new List<ImageItem>();

        public void SetMaterial(Image image, UIAtlasInfo.AlphaChannel channel)
        {
            switch (channel)
            {
                case UIAtlasInfo.AlphaChannel.Channel_R:
                    {
                        image.material = AlphaMaterialR;
                    }
                    break;
                case UIAtlasInfo.AlphaChannel.Channel_G:
                    {
                        image.material = AlphaMaterialG;
                    }
                    break;
                case UIAtlasInfo.AlphaChannel.Channel_B:
                    {
                        image.material = AlphaMaterialB;
                    }
                    break;
                case UIAtlasInfo.AlphaChannel.Channel_R_Gray:
                    {
                        image.material = AlphaMaterialRGray;
                    }
                    break;
                case UIAtlasInfo.AlphaChannel.Channel_G_Gray:
                    {
                        image.material = AlphaMaterialGGray;
                    }
                    break;
                case UIAtlasInfo.AlphaChannel.Channel_B_Gray:
                    {
                        image.material = AlphaMaterialBGray;
                    }
                    break;
            }

            refCount++;
        }

        public void ClearMaterialRef()
        {
            //if (refCount == 0)
            {
                if (_alphaMaterialR != null)
                    GameObject.DestroyImmediate(AlphaMaterialR, true);
                if (_alphaMaterialG != null)
                    GameObject.DestroyImmediate(AlphaMaterialG, true);
                if (_alphaMaterialB != null)
                    GameObject.DestroyImmediate(AlphaMaterialB, true);

                if (_alphaMaterialRGray != null)
                    GameObject.DestroyImmediate(AlphaMaterialRGray, true);
                if (_alphaMaterialGGray != null)
                    GameObject.DestroyImmediate(AlphaMaterialGGray, true);
                if (_alphaMaterialBGray != null)
                    GameObject.DestroyImmediate(AlphaMaterialBGray, true);
            }
        }

        public void LoadEnd(string name, Object obj)
        {

            if (obj is Texture2D)
            {
                Alphatexture = obj as Texture2D;
            }
            else
            {
                //DLog.error("Load Alpha Texture Error " + name);
                return;
            }

            int count = waitToSetList.Count;
            for (int i = 0; i < count; i++)
            {
                ImageItem imageItem = waitToSetList[i];
                SetMaterial(imageItem.image, imageItem.channel);
            }
            waitToSetList.Clear();
        }

        public void AddWaiteList(ImageItem imageItem)
        {
            if (imageItem != null)
            {
                if (Alphatexture == null)
                {
                    waitToSetList.Add(imageItem);
                }
                else
                {
                    SetMaterial(imageItem.image, imageItem.channel);
                }
            }
        }
        public AtlasItem(string textureName, ImageItem _imageItem)
        {
            AddWaiteList(_imageItem);
            GOGUITools.GetAssetAction.SafeInvoke(textureName, LoadEnd, LoadPriority.HighPrior);
        }

        public AtlasItem(string textureName)
        {
            GOGUITools.GetAssetAction.SafeInvoke(textureName, LoadEnd, LoadPriority.HighPrior);
        }
    }

    #region Static 
    static UIAtlasManager _instance = null;
    public static UIAtlasManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new UIAtlasManager();

            //GOERoot.Scene.OnLeaveScene += _instance.OnEnterScene;

            UIAtlasInfo.OnInit();
        }
        return _instance;
    }

    #endregion

    Dictionary<string, AtlasItem> atlasDic = new Dictionary<string, AtlasItem>();

    public bool SetAtlasMaterial(Image image, string textureName, bool isGray = false)
    {
        if (string.IsNullOrEmpty(textureName))
        {
            return false;
        }

        string alphaTexuteName = "";
        if (UIAtlasInfo.textureMap.TryGetValue(textureName, out alphaTexuteName))
        {

        }
        else
        {
            return false;
        }

        UIAtlasInfo.AlphaChannel channel;

        if (UIAtlasInfo.textureChannelMap.TryGetValue(textureName, out channel))
        {

        }

        if (isGray)
        {
            channel = (UIAtlasInfo.AlphaChannel)(3 + (int)channel);
        }

        AtlasItem _atlasItem;

        if (atlasDic.TryGetValue(alphaTexuteName, out _atlasItem))
        {
            if (_atlasItem.Alphatexture != null)
            {
                _atlasItem.SetMaterial(image, channel);
            }
            else
            {
                _atlasItem.AddWaiteList(new ImageItem(image, channel));
            }
        }
        else
        {
            atlasDic[alphaTexuteName] = new AtlasItem(alphaTexuteName, new ImageItem(image, channel));
        }

        return true;
    }

    public void PreLoadAlphaTexture(string textureName)
    {
        if (string.IsNullOrEmpty(textureName))
        {
            return;
        }

        string alphaTexuteName = "";
        if (UIAtlasInfo.textureMap.TryGetValue(textureName, out alphaTexuteName))
        {

        }
        else
        {
            return;
        }


        AtlasItem _atlasItem;

        if (atlasDic.TryGetValue(alphaTexuteName, out _atlasItem))
        {

        }
        else
        {
            atlasDic[alphaTexuteName] = new AtlasItem(alphaTexuteName);
        }

    }

    public void OnEnterScene()
    {
        foreach (KeyValuePair<string, AtlasItem> keyValue in atlasDic)
        {
            keyValue.Value.ClearMaterialRef();
        }
        atlasDic.Clear();
    }

}
