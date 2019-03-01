using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from node.xls
	/// </summary>
	public  class  ConfNode
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfNode>  cacheArray = new List<ConfNode>();
		
		public static List<ConfNode> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfNode()
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
			public readonly int answer;
			public readonly string[] cluedescs;
			public readonly string[] clueicons;
			public readonly string[] cluenames;
			public readonly string feedback;
			public readonly string descs;
			public readonly string name;

		public ConfNode(  		long id,
 		int answer,
 		string[] cluedescs,
 		string[] clueicons,
 		string[] cluenames,
 		string feedback,
 		string descs,
 		string name){
			 this.id = id;
			 this.answer = answer;
			 this.cluedescs = cluedescs;
			 this.clueicons = clueicons;
			 this.cluenames = cluenames;
			 this.feedback = feedback;
			 this.descs = descs;
			 this.name = name;
		}
			
		private static Dictionary<long, ConfNode> dic = new Dictionary<long, ConfNode>();
		
		public static bool GetConfig( long id, out ConfNode config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_Node", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("Node 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfNode Get(long id)
        {
			ConfNode config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfNode config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Node", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("Node 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfNode> list)
        {
            list = new List<ConfNode>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_Node", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfNode config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("Node not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Node key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfNode> list)
        {
            list = new List<ConfNode>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_Node", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfNode config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("Node not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("Node condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfNode GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
								int answer = reader.GetInt32(1);
							string[] cluedescs = (string[])reader.GetArrayData(2, 12);
							string[] clueicons = (string[])reader.GetArrayData(3, 12);
							string[] cluenames = (string[])reader.GetArrayData(4, 12);
								string feedback = reader.GetString(5);
								string descs = reader.GetString(6);
								string name = reader.GetString(7);
		
				ConfNode	new_obj_ConfNode = new ConfNode( 		 id,
 		 answer,
 		 cluedescs,
 		 clueicons,
 		 cluenames,
 		 feedback,
 		 descs,
			name
			);
		
                 return new_obj_ConfNode;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_Node");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfNode _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}