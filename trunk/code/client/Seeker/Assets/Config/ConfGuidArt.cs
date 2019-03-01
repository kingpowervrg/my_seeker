using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from Guid.xlsx
	/// </summary>
	public  class  ConfGuidArt
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfGuidArt>  cacheArray = new List<ConfGuidArt>();
		
		public static List<ConfGuidArt> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfGuidArt()
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
			public readonly string artAnchor;
			public readonly string artPath;
			public readonly float[] artShift;
			public readonly float[] artScale;

		public ConfGuidArt(  		long id,
 		string artAnchor,
 		string artPath,
 		float[] artShift,
 		float[] artScale){
			 this.id = id;
			 this.artAnchor = artAnchor;
			 this.artPath = artPath;
			 this.artShift = artShift;
			 this.artScale = artScale;
		}
			
		private static Dictionary<long, ConfGuidArt> dic = new Dictionary<long, ConfGuidArt>();
		
		public static bool GetConfig( long id, out ConfGuidArt config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_GuidArt", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfGuidArt Get(long id)
        {
			ConfGuidArt config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfGuidArt config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidArt", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfGuidArt> list)
        {
            list = new List<ConfGuidArt>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_GuidArt", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidArt config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfGuidArt> list)
        {
            list = new List<ConfGuidArt>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_GuidArt", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfGuidArt config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("GuidArt condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfGuidArt GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string artAnchor = reader.GetString(1);
								string artPath = reader.GetString(2);
							float[] artShift = (float[])reader.GetArrayData(3, 13);
							float[] artScale = (float[])reader.GetArrayData(4, 13);
		
				ConfGuidArt	new_obj_ConfGuidArt = new ConfGuidArt( 		 id,
 		 artAnchor,
 		 artPath,
 		 artShift,
			artScale
			);
		
                 return new_obj_ConfGuidArt;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_GuidArt");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfGuidArt _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}