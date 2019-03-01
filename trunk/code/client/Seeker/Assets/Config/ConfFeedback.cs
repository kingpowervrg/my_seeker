using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from feedback.xls
	/// </summary>
	public  class  ConfFeedback
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfFeedback>  cacheArray = new List<ConfFeedback>();
		
		public static List<ConfFeedback> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfFeedback()
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
			public readonly string createTime;
			public readonly int status;
			public readonly string rebackWord;
			public readonly string information;
			public readonly string email;
			public readonly string thirdToken;
			public readonly string thirdInfo;
			public readonly string release;
			public readonly string platform;
			public readonly int type;
			public readonly string content;
			public readonly long playerId;

		public ConfFeedback(  		long id,
 		string createTime,
 		int status,
 		string rebackWord,
 		string information,
 		string email,
 		string thirdToken,
 		string thirdInfo,
 		string release,
 		string platform,
 		int type,
 		string content,
 		long playerId){
			 this.id = id;
			 this.createTime = createTime;
			 this.status = status;
			 this.rebackWord = rebackWord;
			 this.information = information;
			 this.email = email;
			 this.thirdToken = thirdToken;
			 this.thirdInfo = thirdInfo;
			 this.release = release;
			 this.platform = platform;
			 this.type = type;
			 this.content = content;
			 this.playerId = playerId;
		}
			
		private static Dictionary<long, ConfFeedback> dic = new Dictionary<long, ConfFeedback>();
		
		public static bool GetConfig( long id, out ConfFeedback config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Feedback", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfFeedback Get(long id)
        {
			ConfFeedback config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfFeedback config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Feedback", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfFeedback> list)
        {
            list = new List<ConfFeedback>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Feedback", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfFeedback config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfFeedback> list)
        {
            list = new List<ConfFeedback>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Feedback", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfFeedback config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Feedback condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfFeedback GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string createTime = reader.GetString(1);
								int status = reader.GetInt32(2);
								string rebackWord = reader.GetString(3);
								string information = reader.GetString(4);
								string email = reader.GetString(5);
								string thirdToken = reader.GetString(6);
								string thirdInfo = reader.GetString(7);
								string release = reader.GetString(8);
								string platform = reader.GetString(9);
								int type = reader.GetInt32(10);
								string content = reader.GetString(11);
								long playerId = reader.GetInt64(12);
		
				ConfFeedback	new_obj_ConfFeedback = new ConfFeedback( 		 id,
 		 createTime,
 		 status,
 		 rebackWord,
 		 information,
 		 email,
 		 thirdToken,
 		 thirdInfo,
 		 release,
 		 platform,
 		 type,
 		 content,
			playerId
			);
		
                 return new_obj_ConfFeedback;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Feedback");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfFeedback _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}