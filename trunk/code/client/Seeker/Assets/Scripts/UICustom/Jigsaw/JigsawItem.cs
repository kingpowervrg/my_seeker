using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SeekerGame
{
    public class JigsawItem : GameUIComponent
    {
        public List<GameUIComponent> m_child_logic_rect_list;
        public List<DragDropMoveParentTexture> m_child_icon_list;

        private Material m_shine_mat;
        private Material m_shadow_mat;
        private Material m_normal_mat;
        protected override void OnInit()
        {
            m_child_logic_rect_list = new List<GameUIComponent>();
            m_child_icon_list = new List<DragDropMoveParentTexture>();

            GameUIComponent rect = this.Make<GameUIComponent>("Rect");
            m_child_logic_rect_list.Add(rect);

            DragDropMoveParentTexture tex = this.Make<DragDropMoveParentTexture>("RawImage");
            m_child_icon_list.Add(tex);

            m_shine_mat = new Material(ShaderModule.Instance.GetShader("Seeker/SpriteSilhouette"));
            m_shine_mat.SetFloat("_SilhouetteWidth", 3.0f);
            m_shine_mat.SetColor("_SilhouetteColor", Color.green);

            m_shadow_mat = new Material(ShaderModule.Instance.GetShader("Seeker/SpriteWithShadow"));
            m_shadow_mat.SetFloat("_TrxX", 20.0f);
            m_shadow_mat.SetFloat("_TrxY", -20.0f);
            m_shadow_mat.SetFloat("_ShadowAlpha", 0.5f);
            m_shadow_mat.EnableKeyword("UNITY_UI_ALPHACLIP");

            m_normal_mat = new Material(ShaderModule.Instance.GetShader("Seeker/UI-Default-StencilGE"));

            //Shine(false);
        }

        public void RenameRect(string name_)
        {
            m_child_logic_rect_list[0].gameObject.name = name_;
        }

        public void SetTex(string tex_name_, Rect rect_)
        {
            m_child_icon_list[0].InitTexture(tex_name_, rect_);
        }

        public void Shine(bool b_)
        {
            if (b_)
            {
                foreach (var icon in m_child_icon_list)
                {
                    icon.Texture.SpriteMaterial = this.m_shine_mat;
                    icon.Texture.SpriteMaterial.EnableKeyword("ENABLESILHOUETTE_ON");
                }
            }
            else
            {
                foreach (var icon in m_child_icon_list)
                {
                    icon.Texture.SpriteMaterial = this.m_normal_mat;
                }
            }
        }

        public void Shadow(bool b_)
        {
            if (b_)
            {
                foreach (var icon in m_child_icon_list)
                {
                    icon.Texture.SpriteMaterial = this.m_shadow_mat;
                }
            }
            else
            {
                foreach (var icon in m_child_icon_list)
                {
                    icon.Texture.SpriteMaterial = this.m_normal_mat;
                }
                GameEvents.UI_Guid_Event.OnJigsawEnd.SafeInvoke(0);
            }
        }
    }
}
