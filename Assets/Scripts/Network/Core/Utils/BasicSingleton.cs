using UnityEngine;

public class BasicSingleton<T> where T : class, new()
{
    private static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }
}