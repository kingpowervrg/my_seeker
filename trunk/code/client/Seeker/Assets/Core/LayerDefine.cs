/********************************************************************
	created:  2018-4-2 13:26:16
	filename: LayerDefine.cs
	author:	  songguangze@outlook.com
	
	purpose:  游戏Layer的定义
*********************************************************************/
using UnityEngine;

namespace EngineCore
{
    public class LayerDefine
    {
        //UI隐藏层
        public static int UIHideLayer = LayerMask.NameToLayer("NoDrawUI");

        //UI 显示层
        public static int UIShowLayer = LayerMask.NameToLayer("UI");

        //场景里需要找的物件
        public static int SceneTargetObjectLayer = LayerMask.NameToLayer("SceneTarget");

        //画板
        public static int SceneDrawingBoardLayer = LayerMask.NameToLayer("DrawingBoard");

        //场景摄像机切换开关
        public static int SceneCameraTrigger = LayerMask.NameToLayer("CameraTrigger");
    }
}