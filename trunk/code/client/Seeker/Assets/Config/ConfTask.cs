using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from task.xls
	/// </summary>
	public  class  ConfTask
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfTask>  cacheArray = new List<ConfTask>();
		
		public static List<ConfTask> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfTask()
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
			public readonly string keyClueIcon;
			public readonly string transactor;
			public readonly int taskWeight;
			public readonly string loopManIcon;
			public readonly string loopBackIcon;
			public readonly string anchor;
			public readonly bool autoSkip;
			public readonly long[] sceneids;
			public readonly string breviary;
			public readonly string backgroundIcon;
			public readonly long[] building;
			public readonly long nextTaskId;
			public readonly long rewardTitleId;
			public readonly int[] rewardPropNums;
			public readonly long[] rewardPropIds;
			public readonly int rewardVit;
			public readonly int rewardExp;
			public readonly int rewardCash;
			public readonly int rewardCoin;
			public readonly int rewardType;
			public readonly long dialogEnd;
			public readonly long dialogBegin;
			public readonly long conditionCartoon;
			public readonly long conditionJigsaw;
			public readonly int[] conditionExhibitsNum;
			public readonly long[] conditionExhibits;
			public readonly long conditionSceneId;
			public readonly long conditionEventId;
			public readonly long conditionDialogueId;
			public readonly long conditionFindId;
			public readonly long conditionReasoningId;
			public readonly int[] conditionPropNums;
			public readonly long[] conditionPropExIds;
			public readonly long[] conditionPropIds;
			public readonly int conditionLevel;
			public readonly string triggerValue;
			public readonly int triggerKey;
			public readonly int type;
			public readonly string levelLimit;
			public readonly string remarks;
			public readonly string descs;
			public readonly string name;

		public ConfTask(  		long id,
 		string keyClueIcon,
 		string transactor,
 		int taskWeight,
 		string loopManIcon,
 		string loopBackIcon,
 		string anchor,
 		bool autoSkip,
 		long[] sceneids,
 		string breviary,
 		string backgroundIcon,
 		long[] building,
 		long nextTaskId,
 		long rewardTitleId,
 		int[] rewardPropNums,
 		long[] rewardPropIds,
 		int rewardVit,
 		int rewardExp,
 		int rewardCash,
 		int rewardCoin,
 		int rewardType,
 		long dialogEnd,
 		long dialogBegin,
 		long conditionCartoon,
 		long conditionJigsaw,
 		int[] conditionExhibitsNum,
 		long[] conditionExhibits,
 		long conditionSceneId,
 		long conditionEventId,
 		long conditionDialogueId,
 		long conditionFindId,
 		long conditionReasoningId,
 		int[] conditionPropNums,
 		long[] conditionPropExIds,
 		long[] conditionPropIds,
 		int conditionLevel,
 		string triggerValue,
 		int triggerKey,
 		int type,
 		string levelLimit,
 		string remarks,
 		string descs,
 		string name){
			 this.id = id;
			 this.keyClueIcon = keyClueIcon;
			 this.transactor = transactor;
			 this.taskWeight = taskWeight;
			 this.loopManIcon = loopManIcon;
			 this.loopBackIcon = loopBackIcon;
			 this.anchor = anchor;
			 this.autoSkip = autoSkip;
			 this.sceneids = sceneids;
			 this.breviary = breviary;
			 this.backgroundIcon = backgroundIcon;
			 this.building = building;
			 this.nextTaskId = nextTaskId;
			 this.rewardTitleId = rewardTitleId;
			 this.rewardPropNums = rewardPropNums;
			 this.rewardPropIds = rewardPropIds;
			 this.rewardVit = rewardVit;
			 this.rewardExp = rewardExp;
			 this.rewardCash = rewardCash;
			 this.rewardCoin = rewardCoin;
			 this.rewardType = rewardType;
			 this.dialogEnd = dialogEnd;
			 this.dialogBegin = dialogBegin;
			 this.conditionCartoon = conditionCartoon;
			 this.conditionJigsaw = conditionJigsaw;
			 this.conditionExhibitsNum = conditionExhibitsNum;
			 this.conditionExhibits = conditionExhibits;
			 this.conditionSceneId = conditionSceneId;
			 this.conditionEventId = conditionEventId;
			 this.conditionDialogueId = conditionDialogueId;
			 this.conditionFindId = conditionFindId;
			 this.conditionReasoningId = conditionReasoningId;
			 this.conditionPropNums = conditionPropNums;
			 this.conditionPropExIds = conditionPropExIds;
			 this.conditionPropIds = conditionPropIds;
			 this.conditionLevel = conditionLevel;
			 this.triggerValue = triggerValue;
			 this.triggerKey = triggerKey;
			 this.type = type;
			 this.levelLimit = levelLimit;
			 this.remarks = remarks;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfTask> dic = new Dictionary<long, ConfTask>();
		
		public static bool GetConfig( long id, out ConfTask config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Task", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Task 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfTask Get(long id)
        {
			ConfTask config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfTask config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Task", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Task 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfTask> list)
        {
            list = new List<ConfTask>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Task", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfTask config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Task not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Task key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfTask> list)
        {
            list = new List<ConfTask>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Task", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfTask config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Task not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Task condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfTask GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string keyClueIcon = reader.GetString(1);
								string transactor = reader.GetString(2);
								int taskWeight = reader.GetInt32(3);
								string loopManIcon = reader.GetString(4);
								string loopBackIcon = reader.GetString(5);
								string anchor = reader.GetString(6);
								bool autoSkip = reader.GetBoolean(7);
							long[] sceneids = (long[])reader.GetArrayData(8, 17);
								string breviary = reader.GetString(9);
								string backgroundIcon = reader.GetString(10);
							long[] building = (long[])reader.GetArrayData(11, 17);
								long nextTaskId = reader.GetInt64(12);
								long rewardTitleId = reader.GetInt64(13);
							int[] rewardPropNums = (int[])reader.GetArrayData(14, 11);
							long[] rewardPropIds = (long[])reader.GetArrayData(15, 17);
								int rewardVit = reader.GetInt32(16);
								int rewardExp = reader.GetInt32(17);
								int rewardCash = reader.GetInt32(18);
								int rewardCoin = reader.GetInt32(19);
								int rewardType = reader.GetInt32(20);
								long dialogEnd = reader.GetInt64(21);
								long dialogBegin = reader.GetInt64(22);
								long conditionCartoon = reader.GetInt64(23);
								long conditionJigsaw = reader.GetInt64(24);
							int[] conditionExhibitsNum = (int[])reader.GetArrayData(25, 11);
							long[] conditionExhibits = (long[])reader.GetArrayData(26, 17);
								long conditionSceneId = reader.GetInt64(27);
								long conditionEventId = reader.GetInt64(28);
								long conditionDialogueId = reader.GetInt64(29);
								long conditionFindId = reader.GetInt64(30);
								long conditionReasoningId = reader.GetInt64(31);
							int[] conditionPropNums = (int[])reader.GetArrayData(32, 11);
							long[] conditionPropExIds = (long[])reader.GetArrayData(33, 17);
							long[] conditionPropIds = (long[])reader.GetArrayData(34, 17);
								int conditionLevel = reader.GetInt32(35);
								string triggerValue = reader.GetString(36);
								int triggerKey = reader.GetInt32(37);
								int type = reader.GetInt32(38);
								string levelLimit = reader.GetString(39);
								string remarks = reader.GetString(40);
								string descs = reader.GetString(41);
								string name = reader.GetString(42);
		
				ConfTask	new_obj_ConfTask = new ConfTask( 		 id,
 		 keyClueIcon,
 		 transactor,
 		 taskWeight,
 		 loopManIcon,
 		 loopBackIcon,
 		 anchor,
 		 autoSkip,
 		 sceneids,
 		 breviary,
 		 backgroundIcon,
 		 building,
 		 nextTaskId,
 		 rewardTitleId,
 		 rewardPropNums,
 		 rewardPropIds,
 		 rewardVit,
 		 rewardExp,
 		 rewardCash,
 		 rewardCoin,
 		 rewardType,
 		 dialogEnd,
 		 dialogBegin,
 		 conditionCartoon,
 		 conditionJigsaw,
 		 conditionExhibitsNum,
 		 conditionExhibits,
 		 conditionSceneId,
 		 conditionEventId,
 		 conditionDialogueId,
 		 conditionFindId,
 		 conditionReasoningId,
 		 conditionPropNums,
 		 conditionPropExIds,
 		 conditionPropIds,
 		 conditionLevel,
 		 triggerValue,
 		 triggerKey,
 		 type,
 		 levelLimit,
 		 remarks,
 		 descs,
			name
			);
		
                 return new_obj_ConfTask;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Task");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfTask _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}