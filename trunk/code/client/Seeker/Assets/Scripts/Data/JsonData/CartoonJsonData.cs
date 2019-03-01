using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Video;

public class CartoonData
{
    public List<CartoonItemJson> M_cartoons { get; set; }
}

public class CartoonVideoNamesJson
{
    public List<String> M_names { get; set; }
}


public class CartoonItemJson
{
    public long Item_id { get; set; }
    public List<CartoonVideoNamesJson> M_cartoons { get; set; }

}







