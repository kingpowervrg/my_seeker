using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from jigsawscene.xls
	/// </summary>
	public  class  ConfJigsawScene
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfJigsawScene>  cacheArray = new List<ConfJigsawScene>();
		
		public static List<ConfJigsawScene> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfJigsawScene()
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
			public readonly int[] costPropNums;
			public readonly long[] costPropIds;
			public readonly int propId;
			public readonly int sceneInfoId;
			public readonly int num4;
			public readonly int type4;
			public readonly int percent4;
			public readonly int num3;
			public readonly int type3;
			public readonly int percent3;
			public readonly int num2;
			public readonly int type2;
			public readonly int percent2;
			public readonly int num1;
			public readonly int type1;
			public readonly int percent1;
			public readonly string taskIds;
			public readonly int difficulty;
			public readonly string thumbnail;
			public readonly string name;
			public readonly string desc2;
			public readonly string desc;

		public ConfJigsawScene(  		long id,
 		int[] costPropNums,
 		long[] costPropIds,
 		int propId,
 		int sceneInfoId,
 		int num4,
 		int type4,
 		int percent4,
 		int num3,
 		int type3,
 		int percent3,
 		int num2,
 		int type2,
 		int percent2,
 		int num1,
 		int type1,
 		int percent1,
 		string taskIds,
 		int difficulty,
 		string thumbnail,
 		string name,
 		string desc2,
 		string desc){
			 this.id = id;
			 this.costPropNums = costPropNums;
			 this.costPropIds = costPropIds;
			 this.propId = propId;
			 this.sceneInfoId = sceneInfoId;
			 this.num4 = num4;
			 this.type4 = type4;
			 this.percent4 = percent4;
			 this.num3 = num3;
			 this.type3 = type3;
			 this.percent3 = percent3;
			 this.num2 = num2;
			 this.type2 = type2;
			 this.percent2 = percent2;
			 this.num1 = num1;
			 this.type1 = type1;
			 this.percent1 = percent1;
			 this.taskIds = taskIds;
			 this.difficulty = difficulty;
			 this.thumbnail = thumbnail;
			 this.name = name;
			 this.desc2 = desc2;
			 this.desc = desc;
		}
			
		private static Dictionary<long, ConfJigsawScene> dic = new Dictionary<long, ConfJigsawScene>();
		
		public static bool GetConfig( long id, out ConfJigsawScene config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_JigsawScene", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfJigsawScene Get(long id)
        {
			ConfJigsawScene config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfJigsawScene config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_JigsawScene", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfJigsawScene> list)
        {
            list = new List<ConfJigsawScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_JigsawScene", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfJigsawScene config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfJigsawScene> list)
        {
            list = new List<ConfJigsawScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_JigsawScene", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfJigsawScene config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("JigsawScene condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfJigsawScene GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							int[] costPropNums = (int[])reader.GetArrayData(1, 11);
							long[] costPropIds = (long[])reader.GetArrayData(2, 17);
								int propId = reader.GetInt32(3);
								int sceneInfoId = reader.GetInt32(4);
								int num4 = reader.GetInt32(5);
								int type4 = reader.GetInt32(6);
								int percent4 = reader.GetInt32(7);
								int num3 = reader.GetInt32(8);
								int type3 = reader.GetInt32(9);
								int percent3 = reader.GetInt32(10);
								int num2 = reader.GetInt32(11);
								int type2 = reader.GetInt32(12);
								int percent2 = reader.GetInt32(13);
								int num1 = reader.GetInt32(14);
								int type1 = reader.GetInt32(15);
								int percent1 = reader.GetInt32(16);
								string taskIds = reader.GetString(17);
								int difficulty = reader.GetInt32(18);
								string thumbnail = reader.GetString(19);
								string name = reader.GetString(20);
								string desc2 = reader.GetString(21);
								string desc = reader.GetString(22);
		
				ConfJigsawScene	new_obj_ConfJigsawScene = new ConfJigsawScene( 		 id,
 		 costPropNums,
 		 costPropIds,
 		 propId,
 		 sceneInfoId,
 		 num4,
 		 type4,
 		 percent4,
 		 num3,
 		 type3,
 		 percent3,
 		 num2,
 		 type2,
 		 percent2,
 		 num1,
 		 type1,
 		 percent1,
 		 taskIds,
 		 difficulty,
 		 thumbnail,
 		 name,
 		 desc2,
			desc
			);
		
                 return new_obj_ConfJigsawScene;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_JigsawScene");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfJigsawScene _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}