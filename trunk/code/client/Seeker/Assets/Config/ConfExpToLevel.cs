using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from exptolevel.xls
	/// </summary>
	public  class  ConfExpToLevel
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfExpToLevel>  cacheArray = new List<ConfExpToLevel>();
		
		public static List<ConfExpToLevel> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfExpToLevel()
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

		public ConfExpToLevel(  		long id,
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
			
		private static Dictionary<long, ConfExpToLevel> dic = new Dictionary<long, ConfExpToLevel>();
		
		public static bool GetConfig( long id, out ConfExpToLevel config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ExpToLevel", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfExpToLevel Get(long id)
        {
			ConfExpToLevel config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfExpToLevel config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ExpToLevel", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfExpToLevel> list)
        {
            list = new List<ConfExpToLevel>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ExpToLevel", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfExpToLevel config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfExpToLevel> list)
        {
            list = new List<ConfExpToLevel>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ExpToLevel", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfExpToLevel config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ExpToLevel condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfExpToLevel GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long building = reader.GetInt64(1);
							int[] rewardNums = (int[])reader.GetArrayData(2, 11);
							long[] rewardIds = (long[])reader.GetArrayData(3, 17);
								int exp = reader.GetInt32(4);
								int level = reader.GetInt32(5);
		
				ConfExpToLevel	new_obj_ConfExpToLevel = new ConfExpToLevel( 		 id,
 		 building,
 		 rewardNums,
 		 rewardIds,
 		 exp,
			level
			);
		
                 return new_obj_ConfExpToLevel;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ExpToLevel");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfExpToLevel _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}