using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UIEffectScale : MonoBehaviour
{
    public class ScaleInfo
    {
        public ParticleSystem ps = null;
        public float mOriginalScale = 1f;
    }

    public float mSize = 1.0f;
    private float mOldSize = 1.0f;
    private ScaleInfo[] mScaleInfoArray = null;
    private bool mDirty = true;
    int renderqueue = -1;
    int oldRenderqueue = -1;
    public float Scale
    {
        get { return mSize; }
        set
        {
            mSize = value;
        }
    }

    public int RenderQueue
    {
        get { return renderqueue; }
        set
        {
            if (value != renderqueue)
            {
                renderqueue = value;
            }
        }
    }

    public bool Dirty
    {
        get { return mDirty; }
        set { mDirty = value; }
    }

    void Start()
    {
        ParticleSystem[] psArray = gameObject.GetComponentsInChildren<ParticleSystem>(true) as ParticleSystem[];

        if (psArray == null || psArray.Length <= 0)
        {
            return;
        }

        mScaleInfoArray = new ScaleInfo[psArray.Length];
        for (int i = 0; i < psArray.Length; ++i)
        {
            mScaleInfoArray[i] = new ScaleInfo();
            mScaleInfoArray[i].ps = psArray[i];
            mScaleInfoArray[i].mOriginalScale = psArray[i].startSize;
        }

        SetSacle();
    }

    // Update is called once per frame
    void Update()
    {
        if (mOldSize != mSize)
        {
            mDirty = true;
            mOldSize = mSize;
        }
        if (renderqueue != oldRenderqueue)
        {
            mDirty = true;
            oldRenderqueue = renderqueue;
        }

        if (mDirty)
        {
            mDirty = false; 
            SetSacle();
        }
    }

    void SetSacle()
    {
        if (gameObject == null || mScaleInfoArray == null)
        {
            return;
        }

        for (int i = 0; i < mScaleInfoArray.Length; ++i)
        {
            if (mScaleInfoArray[i].ps != null)
            {
                mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].mOriginalScale;
                if (mScaleInfoArray[i].ps.gameObject.activeSelf)
                {
                    if (renderqueue >= 0)
                        mScaleInfoArray[i].ps.GetComponent<Renderer>().material.renderQueue = renderqueue;
                }
                else
                    mDirty = true;

            }
        }

        for (int k = 0; k < mScaleInfoArray.Length; ++k)
        {
            ParticleSystem ps = mScaleInfoArray[k].ps;
            if (ps != null)
            {
                ps.Clear(true);
                ps.startSize = mScaleInfoArray[k].mOriginalScale * mSize;

                ps.Play(true);
            }
        }
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (gameObject == null)
        {
            return;
        }

        if (mScaleInfoArray != null)
        {
            for (int i = 0; i < mScaleInfoArray.Length; ++i)
            {
                if (mScaleInfoArray[i].ps != null)
                {
                    mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].mOriginalScale;
                }
            }
        }
    }
}
