using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Find singleton
                instance = FindFirstObjectByType<T>();

                // Create singleton if doesn't exist
                if (instance == null)
                {
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                }
            }
            return instance;
        }
    }
}
