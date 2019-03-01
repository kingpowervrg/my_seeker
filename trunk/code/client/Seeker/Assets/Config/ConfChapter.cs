using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from chapter.xls
	/// </summary>
	public  class  ConfChapter
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfChapter>  cacheArray = new List<ConfChapter>();
		
		public static List<ConfChapter> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfChapter()
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
			public readonly long building;
			public readonly long[] taskIds;
			public readonly string document;
			public readonly long[] clueUnlockTaskIds;
			public readonly string[] clueIds;
			public readonly long[] actorUnlockTaskIds;
			public readonly long[] actorIds;
			public readonly long endSceneId;
			public readonly long startSceneId;
			public readonly long[] sceneUnlockTaskIds;
			public readonly long[] scenceIds;
			public readonly string cover;
			public readonly long nextChapterId;
			public readonly string remarks;
			public readonly string descs;
			public readonly string name;

		public ConfChapter(  		long id,
 		long building,
 		long[] taskIds,
 		string document,
 		long[] clueUnlockTaskIds,
 		string[] clueIds,
 		long[] actorUnlockTaskIds,
 		long[] actorIds,
 		long endSceneId,
 		long startSceneId,
 		long[] sceneUnlockTaskIds,
 		long[] scenceIds,
 		string cover,
 		long nextChapterId,
 		string remarks,
 		string descs,
 		string name){
			 this.id = id;
			 this.building = building;
			 this.taskIds = taskIds;
			 this.document = document;
			 this.clueUnlockTaskIds = clueUnlockTaskIds;
			 this.clueIds = clueIds;
			 this.actorUnlockTaskIds = actorUnlockTaskIds;
			 this.actorIds = actorIds;
			 this.endSceneId = endSceneId;
			 this.startSceneId = startSceneId;
			 this.sceneUnlockTaskIds = sceneUnlockTaskIds;
			 this.scenceIds = scenceIds;
			 this.cover = cover;
			 this.nextChapterId = nextChapterId;
			 this.remarks = remarks;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfChapter> dic = new Dictionary<long, ConfChapter>();
		
		public static bool GetConfig( long id, out ConfChapter config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Chapter", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfChapter Get(long id)
        {
			ConfChapter config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfChapter config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Chapter", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfChapter> list)
        {
            list = new List<ConfChapter>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Chapter", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfChapter config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfChapter> list)
        {
            list = new List<ConfChapter>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Chapter", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfChapter config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Chapter condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfChapter GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long building = reader.GetInt64(1);
							long[] taskIds = (long[])reader.GetArrayData(2, 17);
								string document = reader.GetString(3);
							long[] clueUnlockTaskIds = (long[])reader.GetArrayData(4, 17);
							string[] clueIds = (string[])reader.GetArrayData(5, 12);
							long[] actorUnlockTaskIds = (long[])reader.GetArrayData(6, 17);
							long[] actorIds = (long[])reader.GetArrayData(7, 17);
								long endSceneId = reader.GetInt64(8);
								long startSceneId = reader.GetInt64(9);
							long[] sceneUnlockTaskIds = (long[])reader.GetArrayData(10, 17);
							long[] scenceIds = (long[])reader.GetArrayData(11, 17);
								string cover = reader.GetString(12);
								long nextChapterId = reader.GetInt64(13);
								string remarks = reader.GetString(14);
								string descs = reader.GetString(15);
								string name = reader.GetString(16);
		
				ConfChapter	new_obj_ConfChapter = new ConfChapter( 		 id,
 		 building,
 		 taskIds,
 		 document,
 		 clueUnlockTaskIds,
 		 clueIds,
 		 actorUnlockTaskIds,
 		 actorIds,
 		 endSceneId,
 		 startSceneId,
 		 sceneUnlockTaskIds,
 		 scenceIds,
 		 cover,
 		 nextChapterId,
 		 remarks,
 		 descs,
			name
			);
		
                 return new_obj_ConfChapter;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Chapter");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfChapter _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}