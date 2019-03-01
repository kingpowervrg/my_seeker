using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from GuidNew.xlsx
	/// </summary>
	public  class  ConfGuidNewFunction
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfGuidNewFunction>  cacheArray = new List<ConfGuidNewFunction>();
		
		public static List<ConfGuidNewFunction> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfGuidNewFunction()
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
			public readonly string funcName;
			public readonly string[] funcParams;
			public readonly long[] nextFuncID;

		public ConfGuidNewFunction(  		long id,
 		string funcName,
 		string[] funcParams,
 		long[] nextFuncID){
			 this.id = id;
			 this.funcName = funcName;
			 this.funcParams = funcParams;
			 this.nextFuncID = nextFuncID;
		}
			
		private static Dictionary<long, ConfGuidNewFunction> dic = new Dictionary<long, ConfGuidNewFunction>();
		
		public static bool GetConfig( long id, out ConfGuidNewFunction config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_GuidNewFunction", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfGuidNewFunction Get(long id)
        {
			ConfGuidNewFunction config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfGuidNewFunction config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidNewFunction", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfGuidNewFunction> list)
        {
            list = new List<ConfGuidNewFunction>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidNewFunction", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidNewFunction config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfGuidNewFunction> list)
        {
            list = new List<ConfGuidNewFunction>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_GuidNewFunction", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidNewFunction config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNewFunction condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfGuidNewFunction GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string funcName = reader.GetString(1);
							string[] funcParams = (string[])reader.GetArrayData(2, 12);
							long[] nextFuncID = (long[])reader.GetArrayData(3, 17);
		
				ConfGuidNewFunction	new_obj_ConfGuidNewFunction = new ConfGuidNewFunction( 		 id,
 		 funcName,
 		 funcParams,
			nextFuncID
			);
		
                 return new_obj_ConfGuidNewFunction;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_GuidNewFunction");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfGuidNewFunction _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}