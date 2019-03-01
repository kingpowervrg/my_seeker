using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from serviceconfig.xls
	/// </summary>
	public  class  ConfServiceConfig
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfServiceConfig>  cacheArray = new List<ConfServiceConfig>();
		
		public static List<ConfServiceConfig> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfServiceConfig()
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
			public readonly string editLog;
			public readonly string filedValuePre;
			public readonly string filedComment;
			public readonly string fieldValue;
			public readonly string fieldName;

		public ConfServiceConfig(  		long id,
 		string editLog,
 		string filedValuePre,
 		string filedComment,
 		string fieldValue,
 		string fieldName){
			 this.id = id;
			 this.editLog = editLog;
			 this.filedValuePre = filedValuePre;
			 this.filedComment = filedComment;
			 this.fieldValue = fieldValue;
			 this.fieldName = fieldName;
		}
			
		private static Dictionary<long, ConfServiceConfig> dic = new Dictionary<long, ConfServiceConfig>();
		
		public static bool GetConfig( long id, out ConfServiceConfig config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ServiceConfig", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfServiceConfig Get(long id)
        {
			ConfServiceConfig config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfServiceConfig config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ServiceConfig", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfServiceConfig> list)
        {
            list = new List<ConfServiceConfig>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ServiceConfig", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfServiceConfig config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfServiceConfig> list)
        {
            list = new List<ConfServiceConfig>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ServiceConfig", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfServiceConfig config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ServiceConfig condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfServiceConfig GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string editLog = reader.GetString(1);
								string filedValuePre = reader.GetString(2);
								string filedComment = reader.GetString(3);
								string fieldValue = reader.GetString(4);
								string fieldName = reader.GetString(5);
		
				ConfServiceConfig	new_obj_ConfServiceConfig = new ConfServiceConfig( 		 id,
 		 editLog,
 		 filedValuePre,
 		 filedComment,
 		 fieldValue,
			fieldName
			);
		
                 return new_obj_ConfServiceConfig;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ServiceConfig");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfServiceConfig _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}