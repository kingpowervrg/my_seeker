using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from dropout2.xls
	/// </summary>
	public  class  ConfDropOut2
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfDropOut2>  cacheArray = new List<ConfDropOut2>();
		
		public static List<ConfDropOut2> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfDropOut2()
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
			public readonly string numRate;
			public readonly string merge;
			public readonly string fixed2;

		public ConfDropOut2(  		long id,
 		string numRate,
 		string merge,
 		string fixed2){
			 this.id = id;
			 this.numRate = numRate;
			 this.merge = merge;
			 this.fixed2 = fixed2;
		}
			
		private static Dictionary<long, ConfDropOut2> dic = new Dictionary<long, ConfDropOut2>();
		
		public static bool GetConfig( long id, out ConfDropOut2 config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_DropOut2", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfDropOut2 Get(long id)
        {
			ConfDropOut2 config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfDropOut2 config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_DropOut2", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfDropOut2> list)
        {
            list = new List<ConfDropOut2>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_DropOut2", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfDropOut2 config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfDropOut2> list)
        {
            list = new List<ConfDropOut2>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_DropOut2", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfDropOut2 config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("DropOut2 condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfDropOut2 GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string numRate = reader.GetString(1);
								string merge = reader.GetString(2);
								string fixed2 = reader.GetString(3);
		
				ConfDropOut2	new_obj_ConfDropOut2 = new ConfDropOut2( 		 id,
 		 numRate,
 		 merge,
			fixed2
			);
		
                 return new_obj_ConfDropOut2;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_DropOut2");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfDropOut2 _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}