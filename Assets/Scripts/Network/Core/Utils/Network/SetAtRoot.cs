using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetAtRoot : NetworkBehaviour
{
    Transform _parent;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _parent = transform.parent;
            transform.SetParent(null);
        }
    }
}