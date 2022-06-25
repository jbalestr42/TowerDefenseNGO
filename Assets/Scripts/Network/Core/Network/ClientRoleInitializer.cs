using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientRoleInitializer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            if (IsLocalPlayer)
            {
                OnOwnerPlayerSpawn();
            }
            else
            {
                OnOtherPlayerSpawn();
            }
        }
        else if (IsServer)
        {
            OnServerPlayerSpawn();
        }
    }

    public virtual void OnOwnerPlayerSpawn() { }
    public virtual void OnOtherPlayerSpawn() { }
    public virtual void OnServerPlayerSpawn() { }
}