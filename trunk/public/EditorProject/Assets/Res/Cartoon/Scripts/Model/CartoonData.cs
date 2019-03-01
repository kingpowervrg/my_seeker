using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Video;

public class CartoonVideoNamesJson
{
    public List<String> M_names { get; set; }
}


public class CartoonItemJson
{
    public long Item_id { get; set; }
    public List<CartoonVideoNamesJson> M_cartoons { get; set; }

}


public class CartoonData
{
    public List<CartoonItemJson> M_cartoons { get; set; }
}

public class CartoonClips
{
    public List<VideoClip> M_clips { get; set; }
}

public class CartoonItemWithClips
{
    public long Item_id { get; set; }

    public List<CartoonClips> M_Items { get; set; }

}


