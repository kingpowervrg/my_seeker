using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from scenespecial.xls
	/// </summary>
	public  class  ConfSceneSpecial
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfSceneSpecial>  cacheArray = new List<ConfSceneSpecial>();
		
		public static List<ConfSceneSpecial> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfSceneSpecial()
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
			public readonly int vitConsume;
			public readonly int clickCount;
			public readonly int secondGain;
			public readonly int exhibitTotal;
			public readonly int exhibitTask;
			public readonly int difficulty;
			public readonly int playMode;

		public ConfSceneSpecial(  		long id,
 		int vitConsume,
 		int clickCount,
 		int secondGain,
 		int exhibitTotal,
 		int exhibitTask,
 		int difficulty,
 		int playMode){
			 this.id = id;
			 this.vitConsume = vitConsume;
			 this.clickCount = clickCount;
			 this.secondGain = secondGain;
			 this.exhibitTotal = exhibitTotal;
			 this.exhibitTask = exhibitTask;
			 this.difficulty = difficulty;
			 this.playMode = playMode;
		}
			
		private static Dictionary<long, ConfSceneSpecial> dic = new Dictionary<long, ConfSceneSpecial>();
		
		public static bool GetConfig( long id, out ConfSceneSpecial config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_SceneSpecial", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfSceneSpecial Get(long id)
        {
			ConfSceneSpecial config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfSceneSpecial config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SceneSpecial", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfSceneSpecial> list)
        {
            list = new List<ConfSceneSpecial>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_SceneSpecial", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfSceneSpecial config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfSceneSpecial> list)
        {
            list = new List<ConfSceneSpecial>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_SceneSpecial", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfSceneSpecial config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("SceneSpecial condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfSceneSpecial GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int vitConsume = reader.GetInt32(1);
								int clickCount = reader.GetInt32(2);
								int secondGain = reader.GetInt32(3);
								int exhibitTotal = reader.GetInt32(4);
								int exhibitTask = reader.GetInt32(5);
								int difficulty = reader.GetInt32(6);
								int playMode = reader.GetInt32(7);
		
				ConfSceneSpecial	new_obj_ConfSceneSpecial = new ConfSceneSpecial( 		 id,
 		 vitConsume,
 		 clickCount,
 		 secondGain,
 		 exhibitTotal,
 		 exhibitTask,
 		 difficulty,
			playMode
			);
		
                 return new_obj_ConfSceneSpecial;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_SceneSpecial");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfSceneSpecial _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}