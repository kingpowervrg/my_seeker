using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from etl.xls
	/// </summary>
	public  class  Confetl
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<Confetl>  cacheArray = new List<Confetl>();
		
		public static List<Confetl> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public Confetl()
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
			public readonly long building;
			public readonly int[] rewardNums;
			public readonly long[] rewardIds;
			public readonly int exp;
			public readonly int level;

		public Confetl(  		long id,
 		long building,
 		int[] rewardNums,
 		long[] rewardIds,
 		int exp,
 		int level){
			 this.id = id;
			 this.building = building;
			 this.rewardNums = rewardNums;
			 this.rewardIds = rewardIds;
			 this.exp = exp;
			 this.level = level;
		}
			
		private static Dictionary<long, Confetl> dic = new Dictionary<long, Confetl>();
		
		public static bool GetConfig( long id, out Confetl config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_etl", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("etl 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static Confetl Get(long id)
        {
			Confetl config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out Confetl config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_etl", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("etl 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<Confetl> list)
        {
            list = new List<Confetl>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_etl", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    Confetl config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("etl not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("etl key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<Confetl> list)
        {
            list = new List<Confetl>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_etl", condition);
            if (sqReader != null)
            {
                try
                {
                    Confetl config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("etl not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("etl condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static Confetl GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long building = reader.GetInt64(1);
							int[] rewardNums = (int[])reader.GetArrayData(2, 11);
							long[] rewardIds = (long[])reader.GetArrayData(3, 17);
								int exp = reader.GetInt32(4);
								int level = reader.GetInt32(5);
		
				Confetl	new_obj_Confetl = new Confetl( 		 id,
 		 building,
 		 rewardNums,
 		 rewardIds,
 		 exp,
			level
			);
		
                 return new_obj_Confetl;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_etl");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						Confetl _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}