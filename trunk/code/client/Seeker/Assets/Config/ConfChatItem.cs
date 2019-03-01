using System;
using System.Collections.Generic;
using SqliteDriver;

namespace SeekerGame
{
	/// <summary>
	/// Generated from chat.xls
	/// </summary>
	public  class  ConfChatItem
	{
		public static bool resLoaded = false;
		
		public static bool cacheLoaded = false;
		 
		private static List<ConfChatItem>  cacheArray = new List<ConfChatItem>();
		
		public static List<ConfChatItem> array 
		{
			get
            {
                GetArrrayList();
                return cacheArray;
            }
		}
		
		public ConfChatItem()
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
			public readonly string[] jumpcontens;
			public readonly long[] jumpids;
			public readonly int jumptype;
			public readonly string[] propids;
			public readonly string content;
			public readonly string name;
			public readonly int iconPosition;
			public readonly string icon;
			public readonly string apellation;
			public readonly string description;
			public readonly long chatId;

		public ConfChatItem(  		long id,
 		string[] jumpcontens,
 		long[] jumpids,
 		int jumptype,
 		string[] propids,
 		string content,
 		string name,
 		int iconPosition,
 		string icon,
 		string apellation,
 		string description,
 		long chatId){
			 this.id = id;
			 this.jumpcontens = jumpcontens;
			 this.jumpids = jumpids;
			 this.jumptype = jumptype;
			 this.propids = propids;
			 this.content = content;
			 this.name = name;
			 this.iconPosition = iconPosition;
			 this.icon = icon;
			 this.apellation = apellation;
			 this.description = description;
			 this.chatId = chatId;
		}
			
		private static Dictionary<long, ConfChatItem> dic = new Dictionary<long, ConfChatItem>();
		
		public static bool GetConfig( long id, out ConfChatItem config )
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
			DataTable  sqReader= SQLiteHelper.Instance().GetSelectWhere("conf_ChatItem", id);
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
                    SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem 表找不到SN={0} 的数据\n{1}", id, ex));
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

		public static ConfChatItem Get(long id)
        {
			ConfChatItem config;
             bool _exist = GetConfig(id, out config);

             return config;
        }

         public static bool GetConfig( string fieldName, object fieldValue, out ConfChatItem config )
        {
			DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ChatItem", fieldName, fieldValue);
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
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem 表找不到列={0} 值={1}的数据\n{2}", fieldName, fieldValue, ex));
				}
			   config = null;
			   return false;

			}
            config = null;
            return false;
        }

        public static bool GetConfig(string fieldName, object fieldValue, out List<ConfChatItem> list)
        {
            list = new List<ConfChatItem>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereCondition("conf_ChatItem", fieldName, fieldValue);
            if (sqReader != null)
            {
                try
                {
                    ConfChatItem config;
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
						SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem not found key={0} value={1}", fieldName, fieldValue));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem key={0} value={1} \n {2}", fieldName, fieldValue, ex));
                }
                return false;

            }
            return false;
        }

        public static bool GetConfigByCondition(string condition, out List<ConfChatItem> list)
        {
            list = new List<ConfChatItem>();

            DataTable sqReader = SQLiteHelper.Instance().GetSelectWhereConditionStr("conf_ChatItem", condition);
            if (sqReader != null)
            {
                try
                {
                    ConfChatItem config;
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
                            SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem not found condition={0}", condition));
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
					SqliteDriver.SQLiteHelper.OnError(string.Format("ChatItem condition={0} \n {2}", condition, ex));
                }
                return false;

            }
            return false;
        }

        public static void Clear()
        {
			cacheArray.Clear();
        }
		
		 private static ConfChatItem GetConfByDic(DataTable reader)
         {
		 
								long id = reader.GetInt64(0);
							string[] jumpcontens = (string[])reader.GetArrayData(1, 12);
							long[] jumpids = (long[])reader.GetArrayData(2, 17);
								int jumptype = reader.GetInt32(3);
							string[] propids = (string[])reader.GetArrayData(4, 12);
								string content = reader.GetString(5);
								string name = reader.GetString(6);
								int iconPosition = reader.GetInt32(7);
								string icon = reader.GetString(8);
								string apellation = reader.GetString(9);
								string description = reader.GetString(10);
								long chatId = reader.GetInt64(11);
		
				ConfChatItem	new_obj_ConfChatItem = new ConfChatItem( 		 id,
 		 jumpcontens,
 		 jumpids,
 		 jumptype,
 		 propids,
 		 content,
 		 name,
 		 iconPosition,
 		 icon,
 		 apellation,
 		 description,
			chatId
			);
		
                 return new_obj_ConfChatItem;
         }
		 
		 private static void GetArrrayList()
        {
            if(cacheArray.Count <= 0)
            {
			    DataTable  sqReader = SQLiteHelper.Instance().GetReadFullTable("conf_ChatItem");
                if(sqReader != null)
                {
                    while (sqReader.Read())
                    {
						ConfChatItem _conf= GetConfByDic(sqReader);
						cacheArray.Add(_conf);
						dic[_conf.id] = _conf;
                    }
                    resLoaded = true;
                }
            }
        }

	}
}