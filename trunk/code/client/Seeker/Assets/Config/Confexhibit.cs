using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from exhibit.xlsx
	/// </summary>
	public  class  Confexhibit
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<Confexhibit>  cacheArray = new List<Confexhibit>();
		
		public static List<Confexhibit> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public Confexhibit()
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
			public readonly string model;
			public readonly string name;
			public readonly int isStory;
			public readonly string descs;
			public readonly string iconName;
			public readonly string assetName;
			public readonly float colliderScale;

		public Confexhibit(  		long id,
 		string icon,
 		string model,
 		string name,
 		int isStory,
 		string descs,
 		string iconName,
 		string assetName,
 		float colliderScale){
			 this.id = id;
			 this.icon = icon;
			 this.model = model;
			 this.name = name;
			 this.isStory = isStory;
			 this.descs = descs;
			 this.iconName = iconName;
			 this.assetName = assetName;
			 this.colliderScale = colliderScale;
		}
			
		private static Dictionary<long, Confexhibit> dic = new Dictionary<long, Confexhibit>();
		
		public static bool GetConfig( long id, out Confexhibit config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_exhibit", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static Confexhibit Get(long id)
        {
			Confexhibit config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out Confexhibit config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_exhibit", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<Confexhibit> list)
        {
            list = new List<Confexhibit>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_exhibit", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    Confexhibit config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<Confexhibit> list)
        {
            list = new List<Confexhibit>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_exhibit", condition);
            if (sqReader != null)
            {
                try
                {
                    Confexhibit config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("exhibit condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static Confexhibit GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string icon = reader.GetString(1);
								string model = reader.GetString(2);
								string name = reader.GetString(3);
								int isStory = reader.GetInt32(4);
								string descs = reader.GetString(5);
								string iconName = reader.GetString(6);
								string assetName = reader.GetString(7);
								float colliderScale = reader.GetFloat(8);
		
				Confexhibit	new_obj_Confexhibit = new Confexhibit( 		 id,
 		 icon,
 		 model,
 		 name,
 		 isStory,
 		 descs,
 		 iconName,
 		 assetName,
			colliderScale
			);
		
                 return new_obj_Confexhibit;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_exhibit");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						Confexhibit _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}