using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from officer.xls
	/// </summary>
	public  class  ConfOfficer
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfOfficer>  cacheArray = new List<ConfOfficer>();
		
		public static List<ConfOfficer> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfOfficer()
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
			public readonly int upGainSceneSceond;
			public readonly int upGainSceneVit;
			public readonly string hollowPortrait;
			public readonly long level10SkillId;
			public readonly long up10Formula;
			public readonly long level9SkillId;
			public readonly long up9Formula;
			public readonly long level8SkillId;
			public readonly long up8Formula;
			public readonly long level7SkillId;
			public readonly long up7Formula;
			public readonly long level6SkillId;
			public readonly long up6Formula;
			public readonly long level5SkillId;
			public readonly long up5Formula;
			public readonly long level4SkillId;
			public readonly long up4Formula;
			public readonly long level3SkillId;
			public readonly long up3Formula;
			public readonly long level2SkillId;
			public readonly long up2Formula;
			public readonly long skillId;
			public readonly long unlockFormula;
			public readonly int upGainMemory;
			public readonly int upGainAttention;
			public readonly int upGainWillpower;
			public readonly int upGainOutsight;
			public readonly int secondGain;
			public readonly int vitConsume;
			public readonly int memory;
			public readonly int attention;
			public readonly int willpower;
			public readonly int outsight;
			public readonly int quality;
			public readonly int[] features;
			public readonly int profession;
			public readonly string icon;
			public readonly string portrait;
			public readonly string descs;
			public readonly string name;

		public ConfOfficer(  		long id,
 		int upGainSceneSceond,
 		int upGainSceneVit,
 		string hollowPortrait,
 		long level10SkillId,
 		long up10Formula,
 		long level9SkillId,
 		long up9Formula,
 		long level8SkillId,
 		long up8Formula,
 		long level7SkillId,
 		long up7Formula,
 		long level6SkillId,
 		long up6Formula,
 		long level5SkillId,
 		long up5Formula,
 		long level4SkillId,
 		long up4Formula,
 		long level3SkillId,
 		long up3Formula,
 		long level2SkillId,
 		long up2Formula,
 		long skillId,
 		long unlockFormula,
 		int upGainMemory,
 		int upGainAttention,
 		int upGainWillpower,
 		int upGainOutsight,
 		int secondGain,
 		int vitConsume,
 		int memory,
 		int attention,
 		int willpower,
 		int outsight,
 		int quality,
 		int[] features,
 		int profession,
 		string icon,
 		string portrait,
 		string descs,
 		string name){
			 this.id = id;
			 this.upGainSceneSceond = upGainSceneSceond;
			 this.upGainSceneVit = upGainSceneVit;
			 this.hollowPortrait = hollowPortrait;
			 this.level10SkillId = level10SkillId;
			 this.up10Formula = up10Formula;
			 this.level9SkillId = level9SkillId;
			 this.up9Formula = up9Formula;
			 this.level8SkillId = level8SkillId;
			 this.up8Formula = up8Formula;
			 this.level7SkillId = level7SkillId;
			 this.up7Formula = up7Formula;
			 this.level6SkillId = level6SkillId;
			 this.up6Formula = up6Formula;
			 this.level5SkillId = level5SkillId;
			 this.up5Formula = up5Formula;
			 this.level4SkillId = level4SkillId;
			 this.up4Formula = up4Formula;
			 this.level3SkillId = level3SkillId;
			 this.up3Formula = up3Formula;
			 this.level2SkillId = level2SkillId;
			 this.up2Formula = up2Formula;
			 this.skillId = skillId;
			 this.unlockFormula = unlockFormula;
			 this.upGainMemory = upGainMemory;
			 this.upGainAttention = upGainAttention;
			 this.upGainWillpower = upGainWillpower;
			 this.upGainOutsight = upGainOutsight;
			 this.secondGain = secondGain;
			 this.vitConsume = vitConsume;
			 this.memory = memory;
			 this.attention = attention;
			 this.willpower = willpower;
			 this.outsight = outsight;
			 this.quality = quality;
			 this.features = features;
			 this.profession = profession;
			 this.icon = icon;
			 this.portrait = portrait;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfOfficer> dic = new Dictionary<long, ConfOfficer>();
		
		public static bool GetConfig( long id, out ConfOfficer config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Officer", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Officer 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfOfficer Get(long id)
        {
			ConfOfficer config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfOfficer config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Officer", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Officer 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfOfficer> list)
        {
            list = new List<ConfOfficer>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Officer", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfOfficer config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Officer not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Officer key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfOfficer> list)
        {
            list = new List<ConfOfficer>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Officer", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfOfficer config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Officer not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Officer condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfOfficer GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int upGainSceneSceond = reader.GetInt32(1);
								int upGainSceneVit = reader.GetInt32(2);
								string hollowPortrait = reader.GetString(3);
								long level10SkillId = reader.GetInt64(4);
								long up10Formula = reader.GetInt64(5);
								long level9SkillId = reader.GetInt64(6);
								long up9Formula = reader.GetInt64(7);
								long level8SkillId = reader.GetInt64(8);
								long up8Formula = reader.GetInt64(9);
								long level7SkillId = reader.GetInt64(10);
								long up7Formula = reader.GetInt64(11);
								long level6SkillId = reader.GetInt64(12);
								long up6Formula = reader.GetInt64(13);
								long level5SkillId = reader.GetInt64(14);
								long up5Formula = reader.GetInt64(15);
								long level4SkillId = reader.GetInt64(16);
								long up4Formula = reader.GetInt64(17);
								long level3SkillId = reader.GetInt64(18);
								long up3Formula = reader.GetInt64(19);
								long level2SkillId = reader.GetInt64(20);
								long up2Formula = reader.GetInt64(21);
								long skillId = reader.GetInt64(22);
								long unlockFormula = reader.GetInt64(23);
								int upGainMemory = reader.GetInt32(24);
								int upGainAttention = reader.GetInt32(25);
								int upGainWillpower = reader.GetInt32(26);
								int upGainOutsight = reader.GetInt32(27);
								int secondGain = reader.GetInt32(28);
								int vitConsume = reader.GetInt32(29);
								int memory = reader.GetInt32(30);
								int attention = reader.GetInt32(31);
								int willpower = reader.GetInt32(32);
								int outsight = reader.GetInt32(33);
								int quality = reader.GetInt32(34);
							int[] features = (int[])reader.GetArrayData(35, 11);
								int profession = reader.GetInt32(36);
								string icon = reader.GetString(37);
								string portrait = reader.GetString(38);
								string descs = reader.GetString(39);
								string name = reader.GetString(40);
		
				ConfOfficer	new_obj_ConfOfficer = new ConfOfficer( 		 id,
 		 upGainSceneSceond,
 		 upGainSceneVit,
 		 hollowPortrait,
 		 level10SkillId,
 		 up10Formula,
 		 level9SkillId,
 		 up9Formula,
 		 level8SkillId,
 		 up8Formula,
 		 level7SkillId,
 		 up7Formula,
 		 level6SkillId,
 		 up6Formula,
 		 level5SkillId,
 		 up5Formula,
 		 level4SkillId,
 		 up4Formula,
 		 level3SkillId,
 		 up3Formula,
 		 level2SkillId,
 		 up2Formula,
 		 skillId,
 		 unlockFormula,
 		 upGainMemory,
 		 upGainAttention,
 		 upGainWillpower,
 		 upGainOutsight,
 		 secondGain,
 		 vitConsume,
 		 memory,
 		 attention,
 		 willpower,
 		 outsight,
 		 quality,
 		 features,
 		 profession,
 		 icon,
 		 portrait,
 		 descs,
			name
			);
		
                 return new_obj_ConfOfficer;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Officer");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfOfficer _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}