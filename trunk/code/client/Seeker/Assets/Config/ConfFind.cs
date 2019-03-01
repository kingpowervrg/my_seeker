using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from find.xls
	/// </summary>
	public  class  ConfFind
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfFind>  cacheArray = new List<ConfFind>();
		
		public static List<ConfFind> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfFind()
		{
		}

		public static void Init()
		{
			if (cacheLoaded)
			{
				GetArrrayList();
			}
            
		}
			public readonly long id;
			public readonly long[] finds3;
			public readonly int findtype3;
			public readonly long[] finds2;
			public readonly int findtype2;
			public readonly long[] finds1;
			public readonly int findtype1;
			public readonly int difficulty;
			public readonly int time;
			public readonly int vit;
			public readonly int type;
			public readonly string sceneId;
			public readonly string icon;
			public readonly string breviary;
			public readonly string descs;
			public readonly string name;

		public ConfFind(  		long id,
 		long[] finds3,
 		int findtype3,
 		long[] finds2,
 		int findtype2,
 		long[] finds1,
 		int findtype1,
 		int difficulty,
 		int time,
 		int vit,
 		int type,
 		string sceneId,
 		string icon,
 		string breviary,
 		string descs,
 		string name){
			 this.id = id;
			 this.finds3 = finds3;
			 this.findtype3 = findtype3;
			 this.finds2 = finds2;
			 this.findtype2 = findtype2;
			 this.finds1 = finds1;
			 this.findtype1 = findtype1;
			 this.difficulty = difficulty;
			 this.time = time;
			 this.vit = vit;
			 this.type = type;
			 this.sceneId = sceneId;
			 this.icon = icon;
			 this.breviary = breviary;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfFind> dic = new Dictionary<long, ConfFind>();
		
		public static bool GetConfig( long id, out ConfFind config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Find", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Find 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfFind Get(long id)
        {
			ConfFind config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfFind config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Find", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Find 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfFind> list)
        {
            list = new List<ConfFind>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Find", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfFind config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Find not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Find key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfFind> list)
        {
            list = new List<ConfFind>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Find", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfFind config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Find not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Find condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfFind GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							long[] finds3 = (long[])reader.GetArrayData(1, 17);
								int findtype3 = reader.GetInt32(2);
							long[] finds2 = (long[])reader.GetArrayData(3, 17);
								int findtype2 = reader.GetInt32(4);
							long[] finds1 = (long[])reader.GetArrayData(5, 17);
								int findtype1 = reader.GetInt32(6);
								int difficulty = reader.GetInt32(7);
								int time = reader.GetInt32(8);
								int vit = reader.GetInt32(9);
								int type = reader.GetInt32(10);
								string sceneId = reader.GetString(11);
								string icon = reader.GetString(12);
								string breviary = reader.GetString(13);
								string descs = reader.GetString(14);
								string name = reader.GetString(15);
		
				ConfFind	new_obj_ConfFind = new ConfFind( 		 id,
 		 finds3,
 		 findtype3,
 		 finds2,
 		 findtype2,
 		 finds1,
 		 findtype1,
 		 difficulty,
 		 time,
 		 vit,
 		 type,
 		 sceneId,
 		 icon,
 		 breviary,
 		 descs,
			name
			);
		
                 return new_obj_ConfFind;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Find");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfFind _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}