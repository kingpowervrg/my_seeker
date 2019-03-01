using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from GMCommand.xlsx
	/// </summary>
	public  class  ConfGMCMD
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfGMCMD>  cacheArray = new List<ConfGMCMD>();
		
		public static List<ConfGMCMD> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfGMCMD()
		{
		}

		public static void Init()
		{
			if (cacheLoaded)
			{
				GetArrrayList();
			}
            
		}
			public readonly int id;
			public readonly string messageName;
			public readonly string messageDesc;
			public readonly string messageFormat;

		public ConfGMCMD(  		int id,
 		string messageName,
 		string messageDesc,
 		string messageFormat){
			 this.id = id;
			 this.messageName = messageName;
			 this.messageDesc = messageDesc;
			 this.messageFormat = messageFormat;
		}
			
		private static Dictionary<int, ConfGMCMD> dic = new Dictionary<int, ConfGMCMD>();
		
		public static bool GetConfig( int id, out ConfGMCMD config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_GMCMD", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfGMCMD Get(int id)
        {
			ConfGMCMD config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfGMCMD config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GMCMD", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfGMCMD> list)
        {
            list = new List<ConfGMCMD>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GMCMD", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfGMCMD config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfGMCMD> list)
        {
            list = new List<ConfGMCMD>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_GMCMD", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfGMCMD config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GMCMD condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfGMCMD GetConfByDic(DataTable reader)
         {
		 
								int id = reader.GetInt32(0);
								string messageName = reader.GetString(1);
								string messageDesc = reader.GetString(2);
								string messageFormat = reader.GetString(3);
		
				ConfGMCMD	new_obj_ConfGMCMD = new ConfGMCMD( 		 id,
 		 messageName,
 		 messageDesc,
			messageFormat
			);
		
                 return new_obj_ConfGMCMD;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_GMCMD");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfGMCMD _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}