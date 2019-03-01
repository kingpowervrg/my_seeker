using SQLite4Unity3d;
using System.Collections.Generic;
using System.Text;

[Table("tb_bundlemap")]
public class BundleIndexItemInfo
{
    private string[] m_bundleDependencyArray;
    private string[] m_bundleAssetsArray;
    private HashSet<string> m_bundleAssetHashSet;

    [PrimaryKey]
    public string BundleName { get; set; }
    public string BundleHash { get; set; }
    public string BundleDependency { get; set; }
    public string BundleAssets { get; set; }
    public int BundleSize { get; set; }

    [Ignore]
    public string[] BundleDependencyArray
    {
        get
        {
            if (m_bundleDependencyArray == null)
            {
                if (string.IsNullOrEmpty(BundleDependency))
                    m_bundleDependencyArray = new string[0];
                else
                    m_bundleDependencyArray = BundleDependency.Split(',');
            }

            return m_bundleDependencyArray;
        }
        set
        {
            StringBuilder dependencySerialize = new StringBuilder();
            for (int i = 0; i < value.Length; ++i)
                dependencySerialize.AppendFormat("{0},", value[i]);

            if (dependencySerialize.Length > 0)
                dependencySerialize.Remove(dependencySerialize.Length - 1, 1);

            this.BundleDependency = dependencySerialize.ToString();
        }
    }

    [Ignore]
    public string[] BundleAssetsArray
    {
        get
        {
            if (m_bundleAssetsArray == null)
            {
                if (string.IsNullOrEmpty(BundleAssets))
                    m_bundleAssetsArray = new string[0];
                else
                    m_bundleAssetsArray = BundleAssets.Split(',');
            }

            return m_bundleAssetsArray;
        }
        set
        {
            StringBuilder assetsSerialize = new StringBuilder();
            for (int i = 0; i < value.Length; ++i)
                assetsSerialize.AppendFormat("{0},", value[i]);

            if (assetsSerialize.Length > 0)
                assetsSerialize.Remove(assetsSerialize.Length - 1, 1);

            this.BundleAssets = assetsSerialize.ToString();
        }
    }

    [Ignore]
    public string BundleHashName
    {
        get { return $"{this.BundleHash}.bundle"; }
    }

    /// <summary>
    /// Bundle中是否包含指定资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public bool IsContainAsset(string assetName)
    {
        if (this.m_bundleAssetHashSet == null)
            this.m_bundleAssetHashSet = new HashSet<string>(BundleAssetsArray);

        return this.m_bundleAssetHashSet.Contains(assetName);
    }
}
