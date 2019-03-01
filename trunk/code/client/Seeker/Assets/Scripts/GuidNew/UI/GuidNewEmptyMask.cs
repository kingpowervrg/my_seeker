using System;
using System.Collections.Generic;
using UnityEngine;
namespace SeekerGame.NewGuid
{
    public class GuidNewEmptyMask
    {
        private Material m_maskMat;
        //private Vector2[] m_cornPos;
        private MaskEmptyType m_emptyType;
        private MaskAnimType m_animType;

        private float m_Radius = 0f;

        public GuidNewEmptyMask(Material maskMat, MaskEmptyType emptyType,MaskAnimType animType)
        {
            this.m_maskMat = maskMat;
            //this.m_cornPos = cornPos;
            this.m_emptyType = emptyType;
            this.m_animType = animType;
            //this.m_Radius = Mathf.Max(cornPos[1].x, cornPos[1].y);
        }

        private float tempRadius = 0f;
        public void OpenEmptyMask()
        {
            Vector4[] circle = new Vector4[4];
            Vector4[] rectInfo = new Vector4[4];
            if (m_emptyType == MaskEmptyType.Circle)
            {
                this.m_maskMat.SetVectorArray("circle", new Vector4[] { circle[0] });
            }
            else if(m_emptyType == MaskEmptyType.Rect)
            {
                this.m_maskMat.SetVectorArray("rectInfo", new Vector4[] { rectInfo[0] });
            }
        }
    }
}
