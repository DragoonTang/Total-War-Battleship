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

    [Tooltip("是否为全局单例，设置为true时不会在场景切换时销毁该对象。")]
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
