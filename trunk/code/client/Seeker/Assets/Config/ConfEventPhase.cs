using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from eventphase.xls
	/// </summary>
	public  class  ConfEventPhase
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfEventPhase>  cacheArray = new List<ConfEventPhase>();
		
		public static List<ConfEventPhase> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfEventPhase()
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
			public readonly string normalDialogue;
			public readonly string successDialogue;
			public readonly string normaFeedback;
			public readonly string successFeedback;
			public readonly string descs;
			public readonly int normalBuff;
			public readonly int perfectBuff;
			public readonly int[] keyWords;
			public readonly string name;

		public ConfEventPhase(  		long id,
 		string normalDialogue,
 		string successDialogue,
 		string normaFeedback,
 		string successFeedback,
 		string descs,
 		int normalBuff,
 		int perfectBuff,
 		int[] keyWords,
 		string name){
			 this.id = id;
			 this.normalDialogue = normalDialogue;
			 this.successDialogue = successDialogue;
			 this.normaFeedback = normaFeedback;
			 this.successFeedback = successFeedback;
			 this.descs = descs;
			 this.normalBuff = normalBuff;
			 this.perfectBuff = perfectBuff;
			 this.keyWords = keyWords;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfEventPhase> dic = new Dictionary<long, ConfEventPhase>();
		
		public static bool GetConfig( long id, out ConfEventPhase config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_EventPhase", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfEventPhase Get(long id)
        {
			ConfEventPhase config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfEventPhase config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_EventPhase", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfEventPhase> list)
        {
            list = new List<ConfEventPhase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_EventPhase", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfEventPhase config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfEventPhase> list)
        {
            list = new List<ConfEventPhase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_EventPhase", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfEventPhase config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("EventPhase condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfEventPhase GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string normalDialogue = reader.GetString(1);
								string successDialogue = reader.GetString(2);
								string normaFeedback = reader.GetString(3);
								string successFeedback = reader.GetString(4);
								string descs = reader.GetString(5);
								int normalBuff = reader.GetInt32(6);
								int perfectBuff = reader.GetInt32(7);
							int[] keyWords = (int[])reader.GetArrayData(8, 11);
								string name = reader.GetString(9);
		
				ConfEventPhase	new_obj_ConfEventPhase = new ConfEventPhase( 		 id,
 		 normalDialogue,
 		 successDialogue,
 		 normaFeedback,
 		 successFeedback,
 		 descs,
 		 normalBuff,
 		 perfectBuff,
 		 keyWords,
			name
			);
		
                 return new_obj_ConfEventPhase;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_EventPhase");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfEventPhase _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}