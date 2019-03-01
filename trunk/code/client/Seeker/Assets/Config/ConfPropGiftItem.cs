using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from dropout.xls
	/// </summary>
	public  class  ConfPropGiftItem
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPropGiftItem>  cacheArray = new List<ConfPropGiftItem>();
		
		public static List<ConfPropGiftItem> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPropGiftItem()
		{
		}

		public static void Init()
		{
			if (cacheLoaded)
			{
				GetArrrayList();
			}
            
		}
			public readonly int count;
			public readonly int rate;
			public readonly long dropOutId;
			public readonly long value;

		public ConfPropGiftItem(  		int count,
 		int rate,
 		long dropOutId,
 		long value){
			 this.count = count;
			 this.rate = rate;
			 this.dropOutId = dropOutId;
			 this.value = value;
		}
			
		private static Dictionary<int, ConfPropGiftItem> dic = new Dictionary<int, ConfPropGiftItem>();
		
		public static bool GetConfig( int id, out ConfPropGiftItem config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_PropGiftItem", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPropGiftItem Get(int id)
        {
			ConfPropGiftItem config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPropGiftItem config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPropGiftItem> list)
        {
            list = new List<ConfPropGiftItem>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPropGiftItem> list)
        {
            list = new List<ConfPropGiftItem>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_PropGiftItem", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPropGiftItem GetConfByDic(DataTable reader)
         {
		 
								int count = reader.GetInt32(0);
								int rate = reader.GetInt32(1);
								long dropOutId = reader.GetInt64(2);
								long value = reader.GetInt64(3);
		
				ConfPropGiftItem	new_obj_ConfPropGiftItem = new ConfPropGiftItem( 		 count,
 		 rate,
 		 dropOutId,
			value
			);
		
                 return new_obj_ConfPropGiftItem;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_PropGiftItem");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPropGiftItem _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.count] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}