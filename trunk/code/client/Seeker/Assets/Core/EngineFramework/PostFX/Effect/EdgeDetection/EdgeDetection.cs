using System;
using UnityEngine;

namespace PostFX
{
    [Serializable]
    public class EdgeDetection : PostEffectBase
    {
        public enum EdgeDetectMode
        {
            TriangleDepthNormals = 0,
            RobertsCrossDepthNormals = 1,
            SobelDepth = 2,
            SobelDepthThin = 3,
            TriangleLuminance = 4,
        }


        public EdgeDetectMode mode = EdgeDetectMode.SobelDepthThin;
        public float sensitivityDepth = 1.0f;
        public float sensitivityNormals = 1.0f;
        public float lumThreshold = 0.2f;
        public float edgeExp = 1.0f;
        public float sampleDist = 1.0f;
        public float edgesOnly = 0.0f;
        public Color edgesOnlyBgColor = Color.white;

        private EdgeDetectMode oldMode = EdgeDetectMode.SobelDepthThin;

        public EdgeDetection()
        {
            et = EffectType.EdgeDetection;
        }

        protected override void CreateMaterial()
        {
            if (material == null)
            {
                shader = GetShader("Hidden/EdgeDetect");
                if (shader != null)
                {
                    material = new Material(shader);
                }

            }
        }
        public override void OnEnable()
        {
            oldMode = mode;
            SetCameraFlag();
        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
            {
                CreateMaterial();
            }
            if (material != null)// && mGlowEnable)
            {
                if (mode != oldMode)
                    SetCameraFlag();
                oldMode = mode;
                if (EarlyOutIfNotSupported(source, destination))
                {
                    return;
                }
                Vector2 sensitivity = new Vector2(sensitivityDepth, sensitivityNormals);
                material.SetVector("_Sensitivity", new Vector4(sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
                material.SetFloat("_BgFade", edgesOnly);
                material.SetFloat("_SampleDistance", sampleDist);
                material.SetVector("_BgColor", edgesOnlyBgColor);
                material.SetFloat("_Exponent", edgeExp);
                material.SetFloat("_Threshold", lumThreshold);

                Graphics.Blit(source, destination, material, (int)mode);
            }
        }


        void SetCameraFlag()
        {
            //if (mode == EdgeDetectMode.SobelDepth || mode == EdgeDetectMode.SobelDepthThin)
            //    camera.depthTextureMode |= DepthTextureMode.Depth;
            //else if (mode == EdgeDetectMode.TriangleDepthNormals || mode == EdgeDetectMode.RobertsCrossDepthNormals)
            //    camera.depthTextureMode |= DepthTextureMode.DepthNormals;
        }


        public override void ToParam(object[] o)
        {
            if (o[0] != null)
                this.mode = (EdgeDetectMode)(o[0]);

            if (o[1] != null)
                this.sensitivityDepth = Convert.ToSingle(o[1]);

            if (o[2] != null)
                sensitivityNormals = Convert.ToSingle(o[2]);
            if (o[3] != null)
                lumThreshold = Convert.ToSingle(o[3]);
            if (o[4] != null)
                edgeExp = Convert.ToSingle(o[4]);
            if (o[5] != null)
                sampleDist = Convert.ToSingle(o[5]);
            if (o[6] != null)
                edgesOnly = Convert.ToSingle(o[6]);
            if (o[7] != null)
                edgesOnlyBgColor = (Color)o[7];
        }

    }
}
