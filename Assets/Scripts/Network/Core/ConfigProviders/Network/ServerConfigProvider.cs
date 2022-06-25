using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct ServerConfigVarEditor
{
    public string key;
    public string value;
}

[Serializable]
public struct ServerConfigVar : INetworkSerializable, IEquatable<ServerConfigVar>
{
    public FixedString64Bytes key;
    public FixedString64Bytes value;

    public ServerConfigVar(string key, string value)
    {
        this.key = key;
        this.value = value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref key);
        serializer.SerializeValue(ref value);
    }

    public bool Equals(ServerConfigVar configVar)
    {
        return (key.Value == configVar.key && value.Value == configVar.value);
    }
}

public class ServerConfigProvider : NetworkBehaviour, IConfigProvider
{
    [SerializeField]
    List<ServerConfigVarEditor> _configEditor = new List<ServerConfigVarEditor>();

    NetworkList<ServerConfigVar> _config;

    NetworkVariable<bool> _isInitialized = new NetworkVariable<bool>(false);
    public bool isInitialized => _isInitialized.Value;

    void Awake()
    {
        _config = new NetworkList<ServerConfigVar>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var configVar in _configEditor)
            {
                _config.Add(new ServerConfigVar(configVar.key, configVar.value));
            }
            _isInitialized.Value = true;
        }
    }

    bool AreKeyEquals(string key1, string key2)
    {
        if (key1.StartsWith("-"))
        {
            key1 = key1.Substring(1);
        }
        if (key2.StartsWith("-"))
        {
            key2 = key2.Substring(1);
        }
        return key1 == key2;
    }

    public bool ContainsKey(string key)
    {
        foreach (var configVar in _config)
        {
            if (AreKeyEquals(configVar.key.Value, key))
            {
                return true;
            }
        }
        return false;
    }
    
    public string GetValue(string key, string defaultValue = "")
    {
        foreach (var configVar in _config)
        {
            if (AreKeyEquals(configVar.key.Value, key))
            {
                return configVar.value.Value;
            }
        }
        return defaultValue;
    }
}