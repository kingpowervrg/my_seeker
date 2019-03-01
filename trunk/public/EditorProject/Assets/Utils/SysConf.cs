namespace GOEngine
{
    public class SysConf
    {
        //camera conf
		public static float CAMERA_FOV = 55;
		public static float CAMERA_FAR = 64;
		public static float CAMERA_NEAR = 0.1f;
		
		//index bundle
	    public const string GAME_RES_INDEX_MAP = "ResIdxMapFact.bytes";
	    public const string GAME_RES_INDEX_BUNDLE_NAME = "index_map.bundle";
        public const string BUNDLEMAP_FILE = "bundlemap.map.txt";
        public static string BUNDLEMAP_FILE_SERVER = "bundlemap.map.server.txt";

        public static string GAME_RES_URL;
    }
}
