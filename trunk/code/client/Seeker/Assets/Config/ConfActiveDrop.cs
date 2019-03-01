using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from activedrop.xls
	/// </summary>
	public  class  ConfActiveDrop
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfActiveDrop>  cacheArray = new List<ConfActiveDrop>();
		
		public static List<ConfActiveDrop> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfActiveDrop()
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
			public readonly long[] sceneids;
			public readonly int[] rate;
			public readonly int[] nums;
			public readonly long[] propids;
			public readonly long rewardid;
			public readonly string warmupSource;
			public readonly string backgroundSource;
			public readonly string tips;
			public readonly string rewardSource;
			public readonly string sceneDes;
			public readonly string collectDes;
			public readonly string description;
			public readonly string name;

		public ConfActiveDrop(  		long id,
 		long[] sceneids,
 		int[] rate,
 		int[] nums,
 		long[] propids,
 		long rewardid,
 		string warmupSource,
 		string backgroundSource,
 		string tips,
 		string rewardSource,
 		string sceneDes,
 		string collectDes,
 		string description,
 		string name){
			 this.id = id;
			 this.sceneids = sceneids;
			 this.rate = rate;
			 this.nums = nums;
			 this.propids = propids;
			 this.rewardid = rewardid;
			 this.warmupSource = warmupSource;
			 this.backgroundSource = backgroundSource;
			 this.tips = tips;
			 this.rewardSource = rewardSource;
			 this.sceneDes = sceneDes;
			 this.collectDes = collectDes;
			 this.description = description;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfActiveDrop> dic = new Dictionary<long, ConfActiveDrop>();
		
		public static bool GetConfig( long id, out ConfActiveDrop config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ActiveDrop", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfActiveDrop Get(long id)
        {
			ConfActiveDrop config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfActiveDrop config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ActiveDrop", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfActiveDrop> list)
        {
            list = new List<ConfActiveDrop>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ActiveDrop", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfActiveDrop config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfActiveDrop> list)
        {
            list = new List<ConfActiveDrop>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ActiveDrop", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfActiveDrop config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ActiveDrop condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfActiveDrop GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							long[] sceneids = (long[])reader.GetArrayData(1, 17);
							int[] rate = (int[])reader.GetArrayData(2, 11);
							int[] nums = (int[])reader.GetArrayData(3, 11);
							long[] propids = (long[])reader.GetArrayData(4, 17);
								long rewardid = reader.GetInt64(5);
								string warmupSource = reader.GetString(6);
								string backgroundSource = reader.GetString(7);
								string tips = reader.GetString(8);
								string rewardSource = reader.GetString(9);
								string sceneDes = reader.GetString(10);
								string collectDes = reader.GetString(11);
								string description = reader.GetString(12);
								string name = reader.GetString(13);
		
				ConfActiveDrop	new_obj_ConfActiveDrop = new ConfActiveDrop( 		 id,
 		 sceneids,
 		 rate,
 		 nums,
 		 propids,
 		 rewardid,
 		 warmupSource,
 		 backgroundSource,
 		 tips,
 		 rewardSource,
 		 sceneDes,
 		 collectDes,
 		 description,
			name
			);
		
                 return new_obj_ConfActiveDrop;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ActiveDrop");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfActiveDrop _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}