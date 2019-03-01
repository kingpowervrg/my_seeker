using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from cartoonscene.xls
	/// </summary>
	public  class  ConfCartoonScene
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfCartoonScene>  cacheArray = new List<ConfCartoonScene>();
		
		public static List<ConfCartoonScene> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfCartoonScene()
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
			public readonly int rewardVit;
			public readonly int rewardExp;
			public readonly int rewardCoin;
			public readonly int rewardCash;
			public readonly string[] sceneInfoIds;
			public readonly string thumbnail;
			public readonly string name;

		public ConfCartoonScene(  		long id,
 		int rewardVit,
 		int rewardExp,
 		int rewardCoin,
 		int rewardCash,
 		string[] sceneInfoIds,
 		string thumbnail,
 		string name){
			 this.id = id;
			 this.rewardVit = rewardVit;
			 this.rewardExp = rewardExp;
			 this.rewardCoin = rewardCoin;
			 this.rewardCash = rewardCash;
			 this.sceneInfoIds = sceneInfoIds;
			 this.thumbnail = thumbnail;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfCartoonScene> dic = new Dictionary<long, ConfCartoonScene>();
		
		public static bool GetConfig( long id, out ConfCartoonScene config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_CartoonScene", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfCartoonScene Get(long id)
        {
			ConfCartoonScene config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfCartoonScene config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CartoonScene", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfCartoonScene> list)
        {
            list = new List<ConfCartoonScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CartoonScene", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfCartoonScene config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfCartoonScene> list)
        {
            list = new List<ConfCartoonScene>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_CartoonScene", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfCartoonScene config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CartoonScene condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfCartoonScene GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int rewardVit = reader.GetInt32(1);
								int rewardExp = reader.GetInt32(2);
								int rewardCoin = reader.GetInt32(3);
								int rewardCash = reader.GetInt32(4);
							string[] sceneInfoIds = (string[])reader.GetArrayData(5, 12);
								string thumbnail = reader.GetString(6);
								string name = reader.GetString(7);
		
				ConfCartoonScene	new_obj_ConfCartoonScene = new ConfCartoonScene( 		 id,
 		 rewardVit,
 		 rewardExp,
 		 rewardCoin,
 		 rewardCash,
 		 sceneInfoIds,
 		 thumbnail,
			name
			);
		
                 return new_obj_ConfCartoonScene;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_CartoonScene");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfCartoonScene _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}