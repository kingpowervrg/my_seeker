/********************************************************************
	created:  2018-4-23 17:12:55
	filename: OneTimeBrush.cs
	author:	  songguangze@outlook.com
	
	purpose:  一次性Brush
*********************************************************************/
using EngineCore;
using UnityEngine;

namespace SeekerGame.Rendering
{
    public class OneTimeBrush : PaintBrush
    {
        public static GameObject Torch_GameObject = null;

        public OneTimeBrush() : base(BrushType.ONETIME_BRUSH)
        {

        }

        public override void InitBrush(Vector3 brushPosition)
        {
            CreateBrush();

            this.m_brushName = "OneTimeBrush";
            this.m_brushObject = Torch_GameObject;
            this.m_brushObject.name = this.m_brushName;
            this.m_brushObject.transform.position = brushPosition;
            this.m_brushObject.transform.localScale = new Vector3(BrushSize, BrushSize, BrushSize);
            GameObjectUtil.SetLayer(m_brushObject, LayerDefine.SceneDrawingBoardLayer, true);
            this.m_brushObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 180));
            Texture2D brushTex = Resources.Load<Texture2D>("mats/torchmask");

            MeshRenderer brushRender = this.m_brushObject.GetComponent<MeshRenderer>();


            this.m_brushMaterial = new Material(ShaderModule.Instance.GetShader("SeekGame/PainterBrushTransparency"));
            this.m_brushMaterial.SetTexture("_MainTex", brushTex);
            brushRender.material = this.m_brushMaterial;
            this.m_brushMaterial.SetFloat("_FadeoutFactor", 1);
        }


        private void CreateBrush()
        {
            if (Torch_GameObject == null)
            {
                Torch_GameObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(GameConst.DARK_SCENE_BRUSH_RES_PATH));
                Torch_GameObject.SetActive(false);
            }
        }

        //public override void Tick()
        //{
        //    if (!IsPause)
        //    {
        //        if (this.m_brushStatus == BrushStatus.FADING)
        //        {
        //            this.m_fadeFactor = MathUtil.Remap01(Time.time, this.BrushFadeStartTime, this.BrushFadeEndTime);
        //            if (this.m_fadeFactor < 0 || this.m_fadeFactor > 1)
        //                ForceEnd();
        //            else
        //                this.m_brushMaterial.SetFloat("_FadeoutFactor", 1 - this.m_fadeFactor);
        //        }
        //        else if (this.m_brushStatus == BrushStatus.PAINTING)
        //            this.m_brushMaterial.SetFloat("_FadeoutFactor", 1);
        //    }
        //}

        public override void ForceBeginFade(float beginFadeTime)
        {
            this.BrushFadeStartTime = beginFadeTime;
            this.BrushFadeEndTime = BrushFadeStartTime + BrushFadeTime;
            this.m_brushStatus = BrushStatus.FADING;
        }

        public override void ForceEnd()
        {
            this.m_brushStatus = BrushStatus.FADEEND;
            IsActive = false;
        }
    }
}