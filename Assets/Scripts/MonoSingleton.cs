using UnityEngine;

/// <summary>
/// 单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 创建新实例
                GameObject obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    /// <summary>
    /// 切换场景时是否保留单例
    /// </summary>
    public bool isGlobalSingleton;

    protected virtual void Awake()
    {
        // 防止重复实例
        if (instance == null)
        {
            instance = (T)this;

            if (isGlobalSingleton)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
