using DG.Tweening;
using EngineCore;
using GOGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    public class Combine3DView : BaseViewComponet<CombineUILogic>
    {
        const float C_3D_CAM_ORTH_H_SIZE = 1.2f;
        float C_3D_CAM_ORTH_W_SIZE;
        const string C_3D_ROOT_NAME = "3DUIROOT.prefab";
        Vector3 C_ANCHOR_POS;
        GameTexture m_show_tex;
        GameObject m_3d_root;

        Camera m_3d_cam;
        RenderTexture m_render;
        Transform m_3d_obj_anchors;
        Transform m_3d_obj_pos;

        //string test_obj_name = "FaYuanWai_01_MoTuoChe_01.prefab";
        GameObject m_3d_obj;
        string m_obj_name = null;
        protected override void OnInit()
        {
            base.OnInit();
            m_show_tex = Make<GameTexture>("RawImage");
            if (null == m_render)
            {
                //m_render = RenderTexture.GetTemporary( (int)m_show_tex.Widget.sizeDelta.x, (int)m_show_tex.Widget.sizeDelta.y, 24);
                m_render = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            }

            if (null == m_3d_root)
            {
                EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(C_3D_ROOT_NAME, (assetName, assetObject) =>
              {
                  m_3d_root = (GameObject)assetObject;
                  var root_parent = GameObject.Find("GameClient");
                  m_3d_root.transform.parent = root_parent.transform;
                  m_3d_cam = m_3d_root.transform.Find("3DCamera").GetComponent<Camera>();
                  m_3d_cam.enabled = true;
                  m_3d_cam.targetTexture = m_render;
                  m_show_tex.RawImage.texture = m_render;
                  m_3d_obj_anchors = m_3d_root.transform.Find("Anchor");
                  C_ANCHOR_POS = m_3d_obj_anchors.transform.localPosition;
                  C_3D_CAM_ORTH_W_SIZE = C_3D_CAM_ORTH_H_SIZE * m_3d_cam.aspect;
                  m_3d_obj_pos = m_3d_root.transform.Find("Anchor/Pos1");
                  m_3d_obj_pos.localPosition = Vector3.zero;
                  if (null != m_obj_name)
                      Create3DObj(m_3d_obj_pos, m_obj_name);

              }, LoadPriority.HighPrior);
            }

            m_show_tex.RawImage.texture = m_render;
        }



        void Create3DObj(Transform parent_trf_, string obj_name_)
        {
            if (null == m_3d_root)
            {
                m_obj_name = obj_name_;
                return;
            }

            Matrix4x4 m = m_3d_cam.cameraToWorldMatrix;
            Vector3 center_w_pos = m_3d_cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Mathf.Abs(C_ANCHOR_POS.z)));

            Vector3 left_bottom_pos = m_3d_cam.ViewportToWorldPoint(new Vector3(0f, 0f, Mathf.Abs(C_ANCHOR_POS.z)));
            Vector3 right_top_pos = m_3d_cam.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, Mathf.Abs(C_ANCHOR_POS.z)));

            float cam_w = Mathf.Abs(left_bottom_pos.x - right_top_pos.x);
            float cam_h = Mathf.Abs(left_bottom_pos.y - right_top_pos.y);

            CurViewLogic().GetObjFromCache(obj_name_, (name_, prefab_) =>
           {
               Remove3DObj();

               m_3d_obj = prefab_;


               ChangeLayer(m_3d_obj.transform, "3DUI");
               m_3d_obj.transform.parent = parent_trf_;
               m_3d_obj.transform.position = center_w_pos;
               m_3d_obj.transform.localScale = Vector3.one;

               Bounds obj_render_bounds = m_3d_obj.GetComponent<MeshRenderer>().bounds;


               float w_scaler = cam_w / obj_render_bounds.extents.x;
               float h_scaler = cam_h / obj_render_bounds.extents.y;
               float z_scaler = cam_w / obj_render_bounds.extents.z; //当物体延Y轴旋转90度，那么Z的size变为了宽
               float final_scaler = Math.Min(Mathf.Min(w_scaler, h_scaler), z_scaler);
               m_3d_obj.transform.localScale *= final_scaler * 0.5f;


               var render = m_3d_obj.GetComponent<MeshRenderer>();

               if (render != null || render.material != null)
               {
                   string shaderName = "Legacy Shaders/Self-Illumin/Bumped Diffuse";
                   if (!string.IsNullOrEmpty(shaderName))
                   {
                       if ("Legacy Shaders/Self-Illumin/Bumped Diffuse" != render.material.shader.name)
                           render.material.shader = ShaderModule.Instance.GetShader(shaderName);
                   }
               }
               m_3d_obj.SetActive(true);
           }
            );
        }

        void ChangeLayer(Transform trans, string targetLayer)
        {
            if (LayerMask.NameToLayer(targetLayer) == -1)
            {
                Debug.Log("Layer中不存在,请手动添加LayerName");

                return;
            }
            //遍历更改所有子物体layer
            trans.gameObject.layer = LayerMask.NameToLayer(targetLayer);
            foreach (Transform child in trans)
            {
                ChangeLayer(child, targetLayer);
                Debug.Log(child.name + "子对象Layer更改成功！");
            }
        }


        void Remove3DObj()
        {
            m_3d_obj?.SetActive(false);
            if (null != m_3d_obj)
            {
                m_3d_obj.transform.parent = null;
                m_3d_obj = null;
            }
        }

        void Remove3DRoot()
        {
            if (null != m_3d_root)
            {
                m_3d_root.SetActive(false);
                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(C_3D_ROOT_NAME, m_3d_root);
                m_3d_root = null;
            }
        }

        void RemoveRenderTex()
        {
            if (null != m_render)
            {
                m_3d_cam.enabled = false;
                m_3d_cam.targetTexture = null;
                RenderTexture.ReleaseTemporary(m_render);
                m_render = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            Remove3DObj();

            RemoveRenderTex();

            Remove3DRoot();
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            m_show_tex.AddDragCallBack(TexDraged);

            if (null != m_3d_cam)
            {
                m_3d_cam.enabled = true;
                m_show_tex.RawImage.texture = m_render;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            m_show_tex.RemoveDragCallBack(TexDraged);

            if (null != m_3d_cam)
            {
                m_3d_cam.enabled = false;
            }

            Remove3DObj();
        }


        void TexDraged(GameObject go, Vector2 delta, Vector2 pos)
        {
            if (delta.x > 0)
            {
                TurnRound(false, delta.x);
            }
            else
            {
                TurnRound(true, Mathf.Abs(delta.x));
            }
        }


        void TurnRound(bool is_forward_, float scaler_)
        {
            scaler_ = is_forward_ ? scaler_ : -scaler_;
            m_3d_obj_pos.DOLocalRotate(m_3d_obj_pos.localRotation.eulerAngles + new Vector3(0.0f, scaler_, 0.0f), 0.1f);
            //m_3d_obj_pos.localRotation.eulerAngles = new Vector3(0.0f, scaler_, 0.0f);
        }

        public void Refresh(string prefab_name_)
        {
            Create3DObj(m_3d_obj_pos, prefab_name_);
        }


    }
}
