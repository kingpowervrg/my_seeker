using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from eventattribute.xls
	/// </summary>
	public  class  ConfEventAttribute
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfEventAttribute>  cacheArray = new List<ConfEventAttribute>();
		
		public static List<ConfEventAttribute> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfEventAttribute()
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
			public readonly string name;
			public readonly string icon;
			public readonly int memoryPower;
			public readonly int attentionPower;
			public readonly int willPower;
			public readonly int outsightPower;
			public readonly long type;

		public ConfEventAttribute(  		long id,
 		string name,
 		string icon,
 		int memoryPower,
 		int attentionPower,
 		int willPower,
 		int outsightPower,
 		long type){
			 this.id = id;
			 this.name = name;
			 this.icon = icon;
			 this.memoryPower = memoryPower;
			 this.attentionPower = attentionPower;
			 this.willPower = willPower;
			 this.outsightPower = outsightPower;
			 this.type = type;
		}
			
		private static Dictionary<long, ConfEventAttribute> dic = new Dictionary<long, ConfEventAttribute>();
		
		public static bool GetConfig( long id, out ConfEventAttribute config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_EventAttribute", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfEventAttribute Get(long id)
        {
			ConfEventAttribute config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfEventAttribute config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_EventAttribute", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfEventAttribute> list)
        {
            list = new List<ConfEventAttribute>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_EventAttribute", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfEventAttribute config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfEventAttribute> list)
        {
            list = new List<ConfEventAttribute>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_EventAttribute", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfEventAttribute config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventAttribute condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfEventAttribute GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string name = reader.GetString(1);
								string icon = reader.GetString(2);
								int memoryPower = reader.GetInt32(3);
								int attentionPower = reader.GetInt32(4);
								int willPower = reader.GetInt32(5);
								int outsightPower = reader.GetInt32(6);
								long type = reader.GetInt64(7);
		
				ConfEventAttribute	new_obj_ConfEventAttribute = new ConfEventAttribute( 		 id,
 		 name,
 		 icon,
 		 memoryPower,
 		 attentionPower,
 		 willPower,
 		 outsightPower,
			type
			);
		
                 return new_obj_ConfEventAttribute;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_EventAttribute");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfEventAttribute _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}