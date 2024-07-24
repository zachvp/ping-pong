using UnityEngine;

public class CoreSingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            Debug.AssertFormat(_instance != null, "{0}: No instance of MonoSingleton exists in the scene",
                                                    typeof(CoreSingletonBehavior<T>).Name);
            return _instance;
        }
    }

    public virtual void Awake()
    {
        Debug.AssertFormat(_instance == null, "{0}: More than one instance of MonoSingleton exists in the scene",
                                                typeof(CoreSingletonBehavior<T>).Name);
        _instance = this as T;
    }

    public virtual void OnDestroy()
    {
        _instance = null;
    }
}
