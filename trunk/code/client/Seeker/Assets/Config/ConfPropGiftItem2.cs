using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from PropGift.xls
	/// </summary>
	public  class  ConfPropGiftItem2
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfPropGiftItem2>  cacheArray = new List<ConfPropGiftItem2>();
		
		public static List<ConfPropGiftItem2> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfPropGiftItem2()
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

		public ConfPropGiftItem2(  		int count,
 		int rate,
 		long value,
 		long propGiftId){
			 this.count = count;
			 this.rate = rate;
			 this.value = value;
			 this.propGiftId = propGiftId;
		}
			
		private static Dictionary<int, ConfPropGiftItem2> dic = new Dictionary<int, ConfPropGiftItem2>();
		
		public static bool GetConfig( int id, out ConfPropGiftItem2 config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_PropGiftItem2", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfPropGiftItem2 Get(int id)
        {
			ConfPropGiftItem2 config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfPropGiftItem2 config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem2", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfPropGiftItem2> list)
        {
            list = new List<ConfPropGiftItem2>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_PropGiftItem2", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem2 config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfPropGiftItem2> list)
        {
            list = new List<ConfPropGiftItem2>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_PropGiftItem2", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfPropGiftItem2 config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("PropGiftItem2 condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfPropGiftItem2 GetConfByDic(DataTable reader)
         {
		 
								int count = reader.GetInt32(0);
								int rate = reader.GetInt32(1);
								long value = reader.GetInt64(2);
								long propGiftId = reader.GetInt64(3);
		
				ConfPropGiftItem2	new_obj_ConfPropGiftItem2 = new ConfPropGiftItem2( 		 count,
 		 rate,
 		 value,
			propGiftId
			);
		
                 return new_obj_ConfPropGiftItem2;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_PropGiftItem2");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfPropGiftItem2 _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.count] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}