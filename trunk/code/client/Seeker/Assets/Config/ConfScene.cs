using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from scene.xls
	/// </summary>
	public  class  ConfScene
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfScene>  cacheArray = new List<ConfScene>();
		
		public static List<ConfScene> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfScene()
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
			public readonly int keyExhibit;
			public readonly int vitConsume;
			public readonly int clickCount;
			public readonly int secondGain;
			public readonly string cameraParam;
			public readonly long multiId;
			public readonly string playModeIcon;
			public readonly long[] props;
			public readonly int exhibitTotal;
			public readonly int exhibitTask;
			public readonly int playMode;
			public readonly string exhibitGroupId;
			public readonly string sceneInfo;
			public readonly long dropId;
			public readonly int outputVit;
			public readonly int outputCash;
			public readonly int outputExp;
			public readonly int outputMoney;
			public readonly int difficulty;
			public readonly string name;
			public readonly string thumbnail;
			public readonly string descs;

		public ConfScene(  		long id,
 		int keyExhibit,
 		int vitConsume,
 		int clickCount,
 		int secondGain,
 		string cameraParam,
 		long multiId,
 		string playModeIcon,
 		long[] props,
 		int exhibitTotal,
 		int exhibitTask,
 		int playMode,
 		string exhibitGroupId,
 		string sceneInfo,
 		long dropId,
 		int outputVit,
 		int outputCash,
 		int outputExp,
 		int outputMoney,
 		int difficulty,
 		string name,
 		string thumbnail,
 		string descs){
			 this.id = id;
			 this.keyExhibit = keyExhibit;
			 this.vitConsume = vitConsume;
			 this.clickCount = clickCount;
			 this.secondGain = secondGain;
			 this.cameraParam = cameraParam;
			 this.multiId = multiId;
			 this.playModeIcon = playModeIcon;
			 this.props = props;
			 this.exhibitTotal = exhibitTotal;
			 this.exhibitTask = exhibitTask;
			 this.playMode = playMode;
			 this.exhibitGroupId = exhibitGroupId;
			 this.sceneInfo = sceneInfo;
			 this.dropId = dropId;
			 this.outputVit = outputVit;
			 this.outputCash = outputCash;
			 this.outputExp = outputExp;
			 this.outputMoney = outputMoney;
			 this.difficulty = difficulty;
			 this.name = name;
			 this.thumbnail = thumbnail;
			 this.descs = descs;
		}
			
		private static Dictionary<long, ConfScene> dic = new Dictionary<long, ConfScene>();
		
		public static bool GetConfig( long id, out ConfScene config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Scene", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Scene 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfScene Get(long id)
        {
			ConfScene config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfScene config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Scene", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Scene 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfScene> list)
        {
            list = new List<ConfScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Scene", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfScene config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Scene not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Scene key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfScene> list)
        {
            list = new List<ConfScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Scene", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfScene config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Scene not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Scene condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfScene GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int keyExhibit = reader.GetInt32(1);
								int vitConsume = reader.GetInt32(2);
								int clickCount = reader.GetInt32(3);
								int secondGain = reader.GetInt32(4);
								string cameraParam = reader.GetString(5);
								long multiId = reader.GetInt64(6);
								string playModeIcon = reader.GetString(7);
							long[] props = (long[])reader.GetArrayData(8, 17);
								int exhibitTotal = reader.GetInt32(9);
								int exhibitTask = reader.GetInt32(10);
								int playMode = reader.GetInt32(11);
								string exhibitGroupId = reader.GetString(12);
								string sceneInfo = reader.GetString(13);
								long dropId = reader.GetInt64(14);
								int outputVit = reader.GetInt32(15);
								int outputCash = reader.GetInt32(16);
								int outputExp = reader.GetInt32(17);
								int outputMoney = reader.GetInt32(18);
								int difficulty = reader.GetInt32(19);
								string name = reader.GetString(20);
								string thumbnail = reader.GetString(21);
								string descs = reader.GetString(22);
		
				ConfScene	new_obj_ConfScene = new ConfScene( 		 id,
 		 keyExhibit,
 		 vitConsume,
 		 clickCount,
 		 secondGain,
 		 cameraParam,
 		 multiId,
 		 playModeIcon,
 		 props,
 		 exhibitTotal,
 		 exhibitTask,
 		 playMode,
 		 exhibitGroupId,
 		 sceneInfo,
 		 dropId,
 		 outputVit,
 		 outputCash,
 		 outputExp,
 		 outputMoney,
 		 difficulty,
 		 name,
 		 thumbnail,
			descs
			);
		
                 return new_obj_ConfScene;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Scene");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfScene _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}