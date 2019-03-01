/********************************************************************
	created: 2018-9-23 16:09:47
	filename:BundleMapInfo.cs
	author:	 songguangze@outlook.com
	
	purpose: BundleMap信息
*********************************************************************/
using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("tb_bundleMapInfo")]
public class BundleMapInfo
{
    [PrimaryKey]
    public int BundleMapInfoKey { get; set; }

    [Indexed(Unique = false)]
    public string BundleMapInfoValue { get; set; }
}
