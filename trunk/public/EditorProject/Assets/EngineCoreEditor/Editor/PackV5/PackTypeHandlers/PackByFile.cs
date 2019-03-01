/********************************************************************
	created:  2018-3-28 18:26:43
	filename: PackByDir.cs
	author:	  songguangze@outlook.com
	
	purpose:  按文件分包
*********************************************************************/
using GOEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EngineCore.Editor
{
    [PackType("Per File")]
    public class PackByFile : PackHandlerBase
    {
        public PackByFile(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting) { }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();

            List<string> packDirectories = GetPackDirectories();

            foreach (string directory in packDirectories)
            {
                List<string> directoryFiles = GetDirectoryFileListWithSearchOption(directory);

                for (int i = 0; i < directoryFiles.Count; ++i)
                {
                    string fileRelativePath = EngineFileUtil.GetFileRelativePath(directoryFiles[i], m_packSetting.SrcDir);
                    if (string.IsNullOrEmpty(fileRelativePath))
                        continue;

                    string fileWithoutExtension = Path.GetFileNameWithoutExtension(directoryFiles[i]);
                    string customBundleName = fileRelativePath + "_" + fileWithoutExtension;
                    string bundleName = fileRelativePath == "./" ? MakeAssetbundleName(fileWithoutExtension) : MakeAssetbundleName(customBundleName);

                    if (bundlemapDict.ContainsKey(bundleName))
                        Debug.LogErrorFormat("bundle  {0} is exist", bundleName);

                    bundlemapDict.Add(bundleName, new List<string>() { directoryFiles[i] });
                }
            }

            return bundlemapDict;

        }
    }
}