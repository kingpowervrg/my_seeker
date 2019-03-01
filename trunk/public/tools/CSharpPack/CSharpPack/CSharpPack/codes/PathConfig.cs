using System.IO;


namespace ClientResourcePacker
{
    /// <summary>
    /// 路径配置
    /// </summary>
    public static class PathConfig
    {
        private static string PACK_PROJECT_PATH = Directory.GetCurrentDirectory();

        //模板tmpl 
        public static string TMPL_PATH = Path.Combine(PACK_PROJECT_PATH, "tmpl/");
        public static string CONF_CSHARP_TMPL = Path.Combine(TMPL_PATH, "conf_tmpl.tmpl");
        public static string CONF_LUA_TMPL = Path.Combine(TMPL_PATH, "conf_lua.tmpl");
        public static string CONF_EDITOR_TMPL = Path.Combine(TMPL_PATH, "conf_editor.tmpl");
        public static string CONF_ALL_PARSER_TMPL = Path.Combine(TMPL_PATH, "conf_fact.tmpl");
        public static string CONF_CSHARP_SQLITE_TEMPL = Path.Combine(TMPL_PATH, "conf_sqlite_tmpl.tmpl");


        //项目根目录
        public static string PROJECT_ROOT = Path.Combine(PACK_PROJECT_PATH, "../../../../");

        //Public目录
        public static string PUBLIC_ROOT = Path.Combine(PROJECT_ROOT, @"public");

        //客户端工程
        public static string MAIN_UNITY_PROJECT_PATH = Path.Combine(PROJECT_ROOT, @"code/client/Seeker/Assets");

        //Editor工程
        public static string PACK_UNITY_PROJECT_PATH = Path.Combine(PUBLIC_ROOT, "EditorProject");

        //Excel配置表路径
        public static string EXCEL_CONFIG_PATH = Path.Combine(PUBLIC_ROOT, @"config");

        //客户端生成资源路径
        public static string CLIENT_RES_PATH = Path.Combine(PACK_UNITY_PROJECT_PATH, "Assets/Res");

        //脚本工程ConfXX.cs路径
        public static string SCRIPT_RPOJECT_CONFIG_PATH = Path.Combine(MAIN_UNITY_PROJECT_PATH, @"Config");

        //生成的.byte路径
        // public static string GENERATE_CLIENT_DATA_PATH = Path.Combine(CLIENT_RES_PATH, "Config");

        //生成客户端数据路径
        public static string GENERATE_CLIENT_DATA_PATH = Path.Combine(CLIENT_RES_PATH, "Config");
        public static string SQLITE_DB_PATH = Path.Combine(GENERATE_CLIENT_DATA_PATH, "ConfData.bytes");


        public static string PROJECT_SQLITE_DB_PATH = Path.Combine(PROJECT_ROOT, "code/client/Seeker/Assets/StreamingAssets/");
    }

}
