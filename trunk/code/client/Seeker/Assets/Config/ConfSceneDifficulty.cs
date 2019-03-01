using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from scenedifficulty.xls
	/// </summary>
	public  class  ConfSceneDifficulty
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfSceneDifficulty>  cacheArray = new List<ConfSceneDifficulty>();
		
		public static List<ConfSceneDifficulty> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfSceneDifficulty()
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
			public readonly int[] sceneWeights;
			public readonly long[] sceneIds;
			public readonly int[] rewardNums;
			public readonly long[] rewards;
			public readonly int difficulty;
			public readonly int count;
			public readonly string descs;
			public readonly string name;

		public ConfSceneDifficulty(  		long id,
 		int[] sceneWeights,
 		long[] sceneIds,
 		int[] rewardNums,
 		long[] rewards,
 		int difficulty,
 		int count,
 		string descs,
 		string name){
			 this.id = id;
			 this.sceneWeights = sceneWeights;
			 this.sceneIds = sceneIds;
			 this.rewardNums = rewardNums;
			 this.rewards = rewards;
			 this.difficulty = difficulty;
			 this.count = count;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfSceneDifficulty> dic = new Dictionary<long, ConfSceneDifficulty>();
		
		public static bool GetConfig( long id, out ConfSceneDifficulty config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_SceneDifficulty", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfSceneDifficulty Get(long id)
        {
			ConfSceneDifficulty config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfSceneDifficulty config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SceneDifficulty", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfSceneDifficulty> list)
        {
            list = new List<ConfSceneDifficulty>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SceneDifficulty", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfSceneDifficulty config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfSceneDifficulty> list)
        {
            list = new List<ConfSceneDifficulty>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_SceneDifficulty", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfSceneDifficulty config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneDifficulty condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfSceneDifficulty GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							int[] sceneWeights = (int[])reader.GetArrayData(1, 11);
							long[] sceneIds = (long[])reader.GetArrayData(2, 17);
							int[] rewardNums = (int[])reader.GetArrayData(3, 11);
							long[] rewards = (long[])reader.GetArrayData(4, 17);
								int difficulty = reader.GetInt32(5);
								int count = reader.GetInt32(6);
								string descs = reader.GetString(7);
								string name = reader.GetString(8);
		
				ConfSceneDifficulty	new_obj_ConfSceneDifficulty = new ConfSceneDifficulty( 		 id,
 		 sceneWeights,
 		 sceneIds,
 		 rewardNums,
 		 rewards,
 		 difficulty,
 		 count,
 		 descs,
			name
			);
		
                 return new_obj_ConfSceneDifficulty;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_SceneDifficulty");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfSceneDifficulty _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}