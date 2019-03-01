/********************************************************************
	created:  2018-3-28 16:34:23
	filename: PackByDir.cs
	author:	  songguangze@outlook.com
	
	purpose:  按文件夹分包
*********************************************************************/
using GOEditor;
using System.Collections.Generic;
using System.IO;

namespace EngineCore.Editor
{
    [PackType("Per Dir")]
    public class PackByDir : PackHandlerBase
    {
        public PackByDir(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting) { }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();

            List<string> directories = GetPackDirectories();

            foreach (string directory in directories)
            {
                string relativeDirectoryPath = EngineFileUtil.GetFileRelativePath(directory, m_packSetting.SrcDir);
                if (string.IsNullOrEmpty(relativeDirectoryPath))
                    continue;

                string assetBundleName = MakeAssetbundleName(relativeDirectoryPath);
                List<string> assetFiles = GetDirectoryFileListWithSearchOption(directory);

                bundlemapDict.Add(assetBundleName, assetFiles);
            }

            return bundlemapDict;

        }
    }
}