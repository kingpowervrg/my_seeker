using UnityEngine;
using System;
using System.Collections;
[System.Reflection.Obfuscation(Exclude = true)]
public class ResConfig : MonoBehaviour
{

    public bool AutoDestory = false;
    public bool ReleaseOnLevelLoaded = true;
    public float LifeTime = 2.0f;
    [NonSerialized]
    public bool Paused;
    public Action OnDestroyHandler;
    public string ParticleMaterialName = string.Empty;
    public static Action<string, Action<string, UnityEngine.Object>, int> GotAssetAction;

    void Awake()
    {
        if (ParticleMaterialName != string.Empty)
        {
            gameObject.SetActive(false);
            GotAssetAction(ParticleMaterialName, onGotParticleMat, 0);
        }
    }

    private bool destoryed = false;
    void OnDestroy()
    {
        destoryed = true;
    }

    private void onGotParticleMat(string name, UnityEngine.Object obj)
    {
        if (!destoryed && obj != null && gameObject != null)
        {
            Renderer rend = (obj as GameObject).GetComponent<Renderer>();
            gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial = rend.sharedMaterial;
            gameObject.SetActive(true);
            GameObject.Destroy(obj);
        }
        GameObject.Destroy(this);
    }

    void Update()
    {
        if (this.AutoDestory)
        {
            if (!Paused)
                LifeTime -= Time.deltaTime;
        }
        else
        {
            enabled = false;
        }
        if (LifeTime < 0.0)
        {
            if (OnDestroyHandler != null)
                OnDestroyHandler();
            enabled = false;
        }
    }

    void Start()
    {
        if (LifeTime < 0)
        {
            this.enabled = false;
        }
    }
}
