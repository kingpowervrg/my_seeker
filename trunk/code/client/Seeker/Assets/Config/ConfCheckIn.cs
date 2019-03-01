using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from checkin.xls
	/// </summary>
	public  class  ConfCheckIn
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfCheckIn>  cacheArray = new List<ConfCheckIn>();
		
		public static List<ConfCheckIn> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfCheckIn()
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
			public readonly string startTime;
			public readonly int type;
			public readonly string picture;
			public readonly int count7;
			public readonly long reward7;
			public readonly int count6;
			public readonly long reward6;
			public readonly int count5;
			public readonly long reward5;
			public readonly int count4;
			public readonly long reward4;
			public readonly int count3;
			public readonly long reward3;
			public readonly int count2;
			public readonly long reward2;
			public readonly int count1;
			public readonly long reward1;
			public readonly string descs;

		public ConfCheckIn(  		long id,
 		string startTime,
 		int type,
 		string picture,
 		int count7,
 		long reward7,
 		int count6,
 		long reward6,
 		int count5,
 		long reward5,
 		int count4,
 		long reward4,
 		int count3,
 		long reward3,
 		int count2,
 		long reward2,
 		int count1,
 		long reward1,
 		string descs){
			 this.id = id;
			 this.startTime = startTime;
			 this.type = type;
			 this.picture = picture;
			 this.count7 = count7;
			 this.reward7 = reward7;
			 this.count6 = count6;
			 this.reward6 = reward6;
			 this.count5 = count5;
			 this.reward5 = reward5;
			 this.count4 = count4;
			 this.reward4 = reward4;
			 this.count3 = count3;
			 this.reward3 = reward3;
			 this.count2 = count2;
			 this.reward2 = reward2;
			 this.count1 = count1;
			 this.reward1 = reward1;
			 this.descs = descs;
		}
			
		private static Dictionary<long, ConfCheckIn> dic = new Dictionary<long, ConfCheckIn>();
		
		public static bool GetConfig( long id, out ConfCheckIn config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_CheckIn", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfCheckIn Get(long id)
        {
			ConfCheckIn config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfCheckIn config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CheckIn", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfCheckIn> list)
        {
            list = new List<ConfCheckIn>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CheckIn", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfCheckIn config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfCheckIn> list)
        {
            list = new List<ConfCheckIn>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_CheckIn", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfCheckIn config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CheckIn condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfCheckIn GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string startTime = reader.GetString(1);
								int type = reader.GetInt32(2);
								string picture = reader.GetString(3);
								int count7 = reader.GetInt32(4);
								long reward7 = reader.GetInt64(5);
								int count6 = reader.GetInt32(6);
								long reward6 = reader.GetInt64(7);
								int count5 = reader.GetInt32(8);
								long reward5 = reader.GetInt64(9);
								int count4 = reader.GetInt32(10);
								long reward4 = reader.GetInt64(11);
								int count3 = reader.GetInt32(12);
								long reward3 = reader.GetInt64(13);
								int count2 = reader.GetInt32(14);
								long reward2 = reader.GetInt64(15);
								int count1 = reader.GetInt32(16);
								long reward1 = reader.GetInt64(17);
								string descs = reader.GetString(18);
		
				ConfCheckIn	new_obj_ConfCheckIn = new ConfCheckIn( 		 id,
 		 startTime,
 		 type,
 		 picture,
 		 count7,
 		 reward7,
 		 count6,
 		 reward6,
 		 count5,
 		 reward5,
 		 count4,
 		 reward4,
 		 count3,
 		 reward3,
 		 count2,
 		 reward2,
 		 count1,
 		 reward1,
			descs
			);
		
                 return new_obj_ConfCheckIn;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_CheckIn");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfCheckIn _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}