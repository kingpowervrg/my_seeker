/********************************************************************
	created:  2018-11-29 18:43:26
	filename: TransformHelper.cs
	author:	  songguangze@outlook.com
	
	purpose:  各种Transform操作的帮助函数
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore.Utility
{
    public static class TransformHelper
    {

        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parentTransform"></param>
        /// <param name="localPosition"></param>
        /// <param name="localRotation"></param>
        /// <param name="localScale"></param>
        public static void SetParent(this Transform transform, Transform parentTransform, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            transform.SetParent(parentTransform);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
        }

    }
}