using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from skill.xls
	/// </summary>
	public  class  ConfSkill
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfSkill>  cacheArray = new List<ConfSkill>();
		
		public static List<ConfSkill> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfSkill()
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
			public readonly string descs;
			public readonly string name;
			public readonly string icon;
			public readonly int duration;
			public readonly int cd;
			public readonly int gain;
			public readonly int rate;
			public readonly int type;
			public readonly int phase;
			public readonly int occasion;

		public ConfSkill(  		long id,
 		string descs,
 		string name,
 		string icon,
 		int duration,
 		int cd,
 		int gain,
 		int rate,
 		int type,
 		int phase,
 		int occasion){
			 this.id = id;
			 this.descs = descs;
			 this.name = name;
			 this.icon = icon;
			 this.duration = duration;
			 this.cd = cd;
			 this.gain = gain;
			 this.rate = rate;
			 this.type = type;
			 this.phase = phase;
			 this.occasion = occasion;
		}
			
		private static Dictionary<long, ConfSkill> dic = new Dictionary<long, ConfSkill>();
		
		public static bool GetConfig( long id, out ConfSkill config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Skill", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Skill 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfSkill Get(long id)
        {
			ConfSkill config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfSkill config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Skill", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Skill 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfSkill> list)
        {
            list = new List<ConfSkill>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Skill", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfSkill config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Skill not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Skill key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfSkill> list)
        {
            list = new List<ConfSkill>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Skill", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfSkill config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Skill not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Skill condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfSkill GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string descs = reader.GetString(1);
								string name = reader.GetString(2);
								string icon = reader.GetString(3);
								int duration = reader.GetInt32(4);
								int cd = reader.GetInt32(5);
								int gain = reader.GetInt32(6);
								int rate = reader.GetInt32(7);
								int type = reader.GetInt32(8);
								int phase = reader.GetInt32(9);
								int occasion = reader.GetInt32(10);
		
				ConfSkill	new_obj_ConfSkill = new ConfSkill( 		 id,
 		 descs,
 		 name,
 		 icon,
 		 duration,
 		 cd,
 		 gain,
 		 rate,
 		 type,
 		 phase,
			occasion
			);
		
                 return new_obj_ConfSkill;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Skill");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfSkill _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}