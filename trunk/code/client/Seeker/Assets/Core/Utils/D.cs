using UnityEngine;

public static class D
{
    public static bool OpenLog = true;
    public static bool OpenWarn = true;
    public static bool OpenError = true;
    public static void log(object format, params object[] paramList)
    {
#if !UN_LOG
        if (!OpenLog) return;
        if (format is string && paramList != null && paramList.Length > 0)
            Debug.Log(string.Format(format as string, paramList));
        else
            Debug.Log(format);
#endif
    }

    public static void warn(object format, params object[] paramList)
    {
#if !UN_LOG
        if (!OpenWarn) return;
        if (format is string && paramList != null && paramList.Length > 0)
            Debug.LogWarning(string.Format(format as string, paramList));
        else
            Debug.LogWarning(format);
#endif
    }

    public static void error(object format, params object[] paramList)
    {
#if !UN_LOG
        if (!OpenError) return;
        if (format is string && paramList != null && paramList.Length > 0)
            Debug.LogError(string.Format(format as string, paramList));
        else
            Debug.LogError(format);
#endif
    }
}
