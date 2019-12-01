using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static MonoBehaviourSingleton<T> instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MonoBehaviourSingleton<T>>();

            return (T)instance;
        }
    }

    public static bool IsAvailable()
    {
        return instance != null;
    }

    protected virtual void Initialize()
    {

    }

    private void Awake()
    {
        instance = this;

        Initialize();
    }
}
