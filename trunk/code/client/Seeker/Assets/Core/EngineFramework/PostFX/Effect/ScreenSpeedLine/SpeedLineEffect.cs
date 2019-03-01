using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class SpeedLineEffect : PostEffectBase
    {
        #region Fileds

        const string ShaderName = "sf/Screen-SpeedLine";

        const string ShaderPropertyLineTex = "_LineTex";
        const string ShaderPropertyLineColor = "_LineColor";
        const string ShaderPropertyLineRow = "_Row";
        const string ShaderPropertyLineCol = "_Col";
        const string ShaderPropertyLineSpeed = "_Speed";
        const string ShaderPropertyLineCutOff = "_CutOffR";

        public Texture2D lineTex;
        public Color lineColor = Color.white;
        [Range(1, 10)]
        public float row = 1;
        [Range(1, 10)]
        public float col = 1;
        [Range(0, 100)]
        public float speed = 0;
        [Range(0.0f, 1.0f)]
        public float cutOffR = 1.0f;

        public SpeedLineEffect()
        {
            et = EffectType.ScreenSpeedLine;
        }

        #endregion

        #region Override Methods

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader(ShaderName);
                if (shader != null)
                {
                    material = new Material(shader);
                }
            }
        }

        public override void ToParam(object[] o)
        {
            if (o[0] != null)
            {
                lineTex = o[0] as Texture2D;
            }
            if (o[1] != null)
            {
                lineColor = (Color)o[1];
            }
            if (o[2] != null)
            {
                row = (float)o[2];
            }
            if (o[3] != null)
            {
                col = (float)o[3];
            }
            if (o[4] != null)
            {
                speed = (float)o[4];
            }
            if (o[5] != null)
            {
                cutOffR = (float)o[5];
            }
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material != null)
            {
                material.SetTexture(ShaderPropertyLineTex, lineTex);
                material.SetColor(ShaderPropertyLineColor, lineColor);
                material.SetFloat(ShaderPropertyLineRow, row);
                material.SetFloat(ShaderPropertyLineCol, col);
                material.SetFloat(ShaderPropertyLineSpeed, speed);
                material.SetFloat(ShaderPropertyLineCutOff, cutOffR);

                Graphics.Blit(source, destination, material);
            }
            else Graphics.Blit(source, destination);
        }

        public override void OnDispose()
        {
            base.OnDispose();
        }

        #endregion
    }
}
