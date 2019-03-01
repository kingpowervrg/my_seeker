using System;
using UnityEngine;


namespace PostFX
{
    [Serializable]
    public class SpiritPressure : PostEffectBase
    {
        public LayerMask layer = 1 << 8;
        public Texture lineTex;
        public int rowCount = 4;
        public int columCount = 4;
        public float speed = 20;
        //public Vector4 lineTexTilling = new Vector4(1, 1, 0, 0);

        public Vector4 shakeFactor = new Vector4(0.01f, 20, 0.0125f, 20);


        private RenderTexture target;

        public SpiritPressure()
        {
            et = EffectType.SpiritPressure;
        }

        protected override void CreateMaterial()
        {
            if (base.material == null)
            {
                base.shader = GetShader("Hidden/SpiritPressure");
                if (base.shader != null)
                {
                    base.material = new Material(base.shader);
                }
            }
        }


        private GameObject FindOrNewSetParent(Transform tr, string name, out bool isExsit)
        {
            Transform t = tr.Find(name);

            GameObject obj;
            if (t == null)
            {
                obj = new GameObject(name);
                isExsit = false;
            }
            else
            {
                obj = t.gameObject;
                isExsit = true;
            }
            obj.transform.SetParent(tr);
            return obj;
        }
        private Camera cam;


        private void CameraSettings()
        {
            bool isExsit;
            GameObject go = FindOrNewSetParent(PostEffectBehaviour.newCamera.transform, "camera", out isExsit);
            target = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

            if (!isExsit)
            {
                go.transform.localPosition = Vector3.zero;
                cam = go.AddComponent<Camera>();
            }
            else
            {
                cam = go.GetComponent<Camera>();
            }

            cam.CopyFrom(PostEffectBehaviour.newCamera);
            cam.cullingMask = layer.value;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.depth = -99;
            cam.targetTexture = target;
            cam.backgroundColor = new Color(0, 0, 0, 0);

        }

        public override void PreProcess(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
            {
                CreateMaterial();
            }
            if (material != null)// && mGlowEnable)
            {
                material.SetTexture("_Flags", target);

                material.SetInt("_RowCount", rowCount);
                material.SetInt("_ColumCount", columCount);
                material.SetFloat("_Speed", speed);
                material.SetTexture("_LineTex", lineTex);
                material.SetVector("_ShakeFactor", shakeFactor);
                //material.SetTextureScale("_LineTex", new Vector2(lineTexTilling.x, lineTexTilling.y));
                //material.SetTextureOffset("_LineTex", new Vector2(lineTexTilling.z, lineTexTilling.w));

                Graphics.Blit(source, destination, material);
            }
        }
        public override void ToParam(object[] o)
        {
            if (o[0] != null)
            {
                layer = (LayerMask)(o[0]);
            }
            if (o[1] != null)
            {
                lineTex = (Texture)(o[1]);
            }
            if (o[3] != null)
            {
                rowCount = (int)o[3];
            }
            if (o[4] != null)
            {
                columCount = (int)o[4];
            }
            if (o[5] != null)
            {
                speed = (float)o[5];
            }
            if (o[6] != null)
            {
                shakeFactor = (Vector4)o[6];
            }
        }

        public override void OnDispose()
        {
            if (target != null)
            {
                target.Release();
                target = null;
            }

        }
    }
}