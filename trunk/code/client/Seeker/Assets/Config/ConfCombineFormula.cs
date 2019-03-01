using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from combineformula.xls
	/// </summary>
	public  class  ConfCombineFormula
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfCombineFormula>  cacheArray = new List<ConfCombineFormula>();
		
		public static List<ConfCombineFormula> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfCombineFormula()
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
			public readonly int mixLimit;
			public readonly long dropGroupId;
			public readonly int serialNumber;
			public readonly int outputCount;
			public readonly long outputId;
			public readonly int rate;
			public readonly int type;
			public readonly int special4Count;
			public readonly long specialPropId4;
			public readonly int special3Count;
			public readonly long specialPropId3;
			public readonly int special2Count;
			public readonly long specialPropId2;
			public readonly int special1Count;
			public readonly long specialPropId1;
			public readonly long propId6;
			public readonly long propId5;
			public readonly long propId4;
			public readonly long propId3;
			public readonly long propId2;
			public readonly long propId1;
			public readonly string descs;
			public readonly string name;

		public ConfCombineFormula(  		long id,
 		int mixLimit,
 		long dropGroupId,
 		int serialNumber,
 		int outputCount,
 		long outputId,
 		int rate,
 		int type,
 		int special4Count,
 		long specialPropId4,
 		int special3Count,
 		long specialPropId3,
 		int special2Count,
 		long specialPropId2,
 		int special1Count,
 		long specialPropId1,
 		long propId6,
 		long propId5,
 		long propId4,
 		long propId3,
 		long propId2,
 		long propId1,
 		string descs,
 		string name){
			 this.id = id;
			 this.mixLimit = mixLimit;
			 this.dropGroupId = dropGroupId;
			 this.serialNumber = serialNumber;
			 this.outputCount = outputCount;
			 this.outputId = outputId;
			 this.rate = rate;
			 this.type = type;
			 this.special4Count = special4Count;
			 this.specialPropId4 = specialPropId4;
			 this.special3Count = special3Count;
			 this.specialPropId3 = specialPropId3;
			 this.special2Count = special2Count;
			 this.specialPropId2 = specialPropId2;
			 this.special1Count = special1Count;
			 this.specialPropId1 = specialPropId1;
			 this.propId6 = propId6;
			 this.propId5 = propId5;
			 this.propId4 = propId4;
			 this.propId3 = propId3;
			 this.propId2 = propId2;
			 this.propId1 = propId1;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfCombineFormula> dic = new Dictionary<long, ConfCombineFormula>();
		
		public static bool GetConfig( long id, out ConfCombineFormula config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_CombineFormula", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfCombineFormula Get(long id)
        {
			ConfCombineFormula config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfCombineFormula config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CombineFormula", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfCombineFormula> list)
        {
            list = new List<ConfCombineFormula>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_CombineFormula", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfCombineFormula config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfCombineFormula> list)
        {
            list = new List<ConfCombineFormula>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_CombineFormula", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfCombineFormula config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("CombineFormula condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfCombineFormula GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int mixLimit = reader.GetInt32(1);
								long dropGroupId = reader.GetInt64(2);
								int serialNumber = reader.GetInt32(3);
								int outputCount = reader.GetInt32(4);
								long outputId = reader.GetInt64(5);
								int rate = reader.GetInt32(6);
								int type = reader.GetInt32(7);
								int special4Count = reader.GetInt32(8);
								long specialPropId4 = reader.GetInt64(9);
								int special3Count = reader.GetInt32(10);
								long specialPropId3 = reader.GetInt64(11);
								int special2Count = reader.GetInt32(12);
								long specialPropId2 = reader.GetInt64(13);
								int special1Count = reader.GetInt32(14);
								long specialPropId1 = reader.GetInt64(15);
								long propId6 = reader.GetInt64(16);
								long propId5 = reader.GetInt64(17);
								long propId4 = reader.GetInt64(18);
								long propId3 = reader.GetInt64(19);
								long propId2 = reader.GetInt64(20);
								long propId1 = reader.GetInt64(21);
								string descs = reader.GetString(22);
								string name = reader.GetString(23);
		
				ConfCombineFormula	new_obj_ConfCombineFormula = new ConfCombineFormula( 		 id,
 		 mixLimit,
 		 dropGroupId,
 		 serialNumber,
 		 outputCount,
 		 outputId,
 		 rate,
 		 type,
 		 special4Count,
 		 specialPropId4,
 		 special3Count,
 		 specialPropId3,
 		 special2Count,
 		 specialPropId2,
 		 special1Count,
 		 specialPropId1,
 		 propId6,
 		 propId5,
 		 propId4,
 		 propId3,
 		 propId2,
 		 propId1,
 		 descs,
			name
			);
		
                 return new_obj_ConfCombineFormula;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_CombineFormula");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfCombineFormula _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}