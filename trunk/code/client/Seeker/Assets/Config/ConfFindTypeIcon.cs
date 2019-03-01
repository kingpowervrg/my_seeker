using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from findtypeicon.xls
	/// </summary>
	public  class  ConfFindTypeIcon
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfFindTypeIcon>  cacheArray = new List<ConfFindTypeIcon>();
		
		public static List<ConfFindTypeIcon> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfFindTypeIcon()
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
			public readonly string icon;
			public readonly string name;

		public ConfFindTypeIcon(  		int id,
 		string icon,
 		string name){
			 this.id = id;
			 this.icon = icon;
			 this.name = name;
		}
			
		private static Dictionary<int, ConfFindTypeIcon> dic = new Dictionary<int, ConfFindTypeIcon>();
		
		public static bool GetConfig( int id, out ConfFindTypeIcon config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_FindTypeIcon", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfFindTypeIcon Get(int id)
        {
			ConfFindTypeIcon config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfFindTypeIcon config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_FindTypeIcon", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfFindTypeIcon> list)
        {
            list = new List<ConfFindTypeIcon>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_FindTypeIcon", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfFindTypeIcon config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfFindTypeIcon> list)
        {
            list = new List<ConfFindTypeIcon>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_FindTypeIcon", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfFindTypeIcon config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindTypeIcon condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfFindTypeIcon GetConfByDic(DataTable reader)
         {
		 
								int id = reader.GetInt32(0);
								string icon = reader.GetString(1);
								string name = reader.GetString(2);
		
				ConfFindTypeIcon	new_obj_ConfFindTypeIcon = new ConfFindTypeIcon( 		 id,
 		 icon,
			name
			);
		
                 return new_obj_ConfFindTypeIcon;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_FindTypeIcon");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfFindTypeIcon _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}