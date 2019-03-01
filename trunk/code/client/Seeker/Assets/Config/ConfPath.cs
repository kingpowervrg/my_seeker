using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from path.xls
	/// </summary>
	public  class  ConfPath
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPath>  cacheArray = new List<ConfPath>();
		
		public static List<ConfPath> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPath()
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
			public readonly string plat;
			public readonly string version;
			public readonly int status;
			public readonly string path;

		public ConfPath(  		long id,
 		string plat,
 		string version,
 		int status,
 		string path){
			 this.id = id;
			 this.plat = plat;
			 this.version = version;
			 this.status = status;
			 this.path = path;
		}
			
		private static Dictionary<long, ConfPath> dic = new Dictionary<long, ConfPath>();
		
		public static bool GetConfig( long id, out ConfPath config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Path", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Path 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPath Get(long id)
        {
			ConfPath config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPath config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Path", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Path 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPath> list)
        {
            list = new List<ConfPath>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Path", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPath config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Path not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Path key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPath> list)
        {
            list = new List<ConfPath>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Path", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPath config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Path not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Path condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPath GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string plat = reader.GetString(1);
								string version = reader.GetString(2);
								int status = reader.GetInt32(3);
								string path = reader.GetString(4);
		
				ConfPath	new_obj_ConfPath = new ConfPath( 		 id,
 		 plat,
 		 version,
 		 status,
			path
			);
		
                 return new_obj_ConfPath;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Path");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPath _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}