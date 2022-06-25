using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerActivator : ALocalPlayerInitializer
{
    [SerializeField]
    List<GameObject> _objectsToActivate = new List<GameObject>();

    public override void OnLocalPlayerInitialized(GameObject player)
    {
        NetworkClientBase playerState = player.GetComponent<NetworkClientBase>();
        Debug.Log("[LocalPlayerActivator] ActivateLocalObject " + playerState.id);
        for (int i = 0; i < _objectsToActivate.Count; i++)
        {
            bool shouldActivate = playerState != null && playerState.role == ClientRole.Player && playerState.id == i;
            _objectsToActivate[i].SetActive(shouldActivate);
        }
    }
}