using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from inapppurchase.xlsx
	/// </summary>
	public  class  Confinapppurchase
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<Confinapppurchase>  cacheArray = new List<Confinapppurchase>();
		
		public static List<Confinapppurchase> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public Confinapppurchase()
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
			public readonly string gameid;
			public readonly string googleid;
			public readonly string appleid;
			public readonly string desc;
			public readonly int type;

		public Confinapppurchase(  		long id,
 		string gameid,
 		string googleid,
 		string appleid,
 		string desc,
 		int type){
			 this.id = id;
			 this.gameid = gameid;
			 this.googleid = googleid;
			 this.appleid = appleid;
			 this.desc = desc;
			 this.type = type;
		}
			
		private static Dictionary<long, Confinapppurchase> dic = new Dictionary<long, Confinapppurchase>();
		
		public static bool GetConfig( long id, out Confinapppurchase config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_inapppurchase", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static Confinapppurchase Get(long id)
        {
			Confinapppurchase config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out Confinapppurchase config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_inapppurchase", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<Confinapppurchase> list)
        {
            list = new List<Confinapppurchase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_inapppurchase", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    Confinapppurchase config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<Confinapppurchase> list)
        {
            list = new List<Confinapppurchase>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_inapppurchase", condition);
            if (sqReader != null)
            {
                try
                {
                    Confinapppurchase config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("inapppurchase condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static Confinapppurchase GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string gameid = reader.GetString(1);
								string googleid = reader.GetString(2);
								string appleid = reader.GetString(3);
								string desc = reader.GetString(4);
								int type = reader.GetInt32(5);
		
				Confinapppurchase	new_obj_Confinapppurchase = new Confinapppurchase( 		 id,
 		 gameid,
 		 googleid,
 		 appleid,
 		 desc,
			type
			);
		
                 return new_obj_Confinapppurchase;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_inapppurchase");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						Confinapppurchase _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}