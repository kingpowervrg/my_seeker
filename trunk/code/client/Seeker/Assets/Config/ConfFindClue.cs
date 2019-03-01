using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from findclue.xls
	/// </summary>
	public  class  ConfFindClue
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfFindClue>  cacheArray = new List<ConfFindClue>();
		
		public static List<ConfFindClue> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfFindClue()
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
			public readonly string icon;
			public readonly string detail;
			public readonly string descs;
			public readonly string name;

		public ConfFindClue(  		long id,
 		string icon,
 		string detail,
 		string descs,
 		string name){
			 this.id = id;
			 this.icon = icon;
			 this.detail = detail;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfFindClue> dic = new Dictionary<long, ConfFindClue>();
		
		public static bool GetConfig( long id, out ConfFindClue config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_FindClue", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfFindClue Get(long id)
        {
			ConfFindClue config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfFindClue config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_FindClue", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfFindClue> list)
        {
            list = new List<ConfFindClue>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_FindClue", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfFindClue config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfFindClue> list)
        {
            list = new List<ConfFindClue>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_FindClue", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfFindClue config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("FindClue condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfFindClue GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string icon = reader.GetString(1);
								string detail = reader.GetString(2);
								string descs = reader.GetString(3);
								string name = reader.GetString(4);
		
				ConfFindClue	new_obj_ConfFindClue = new ConfFindClue( 		 id,
 		 icon,
 		 detail,
 		 descs,
			name
			);
		
                 return new_obj_ConfFindClue;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_FindClue");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfFindClue _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}