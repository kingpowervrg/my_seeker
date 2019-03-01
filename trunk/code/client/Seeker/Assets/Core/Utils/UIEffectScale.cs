using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
[ExecuteInEditMode]
public class UIEffectScale : MonoBehaviour
{
    public class ScaleInfo
    {
        public ParticleSystem ps = null;
        public Renderer renderer = null;
        public Material originalMaterial = null;
        public Material stencilMaterial = null;
        public float mOriginalScale = 1f;
        public Vector3 mOriginalRealScale = Vector3.one;
    }

    public float mSize = 1.0f;
    private float mOldSize = 1.0f;
    private ScaleInfo[] mScaleInfoArray = null;
    int order = -1;
    bool mDirty = true;
    int m_StencilValue;
    public bool maskable = false;
    bool m_ShouldRecalculateStencil;

    public float Scale
    {
        get { return mSize; }
        set
        {
            mSize = value;
        }
    }
    private Vector3 m_realScale;
    public Vector3 RealScale
    {
        set
        {
            m_realScale = value;
            SetSacle();
            //for (int i = 0; i < mScaleInfoArray.Length; i++)
            //{
            //    Vector3 scale = mScaleInfoArray[i].ps.transform.localScale;
            //    mScaleInfoArray[i].ps.transform.localScale = new Vector3(scale.x * value.x,scale.y*value.y,scale.z*value.z);
            //}
        }
    }

    public bool Dirty
    {
        get { return mDirty; }
        set { mDirty = value; }
    }

    public void ForceUpdate()
    {
        Update();
    }

    public bool Maskable
    {
        get
        { return maskable; }
        set
        {
            if (maskable != value)
            {
                maskable = value;
                if (value)
                    m_ShouldRecalculateStencil = true;
                mDirty = true;
            }
        }
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
            var ps = psArray[i];

            mScaleInfoArray[i].ps = ps;
            mScaleInfoArray[i].renderer = ps.GetComponent<Renderer>();
            mScaleInfoArray[i].originalMaterial = mScaleInfoArray[i].renderer.sharedMaterial;
            mScaleInfoArray[i].mOriginalScale = psArray[i].startSize;
            mScaleInfoArray[i].mOriginalRealScale = ps.transform.localScale;

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
        if (mDirty)
        {
            mDirty = false;
            m_ShouldRecalculateStencil = true;
            SetSacle();
            if (order >= 0)
                SetSortingOrder(order);
            if (mScaleInfoArray != null)
            {
                for (int i = 0; i < mScaleInfoArray.Length; ++i)
                {
                    var info = mScaleInfoArray[i];
                    SetModifiedMaterial(info);
                }
            }
        }
    }

    public void SetSortingOrder(int order)
    {
        this.order = order;
        if (mScaleInfoArray != null)
        {
            for (int k = 0; k < mScaleInfoArray.Length; ++k)
            {
                mScaleInfoArray[k].renderer.sortingOrder = order;
            }
        }
    }

    /// <summary>
    /// 设置SortingLayer
    /// </summary>
    /// <param name="sortingLayerName"></param>
    public void SetSortingLayerName(string sortingLayerName)
    {
        if (this.mScaleInfoArray != null)
        {
            for (int i = 0; i < mScaleInfoArray.Length; ++i)
                mScaleInfoArray[i].renderer.sortingLayerName = sortingLayerName;

            Dirty = true;
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
                Vector3 scale = mScaleInfoArray[i].mOriginalRealScale;
                mScaleInfoArray[i].ps.transform.localScale = new Vector3(scale.x * m_realScale.x, scale.y * m_realScale.y, scale.z);
            }
        }

        for (int k = 0; k < mScaleInfoArray.Length; ++k)
        {
            ParticleSystem ps = mScaleInfoArray[k].ps;
            if (ps != null)
            {
                ps.Clear(true);
                ps.startSize = mScaleInfoArray[k].mOriginalScale * mSize;

                //ps.Play(true);   会引起Unity底层Transform 的bug 
            }
        }
    }

    void SetModifiedMaterial(ScaleInfo info)
    {
        if (info.ps != null && info.ps.emission.enabled)
        {
            var toUse = info.renderer.sharedMaterial;

            if (m_ShouldRecalculateStencil)
            {
                if (Maskable)
                {
                    var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                    m_StencilValue = MaskUtilities.GetStencilDepth(transform, rootCanvas);
                }
                else
                    m_StencilValue = 0;
                m_ShouldRecalculateStencil = false;
            }

            // if we have a Mask component then it will
            // generate the mask material. This is an optimisation
            // it adds some coupling between components though :(
            if (m_StencilValue > 0)
            {
                var maskMat = StencilMaterial.Add(toUse, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                StencilMaterial.Remove(info.stencilMaterial);
                info.stencilMaterial = maskMat;
                toUse = info.stencilMaterial;
            }
            info.renderer.material = toUse;
        }
    }

    void OnEnable()
    {
        m_ShouldRecalculateStencil = true;
        mDirty = true;
    }

    void OnDisable()
    {
        if (gameObject == null)
        {
            return;
        }
        m_ShouldRecalculateStencil = true;
        if (mScaleInfoArray != null)
        {
            for (int i = 0; i < mScaleInfoArray.Length; ++i)
            {
                var info = mScaleInfoArray[i];
                if (info.ps != null)
                {
                    info.ps.startSize = info.mOriginalScale;
                    if (info.ps.emission.enabled)
                        StencilMaterial.Remove(info.stencilMaterial);
                    info.stencilMaterial = null;
                    info.renderer.material = info.originalMaterial;
                }
            }
        }
    }


}
