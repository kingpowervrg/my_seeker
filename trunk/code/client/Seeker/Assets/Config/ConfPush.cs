using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from push.xls
	/// </summary>
	public  class  ConfPush
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPush>  cacheArray = new List<ConfPush>();
		
		public static List<ConfPush> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPush()
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
			public readonly string bolckDesc;
			public readonly int bolckType;
			public readonly int manualGroup;
			public readonly int createPlayerDays;
			public readonly int manualType;
			public readonly int type;
			public readonly string background;
			public readonly string icon;
			public readonly int[] channel;
			public readonly int[] ostype;
			public readonly int activeDay;
			public readonly string beginTime;
			public readonly long chargeid;
			public readonly long giftid;
			public readonly string descs;
			public readonly string name;

		public ConfPush(  		long id,
 		string bolckDesc,
 		int bolckType,
 		int manualGroup,
 		int createPlayerDays,
 		int manualType,
 		int type,
 		string background,
 		string icon,
 		int[] channel,
 		int[] ostype,
 		int activeDay,
 		string beginTime,
 		long chargeid,
 		long giftid,
 		string descs,
 		string name){
			 this.id = id;
			 this.bolckDesc = bolckDesc;
			 this.bolckType = bolckType;
			 this.manualGroup = manualGroup;
			 this.createPlayerDays = createPlayerDays;
			 this.manualType = manualType;
			 this.type = type;
			 this.background = background;
			 this.icon = icon;
			 this.channel = channel;
			 this.ostype = ostype;
			 this.activeDay = activeDay;
			 this.beginTime = beginTime;
			 this.chargeid = chargeid;
			 this.giftid = giftid;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfPush> dic = new Dictionary<long, ConfPush>();
		
		public static bool GetConfig( long id, out ConfPush config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Push", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Push 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPush Get(long id)
        {
			ConfPush config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPush config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Push", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Push 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPush> list)
        {
            list = new List<ConfPush>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Push", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPush config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Push not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Push key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPush> list)
        {
            list = new List<ConfPush>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Push", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPush config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Push not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Push condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPush GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string bolckDesc = reader.GetString(1);
								int bolckType = reader.GetInt32(2);
								int manualGroup = reader.GetInt32(3);
								int createPlayerDays = reader.GetInt32(4);
								int manualType = reader.GetInt32(5);
								int type = reader.GetInt32(6);
								string background = reader.GetString(7);
								string icon = reader.GetString(8);
							int[] channel = (int[])reader.GetArrayData(9, 11);
							int[] ostype = (int[])reader.GetArrayData(10, 11);
								int activeDay = reader.GetInt32(11);
								string beginTime = reader.GetString(12);
								long chargeid = reader.GetInt64(13);
								long giftid = reader.GetInt64(14);
								string descs = reader.GetString(15);
								string name = reader.GetString(16);
		
				ConfPush	new_obj_ConfPush = new ConfPush( 		 id,
 		 bolckDesc,
 		 bolckType,
 		 manualGroup,
 		 createPlayerDays,
 		 manualType,
 		 type,
 		 background,
 		 icon,
 		 channel,
 		 ostype,
 		 activeDay,
 		 beginTime,
 		 chargeid,
 		 giftid,
 		 descs,
			name
			);
		
                 return new_obj_ConfPush;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Push");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPush _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}