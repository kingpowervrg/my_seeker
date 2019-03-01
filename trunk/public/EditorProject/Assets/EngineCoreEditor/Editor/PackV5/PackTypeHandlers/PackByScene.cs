/********************************************************************
	created:  2018-3-28 18:43:21
	filename: PackByScene.cs
	author:	  songguangze@outlook.com
	
	purpose:  按场景分包
*********************************************************************/
using GOEditor;
using System.Collections.Generic;
using System.IO;

namespace EngineCore.Editor
{
    [PackType("Scene")]
    public class PackByScene : PackHandlerBase
    {
        public PackByScene(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting)
        {
            //只处理.unity的文件
            packBundleSetting.SearchFilters = "*.unity";
        }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();

            List<string> packDirectories = GetPackDirectories();

            foreach (string directory in packDirectories)
            {
                List<string> directoryFiles = GetDirectoryFileListWithSearchOption(directory);

                for (int i = 0; i < directoryFiles.Count; ++i)
                {
                    string fileWithoutExtension = Path.GetFileNameWithoutExtension(directoryFiles[i]);

                    //直接用场景名做为Bundle名
                    string bundleName = fileWithoutExtension + AssetBundle_PostFix;

                    bundlemapDict.Add(bundleName, new List<string>() { directoryFiles[i] });
                }
            }

            return bundlemapDict;
        }


    }
}