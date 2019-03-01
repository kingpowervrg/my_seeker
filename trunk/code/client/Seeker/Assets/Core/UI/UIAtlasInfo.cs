using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Code Generate  by ExtractAlphaTool.cs


static class UIAtlasInfo
{
    public enum AlphaChannel
    {
        Channel_R,
        Channel_G,
        Channel_B,
        Channel_R_Gray,
        Channel_G_Gray,
        Channel_B_Gray
    };
    public static Dictionary<string, string> textureMap = new Dictionary<string, string>();
    public static Dictionary<string, AlphaChannel> textureChannelMap = new Dictionary<string, AlphaChannel>();

    public static void OnInit()
    {



    }
}

