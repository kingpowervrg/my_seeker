/// <summary>
/// 单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class, new()
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }

    public virtual void Destroy()
    {
        _instance = null;
    }

}
