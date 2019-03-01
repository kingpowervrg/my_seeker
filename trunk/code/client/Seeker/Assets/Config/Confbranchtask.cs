using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from branchtask.xlsx
	/// </summary>
	public  class  Confbranchtask
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<Confbranchtask>  cacheArray = new List<Confbranchtask>();
		
		public static List<Confbranchtask> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public Confbranchtask()
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
			public readonly long[] taskids;

		public Confbranchtask(  		long id,
 		long[] taskids){
			 this.id = id;
			 this.taskids = taskids;
		}
			
		private static Dictionary<long, Confbranchtask> dic = new Dictionary<long, Confbranchtask>();
		
		public static bool GetConfig( long id, out Confbranchtask config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_branchtask", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static Confbranchtask Get(long id)
        {
			Confbranchtask config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out Confbranchtask config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_branchtask", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<Confbranchtask> list)
        {
            list = new List<Confbranchtask>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_branchtask", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    Confbranchtask config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<Confbranchtask> list)
        {
            list = new List<Confbranchtask>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_branchtask", condition);
            if (sqReader != null)
            {
                try
                {
                    Confbranchtask config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("branchtask condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static Confbranchtask GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							long[] taskids = (long[])reader.GetArrayData(1, 17);
		
				Confbranchtask	new_obj_Confbranchtask = new Confbranchtask( 		 id,
			taskids
			);
		
                 return new_obj_Confbranchtask;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_branchtask");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						Confbranchtask _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}