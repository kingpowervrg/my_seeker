using System;
using System.Collections.Generic;
namespace GOEngine.Implement
{
	public class IndexMapItem
	{
		public string name = string.Empty;
		public string bundlename = string.Empty;
		public long bundlelength = 0;

       //IndexMap分隔字符//
        private const string INDEX_MAP_SPLITER = ":";
		public override string ToString()
		{
            return name + INDEX_MAP_SPLITER
                    + bundlename + INDEX_MAP_SPLITER
						+ bundlelength;
		}

        public void FromString(string str)
		{
			//为了向下兼容，也识别" : "//
			string[] parts = str.Split(GetSpliterList(), StringSplitOptions.None);
			name = parts[0];
			bundlename = parts[1];
			bundlelength = long.Parse(parts[2]);
		}

        public static string[] GetSpliterList()
		{
			//为了向下兼容，也识别" : "//
            string[] spliters = new string[] { " : ", INDEX_MAP_SPLITER };
			return spliters; 
		}

        public static List<IndexMapItem> Read(string stream)
		{
			List<IndexMapItem> itemList = new List<IndexMapItem>();
			string[] lines = stream.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
			foreach(string line in lines)
			{
				IndexMapItem item = new IndexMapItem();
				item.FromString(line);
				itemList.Add(item);
			}
			return itemList;
		}
	}
}

