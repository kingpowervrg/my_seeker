using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from PropGift.xls
	/// </summary>
	public  class  ConfPropGift
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPropGift>  cacheArray = new List<ConfPropGift>();
		
		public static List<ConfPropGift> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPropGift()
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
			public readonly string description;
			public readonly string name;
			public readonly string icon;

		public ConfPropGift(  		long id,
 		string description,
 		string name,
 		string icon){
			 this.id = id;
			 this.description = description;
			 this.name = name;
			 this.icon = icon;
		}
			
		private static Dictionary<long, ConfPropGift> dic = new Dictionary<long, ConfPropGift>();
		
		public static bool GetConfig( long id, out ConfPropGift config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_PropGift", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPropGift Get(long id)
        {
			ConfPropGift config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPropGift config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGift", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPropGift> list)
        {
            list = new List<ConfPropGift>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGift", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGift config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPropGift> list)
        {
            list = new List<ConfPropGift>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_PropGift", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGift config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGift condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPropGift GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string description = reader.GetString(1);
								string name = reader.GetString(2);
								string icon = reader.GetString(3);
		
				ConfPropGift	new_obj_ConfPropGift = new ConfPropGift( 		 id,
 		 description,
 		 name,
			icon
			);
		
                 return new_obj_ConfPropGift;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_PropGift");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPropGift _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}