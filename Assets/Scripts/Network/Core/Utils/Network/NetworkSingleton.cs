using Unity.Netcode;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            return _instance;
        }
    }
 
    public override void OnDestroy()
    {
        base.OnDestroy();
        _instance = null;
    }
}