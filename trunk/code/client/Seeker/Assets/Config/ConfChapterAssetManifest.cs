using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from chapterManifest.xls
	/// </summary>
	public  class  ConfChapterAssetManifest
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfChapterAssetManifest>  cacheArray = new List<ConfChapterAssetManifest>();
		
		public static List<ConfChapterAssetManifest> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfChapterAssetManifest()
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
			public readonly string AssetsName;

		public ConfChapterAssetManifest(  		int id,
 		int chapterID,
 		string AssetsName){
			 this.id = id;
			 this.chapterID = chapterID;
			 this.AssetsName = AssetsName;
		}
			
		private static Dictionary<int, ConfChapterAssetManifest> dic = new Dictionary<int, ConfChapterAssetManifest>();
		
		public static bool GetConfig( int id, out ConfChapterAssetManifest config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ChapterAssetManifest", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfChapterAssetManifest Get(int id)
        {
			ConfChapterAssetManifest config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfChapterAssetManifest config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ChapterAssetManifest", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfChapterAssetManifest> list)
        {
            list = new List<ConfChapterAssetManifest>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ChapterAssetManifest", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfChapterAssetManifest config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfChapterAssetManifest> list)
        {
            list = new List<ConfChapterAssetManifest>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ChapterAssetManifest", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfChapterAssetManifest config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChapterAssetManifest condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfChapterAssetManifest GetConfByDic(DataTable reader)
         {
		 
								int id = reader.GetInt32(0);
								int chapterID = reader.GetInt32(1);
								string AssetsName = reader.GetString(2);
		
				ConfChapterAssetManifest	new_obj_ConfChapterAssetManifest = new ConfChapterAssetManifest( 		 id,
 		 chapterID,
			AssetsName
			);
		
                 return new_obj_ConfChapterAssetManifest;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ChapterAssetManifest");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfChapterAssetManifest _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}