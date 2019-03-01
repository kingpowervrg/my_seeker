using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from title.xls
	/// </summary>
	public  class  ConfTitle
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfTitle>  cacheArray = new List<ConfTitle>();
		
		public static List<ConfTitle> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfTitle()
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
			public readonly string benefit;
			public readonly string source;
			public readonly int expire;
			public readonly string icon;
			public readonly string desc;
			public readonly string info;
			public readonly string name;

		public ConfTitle(  		long id,
 		string benefit,
 		string source,
 		int expire,
 		string icon,
 		string desc,
 		string info,
 		string name){
			 this.id = id;
			 this.benefit = benefit;
			 this.source = source;
			 this.expire = expire;
			 this.icon = icon;
			 this.desc = desc;
			 this.info = info;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfTitle> dic = new Dictionary<long, ConfTitle>();
		
		public static bool GetConfig( long id, out ConfTitle config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Title", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Title 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfTitle Get(long id)
        {
			ConfTitle config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfTitle config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Title", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Title 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfTitle> list)
        {
            list = new List<ConfTitle>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Title", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfTitle config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Title not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Title key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfTitle> list)
        {
            list = new List<ConfTitle>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Title", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfTitle config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Title not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Title condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfTitle GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string benefit = reader.GetString(1);
								string source = reader.GetString(2);
								int expire = reader.GetInt32(3);
								string icon = reader.GetString(4);
								string desc = reader.GetString(5);
								string info = reader.GetString(6);
								string name = reader.GetString(7);
		
				ConfTitle	new_obj_ConfTitle = new ConfTitle( 		 id,
 		 benefit,
 		 source,
 		 expire,
 		 icon,
 		 desc,
 		 info,
			name
			);
		
                 return new_obj_ConfTitle;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Title");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfTitle _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}