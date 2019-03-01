using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from demoscene.xlsx
	/// </summary>
	public  class  ConfDemoScene
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfDemoScene>  cacheArray = new List<ConfDemoScene>();
		
		public static List<ConfDemoScene> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfDemoScene()
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
			public readonly long demoSceneID;

		public ConfDemoScene(  		long id,
 		long demoSceneID){
			 this.id = id;
			 this.demoSceneID = demoSceneID;
		}
			
		private static Dictionary<long, ConfDemoScene> dic = new Dictionary<long, ConfDemoScene>();
		
		public static bool GetConfig( long id, out ConfDemoScene config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_DemoScene", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfDemoScene Get(long id)
        {
			ConfDemoScene config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfDemoScene config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_DemoScene", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfDemoScene> list)
        {
            list = new List<ConfDemoScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_DemoScene", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfDemoScene config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfDemoScene> list)
        {
            list = new List<ConfDemoScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_DemoScene", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfDemoScene config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("DemoScene condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfDemoScene GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long demoSceneID = reader.GetInt64(1);
		
				ConfDemoScene	new_obj_ConfDemoScene = new ConfDemoScene( 		 id,
			demoSceneID
			);
		
                 return new_obj_ConfDemoScene;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_DemoScene");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfDemoScene _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}