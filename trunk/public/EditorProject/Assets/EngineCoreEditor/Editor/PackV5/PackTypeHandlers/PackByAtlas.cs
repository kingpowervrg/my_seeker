/********************************************************************
	created:  2018-4-2 13:45:57
	filename: PackByAtlas.cs
	author:	  songguangze@outlook.com
	
	purpose:  按Atlas分包
*********************************************************************/
using GOEditor;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace EngineCore.Editor
{
    [PackType("Per Atlas")]
    public class PackByAtlas : PackHandlerBase
    {
        public PackByAtlas(GOEPackV5 packSetting, PackBundleSetting packBundleSetting) : base(packSetting, packBundleSetting)
        {

        }

        public override Dictionary<string, List<string>> GetAssetsDictByPackSetting()
        {
            Dictionary<string, List<string>> bundlemapDict = new Dictionary<string, List<string>>();
            List<string> directories = GetPackDirectories();
            StringBuilder sbErrorMsg = new StringBuilder();

            foreach (string directory in directories)
            {
                List<string> directoryFiles = GetDirectoryFileListWithSearchOption(directory);

                for (int i = 0; i < directoryFiles.Count; ++i)
                {
                    string spritePath = directoryFiles[i];
                    TextureImporter textureImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;

                    if (textureImporter == null)
                        sbErrorMsg.AppendLine(string.Format("文件:{0} 不是一个有效的图片", spritePath));

                    if (textureImporter.textureType != TextureImporterType.Sprite)
                        sbErrorMsg.AppendLine(string.Format("必须将文件:{0} \n设置为Sprite才能按照图集打包", spritePath));

                    if (string.IsNullOrEmpty(textureImporter.spritePackingTag))
                        sbErrorMsg.AppendLine(string.Format("必须给文件:{0} \n设置图集名才能按照图集打包", spritePath));

                    ProcessTexture(textureImporter);

                    string bundleName = MakeAssetbundleName(textureImporter.spritePackingTag);
                    if (!bundlemapDict.ContainsKey(bundleName))
                        bundlemapDict.Add(bundleName, new List<string>());

                    bundlemapDict[bundleName].Add(spritePath);
                }
            }

            if (sbErrorMsg.Length != 0)
                throw new System.Exception(sbErrorMsg.ToString());

            return bundlemapDict;

        }

        /// <summary>
        /// 处理Sprite
        /// </summary>
        /// <param name="importer"></param>
        private void ProcessTexture(TextureImporter importer)
        {

        }
    }
}