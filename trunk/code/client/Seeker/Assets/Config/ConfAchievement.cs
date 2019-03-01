using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from achievement.xls
	/// </summary>
	public  class  ConfAchievement
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfAchievement>  cacheArray = new List<ConfAchievement>();
		
		public static List<ConfAchievement> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfAchievement()
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
			public readonly string nobility;
			public readonly string rewardicon3;
			public readonly int cash3;
			public readonly int reward3;
			public readonly int progress3;
			public readonly string rewardicon2;
			public readonly int cash2;
			public readonly int reward2;
			public readonly int progress2;
			public readonly string rewardicon1;
			public readonly int cash1;
			public readonly int reward1;
			public readonly int progress1;
			public readonly int type;
			public readonly string info;
			public readonly string desc;
			public readonly string name;

		public ConfAchievement(  		long id,
 		string nobility,
 		string rewardicon3,
 		int cash3,
 		int reward3,
 		int progress3,
 		string rewardicon2,
 		int cash2,
 		int reward2,
 		int progress2,
 		string rewardicon1,
 		int cash1,
 		int reward1,
 		int progress1,
 		int type,
 		string info,
 		string desc,
 		string name){
			 this.id = id;
			 this.nobility = nobility;
			 this.rewardicon3 = rewardicon3;
			 this.cash3 = cash3;
			 this.reward3 = reward3;
			 this.progress3 = progress3;
			 this.rewardicon2 = rewardicon2;
			 this.cash2 = cash2;
			 this.reward2 = reward2;
			 this.progress2 = progress2;
			 this.rewardicon1 = rewardicon1;
			 this.cash1 = cash1;
			 this.reward1 = reward1;
			 this.progress1 = progress1;
			 this.type = type;
			 this.info = info;
			 this.desc = desc;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfAchievement> dic = new Dictionary<long, ConfAchievement>();
		
		public static bool GetConfig( long id, out ConfAchievement config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Achievement", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfAchievement Get(long id)
        {
			ConfAchievement config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfAchievement config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Achievement", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfAchievement> list)
        {
            list = new List<ConfAchievement>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Achievement", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfAchievement config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfAchievement> list)
        {
            list = new List<ConfAchievement>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Achievement", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfAchievement config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Achievement condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfAchievement GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string nobility = reader.GetString(1);
								string rewardicon3 = reader.GetString(2);
								int cash3 = reader.GetInt32(3);
								int reward3 = reader.GetInt32(4);
								int progress3 = reader.GetInt32(5);
								string rewardicon2 = reader.GetString(6);
								int cash2 = reader.GetInt32(7);
								int reward2 = reader.GetInt32(8);
								int progress2 = reader.GetInt32(9);
								string rewardicon1 = reader.GetString(10);
								int cash1 = reader.GetInt32(11);
								int reward1 = reader.GetInt32(12);
								int progress1 = reader.GetInt32(13);
								int type = reader.GetInt32(14);
								string info = reader.GetString(15);
								string desc = reader.GetString(16);
								string name = reader.GetString(17);
		
				ConfAchievement	new_obj_ConfAchievement = new ConfAchievement( 		 id,
 		 nobility,
 		 rewardicon3,
 		 cash3,
 		 reward3,
 		 progress3,
 		 rewardicon2,
 		 cash2,
 		 reward2,
 		 progress2,
 		 rewardicon1,
 		 cash1,
 		 reward1,
 		 progress1,
 		 type,
 		 info,
 		 desc,
			name
			);
		
                 return new_obj_ConfAchievement;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Achievement");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfAchievement _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}