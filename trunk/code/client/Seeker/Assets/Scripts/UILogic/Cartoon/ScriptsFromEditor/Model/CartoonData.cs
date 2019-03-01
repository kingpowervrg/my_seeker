using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Video;

public class CartoonClips
{
    public List<VideoClip> M_clips { get; set; }
}

public class CartoonItemWithClips
{
    public long Item_id { get; set; }

    public List<CartoonClips> M_Items { get; set; }

}

public class CartoonGameEntryData
{
    public long M_cartoon_id { get; set; }
}


