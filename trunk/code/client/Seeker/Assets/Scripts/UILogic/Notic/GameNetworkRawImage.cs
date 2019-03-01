using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SeekerGame
{
    public class GameNetworkRawImage : GameUIComponent
    {
        private RawImage image;
        public Action<Texture> OnLoadFinish;

        GOGUI.LazyImageLoader lazyLoader;

        private float m_overTime = 30f;
        private bool m_isDispose = false;

        private float m_width, m_height;

        private string m_http_img_name = string.Empty;
        private string m_imageName = string.Empty;

        protected override void OnInit()
        {
            base.OnInit();
            image = GetComponent<RawImage>();
            m_isDispose = false;
            m_width = this.Widget.sizeDelta.x;
            m_height = this.Widget.sizeDelta.y;
            m_http_img_name = string.Empty;
            //m_coroutine = gameObject.AddComponent<CoroutineManager>();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            m_isDispose = false;
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void Dispose()
        {
            base.Dispose();
            m_isDispose = true;
        }

        public string TextureName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {

                    //m_coroutine.startCoroutine(startLoadImage(value));
                    //GameObject.start
                    if (value.StartsWithFast("http://"))
                    {
                        if (m_http_img_name == value)
                            return;

                        m_http_img_name = value;
                        //LazyLoader.enabled = false;
                        FrameMgr.Instance.StartCoroutine(startLoadImage(value));
                    }
                    else if (value.StartsWithFast("https://"))
                    {
                        if (m_http_img_name == value)
                            return;

                        m_http_img_name = value;
                        //LazyLoader.enabled = false;
                        FrameMgr.Instance.StartCoroutine(GetText(value));
                    }
                    else
                    {
                        LazyLoader.enabled = true;

                        if (LazyLoader)
                        {
                            LazyLoader.RawImageName = value;
                            this.m_imageName = value;
                        }
                        else
                            return;

                        LazyLoader.LoadRawImage();
                    }
                }
            }
            get
            {
                if (!string.IsNullOrEmpty(m_http_img_name))
                    return m_http_img_name;
                else
                    return this.m_imageName;
            }
        }

        public void SetTexture(Texture tex_)
        {
            image.texture = tex_;
        }

        public GOGUI.LazyImageLoader LazyLoader
        {
            get
            {
                if (!lazyLoader)
                    lazyLoader = GetComponent<GOGUI.LazyImageLoader>();

                return lazyLoader;
            }
        }

        private System.Collections.IEnumerator startLoadImage(string url)
        {
            float loadTime = Time.time;
            WWW www = new WWW(url);
            yield return www;
            while (www != null && !www.isDone)
            {

                if (Time.time - loadTime >= m_overTime || m_isDispose)
                {
                    www.Dispose();
                    yield break;
                }
                yield return www;
            }
            if (string.IsNullOrEmpty(www.error))
            {
                if (image != null)
                {
                    image.texture = www.texture;
                }
                if (OnLoadFinish != null)
                {
                    OnLoadFinish(image.texture);
                }
            }
            www.Dispose();
        }

        public Color Color
        {
            set
            {
                image.color = value;
            }
            get
            {
                return image.color;
            }
        }

        private System.Collections.IEnumerator GetText(string url)
        {
            float loadTime = Time.time;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                while (www != null && !www.isDone)
                {
                    //if (www.isNetworkError)
                    //{
                    //    Debug.LogError("SEEKER 2 : " + www.error);
                    //    yield break;
                    //}

                    if (Time.time - loadTime >= m_overTime || m_isDispose)
                    {
                        www.Dispose();
                        yield break;
                    }
                    yield return www;
                }

                if (www.isNetworkError)
                {
                    Debug.LogError(www.error);
                }
                else if (www.isDone)
                {
                    int width = (int)m_width;
                    int height = (int)m_height;
                    byte[] results = www.downloadHandler.data;
                    Debug.Log("SEEKER2 : DOWNLOAD COMPELETE! RESULTS SIZE = " + results.Length);
                    Texture2D texture = new Texture2D(width, height);
                    texture.LoadImage(results);
                    yield return new WaitForSeconds(0.01f);

                    if (image != null)
                    {
                        image.texture = texture;
                    }
                    if (OnLoadFinish != null)
                    {
                        OnLoadFinish(image.texture);
                    }

                    //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    //transform.GetComponent<Image>().sprite = sprite;
                    //yield return new WaitForSeconds(0.01f);
                    //Resources.UnloadUnusedAssets();
                }
            }

        }
    }
}

