using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from charge.xls
	/// </summary>
	public  class  ConfCharge
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfCharge>  cacheArray = new List<ConfCharge>();
		
		public static List<ConfCharge> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfCharge()
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
			public readonly int type;
			public readonly string discountIcon;
			public readonly int source;
			public readonly string chargeSouceId;
			public readonly int plusPercent;
			public readonly int addCount;
			public readonly int dollar;
			public readonly int cashCount;
			public readonly string icon;
			public readonly string desc;

		public ConfCharge(  		long id,
 		int type,
 		string discountIcon,
 		int source,
 		string chargeSouceId,
 		int plusPercent,
 		int addCount,
 		int dollar,
 		int cashCount,
 		string icon,
 		string desc){
			 this.id = id;
			 this.type = type;
			 this.discountIcon = discountIcon;
			 this.source = source;
			 this.chargeSouceId = chargeSouceId;
			 this.plusPercent = plusPercent;
			 this.addCount = addCount;
			 this.dollar = dollar;
			 this.cashCount = cashCount;
			 this.icon = icon;
			 this.desc = desc;
		}
			
		private static Dictionary<long, ConfCharge> dic = new Dictionary<long, ConfCharge>();
		
		public static bool GetConfig( long id, out ConfCharge config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Charge", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Charge 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfCharge Get(long id)
        {
			ConfCharge config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfCharge config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Charge", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Charge 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfCharge> list)
        {
            list = new List<ConfCharge>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Charge", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfCharge config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Charge not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Charge key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfCharge> list)
        {
            list = new List<ConfCharge>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Charge", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfCharge config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Charge not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Charge condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfCharge GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int type = reader.GetInt32(1);
								string discountIcon = reader.GetString(2);
								int source = reader.GetInt32(3);
								string chargeSouceId = reader.GetString(4);
								int plusPercent = reader.GetInt32(5);
								int addCount = reader.GetInt32(6);
								int dollar = reader.GetInt32(7);
								int cashCount = reader.GetInt32(8);
								string icon = reader.GetString(9);
								string desc = reader.GetString(10);
		
				ConfCharge	new_obj_ConfCharge = new ConfCharge( 		 id,
 		 type,
 		 discountIcon,
 		 source,
 		 chargeSouceId,
 		 plusPercent,
 		 addCount,
 		 dollar,
 		 cashCount,
 		 icon,
			desc
			);
		
                 return new_obj_ConfCharge;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Charge");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfCharge _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}