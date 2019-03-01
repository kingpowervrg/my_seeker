using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from chat.xls
	/// </summary>
	public  class  ConfChat
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfChat>  cacheArray = new List<ConfChat>();
		
		public static List<ConfChat> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfChat()
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
			public readonly long[] rewards;
			public readonly string sceanid;
			public readonly string description;

		public ConfChat(  		long id,
 		long[] rewards,
 		string sceanid,
 		string description){
			 this.id = id;
			 this.rewards = rewards;
			 this.sceanid = sceanid;
			 this.description = description;
		}
			
		private static Dictionary<long, ConfChat> dic = new Dictionary<long, ConfChat>();
		
		public static bool GetConfig( long id, out ConfChat config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Chat", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Chat 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfChat Get(long id)
        {
			ConfChat config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfChat config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Chat", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chat 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfChat> list)
        {
            list = new List<ConfChat>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Chat", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfChat config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Chat not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chat key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfChat> list)
        {
            list = new List<ConfChat>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Chat", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfChat config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Chat not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chat condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfChat GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							long[] rewards = (long[])reader.GetArrayData(1, 17);
								string sceanid = reader.GetString(2);
								string description = reader.GetString(3);
		
				ConfChat	new_obj_ConfChat = new ConfChat( 		 id,
 		 rewards,
 		 sceanid,
			description
			);
		
                 return new_obj_ConfChat;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Chat");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfChat _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}