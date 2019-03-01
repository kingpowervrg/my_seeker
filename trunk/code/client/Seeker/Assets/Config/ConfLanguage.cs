using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from language.xlsx
	/// </summary>
	public  class  ConfLanguage
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfLanguage>  cacheArray = new List<ConfLanguage>();
		
		public static List<ConfLanguage> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfLanguage()
		{
		}

		public static void Init()
		{
			if (cacheLoaded)
			{
				GetArrrayList();
			}
            
		}
			public readonly string id;
			public readonly string Chinese;
			public readonly string English;
			public readonly string ChineseTraditional;

		public ConfLanguage(  		string id,
 		string Chinese,
 		string English,
 		string ChineseTraditional){
			 this.id = id;
			 this.Chinese = Chinese;
			 this.English = English;
			 this.ChineseTraditional = ChineseTraditional;
		}
			
		private static Dictionary<string, ConfLanguage> dic = new Dictionary<string, ConfLanguage>();
		
		public static bool GetConfig( string id, out ConfLanguage config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Language", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Language 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfLanguage Get(string id)
        {
			ConfLanguage config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfLanguage config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Language", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Language 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfLanguage> list)
        {
            list = new List<ConfLanguage>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Language", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfLanguage config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Language not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Language key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfLanguage> list)
        {
            list = new List<ConfLanguage>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Language", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfLanguage config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Language not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Language condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfLanguage GetConfByDic(DataTable reader)
         {
		 
								string id = reader.GetString(0);
								string Chinese = reader.GetString(1);
								string English = reader.GetString(2);
								string ChineseTraditional = reader.GetString(3);
		
				ConfLanguage	new_obj_ConfLanguage = new ConfLanguage( 		 id,
 		 Chinese,
 		 English,
			ChineseTraditional
			);
		
                 return new_obj_ConfLanguage;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Language");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfLanguage _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}