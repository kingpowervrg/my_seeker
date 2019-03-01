using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerGame
{
    public class CommonData
    {
        private static long m_millisRecoverOneVit;
        public static long MillisRecoverOneVit
        {
            get { return m_millisRecoverOneVit; }
            set { m_millisRecoverOneVit = value; }
        }
        public static float MAXVIT
        {
            get { return 120; }
        }

        public const int SHOPMAXBUY = 200;

        public const int C_SCENE_TYPE_ID = 10000000;
        public const int C_SEEK_SCENE_START_ID = 1;
        public const int C_JIGSAW_SCENE_START_ID = 2;
        public const int C_CARTOON_SCENE_START_ID = 4;

        public const float C_JIGSAW_LEFT_LIMIT_SCREEN_X = 55.0f;
        //public static float S_JIGSAW_RIGHT_LIMIT_WORLD_X;

        //中等方图
        public static List<string> DEFAULT_PLAYER_IMAGE_LIST = new List<string>()
            {
               "image_player_size1_1.png",
               "image_player_size1_2.png",
               "image_player_size1_3.png",
               "image_player_size1_4.png",
            };

        //漫画头像
        public static List<string> CartoonHEAD = new List<string>
        {
            "image_comics_newhand_role_1.png",
            "image_comics_newhand_role_2.png",
            "image_comics_newhand_role_3.png",
            "image_comics_newhand_role_4.png"
        };

        //小圆图
        public static List<string> LitterHEAD = new List<string>
        {
            "image_player_size2_1.png",
            "image_player_size2_2.png",
            "image_player_size2_3.png",
            "image_player_size2_4.png",
        };

        public static List<string> BigPortrait = new List<string>
        {
            "image_player_size4_1.png",
            "image_player_size4_2.png",
            "image_player_size4_3.png",
            "image_player_size4_4.png",
        };

        //中等方图
        public static List<string> Size3HEAD = new List<string>()
            {
               "image_player_size3_1.png",
               "image_player_size3_2.png",
               "image_player_size3_3.png",
               "image_player_size3_4.png",
            };

        public static List<string> SpineHEAD = new List<string>()
        {
            "juzhang_SkeletonData.asset",
            "juzhang_meizi_01_SkeletonData.asset",
             "juzhang_laonianren_01_SkeletonData.asset",
            "daheiniu_SkeletonData.asset"
        };

        public static string GetLitterHEAD(string bigHead)
        {
            for (int i = 0; i < DEFAULT_PLAYER_IMAGE_LIST.Count; i++)
            {
                if (DEFAULT_PLAYER_IMAGE_LIST[i].Equals(bigHead))
                {
                    return LitterHEAD[i];
                }
            }
            return bigHead;
        }

        public static string GetSpineHead(string player_icon)
        {
            for (int i = 0; i < DEFAULT_PLAYER_IMAGE_LIST.Count; i++)
            {
                if (DEFAULT_PLAYER_IMAGE_LIST[i].Equals(player_icon) || LitterHEAD[i].Equals(player_icon))
                {
                    return SpineHEAD[i];
                }
            }
            return SpineHEAD[0];
        }

        public static string GetBigPortrait(string player_icon)
        {
            for (int i = 0; i < DEFAULT_PLAYER_IMAGE_LIST.Count; i++)
            {
                if (DEFAULT_PLAYER_IMAGE_LIST[i].Equals(player_icon) || LitterHEAD[i].Equals(player_icon))
                {
                    return BigPortrait[i];
                }
            }
            return BigPortrait[0];
        }

        public static string GetSize3HEADByDefault(string defaultIcon)
        {
            for (int i = 0; i < DEFAULT_PLAYER_IMAGE_LIST.Count; i++)
            {
                if (DEFAULT_PLAYER_IMAGE_LIST[i].Equals(defaultIcon))
                {
                    return Size3HEAD[i];
                }
            }
            return defaultIcon;
        }

        public static string GetSize1BySize2(string defaultIcon)
        {
            for (int i = 0; i < LitterHEAD.Count; i++)
            {
                if (LitterHEAD[i].Equals(defaultIcon))
                {
                    return DEFAULT_PLAYER_IMAGE_LIST[i];
                }
            }
            return defaultIcon;
        }

    }
}
