/********************************************************************
	created:  2018-4-2 17:15:58
	filename: PackByUIPrefab.cs
	author:	  songguangze@outlook.com
	
	purpose:  UI分包
*********************************************************************/
using GOEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCore.Editor
{
    [PackType("UIPrefab")]
    public class PackByUIPrefab : PackHandlerBase
    {
        public PackByUIPrefab(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting)
        {

        }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundleDict = new Dictionary<string, List<string>>();

            List<string> directories = GetPackDirectories();

            foreach (string directory in directories)
            {
                List<string> directoryFiles = GetDirectoryFileListWithSearchOption(directory);

                for (int i = 0; i < directoryFiles.Count; ++i)
                {
                    string directoryFile = directoryFiles[i];

                    string fileName = Path.GetFileNameWithoutExtension(directoryFile);
                    string uiRealName = fileName;
                    if (fileName.StartsWith("UI_"))
                    {
                        int indexOfPrefix = fileName.IndexOf('_') + 1;
                        uiRealName = fileName.Substring(indexOfPrefix, fileName.Length - indexOfPrefix);
                    }

                    string bundleName = MakeAssetbundleName(uiRealName);
                    string generatedUIPrefabPath = GenerateUIPrefabDirectory + fileName + ".prefab";

                    //生成UIPrefab
                    GenerateUIPrefab(directoryFile, generatedUIPrefabPath);

                    bundleDict.Add(bundleName, new List<string> { generatedUIPrefabPath });
                }

            }

            return bundleDict;

        }


        /// <summary>
        /// 收集UI上的图、字体、动画等资源信息，生成新的Prefab
        /// </summary>
        /// <param name="file"></param>
        /// <param name="genPath"></param>
        private void GenerateUIPrefab(string file, string genPath)
        {
            string hash, md5File;
            if (!ShouldGenerateAsset(file, genPath, out hash, out md5File))
                return;

            Object asset = AssetDatabase.LoadMainAssetAtPath(file);
            if (!asset)
                Debug.LogError("Cannot load asset:" + file);

            GameObject obj = Object.Instantiate(asset) as GameObject;

            obj.SetActive(false);

            UnityEngine.UI.Image[] textures = obj.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            Dictionary<UnityEngine.UI.Image, string> replaceNeeded = new Dictionary<UnityEngine.UI.Image, string>();
            foreach (var i in textures)
            {
                if (i is GOGUI.AnimatedImage)
                {
                    GOGUI.AnimatedImage anim = (GOGUI.AnimatedImage)i;
                    if (anim.Sprites != null && anim.Sprites.Length > 0)
                    {
                        GameObject go = i.gameObject;
                        GOGUI.LazyImageLoader li = GetLazyLoader(go);
                        li.AnimatedImageNames = new string[anim.Sprites.Length];
                        li.AnimatedImage = anim;
                        for (int j = 0; j < anim.Sprites.Length; j++)
                        {
                            string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(anim.Sprites[j]));
                            if (path == "unity_builtin_extra")
                                continue;
                            li.AnimatedImageNames[j] = path;
                            anim.Sprites[j] = null;
                        }
                        anim.sprite = null;
                    }
                }
                else
                {
                    if (i.sprite)
                    {
                        string ap = AssetDatabase.GetAssetPath(i.sprite);
                        TextureImporter ti = AssetImporter.GetAtPath(ap) as TextureImporter;
                        bool isBigTextureSprite = false;
                        if (ti != null && string.IsNullOrEmpty(ti.spritePackingTag))
                            isBigTextureSprite = true;
                        string path = isBigTextureSprite ? System.IO.Path.GetFileNameWithoutExtension(ap) : System.IO.Path.GetFileName(ap);
                        if (path == "unity_builtin_extra")
                            continue;

                        replaceNeeded[i] = path;
                    }
                    else if (i.sprite != null)
                    {
                        i.sprite = null;
                    }
                }
            }

            SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var i in srs)
            {
                if (i.sprite)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.sprite));
                    if (path == "unity_builtin_extra")
                        continue;
                    GameObject go = i.gameObject;
                    GOGUI.LazyImageLoader li = GetLazyLoader(go);
                    li.SpriteName = path;
                    li.SpriteRenderer = i;
                    i.sprite = null;
                }
                else if (i.sprite != null)
                {
                    i.sprite = null;
                }

            }

            UnityEngine.UI.Button[] btns = obj.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (var i in btns)
            {
                if (i.spriteState.pressedSprite)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.spriteState.pressedSprite));
                    if (path == "unity_builtin_extra")
                        continue;
                    GameObject go = i.gameObject;
                    GOGUI.LazyImageLoader li = GetLazyLoader(go);
                    li.ButtonPressedName = path;
                    li.Button = i;

                    var state = i.spriteState;
                    state.pressedSprite = null;
                    state.disabledSprite = null;
                    i.spriteState = state;
                }
            }
            UnityEngine.UI.RawImage[] imgs = obj.GetComponentsInChildren<UnityEngine.UI.RawImage>(true);

            foreach (var i in imgs)
            {
                if (i.texture)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.texture));
                    if (path == "unity_builtin_extra")
                        continue;
                    GameObject go = i.gameObject;
                    GOGUI.LazyImageLoader li = GetLazyLoader(go);
                    li.RawImageName = path;
                    li.RawImage = i;
                    i.texture = null;
                }
            }

            GOGUI.AdvancedText[] texts = obj.GetComponentsInChildren<GOGUI.AdvancedText>(true);
            foreach (var i in texts)
            {
                if (i.ImageFont)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.ImageFont));
                    if (path == "unity_builtin_extra")
                        continue;
                    GameObject go = i.gameObject;
                    GOGUI.LazyImageLoader li = GetLazyLoader(go);
                    li.ImageFontName = path;
                    li.AdvancedText = i;
                    i.ImageFont = null;
                }
                if (i.font)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.font));
                    if (path == "unity default resources")
                        continue;
                    GameObject go = i.gameObject;
                    GOGUI.LazyImageLoader li = GetLazyLoader(go);
                    li.FontName = path;
                    li.Text = i;
                    i.font = null;
                }
            }

            Dictionary<UnityEngine.UI.Text, string> txtReplaceNeeded = new Dictionary<UnityEngine.UI.Text, string>();
            UnityEngine.UI.Text[] texts2 = obj.GetComponentsInChildren<UnityEngine.UI.Text>(true);
            foreach (var i in texts2)
            {
                if (i is GOGUI.AdvancedText)
                    continue;
                if (i.font)
                {
                    string path = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(i.font));
                    if (path == "unity default resources")
                        continue;

                    txtReplaceNeeded[i] = path;
                    //MakeLazyLoadText(i, path);
                }

                i.raycastTarget = false;
            }

            //Fix LazyLoadImage
            UnityEngine.UI.Toggle[] tgs = obj.GetComponentsInChildren<UnityEngine.UI.Toggle>(true);
            foreach (var i in tgs)
            {
                if (!i)
                    continue;
                string path;
                UnityEngine.UI.Image img = i.targetGraphic as UnityEngine.UI.Image;
                if (img && replaceNeeded.TryGetValue(img, out path))
                {
                    i.targetGraphic = MakeLazyLoadImage(img, path);
                }
                img = i.graphic as UnityEngine.UI.Image;
                if (img && replaceNeeded.TryGetValue(img, out path))
                {
                    i.graphic = MakeLazyLoadImage(img, path);
                }
            }

            foreach (var i in replaceNeeded)
            {
                if (i.Key)
                {
                    MakeLazyLoadImage(i.Key, i.Value);
                }
            }

            //Fix LazyLoadText
            UnityEngine.UI.InputField[] ifs = obj.GetComponentsInChildren<UnityEngine.UI.InputField>(true);
            foreach (var i in ifs)
            {
                if (!i)
                    continue;
                string path;
                UnityEngine.UI.Text img = i.textComponent as UnityEngine.UI.Text;
                if (img && txtReplaceNeeded.TryGetValue(img, out path))
                {
                    i.textComponent = MakeLazyLoadText(img, path);
                }
                img = i.placeholder as UnityEngine.UI.Text;
                if (img && txtReplaceNeeded.TryGetValue(img, out path))
                {
                    i.placeholder = MakeLazyLoadText(img, path);
                }
            }

            foreach (var i in txtReplaceNeeded)
            {
                if (i.Key)
                {
                    MakeLazyLoadText(i.Key, i.Value);
                }
            }

            Camera camera = obj.GetComponentInChildren<Camera>();
            if (camera != null)
            {
                MonoBehaviour.DestroyImmediate(camera);
            }

            AudioListener lis = obj.GetComponentInChildren<AudioListener>();
            if (lis != null)
            {
                MonoBehaviour.DestroyImmediate(lis);
            }

            ResConfig config = obj.AddComponent<ResConfig>();
            config.ReleaseOnLevelLoaded = false;

            GameObject objPrefab = UnityEditor.PrefabUtility.CreatePrefab(genPath, obj);
            //objPrefab.name = obj.name + GOEPack.m_prefabExt;

            UnityEngine.Object.DestroyImmediate(obj);

            using (System.IO.StreamWriter sw = new StreamWriter(md5File, false, System.Text.Encoding.ASCII))
            {
                sw.WriteLine(hash);
                sw.Flush();
            }
        }

        GOGUI.LazyLoadImage MakeLazyLoadImage(UnityEngine.UI.Image img, string path)
        {
            GameObject go = img.gameObject;
            Color color = img.color;
            Material mat = img.material;
            bool rt = img.raycastTarget;
            bool pa = img.preserveAspect;
            UnityEngine.UI.Image.Type type = img.type;
            bool clockwise = img.fillClockwise;
            bool fc = img.fillCenter;
            float fa = img.fillAmount;
            UnityEngine.UI.Image.FillMethod fm = img.fillMethod;
            int fo = img.fillOrigin;
            GameObject.DestroyImmediate(img);

            GOGUI.LazyLoadImage li = go.AddComponent<GOGUI.LazyLoadImage>();
            li.SpriteName = path;
            li.color = color;
            li.material = mat;
            li.raycastTarget = rt;
            li.type = type;
            li.preserveAspect = pa;
            li.fillClockwise = clockwise;
            li.fillCenter = fc;
            li.fillAmount = fa;
            li.fillMethod = fm;
            li.fillOrigin = fo;

            return li;
        }

        GOGUI.LazyLoadText MakeLazyLoadText(UnityEngine.UI.Text txt, string path)
        {
            GameObject go = txt.gameObject;
            string text = txt.text;
            FontStyle fs = txt.fontStyle;
            int size = txt.fontSize;
            float ls = txt.lineSpacing;
            bool rt = txt.supportRichText;
            TextAnchor anchor = txt.alignment;
            bool ag = txt.alignByGeometry;
            var hw = txt.horizontalOverflow;
            var vw = txt.verticalOverflow;
            bool bf = txt.resizeTextForBestFit;
            var color = txt.color;
            var mat = txt.material;
            bool ray = txt.raycastTarget;

            GameObject.DestroyImmediate(txt);

            GOGUI.LazyLoadText li = go.AddComponent<GOGUI.LazyLoadText>();
            li.text = text;
            li.FontName = path;
            li.font = null;
            li.fontStyle = fs;
            li.fontSize = size;
            li.lineSpacing = ls;
            li.supportRichText = rt;
            li.alignment = anchor;
            li.alignByGeometry = ag;
            li.horizontalOverflow = hw;
            li.verticalOverflow = vw;
            li.resizeTextForBestFit = bf;
            li.color = color;
            li.material = mat;
            li.raycastTarget = ray;
            li.resizeTextMaxSize = txt.resizeTextMaxSize;
            li.resizeTextMinSize = txt.resizeTextMinSize;

            return li;
        }

        GOGUI.LazyImageLoader GetLazyLoader(GameObject go)
        {
            GOGUI.LazyImageLoader res = go.GetComponent<GOGUI.LazyImageLoader>();
            if (!res)
                res = go.AddComponent<GOGUI.LazyImageLoader>();

            return res;
        }


        private bool ShouldGenerateAsset(string file, string genPath, out string hash, out string md5File)
        {
            string genFolder = System.IO.Path.GetDirectoryName(genPath);
            string genFileName = System.IO.Path.GetFileNameWithoutExtension(genPath);
            md5File = genFolder + "/" + genFileName + ".md5";
            using (System.IO.FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                var md5 = System.Security.Cryptography.MD5.Create();
                hash = Convert.ToBase64String(md5.ComputeHash(buffer));
            }
            if (System.IO.File.Exists(md5File))
            {
                if (System.IO.File.Exists(genPath))
                {
                    using (System.IO.StreamReader sr = new StreamReader(md5File, System.Text.Encoding.ASCII))
                    {
                        if (hash != sr.ReadLine())
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return true;
            }
            else
                return true;
        }


        /// <summary>
        /// 生成的UIPrefab目录(与当前的目录层级一致)
        /// </summary>
        private string GenerateUIPrefabDirectory
        {
            get
            {
                string generatePrefabDir = this.m_packSetting.SrcDir + "/" + "generated/";
                if (!Directory.Exists(generatePrefabDir))
                    Directory.CreateDirectory(generatePrefabDir);

                return generatePrefabDir;
            }
        }

    }
}