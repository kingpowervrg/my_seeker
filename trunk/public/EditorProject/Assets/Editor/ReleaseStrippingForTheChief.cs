using EngineCore.Editor;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;

public class ReleaseStrippingForTheChief : IReleaseStripping
{
    public Dictionary<string, object> GetStrippingAssets()
    {
        string clientConfigDbPath = Path.Combine(PathConfig.CONFIG_DATA, "ConfData.bytes");

        SQLiteConnection dbConfData = new SQLiteConnection("Assets/Res/Config/ConfData.bytes");
        List<AssetManifestConfig> existAllAssetManifestList = dbConfData.Query<AssetManifestConfig>("SELECT * FROM conf_AssetManifest");
        dbConfData.Close();

        Dictionary<string, object> confAssetManifestDict = new Dictionary<string, object>();
        for (int i = 0; i < existAllAssetManifestList.Count; ++i)
            confAssetManifestDict.Add(existAllAssetManifestList[i].AssetBundleName, existAllAssetManifestList[i]);

        return confAssetManifestDict;
    }


    [Table("conf_AssetManifest")]
    public class AssetManifestConfig
    {
        [PrimaryKey]
        public int sn { get; set; }
        public int chapterID { get; set; }
        public string AssetBundleName { get; set; }
        public int AssetLevel { get; set; }
    }

    public enum AssetLevel
    {
        BUILDIN,            //内置资源
        REQUIRED,           //必要资源（进游戏下载）
        OPTIONAL            //可选资源
    }
}
