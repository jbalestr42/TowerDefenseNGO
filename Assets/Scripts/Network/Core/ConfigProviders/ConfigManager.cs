using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class ConfigManager : Singleton<ConfigManager>, IConfigProvider
{
    List<IConfigProvider> _providers = new List<IConfigProvider>();

    public bool isInitialized => true;

    public override string ToString()
    {
        string s = "";
        foreach (IConfigProvider provider in _providers)
        {
            s += $"{provider.GetType()} = {provider.isInitialized}\n";
        }
        return s;
    }

    public void AddProvider(IConfigProvider provider)
    {
        _providers.Add(provider);
    }

    public bool ContainsKey(string key)
    {
        foreach (IConfigProvider provider in _providers)
        {
            if (provider.isInitialized && provider.ContainsKey(key))
            {
                return true;
            }
        }
        return false;
    }
    
    public string GetValue(string key, string defaultValue = "")
    {
        foreach (IConfigProvider provider in _providers)
        {
            if (provider.isInitialized && provider.ContainsKey(key))
            {
                return provider.GetValue(key);
            }
        }
        return defaultValue;
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        if (!ContainsKey(key))
        {
            return defaultValue;
        }

        bool value = false;
        string stringValue = GetValue(key);

        if (Boolean.TryParse(stringValue, out value))
        {
            return value;
        }
        else if (stringValue == "1")
        {
            return true;
        }
        else if (stringValue == "0")
        {
            return false;
        }
        return defaultValue;
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        if (!ContainsKey(key))
        {
            UnityEngine.Debug.Log("Don't find key ");
            return defaultValue;
        }

        int value = 0;
        if (Int32.TryParse(GetValue(key), out value))
        {
            return value;
        }
        UnityEngine.Debug.Log("Failed to parse int ");
        return defaultValue;
    }

    public float GetFloatValue(string key, float defaultValue = 0f)
    {
        if (!ContainsKey(key))
        {
            UnityEngine.Debug.Log("Don't find key ");
            return defaultValue;
        }

        float value = 0f;
        string stringValue = GetValue(key);
        if (float.TryParse(stringValue.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out value))
        {
            return value;
        }
        UnityEngine.Debug.Log("Failed to parse float ");
        return defaultValue;
    }
}