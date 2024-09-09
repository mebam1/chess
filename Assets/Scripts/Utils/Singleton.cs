using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected bool shouldDestroyOnSceneChange = false;
    protected bool thisWillBeDestroyedNextFrame = false;
    static public T Instance => instance;
    static private T instance;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (instance == null)
        {
            instance = this as T;
            if (!shouldDestroyOnSceneChange)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            thisWillBeDestroyedNextFrame = true;
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }

    protected virtual void Dispose()
    {
        if (this is T me && me == instance)
            instance = null;
    }
}