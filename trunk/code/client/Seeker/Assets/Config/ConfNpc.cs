using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from npc.xls
	/// </summary>
	public  class  ConfNpc
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfNpc>  cacheArray = new List<ConfNpc>();
		
		public static List<ConfNpc> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfNpc()
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
			public readonly long dialogueId4;
			public readonly long branchDescsUnlockTaskId;
			public readonly string branchDescs;
			public readonly long dialogueId3;
			public readonly long caseDescsUnlockTaskId;
			public readonly string caseDescs;
			public readonly long dialogueId2;
			public readonly long backgroundUnlockTaskId;
			public readonly string background;
			public readonly long dialogueId;
			public readonly string otherFeatures;
			public readonly string race;
			public readonly string profession;
			public readonly string eyeColor;
			public readonly string shoeSize;
			public readonly string shape;
			public readonly string weight;
			public readonly string height;
			public readonly string horoscope;
			public readonly int sex;
			public readonly int age;
			public readonly string name;
			public readonly string icon;
			public readonly long unlockTaskId;
			public readonly int identityType;
			public readonly string identity;
			public readonly string remarks;

		public ConfNpc(  		long id,
 		long dialogueId4,
 		long branchDescsUnlockTaskId,
 		string branchDescs,
 		long dialogueId3,
 		long caseDescsUnlockTaskId,
 		string caseDescs,
 		long dialogueId2,
 		long backgroundUnlockTaskId,
 		string background,
 		long dialogueId,
 		string otherFeatures,
 		string race,
 		string profession,
 		string eyeColor,
 		string shoeSize,
 		string shape,
 		string weight,
 		string height,
 		string horoscope,
 		int sex,
 		int age,
 		string name,
 		string icon,
 		long unlockTaskId,
 		int identityType,
 		string identity,
 		string remarks){
			 this.id = id;
			 this.dialogueId4 = dialogueId4;
			 this.branchDescsUnlockTaskId = branchDescsUnlockTaskId;
			 this.branchDescs = branchDescs;
			 this.dialogueId3 = dialogueId3;
			 this.caseDescsUnlockTaskId = caseDescsUnlockTaskId;
			 this.caseDescs = caseDescs;
			 this.dialogueId2 = dialogueId2;
			 this.backgroundUnlockTaskId = backgroundUnlockTaskId;
			 this.background = background;
			 this.dialogueId = dialogueId;
			 this.otherFeatures = otherFeatures;
			 this.race = race;
			 this.profession = profession;
			 this.eyeColor = eyeColor;
			 this.shoeSize = shoeSize;
			 this.shape = shape;
			 this.weight = weight;
			 this.height = height;
			 this.horoscope = horoscope;
			 this.sex = sex;
			 this.age = age;
			 this.name = name;
			 this.icon = icon;
			 this.unlockTaskId = unlockTaskId;
			 this.identityType = identityType;
			 this.identity = identity;
			 this.remarks = remarks;
		}
			
		private static Dictionary<long, ConfNpc> dic = new Dictionary<long, ConfNpc>();
		
		public static bool GetConfig( long id, out ConfNpc config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Npc", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Npc 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfNpc Get(long id)
        {
			ConfNpc config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfNpc config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Npc", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Npc 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfNpc> list)
        {
            list = new List<ConfNpc>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Npc", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfNpc config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Npc not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Npc key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfNpc> list)
        {
            list = new List<ConfNpc>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Npc", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfNpc config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Npc not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Npc condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfNpc GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								long dialogueId4 = reader.GetInt64(1);
								long branchDescsUnlockTaskId = reader.GetInt64(2);
								string branchDescs = reader.GetString(3);
								long dialogueId3 = reader.GetInt64(4);
								long caseDescsUnlockTaskId = reader.GetInt64(5);
								string caseDescs = reader.GetString(6);
								long dialogueId2 = reader.GetInt64(7);
								long backgroundUnlockTaskId = reader.GetInt64(8);
								string background = reader.GetString(9);
								long dialogueId = reader.GetInt64(10);
								string otherFeatures = reader.GetString(11);
								string race = reader.GetString(12);
								string profession = reader.GetString(13);
								string eyeColor = reader.GetString(14);
								string shoeSize = reader.GetString(15);
								string shape = reader.GetString(16);
								string weight = reader.GetString(17);
								string height = reader.GetString(18);
								string horoscope = reader.GetString(19);
								int sex = reader.GetInt32(20);
								int age = reader.GetInt32(21);
								string name = reader.GetString(22);
								string icon = reader.GetString(23);
								long unlockTaskId = reader.GetInt64(24);
								int identityType = reader.GetInt32(25);
								string identity = reader.GetString(26);
								string remarks = reader.GetString(27);
		
				ConfNpc	new_obj_ConfNpc = new ConfNpc( 		 id,
 		 dialogueId4,
 		 branchDescsUnlockTaskId,
 		 branchDescs,
 		 dialogueId3,
 		 caseDescsUnlockTaskId,
 		 caseDescs,
 		 dialogueId2,
 		 backgroundUnlockTaskId,
 		 background,
 		 dialogueId,
 		 otherFeatures,
 		 race,
 		 profession,
 		 eyeColor,
 		 shoeSize,
 		 shape,
 		 weight,
 		 height,
 		 horoscope,
 		 sex,
 		 age,
 		 name,
 		 icon,
 		 unlockTaskId,
 		 identityType,
 		 identity,
			remarks
			);
		
                 return new_obj_ConfNpc;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Npc");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfNpc _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}