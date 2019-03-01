using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utils
{
    public struct PackedBundleInfo
    {
        public string name;
        public string hash;
        public string[] dependency;
        public string[] assets;
    }
    public class ManifestParseHelper
    {
        private static List<PackedBundleInfo> sInfoList;
        /// <summary>
        /// 初始化读取
        /// </summary>
        /// <param name="folder"></param>
        public static void ParseManifest(string folder)
        {
            if (sInfoList != null)
                return;

            sInfoList = new List<PackedBundleInfo>();
            ManifestDoc allDoc = new ManifestDoc(folder + "/bundlemap.bundle.manifest");

            ManifestTreeNode node = allDoc.root["AssetBundleManifest"]["AssetBundleInfos"];

            List<ManifestTreeNode> childList = node.Children;
            List<string> valueList = new List<string>();
            for (int i = 0; i < childList.Count; i++)
            {
                ManifestDoc singleDoc = new ManifestDoc(folder + "/" + childList[i]["Name"].nodeValue + ".manifest");
                PackedBundleInfo info = new PackedBundleInfo();

                info.name = childList[i]["Name"].nodeValue;
                info.hash = singleDoc.root["Hashes"]["AssetFileHash"]["Hash"].nodeValue;


                List<ManifestTreeNode> assetList = singleDoc.root["Assets"].Children;
                valueList.Clear();
                for (int j = 0; j < assetList.Count; j++)
                    valueList.Add(assetList[j].nodeKey);
                info.assets = valueList.ToArray();


                List<ManifestTreeNode> depList = childList[i]["Dependencies"].Children;
                valueList.Clear();
                for (int j = 0; j < depList.Count; j++)
                    valueList.Add(depList[j].nodeValue);

                info.dependency = valueList.ToArray();

                sInfoList.Add(info);
            }


        }
        /// <summary>
        /// 获取所有Bundle文件名列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllBundleNames()
        {
            if (sInfoList == null)
                throw new Exception("ManifestParseHelper 未初始化");

            List<string> list = new List<string>();
            for (int i = 0; i < sInfoList.Count; i++)
            {
                list.Add(sInfoList[i].name);
            }
            return list;
        }
        /// <summary>
        /// 获取单个信息PackedBundleInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PackedBundleInfo GetPackedBundleInfo(string name)
        {
            if (sInfoList == null)
                throw new Exception("ManifestParseHelper 未初始化");

            for (int i = 0; i < sInfoList.Count; i++)
            {
                if (sInfoList[i].name.Equals(name))
                    return sInfoList[i];
            }
            return new PackedBundleInfo();
        }
    }

}
