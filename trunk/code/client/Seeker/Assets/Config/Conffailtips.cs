using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from failtips.xlsx
	/// </summary>
	public  class  Conffailtips
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<Conffailtips>  cacheArray = new List<Conffailtips>();
		
		public static List<Conffailtips> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public Conffailtips()
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
			public readonly string ui;
			public readonly string descchn;
			public readonly string desc;
			public readonly string icon;

		public Conffailtips(  		long id,
 		string ui,
 		string descchn,
 		string desc,
 		string icon){
			 this.id = id;
			 this.ui = ui;
			 this.descchn = descchn;
			 this.desc = desc;
			 this.icon = icon;
		}
			
		private static Dictionary<long, Conffailtips> dic = new Dictionary<long, Conffailtips>();
		
		public static bool GetConfig( long id, out Conffailtips config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_failtips", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("failtips 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static Conffailtips Get(long id)
        {
			Conffailtips config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out Conffailtips config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_failtips", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("failtips 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<Conffailtips> list)
        {
            list = new List<Conffailtips>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_failtips", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    Conffailtips config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("failtips not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("failtips key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<Conffailtips> list)
        {
            list = new List<Conffailtips>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_failtips", condition);
            if (sqReader != null)
            {
                try
                {
                    Conffailtips config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("failtips not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("failtips condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static Conffailtips GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string ui = reader.GetString(1);
								string descchn = reader.GetString(2);
								string desc = reader.GetString(3);
								string icon = reader.GetString(4);
		
				Conffailtips	new_obj_Conffailtips = new Conffailtips( 		 id,
 		 ui,
 		 descchn,
 		 desc,
			icon
			);
		
                 return new_obj_Conffailtips;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_failtips");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						Conffailtips _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}