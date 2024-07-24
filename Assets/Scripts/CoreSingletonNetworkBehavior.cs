using Unity.Netcode;
using UnityEngine;

public class CoreSingletonNetworkBehavior<T> : NetworkBehaviour where T : NetworkBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            Debug.AssertFormat(_instance != null, "{0}: No instance of Singleton exists in the scene",
                                                    typeof(T).Name);
            return _instance;
        }
    }

    public virtual void Awake()
    {
        Debug.AssertFormat(_instance == null, "{0}: More than one instance of Singleton exists in the scene",
                                                typeof(T).Name);
        _instance = this as T;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _instance = null;
    }
}
