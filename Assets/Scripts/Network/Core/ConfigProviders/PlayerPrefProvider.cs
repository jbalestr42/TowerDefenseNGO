using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefProvider : MonoBehaviour, IConfigProvider
{
    public bool isInitialized => true;

    public bool ContainsKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    
    public string GetValue(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void SetValue(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
}