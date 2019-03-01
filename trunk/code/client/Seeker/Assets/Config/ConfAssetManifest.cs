using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from AssetsManifest.xls
	/// </summary>
	public  class  ConfAssetManifest
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfAssetManifest>  cacheArray = new List<ConfAssetManifest>();
		
		public static List<ConfAssetManifest> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfAssetManifest()
		{
		}

		public static void Init()
		{
			if (cacheLoaded)
			{
				GetArrrayList();
			}
            
		}
			public readonly int id;
			public readonly int chapterID;
			public readonly string AssetBundleName;
			public readonly int AssetLevel;

		public ConfAssetManifest(  		int id,
 		int chapterID,
 		string AssetBundleName,
 		int AssetLevel){
			 this.id = id;
			 this.chapterID = chapterID;
			 this.AssetBundleName = AssetBundleName;
			 this.AssetLevel = AssetLevel;
		}
			
		private static Dictionary<int, ConfAssetManifest> dic = new Dictionary<int, ConfAssetManifest>();
		
		public static bool GetConfig( int id, out ConfAssetManifest config )
		{
			if (dic.TryGetValue(id, out config))
            {
                return config != null;
            }
			if(cacheLoaded)
			{
				config = null;
				return false;
			}
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_AssetManifest", id);
            if (sqReader != null )
            {
				try
                {
					sqReader.Read();
					if(sqReader.HasRows)
						config = GetConfByDic(sqReader);
					else
					{
					    dic[id] = null;
						config = null;
						return false;
					}
					dic[id] = config;
				    return true;
                }
                catch (Exception ex)
                {
                    SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest 表找不到SN={0} 的数据\n{1}", id, ex));
                }
				config = null;
                return false;
            }
            else
            {
                config = null;
                return false;
            }
		}

		public static ConfAssetManifest Get(int id)
        {
			ConfAssetManifest config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfAssetManifest config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_AssetManifest", fieldName, fieldValue);
			if (sqReader != null )
			{
				try
				{
					sqReader.Read();
					if(sqReader.HasRows)
						config = GetConfByDic(sqReader);
					else
					{
						config = null;
						return false;
					}
					return true;
				}
				catch (Exception ex)
				{
					SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfAssetManifest> list)
        {
            list = new List<ConfAssetManifest>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_AssetManifest", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfAssetManifest config;
                    while (sqReader.Read())
                    {
                        if (sqReader.HasRows)
                        {
                            config = GetConfByDic(sqReader);
                            list.Add(config);
                        }
                        else
                        {
                            config = null;
						SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfAssetManifest> list)
        {
            list = new List<ConfAssetManifest>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_AssetManifest", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfAssetManifest config;
                    while (sqReader.Read())
                    {
                        if (sqReader.HasRows)
                        {
                            config = GetConfByDic(sqReader);
                            list.Add(config);
                        }
                        else
                        {
                            config = null;
                            SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("AssetManifest condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfAssetManifest GetConfByDic(DataTable reader)
         {
		 
								int id = reader.GetInt32(0);
								int chapterID = reader.GetInt32(1);
								string AssetBundleName = reader.GetString(2);
								int AssetLevel = reader.GetInt32(3);
		
				ConfAssetManifest	new_obj_ConfAssetManifest = new ConfAssetManifest( 		 id,
 		 chapterID,
 		 AssetBundleName,
			AssetLevel
			);
		
                 return new_obj_ConfAssetManifest;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_AssetManifest");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfAssetManifest _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}