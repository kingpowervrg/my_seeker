using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from PropGift.xls
	/// </summary>
	public  class  ConfPropGiftItem1
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPropGiftItem1>  cacheArray = new List<ConfPropGiftItem1>();
		
		public static List<ConfPropGiftItem1> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPropGiftItem1()
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
			public readonly long value;
			public readonly long propGiftId;

		public ConfPropGiftItem1(  		int count,
 		int rate,
 		long value,
 		long propGiftId){
			 this.count = count;
			 this.rate = rate;
			 this.value = value;
			 this.propGiftId = propGiftId;
		}
			
		private static Dictionary<int, ConfPropGiftItem1> dic = new Dictionary<int, ConfPropGiftItem1>();
		
		public static bool GetConfig( int id, out ConfPropGiftItem1 config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_PropGiftItem1", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPropGiftItem1 Get(int id)
        {
			ConfPropGiftItem1 config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPropGiftItem1 config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem1", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPropGiftItem1> list)
        {
            list = new List<ConfPropGiftItem1>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem1", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem1 config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPropGiftItem1> list)
        {
            list = new List<ConfPropGiftItem1>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_PropGiftItem1", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem1 config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem1 condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPropGiftItem1 GetConfByDic(DataTable reader)
         {
		 
								int count = reader.GetInt32(0);
								int rate = reader.GetInt32(1);
								long value = reader.GetInt64(2);
								long propGiftId = reader.GetInt64(3);
		
				ConfPropGiftItem1	new_obj_ConfPropGiftItem1 = new ConfPropGiftItem1( 		 count,
 		 rate,
 		 value,
			propGiftId
			);
		
                 return new_obj_ConfPropGiftItem1;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_PropGiftItem1");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPropGiftItem1 _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.count] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}