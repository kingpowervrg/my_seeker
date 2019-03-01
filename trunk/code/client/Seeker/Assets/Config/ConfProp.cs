using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from prop.xls
	/// </summary>
	public  class  ConfProp
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfProp>  cacheArray = new List<ConfProp>();
		
		public static List<ConfProp> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfProp()
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
			public readonly long officerId;
			public readonly string exhibit;
			public readonly string discriminator;
			public readonly int bindType;
			public readonly long dropout;
			public readonly int value;
			public readonly string[] address;
			public readonly int price;
			public readonly int tradeLimit;
			public readonly int heapSize;
			public readonly long skillId;
			public readonly string description;
			public readonly int type;
			public readonly string instruction;
			public readonly string name;
			public readonly string icon;

		public ConfProp(  		long id,
 		long officerId,
 		string exhibit,
 		string discriminator,
 		int bindType,
 		long dropout,
 		int value,
 		string[] address,
 		int price,
 		int tradeLimit,
 		int heapSize,
 		long skillId,
 		string description,
 		int type,
 		string instruction,
 		string name,
 		string icon){
			 this.id = id;
			 this.officerId = officerId;
			 this.exhibit = exhibit;
			 this.discriminator = discriminator;
			 this.bindType = bindType;
			 this.dropout = dropout;
			 this.value = value;
			 this.address = address;
			 this.price = price;
			 this.tradeLimit = tradeLimit;
			 this.heapSize = heapSize;
			 this.skillId = skillId;
			 this.description = description;
			 this.type = type;
			 this.instruction = instruction;
			 this.name = name;
			 this.icon = icon;
		}
			
		private static Dictionary<long, ConfProp> dic = new Dictionary<long, ConfProp>();
		
		public static bool GetConfig( long id, out ConfProp config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Prop", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Prop 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfProp Get(long id)
        {
			ConfProp config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfProp config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Prop", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Prop 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfProp> list)
        {
            list = new List<ConfProp>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Prop", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfProp config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Prop not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Prop key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfProp> list)
        {
            list = new List<ConfProp>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Prop", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfProp config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Prop not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Prop condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfProp GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long officerId = reader.GetInt64(1);
								string exhibit = reader.GetString(2);
								string discriminator = reader.GetString(3);
								int bindType = reader.GetInt32(4);
								long dropout = reader.GetInt64(5);
								int value = reader.GetInt32(6);
							string[] address = (string[])reader.GetArrayData(7, 12);
								int price = reader.GetInt32(8);
								int tradeLimit = reader.GetInt32(9);
								int heapSize = reader.GetInt32(10);
								long skillId = reader.GetInt64(11);
								string description = reader.GetString(12);
								int type = reader.GetInt32(13);
								string instruction = reader.GetString(14);
								string name = reader.GetString(15);
								string icon = reader.GetString(16);
		
				ConfProp	new_obj_ConfProp = new ConfProp( 		 id,
 		 officerId,
 		 exhibit,
 		 discriminator,
 		 bindType,
 		 dropout,
 		 value,
 		 address,
 		 price,
 		 tradeLimit,
 		 heapSize,
 		 skillId,
 		 description,
 		 type,
 		 instruction,
 		 name,
			icon
			);
		
                 return new_obj_ConfProp;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Prop");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfProp _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}