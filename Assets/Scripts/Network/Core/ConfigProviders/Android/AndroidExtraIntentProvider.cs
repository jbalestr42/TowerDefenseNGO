using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidExtraIntentProvider : IConfigProvider
{
    bool _isInitialized = false;
    public bool isInitialized => _isInitialized;

#if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaObject _intent;

    public AndroidExtraIntentProvider()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        _intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        _isInitialized = true;
    }
#endif

    public bool ContainsKey(string key)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return _intent.Call<bool>("hasExtra", $"-{key}");
#else
        return false;
#endif
    }
    
    public string GetValue(string key, string defaultValue = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return _intent.Call<string>("getStringExtra", $"-{key}");
#else
        return defaultValue;
#endif
    }
}