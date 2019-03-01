/********************************************************************
	created:  2018-4-4 14:8:57
	filename: EntityType.cs
	author:	  songguangze@outlook.com
	
	purpose:  实体类型
*********************************************************************/
namespace SeekerGame
{
    public enum EntityType
    {
        None,
        Scene_Object,           //场景物件
        Scene_Decoration,       //场景装饰物
        Scene_Exhibit,          //场景展示物（展厅专用）
        Effect,                 //特效实体
    }
}