using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from reasoning.xls
	/// </summary>
	public  class  ConfReasoning
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfReasoning>  cacheArray = new List<ConfReasoning>();
		
		public static List<ConfReasoning> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfReasoning()
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
			public readonly long[] nodes;
			public readonly string descs;
			public readonly string name;

		public ConfReasoning(  		long id,
 		long[] nodes,
 		string descs,
 		string name){
			 this.id = id;
			 this.nodes = nodes;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfReasoning> dic = new Dictionary<long, ConfReasoning>();
		
		public static bool GetConfig( long id, out ConfReasoning config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Reasoning", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfReasoning Get(long id)
        {
			ConfReasoning config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfReasoning config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Reasoning", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfReasoning> list)
        {
            list = new List<ConfReasoning>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Reasoning", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfReasoning config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfReasoning> list)
        {
            list = new List<ConfReasoning>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Reasoning", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfReasoning config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Reasoning condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfReasoning GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							long[] nodes = (long[])reader.GetArrayData(1, 17);
								string descs = reader.GetString(2);
								string name = reader.GetString(3);
		
				ConfReasoning	new_obj_ConfReasoning = new ConfReasoning( 		 id,
 		 nodes,
 		 descs,
			name
			);
		
                 return new_obj_ConfReasoning;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Reasoning");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfReasoning _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}