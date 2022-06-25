using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigInitializer : MonoBehaviour
{
    [SerializeField]
    ServerConfigProvider _serverConfigProvider = null;

    [SerializeField]
    PlayerPrefProvider _playerConfigProvider = null;

    void Awake()
    {
        if (_playerConfigProvider != null)
        {
            ConfigManager.instance.AddProvider(_playerConfigProvider);
        }

        if (_serverConfigProvider != null)
        {
            ConfigManager.instance.AddProvider(_serverConfigProvider);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        ConfigManager.instance.AddProvider(new AndroidExtraIntentProvider());
#else
        ConfigManager.instance.AddProvider(new CommandLineProvider());
#endif
    }
}