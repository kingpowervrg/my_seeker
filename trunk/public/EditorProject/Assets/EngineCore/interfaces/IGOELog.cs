using System;
namespace GOEngine
{
    public interface IGOELog
    {
        void LogError(string msg);
        void LogException(Exception ex);
        void LogMsg(string msg);
        void LogWarning(string msg);
        string Name { set; }
    }
}
