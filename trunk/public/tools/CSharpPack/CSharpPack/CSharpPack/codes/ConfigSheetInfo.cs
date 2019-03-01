using System.Collections.Generic;

namespace ClientResourcePacker
{
    /// <summary>
    /// Excel Sheet 解析后的实体
    /// </summary>
    public class ConfigSheetInfo
    {
        public string ConfigSheetName;
        public ConfigData SheetData;
    }

    public class ConfigData
    {
        public string[] cl_list;            //客户端还是服务器数据
        public string[] type_list;          //数据类型
        public string[] name_list;          //生成数据字段的名
        public string[] msg_list;           //注释
        public bool[] arr_list;
        public List<string[]> data_list;    //数据
    }

    public class ConfigSqliteData : ConfigData
    {
        public string[] sqliteDatatype_list;

        public ConfigSqliteData()
        {

        }

        public ConfigSqliteData(int count)
        {
            this.cl_list = new string[count];
            this.type_list = new string[count];
            this.name_list = new string[count];
            this.msg_list = new string[count];
            this.arr_list = new bool[count];
            this.data_list = new List<string[]>();
            this.sqliteDatatype_list = new string[count];
        }
    }

    public class ConfigParser
    {
        public string[] cls_name_list;
    }

    /// <summary>
    /// Razor 解析类，需要与Template里的字段一致
    /// </summary>
    public class SheetMsg
    {
        public string[] AttributeTypes { get; set; }            //字段类型
        public int[] AttributeFlags { get; set; }              //字段类型代号
        public string[] AttributeNames { get; set; }            //字段名称
        public string TableName { get; set; }                   //配置表名
        public int SheetColumns { get; set; }
        public string ExcelName { get; set; }
    }

    public enum GeneratePlatform
    {
        CLIENT,
        SERVER,
        EDITOR,
    }
}
