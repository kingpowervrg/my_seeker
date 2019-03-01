using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from building.xls
	/// </summary>
	public  class  ConfBuilding
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfBuilding>  cacheArray = new List<ConfBuilding>();
		
		public static List<ConfBuilding> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfBuilding()
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
			public readonly string sceneIds;
			public readonly string taskIds;
			public readonly string anchors;
			public readonly string descs;
			public readonly int accessibles;
			public readonly int propNum;
			public readonly long propId;
			public readonly int coin;
			public readonly int cash;
			public readonly long unlockTask;
			public readonly int unlockLevel;
			public readonly string[] unlockMaterial;
			public readonly string[] unlockResource;
			public readonly string lockResource;
			public readonly string stateName;
			public readonly string name;

		public ConfBuilding(  		long id,
 		string sceneIds,
 		string taskIds,
 		string anchors,
 		string descs,
 		int accessibles,
 		int propNum,
 		long propId,
 		int coin,
 		int cash,
 		long unlockTask,
 		int unlockLevel,
 		string[] unlockMaterial,
 		string[] unlockResource,
 		string lockResource,
 		string stateName,
 		string name){
			 this.id = id;
			 this.sceneIds = sceneIds;
			 this.taskIds = taskIds;
			 this.anchors = anchors;
			 this.descs = descs;
			 this.accessibles = accessibles;
			 this.propNum = propNum;
			 this.propId = propId;
			 this.coin = coin;
			 this.cash = cash;
			 this.unlockTask = unlockTask;
			 this.unlockLevel = unlockLevel;
			 this.unlockMaterial = unlockMaterial;
			 this.unlockResource = unlockResource;
			 this.lockResource = lockResource;
			 this.stateName = stateName;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfBuilding> dic = new Dictionary<long, ConfBuilding>();
		
		public static bool GetConfig( long id, out ConfBuilding config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Building", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Building 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfBuilding Get(long id)
        {
			ConfBuilding config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfBuilding config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Building", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Building 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfBuilding> list)
        {
            list = new List<ConfBuilding>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Building", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfBuilding config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Building not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Building key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfBuilding> list)
        {
            list = new List<ConfBuilding>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Building", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfBuilding config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Building not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Building condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfBuilding GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								string sceneIds = reader.GetString(1);
								string taskIds = reader.GetString(2);
								string anchors = reader.GetString(3);
								string descs = reader.GetString(4);
								int accessibles = reader.GetInt32(5);
								int propNum = reader.GetInt32(6);
								long propId = reader.GetInt64(7);
								int coin = reader.GetInt32(8);
								int cash = reader.GetInt32(9);
								long unlockTask = reader.GetInt64(10);
								int unlockLevel = reader.GetInt32(11);
							string[] unlockMaterial = (string[])reader.GetArrayData(12, 12);
							string[] unlockResource = (string[])reader.GetArrayData(13, 12);
								string lockResource = reader.GetString(14);
								string stateName = reader.GetString(15);
								string name = reader.GetString(16);
		
				ConfBuilding	new_obj_ConfBuilding = new ConfBuilding( 		 id,
 		 sceneIds,
 		 taskIds,
 		 anchors,
 		 descs,
 		 accessibles,
 		 propNum,
 		 propId,
 		 coin,
 		 cash,
 		 unlockTask,
 		 unlockLevel,
 		 unlockMaterial,
 		 unlockResource,
 		 lockResource,
 		 stateName,
			name
			);
		
                 return new_obj_ConfBuilding;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Building");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfBuilding _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}