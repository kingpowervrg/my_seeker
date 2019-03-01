using UnityEngine;
using UnityEngine.UI;

public class ChangeStencil : MonoBehaviour
{

    public float stencilRef;
    public float stencilComp;
    public float stencilPass;
    public bool alphaClip;
    // Use this for initialization

    Material m_mat;
    void Start()
    {
        RawImage tex = this.GetComponent<RawImage>();
        Material mat_copy = new Material(Shader.Find("UI/Default"));
        if (null == mat_copy)
            return;

        tex.material = mat_copy;
        m_mat = tex.material;
        //m_mat.SetFloat("_ColorMask", 1.0f);
    }

    void Update()
    {
        if (null != m_mat)
        {
            {
                
                m_mat.SetFloat("_Stencil", stencilRef);
                m_mat.SetFloat("_StencilComp", stencilComp);
                m_mat.SetFloat("_StencilOp", stencilPass);
                m_mat.SetFloat("_UseUIAlphaClip", alphaClip == false ? 0.0f : 1.0f);
            }
        }

    }


}
