using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from police_rank.xlsx
	/// </summary>
	public  class  ConfPoliceRankIcon
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPoliceRankIcon>  cacheArray = new List<ConfPoliceRankIcon>();
		
		public static List<ConfPoliceRankIcon> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPoliceRankIcon()
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

		public ConfPoliceRankIcon(  		int id,
 		string icon){
			 this.id = id;
			 this.icon = icon;
		}
			
		private static Dictionary<int, ConfPoliceRankIcon> dic = new Dictionary<int, ConfPoliceRankIcon>();
		
		public static bool GetConfig( int id, out ConfPoliceRankIcon config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_PoliceRankIcon", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPoliceRankIcon Get(int id)
        {
			ConfPoliceRankIcon config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPoliceRankIcon config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PoliceRankIcon", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPoliceRankIcon> list)
        {
            list = new List<ConfPoliceRankIcon>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PoliceRankIcon", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPoliceRankIcon config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPoliceRankIcon> list)
        {
            list = new List<ConfPoliceRankIcon>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_PoliceRankIcon", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPoliceRankIcon config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PoliceRankIcon condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPoliceRankIcon GetConfByDic(DataTable reader)
         {
		 
								int id = reader.GetInt32(0);
								string icon = reader.GetString(1);
		
				ConfPoliceRankIcon	new_obj_ConfPoliceRankIcon = new ConfPoliceRankIcon( 		 id,
			icon
			);
		
                 return new_obj_ConfPoliceRankIcon;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_PoliceRankIcon");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPoliceRankIcon _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}