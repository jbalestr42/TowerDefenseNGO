using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigProvider
{
    bool isInitialized { get; }
    bool ContainsKey(string key);
    string GetValue(string key, string defaultValue = "");
}