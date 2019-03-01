using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from Guid.xlsx
	/// </summary>
	public  class  ConfGuid
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfGuid>  cacheArray = new List<ConfGuid>();
		
		public static List<ConfGuid> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfGuid()
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
			public readonly bool isMain;
			public readonly int triggerMethod;
			public readonly string uiName;
			public readonly int[] maskType;
			public readonly string[] maskName;
			public readonly string[] btnName;
			public readonly int eventPassType;
			public readonly int type;
			public readonly string typeValue;
			public readonly long nextId;
			public readonly long[] artIDs;
			public readonly bool canBreak;
			public readonly bool isSkip;
			public readonly string[] hideUINode;

		public ConfGuid(  		long id,
 		bool isMain,
 		int triggerMethod,
 		string uiName,
 		int[] maskType,
 		string[] maskName,
 		string[] btnName,
 		int eventPassType,
 		int type,
 		string typeValue,
 		long nextId,
 		long[] artIDs,
 		bool canBreak,
 		bool isSkip,
 		string[] hideUINode){
			 this.id = id;
			 this.isMain = isMain;
			 this.triggerMethod = triggerMethod;
			 this.uiName = uiName;
			 this.maskType = maskType;
			 this.maskName = maskName;
			 this.btnName = btnName;
			 this.eventPassType = eventPassType;
			 this.type = type;
			 this.typeValue = typeValue;
			 this.nextId = nextId;
			 this.artIDs = artIDs;
			 this.canBreak = canBreak;
			 this.isSkip = isSkip;
			 this.hideUINode = hideUINode;
		}
			
		private static Dictionary<long, ConfGuid> dic = new Dictionary<long, ConfGuid>();
		
		public static bool GetConfig( long id, out ConfGuid config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Guid", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Guid 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfGuid Get(long id)
        {
			ConfGuid config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfGuid config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Guid", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Guid 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfGuid> list)
        {
            list = new List<ConfGuid>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Guid", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfGuid config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Guid not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Guid key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfGuid> list)
        {
            list = new List<ConfGuid>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Guid", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfGuid config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Guid not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Guid condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfGuid GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								bool isMain = reader.GetBoolean(1);
								int triggerMethod = reader.GetInt32(2);
								string uiName = reader.GetString(3);
							int[] maskType = (int[])reader.GetArrayData(4, 11);
							string[] maskName = (string[])reader.GetArrayData(5, 12);
							string[] btnName = (string[])reader.GetArrayData(6, 12);
								int eventPassType = reader.GetInt32(7);
								int type = reader.GetInt32(8);
								string typeValue = reader.GetString(9);
								long nextId = reader.GetInt64(10);
							long[] artIDs = (long[])reader.GetArrayData(11, 17);
								bool canBreak = reader.GetBoolean(12);
								bool isSkip = reader.GetBoolean(13);
							string[] hideUINode = (string[])reader.GetArrayData(14, 12);
		
				ConfGuid	new_obj_ConfGuid = new ConfGuid( 		 id,
 		 isMain,
 		 triggerMethod,
 		 uiName,
 		 maskType,
 		 maskName,
 		 btnName,
 		 eventPassType,
 		 type,
 		 typeValue,
 		 nextId,
 		 artIDs,
 		 canBreak,
 		 isSkip,
			hideUINode
			);
		
                 return new_obj_ConfGuid;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Guid");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfGuid _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}