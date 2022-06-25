using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineProvider : IConfigProvider
{
    Dictionary<string, string> _args = null;
    public Dictionary<string, string> args => GetArgs();

    public bool isInitialized => args != null;

    Dictionary<string, string> GetArgs()
    {
        if (_args == null)
        {
            _args = new Dictionary<string, string>();

            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    string value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    if (_args.ContainsKey(arg.Substring(1)))
                    {
                        _args.Add(arg.Substring(1), value);
                    }
                }
            }
        }
        return _args;
    }

    public bool ContainsKey(string key)
    {
        return args.ContainsKey(key);
    }

    public string GetValue(string key, string defaultValue = "")
    {
        if (ContainsKey(key))
        {
            return args[key];
        }
        return defaultValue;
    }
}