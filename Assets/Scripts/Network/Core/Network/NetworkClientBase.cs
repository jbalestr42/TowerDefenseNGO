using System;
using Unity.Netcode;
using UnityEngine;

/*
* This class is used to initialize the client role/id
*/
public class NetworkClientBase : NetworkBehaviour
{
    NetworkVariable<ClientRole> _role = new NetworkVariable<ClientRole>(ClientRole.Player);
    public ClientRole role { get { return _role.Value; } set { _role.Value = value; } }
    NetworkVariable<int> _id = new NetworkVariable<int>(-1);
    public int id => _id.Value;

    public bool isInitialized => id != -1;

    public override void OnNetworkSpawn()
    {
        ClientRole clientRole = (ClientRole)Enum.Parse(typeof(ClientRole), PlayerPrefs.GetString("client_role", ClientRole.Player.ToString()));
        if (IsClient)
        {
            if (IsOwner)
            {
                UIDebugManager.AddMessage(this);
                _id.OnValueChanged += OnOwnerInitializationDone;
                InitializePlayerServerRpc(clientRole);
            }
            else
            {
                _id.OnValueChanged += OnClientInitializationDone;
            }
        }
        else
        {
            _id.OnValueChanged += OnServerInitializationDone;
        }
    }

    public override string ToString()
    {
        return $"Role: {role} | Id: {id}";
    }

    [ServerRpc]
    public void InitializePlayerServerRpc(ClientRole clientRole)
    {
        Debug.Log("[InitializePlayerServerRpc] " + clientRole);
        _role.Value = clientRole;
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        _id.Value = playerManager.GetIdFromRole(_role.Value);
    }

    public void OnOwnerInitializationDone(int oldValue, int newValue)
    {
        Debug.Log("[OnOwnerInitializationDone]");
        LocalPlayer.instance.networkPlayer = gameObject;
        LocalPlayer.instance.OnLocalPlayerInitialized.Invoke(gameObject);
    }

    public void OnClientInitializationDone(int oldValue, int newValue)
    {
        Debug.Log("[OnClientInitializationDone]");
        LocalPlayer.instance.OnClientPlayerInitialized.Invoke(gameObject);
    }

    public void OnServerInitializationDone(int oldValue, int newValue)
    {
        Debug.Log("[OnServerInitializationDone]");
        LocalPlayer.instance.OnServerPlayerInitialized.Invoke(gameObject);
    }

    [ServerRpc]
    public void RequestOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.Log($"Unable to change ownership for client {newOwnerClientId}");
        }
    }

    [ServerRpc]
    public void RemoveOwnershipServerRpc(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.RemoveOwnership();
        }
        else
        {
            Debug.Log($"Unable to remove ownership for object {networkObject}");
        }
    }
}