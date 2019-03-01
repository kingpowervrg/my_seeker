using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from activitybase.xls
	/// </summary>
	public  class  ConfActivityBase
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfActivityBase>  cacheArray = new List<ConfActivityBase>();
		
		public static List<ConfActivityBase> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfActivityBase()
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
			public readonly int days;
			public readonly int acceptType;
			public readonly string endTime;
			public readonly string startTime;
			public readonly string warmupTime;
			public readonly string targetPrefab;
			public readonly int targetType;
			public readonly string descs;
			public readonly int type;
			public readonly long activeId;
			public readonly int activeType;
			public readonly string icon;

		public ConfActivityBase(  		long id,
 		int days,
 		int acceptType,
 		string endTime,
 		string startTime,
 		string warmupTime,
 		string targetPrefab,
 		int targetType,
 		string descs,
 		int type,
 		long activeId,
 		int activeType,
 		string icon){
			 this.id = id;
			 this.days = days;
			 this.acceptType = acceptType;
			 this.endTime = endTime;
			 this.startTime = startTime;
			 this.warmupTime = warmupTime;
			 this.targetPrefab = targetPrefab;
			 this.targetType = targetType;
			 this.descs = descs;
			 this.type = type;
			 this.activeId = activeId;
			 this.activeType = activeType;
			 this.icon = icon;
		}
			
		private static Dictionary<long, ConfActivityBase> dic = new Dictionary<long, ConfActivityBase>();
		
		public static bool GetConfig( long id, out ConfActivityBase config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ActivityBase", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfActivityBase Get(long id)
        {
			ConfActivityBase config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfActivityBase config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ActivityBase", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfActivityBase> list)
        {
            list = new List<ConfActivityBase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ActivityBase", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfActivityBase config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfActivityBase> list)
        {
            list = new List<ConfActivityBase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ActivityBase", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfActivityBase config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActivityBase condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfActivityBase GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int days = reader.GetInt32(1);
								int acceptType = reader.GetInt32(2);
								string endTime = reader.GetString(3);
								string startTime = reader.GetString(4);
								string warmupTime = reader.GetString(5);
								string targetPrefab = reader.GetString(6);
								int targetType = reader.GetInt32(7);
								string descs = reader.GetString(8);
								int type = reader.GetInt32(9);
								long activeId = reader.GetInt64(10);
								int activeType = reader.GetInt32(11);
								string icon = reader.GetString(12);
		
				ConfActivityBase	new_obj_ConfActivityBase = new ConfActivityBase( 		 id,
 		 days,
 		 acceptType,
 		 endTime,
 		 startTime,
 		 warmupTime,
 		 targetPrefab,
 		 targetType,
 		 descs,
 		 type,
 		 activeId,
 		 activeType,
			icon
			);
		
                 return new_obj_ConfActivityBase;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ActivityBase");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfActivityBase _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}