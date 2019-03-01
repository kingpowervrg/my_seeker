namespace SeekerGame
{
    /// <summary>
    /// 游戏Module
    /// </summary>
    public enum GameModuleTypes : byte
    {
        SCENE_MODULE = 100,
        LOCALIZE_MODULE,        //本地化
        PING_MODULE,
        GUID_MODULE,
        UBS_MODULE,
        IAP_MODULE,
        GAMEUPDATE_MODULE       //游戏更新Module
    }
}
