/********************************************************************
	created:  2018-12-24 11:0:3
	filename: AndroidPathResolver.cs
	author:	  songguangze@outlook.com
	
	purpose:  Android下资源路径
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EngineCore
{
    public class AndroidPathResolver : PathResolver
    {
        public override string GetPath(string assetName, bool isWWW)
        {
            string assetPath = string.Empty;

            if (EngineDelegateCore.DynamicResource)
                assetPath = GetDynamicPath(assetName);

            if (!File.Exists(assetPath))
                assetPath = GetBuildinAssetPath(assetName);

            return assetPath;
        }

        protected override string GetBuildinAssetPath(string assetName)
        {
            return $"{ApplicationStreamingAssetsPath}/{assetName}";
        }

        protected override string SetToWWWPath(string path)
        {
            return path;
        }
    }
}