/********************************************************************
	created:  2018-3-28 19:29:52
	filename: PackByOneBundle.cs
	author:	  songguangze@outlook.com
	
	purpose:  所有文件打包到同一Bundle中
*********************************************************************/
using GOEditor;
using System.Collections.Generic;

namespace EngineCore.Editor
{
    [PackType("OneBunlde")]
    public class PackByOneBundle : PackHandlerBase
    {
        public PackByOneBundle(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting)
        {
            if (string.IsNullOrEmpty(packBundleSetting.BundleName))
                throw new System.Exception("打包到同一Bundle必须在打包选项中指定Bundlename   " + packSetting.SrcDir);
        }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();

            List<string> packDirectories = GetPackDirectories();

            List<string> fileList = new List<string>();

            foreach (string directory in packDirectories)
            {
                List<string> directoryFiles = GetDirectoryFileListWithSearchOption(directory);
                fileList.AddRange(directoryFiles);
            }

            string bundleFullName = m_packBundleSetting.BundleName + AssetBundle_PostFix;

            bundlemapDict.Add(bundleFullName, fileList);

            return bundlemapDict;
        }
    }
}