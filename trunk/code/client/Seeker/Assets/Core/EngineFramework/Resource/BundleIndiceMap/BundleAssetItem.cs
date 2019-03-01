using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("tb_assetsIndex")]
public class BundleAssetItem
{
    [PrimaryKey]
    public string AssetName { get; set; }

    [Indexed(Unique = false)]
    public string AssetBelongsBundle { get; set; }
}
