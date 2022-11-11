using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnableNetworkRole : NetworkBehaviour
{
    [SerializeField] bool enableIfServer = false;
    [SerializeField] bool enableIfClient = false;

    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(enableIfServer == IsServer || enableIfClient == IsClient);
    }
}
