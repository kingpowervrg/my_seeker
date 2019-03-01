using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from keywords.xlsx
	/// </summary>
	public  class  ConfKeyWords
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfKeyWords>  cacheArray = new List<ConfKeyWords>();
		
		public static List<ConfKeyWords> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfKeyWords()
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
			public readonly string word;
			public readonly string icon;

		public ConfKeyWords(  		long id,
 		string word,
 		string icon){
			 this.id = id;
			 this.word = word;
			 this.icon = icon;
		}
			
		private static Dictionary<long, ConfKeyWords> dic = new Dictionary<long, ConfKeyWords>();
		
		public static bool GetConfig( long id, out ConfKeyWords config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_KeyWords", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfKeyWords Get(long id)
        {
			ConfKeyWords config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfKeyWords config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_KeyWords", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfKeyWords> list)
        {
            list = new List<ConfKeyWords>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_KeyWords", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfKeyWords config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfKeyWords> list)
        {
            list = new List<ConfKeyWords>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_KeyWords", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfKeyWords config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("KeyWords condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfKeyWords GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string word = reader.GetString(1);
								string icon = reader.GetString(2);
		
				ConfKeyWords	new_obj_ConfKeyWords = new ConfKeyWords( 		 id,
 		 word,
			icon
			);
		
                 return new_obj_ConfKeyWords;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_KeyWords");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfKeyWords _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}