using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public enum ENUM_CARTOON_ITEM_TYPE
    {
        E_INVALID,
        E_SQUARE_FIXED, //固定正方
        E_SQUARE_DRAG, //可移动正方
        E_SQUARE_ROTATE, //可旋转正方
        E_LONGRECT_ROTATE, //可旋转长方形

    }

    public enum ENUM_ITEM_DIRECTION
    {
        E_NONE,
        E_UP,
        E_RIGHT,
        E_DOWN,
        E_LEFT,
    }

    public enum ENUM_ROTATE_DIR
    {
        E_CLOCKWISE,
        E_ANTI_CLOCKWISE,
    }


