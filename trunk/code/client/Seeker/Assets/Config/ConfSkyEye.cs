using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from skyeye.xls
	/// </summary>
	public  class  ConfSkyEye
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfSkyEye>  cacheArray = new List<ConfSkyEye>();
		
		public static List<ConfSkyEye> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfSkyEye()
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
			public readonly int[] rewardNums;
			public readonly long[] rewards;
			public readonly long refuteId;
			public readonly long keyCollectorId;
			public readonly long[] collectorIds;
			public readonly long npcId;
			public readonly string descs;
			public readonly string name;

		public ConfSkyEye(  		long id,
 		int[] rewardNums,
 		long[] rewards,
 		long refuteId,
 		long keyCollectorId,
 		long[] collectorIds,
 		long npcId,
 		string descs,
 		string name){
			 this.id = id;
			 this.rewardNums = rewardNums;
			 this.rewards = rewards;
			 this.refuteId = refuteId;
			 this.keyCollectorId = keyCollectorId;
			 this.collectorIds = collectorIds;
			 this.npcId = npcId;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfSkyEye> dic = new Dictionary<long, ConfSkyEye>();
		
		public static bool GetConfig( long id, out ConfSkyEye config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_SkyEye", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfSkyEye Get(long id)
        {
			ConfSkyEye config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfSkyEye config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SkyEye", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfSkyEye> list)
        {
            list = new List<ConfSkyEye>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SkyEye", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfSkyEye config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfSkyEye> list)
        {
            list = new List<ConfSkyEye>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_SkyEye", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfSkyEye config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SkyEye condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfSkyEye GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							int[] rewardNums = (int[])reader.GetArrayData(1, 11);
							long[] rewards = (long[])reader.GetArrayData(2, 17);
								long refuteId = reader.GetInt64(3);
								long keyCollectorId = reader.GetInt64(4);
							long[] collectorIds = (long[])reader.GetArrayData(5, 17);
								long npcId = reader.GetInt64(6);
								string descs = reader.GetString(7);
								string name = reader.GetString(8);
		
				ConfSkyEye	new_obj_ConfSkyEye = new ConfSkyEye( 		 id,
 		 rewardNums,
 		 rewards,
 		 refuteId,
 		 keyCollectorId,
 		 collectorIds,
 		 npcId,
 		 descs,
			name
			);
		
                 return new_obj_ConfSkyEye;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_SkyEye");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfSkyEye _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}