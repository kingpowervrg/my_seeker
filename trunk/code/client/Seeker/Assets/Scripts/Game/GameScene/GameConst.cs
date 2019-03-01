/********************************************************************
	created:  2018-4-19 16:47:37
	filename: GameConst.cs
	author:	  songguangze@outlook.com
	
	purpose:  定义一些全局变量
*********************************************************************/

namespace SeekerGame
{
    public class GameConst
    {
        public const int PUNISH_TIME = 5;      //每次惩罚时间
        public const int TOTAL_TIME = 300;      //每局时间
        public const float PUNISH_THRESHOLD = 1f;         //连点阈值
        public const string STREAMER_SHADER = "Seeker/Exhibit/ExhibitStreamer"; //物件提示shader
        public const string STREAMER_EFFECT_TEXTURE = "mats/o_mask__57";

        public const string DARK_SCENE_BRUSH_TEX_NAME = "mask";
        public const float DARK_SCENE_BRUSH_SIZE = 5;
        public const string DARK_SCENE_BRUSH_RES_PATH = "Prefabs/torch";
        public const float DARK_SCENE_BRUSH_FADE_TIME = 3f;

        public const float TRAIL_BRUSH_TIME = 3f;     //画笔时间
        public const float TRAIL_BRUSH_SIZE = 0.8f;   //画笔大小
        public const float BRUSH_FADEOUT_TIME = 20f;        //画笔渐变时间
        public const int FOGGY_DENSITY = 3;     //雾的稠密度

        //标准分辨率
        public const int REFERENCE_RESOLUTION_X = 1136;
        public const int REFERENCE_RESOLUTION_Y = 640;

        public const int MAX_HINT_ITEM_COUNT = 6;           //每次最多提示的物件数

        //名字最长字符限制
        public const int MAX_NAME_CHAR_COUNT = 20;

        //新手引导咖啡厅寻物场景
        public const long GUIDESCENEID = 10108000;

        public const string CASH_ICON = "icon_mainpanel_cash_2.png";
        public const string COIN_ICON = "icon_mainpanel_coin_2.png";
        public const string EXP_ICON = "icon_mainpanel_exp_2.png";
        public const string VIT_ICON = "icon_mainpanel_energy_2.png";


        public const string LOADING_IMAGE_BUNDLE = "loading_images.bundle";
        public const string EXHIBIT_ICON_BUNDLE = "atlas_exhibit_item_icon.bundle";
        public const string ATLAS_ICON_BUNDLE = "atlas_atlas_1.bundle";
        public const string SHADER_BUNDLE = "shader.bundle";
        public const string COMMON_BG = "common_bigtex.bundle";

        public const string POST_FX_SHADERVARIANT_NAME = "PostFXShaderCollection";

        public const string ServerIP = "http://game.fotoable-conan.com/api-web";

        public const string TEST_ServerIP = "http://game.fotoable-conan.com/api-web-staging";


#if MARKET_GOOGLE_PLAY
        public const string MARKET_FLAG = "GOOGLEPLAY";
#else
        public const string MARKET_FLAG = "";
#endif

    }
}