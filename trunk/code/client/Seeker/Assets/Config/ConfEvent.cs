using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from event.xls
	/// </summary>
	public  class  ConfEvent
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfEvent>  cacheArray = new List<ConfEvent>();
		
		public static List<ConfEvent> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfEvent()
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
			public readonly string conclusion;
			public readonly long perfectDropId;
			public readonly long normalDropId;
			public readonly int cashGain;
			public readonly int coinGain;
			public readonly int expGain;
			public readonly int vitGain;
			public readonly long[] phases;
			public readonly int perfectMark;
			public readonly int passMark;
			public readonly int vitConsume;
			public readonly int type;
			public readonly string sceneInfo;
			public readonly string conclusionFail;
			public readonly string background;
			public readonly string descInfo;
			public readonly string descs;
			public readonly string name;

		public ConfEvent(  		long id,
 		string conclusion,
 		long perfectDropId,
 		long normalDropId,
 		int cashGain,
 		int coinGain,
 		int expGain,
 		int vitGain,
 		long[] phases,
 		int perfectMark,
 		int passMark,
 		int vitConsume,
 		int type,
 		string sceneInfo,
 		string conclusionFail,
 		string background,
 		string descInfo,
 		string descs,
 		string name){
			 this.id = id;
			 this.conclusion = conclusion;
			 this.perfectDropId = perfectDropId;
			 this.normalDropId = normalDropId;
			 this.cashGain = cashGain;
			 this.coinGain = coinGain;
			 this.expGain = expGain;
			 this.vitGain = vitGain;
			 this.phases = phases;
			 this.perfectMark = perfectMark;
			 this.passMark = passMark;
			 this.vitConsume = vitConsume;
			 this.type = type;
			 this.sceneInfo = sceneInfo;
			 this.conclusionFail = conclusionFail;
			 this.background = background;
			 this.descInfo = descInfo;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfEvent> dic = new Dictionary<long, ConfEvent>();
		
		public static bool GetConfig( long id, out ConfEvent config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Event", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Event 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfEvent Get(long id)
        {
			ConfEvent config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfEvent config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Event", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Event 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfEvent> list)
        {
            list = new List<ConfEvent>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Event", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfEvent config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Event not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Event key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfEvent> list)
        {
            list = new List<ConfEvent>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Event", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfEvent config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Event not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Event condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfEvent GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string conclusion = reader.GetString(1);
								long perfectDropId = reader.GetInt64(2);
								long normalDropId = reader.GetInt64(3);
								int cashGain = reader.GetInt32(4);
								int coinGain = reader.GetInt32(5);
								int expGain = reader.GetInt32(6);
								int vitGain = reader.GetInt32(7);
							long[] phases = (long[])reader.GetArrayData(8, 17);
								int perfectMark = reader.GetInt32(9);
								int passMark = reader.GetInt32(10);
								int vitConsume = reader.GetInt32(11);
								int type = reader.GetInt32(12);
								string sceneInfo = reader.GetString(13);
								string conclusionFail = reader.GetString(14);
								string background = reader.GetString(15);
								string descInfo = reader.GetString(16);
								string descs = reader.GetString(17);
								string name = reader.GetString(18);
		
				ConfEvent	new_obj_ConfEvent = new ConfEvent( 		 id,
 		 conclusion,
 		 perfectDropId,
 		 normalDropId,
 		 cashGain,
 		 coinGain,
 		 expGain,
 		 vitGain,
 		 phases,
 		 perfectMark,
 		 passMark,
 		 vitConsume,
 		 type,
 		 sceneInfo,
 		 conclusionFail,
 		 background,
 		 descInfo,
 		 descs,
			name
			);
		
                 return new_obj_ConfEvent;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Event");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfEvent _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}