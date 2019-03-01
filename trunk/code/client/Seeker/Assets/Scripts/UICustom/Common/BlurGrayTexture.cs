using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using EngineCore;

namespace SeekerGame
{
    public class BlurGrayTexture : GameTexture
    {
        private Material m_blur_mat;
        private bool m_is_gray;
        private float m_cur_blur_scaler = 0.0f;
        protected override void OnInit()
        {
            base.OnInit();

            m_is_gray = false;
            m_cur_blur_scaler = 0.0f;
        }

        public void InitBlurMaterial()
        {
            Material mat = new Material(ShaderModule.Instance.GetShader("SeekerGame/ImageBlur"));
            mat.renderQueue = 3000;
            RawImage.material = mat;
            this.m_blur_mat = mat;
        }

        public override void SetGray(bool gray)
        {
            if (this.RawImage)
            {
                if (gray)
                {
                    this.RawImage.material = GrayMaterial;
                }
                else
                {
                    this.RawImage.material = this.m_blur_mat;
                }

                m_is_gray = gray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blur_scaler_ 0到5"></param>
        public void SetBlur(float blur_scaler_)
        {
            if (this.m_is_gray)
                return;

            if (Mathf.Abs(this.m_cur_blur_scaler - blur_scaler_) < 0.001f)
            {
                return;
            }

            this.m_cur_blur_scaler = blur_scaler_;

            this.RawImage.material.SetFloat("_BlurRadius", blur_scaler_);
        }

    }
}
