using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum ClientRole
{
    Player = 0,
    Actor = 1,
    Observer = 2
}

public class PlayerManager : NetworkBehaviour
{
    [Serializable]
    public class ClientRolePair
    {
        public ClientRole role;
        public GameObject prefab;
    }

    [SerializeField]
    List<ClientRolePair> _clientPrefabs = new List<ClientRolePair>();

    [SerializeField]
    Dictionary<ClientRole, List<GameObject>> _clients;

    void Awake()
    {
        _clients = new Dictionary<ClientRole, List<GameObject>>();
        _clients[ClientRole.Player] = new List<GameObject>();
        _clients[ClientRole.Actor] = new List<GameObject>();
        _clients[ClientRole.Observer] = new List<GameObject>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (IsServer)
        {
            UIDebugManager.AddMessage(this);
        }
    }

    public override string ToString()
    {
        string s = "";

        foreach (var kvp in _clients)
        {
            foreach (GameObject client in kvp.Value)
            {
                s += client.GetComponent<NetworkClientBase>().ToString() + "\n";
            }
        }

        return s;
    }

    public void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[PlayerManager] Client '{clientId}' connected");
        if (IsClient && NetworkManager.Singleton.LocalClientId == clientId)
        {
            ClientRole clientRole = (ClientRole)Enum.Parse(typeof(ClientRole), PlayerPrefs.GetString("client_role", ClientRole.Player.ToString()));
            Debug.Log($"Client '{NetworkManager.Singleton.LocalClientId}' requests player spawn for role '{clientRole}'");
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, clientRole);
        }
    }

    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[PlayerManager] Client '{clientId}' disconnected");
        foreach (var roleList in _clients)
        {
            GameObject toRemove = null;
            foreach (GameObject client in roleList.Value)
            {
                if (client.GetComponent<NetworkObject>().OwnerClientId == clientId)
                {
                    toRemove = client;
                }
            }

            if (toRemove != null)
            {
                Debug.Log($"[PlayerManager] Client '{clientId}' removed");
                roleList.Value.Remove(toRemove);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, ClientRole clientRole)
    {
        SpawnPlayer(clientId, clientRole);
    }

    public void SpawnPlayer(ulong clientId, ClientRole clientRole)
    {
        ClientRolePair client = _clientPrefabs.Find(pair => pair.role == clientRole);

        if (client != null && client.prefab != null)
        {
            Debug.Log($"Spawning '{clientRole}' for client {clientId}");
            Transform spawnPoint = SpawnManager.instance.GetNextSpawnPoint();
            GameObject player = Instantiate(client.prefab, spawnPoint.position, spawnPoint.rotation);
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId, true);

            _clients[clientRole].Add(player);
        }
        else
        {
            Debug.Log($"Wrong role '{clientRole}' for client {clientId}");
        }
    }

    public int GetIdFromRole(ClientRole clientRole)
    {
        int id = 0;
        bool idFound = false;
        while (!idFound)
        {
            idFound = true;
            foreach (var client in _clients[clientRole])
            {
                if (client.GetComponent<NetworkClientBase>().id == id)
                {
                    idFound = false;
                    id++;
                    break;
                }
            }
        }
        return id;
    }

    public List<GameObject> GetClientsPerRole(ClientRole clientRole)
    {
        return _clients[clientRole];
    }
}