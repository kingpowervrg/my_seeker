/********************************************************************
	created:  2018-12-6 17:55:47
	filename: PackByRawAsset.cs
	author:	  songguangze@outlook.com
	
	purpose:  直接使用原始资源,不打包
*********************************************************************/
using GOEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore.Editor
{
    [PackType("Raw Asset")]
    public class PackByRawAsset : PackHandlerBase
    {
        public PackByRawAsset(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting) { }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();

            List<string> directories = GetPackDirectories();

            foreach (string directory in directories)
            {
                string assetBundleName = directory;
                List<string> assetFiles = GetDirectoryFileListWithSearchOption(directory);

                bundlemapDict.Add(assetBundleName, assetFiles);
            }

            return bundlemapDict;
        }
    }
}
