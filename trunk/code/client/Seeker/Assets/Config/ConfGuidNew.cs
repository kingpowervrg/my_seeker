using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from GuidNew.xlsx
	/// </summary>
	public  class  ConfGuidNew
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfGuidNew>  cacheArray = new List<ConfGuidNew>();
		
		public static List<ConfGuidNew> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfGuidNew()
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
			public readonly int groupId;
			public readonly int type;
			public readonly string typeValue;
			public readonly long nextId;
			public readonly bool funcFinsh;
			public readonly bool isSkip;
			public readonly long[] initFuncIds;
			public readonly long[] preFuncId;
			public readonly long[] funcIds;

		public ConfGuidNew(  		long id,
 		int groupId,
 		int type,
 		string typeValue,
 		long nextId,
 		bool funcFinsh,
 		bool isSkip,
 		long[] initFuncIds,
 		long[] preFuncId,
 		long[] funcIds){
			 this.id = id;
			 this.groupId = groupId;
			 this.type = type;
			 this.typeValue = typeValue;
			 this.nextId = nextId;
			 this.funcFinsh = funcFinsh;
			 this.isSkip = isSkip;
			 this.initFuncIds = initFuncIds;
			 this.preFuncId = preFuncId;
			 this.funcIds = funcIds;
		}
			
		private static Dictionary<long, ConfGuidNew> dic = new Dictionary<long, ConfGuidNew>();
		
		public static bool GetConfig( long id, out ConfGuidNew config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_GuidNew", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfGuidNew Get(long id)
        {
			ConfGuidNew config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfGuidNew config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidNew", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfGuidNew> list)
        {
            list = new List<ConfGuidNew>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidNew", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidNew config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfGuidNew> list)
        {
            list = new List<ConfGuidNew>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_GuidNew", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidNew config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidNew condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfGuidNew GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int groupId = reader.GetInt32(1);
								int type = reader.GetInt32(2);
								string typeValue = reader.GetString(3);
								long nextId = reader.GetInt64(4);
								bool funcFinsh = reader.GetBoolean(5);
								bool isSkip = reader.GetBoolean(6);
							long[] initFuncIds = (long[])reader.GetArrayData(7, 17);
							long[] preFuncId = (long[])reader.GetArrayData(8, 17);
							long[] funcIds = (long[])reader.GetArrayData(9, 17);
		
				ConfGuidNew	new_obj_ConfGuidNew = new ConfGuidNew( 		 id,
 		 groupId,
 		 type,
 		 typeValue,
 		 nextId,
 		 funcFinsh,
 		 isSkip,
 		 initFuncIds,
 		 preFuncId,
			funcIds
			);
		
                 return new_obj_ConfGuidNew;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_GuidNew");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfGuidNew _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}