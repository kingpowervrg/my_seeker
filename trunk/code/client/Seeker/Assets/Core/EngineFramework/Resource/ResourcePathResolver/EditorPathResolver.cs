/********************************************************************
	created:  2018-12-24 0:27:13
	filename: EditorPathResolver.cs
	author:	  songguangze@outlook.com
	
	purpose:  Editor及Standalone下资源路径
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EngineCore
{
    public class EditorPathResolver : PathResolver
    {
        private readonly string EDITOR_STANDALONE_RES_DIR = "../../../bin/res";

        private PathResolver m_simulatePlayerPathResolver = null;

        public EditorPathResolver()
        {
            if (EngineDelegateCore.Editor_Simulate_Player)
            {
#if UNITY_ANDROID
                this.m_simulatePlayerPathResolver = new AndroidPathResolver();
#elif UNITY_IOS
                this.m_simulatePlayerPathResolver = new IOSPathResolver();
#endif
            }
        }

        public override string GetPath(string assetName, bool isWWW)
        {
            if (EngineDelegateCore.Editor_Simulate_Player)
            {
                if (this.m_simulatePlayerPathResolver == null)
                {
                    Debug.LogError($"can't find path resolver of target platform {Application.platform.ToString()},please set EngineDelegateCore.Editor_Simulate_Playe=false");
                    return string.Empty;
                }
                else
                    return this.m_simulatePlayerPathResolver.GetPath(assetName, isWWW);
            }
            else
            {
                string assetPath = string.Empty;

                if (EngineDelegateCore.DynamicResource)
                    assetPath = GetDynamicPath(assetName);

                if (!File.Exists(assetPath))
                    assetPath = GetBuildinAssetPath(assetName);

                if (isWWW)
                    assetPath = SetToWWWPath(assetPath);

                return assetPath;
            }
        }

        protected override string GetBuildinAssetPath(string assetName)
        {
            string buildinAssetPath = $"{PathResolver.ApplicationDataPath}/{ EDITOR_STANDALONE_RES_DIR}/{assetName}";

            return buildinAssetPath;
        }

        protected override string SetToWWWPath(string path)
        {
            return $"file:///{path}";
        }
    }
}