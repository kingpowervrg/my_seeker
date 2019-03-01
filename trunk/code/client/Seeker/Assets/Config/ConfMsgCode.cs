using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from msgcode.xls
	/// </summary>
	public  class  ConfMsgCode
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfMsgCode>  cacheArray = new List<ConfMsgCode>();
		
		public static List<ConfMsgCode> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfMsgCode()
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
			public readonly string errorStr;
			public readonly string popStr;

		public ConfMsgCode(  		long id,
 		string errorStr,
 		string popStr){
			 this.id = id;
			 this.errorStr = errorStr;
			 this.popStr = popStr;
		}
			
		private static Dictionary<long, ConfMsgCode> dic = new Dictionary<long, ConfMsgCode>();
		
		public static bool GetConfig( long id, out ConfMsgCode config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_MsgCode", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfMsgCode Get(long id)
        {
			ConfMsgCode config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfMsgCode config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_MsgCode", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfMsgCode> list)
        {
            list = new List<ConfMsgCode>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_MsgCode", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfMsgCode config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfMsgCode> list)
        {
            list = new List<ConfMsgCode>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_MsgCode", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfMsgCode config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("MsgCode condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfMsgCode GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string errorStr = reader.GetString(1);
								string popStr = reader.GetString(2);
		
				ConfMsgCode	new_obj_ConfMsgCode = new ConfMsgCode( 		 id,
 		 errorStr,
			popStr
			);
		
                 return new_obj_ConfMsgCode;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_MsgCode");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfMsgCode _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}