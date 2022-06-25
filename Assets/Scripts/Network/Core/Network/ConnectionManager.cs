using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;
using ParrelSync;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    bool _startEditorAsHost = false;

    [SerializeField]
    string _sceneToLoadOnConnectionLost = "Lobby";

    void Start()
    {
        if (ConfigManager.instance.ContainsKey("ip"))
        {
            UNetTransport transport = GetComponent<UNetTransport>();
            transport.ConnectAddress = ConfigManager.instance.GetValue("ip");
        }

#if UNITY_SERVER
        StartServer();
#else
        string role = PlayerPrefs.GetString("role");
        if (Application.isEditor)
        {
            if (ClonesManager.IsClone())
            {
                StartClient();
            }
            else if (_startEditorAsHost)
            {
                StartHost();
            }
            else
            {
                StartServer();
            }
        }
        else if (role == "server")
        {
            StartServer();
        }
        else if (role == "host")
        {
            StartHost();
        }
        else if (role == "client")
        {
            StartClient();
        }
        else
        {
            Debug.LogError($"The role '{role}' is incorrect.");
        }
#endif
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[ConnectionManager] Client '{clientId}' connected");
    }

    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[ConnectionManager] Client '{clientId}' disconnected");
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene(_sceneToLoadOnConnectionLost, LoadSceneMode.Single);
        }
    }

    public void StartClient()
    {
        Debug.Log("[ConnectionManager] Client started");
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        Debug.Log("[ConnectionManager] Server started");
        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        Debug.Log("[ConnectionManager] Host started");
        NetworkManager.Singleton.StartHost();
    }
}