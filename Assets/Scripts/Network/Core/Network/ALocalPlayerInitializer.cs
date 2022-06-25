using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/*
* This class call the event OnLocalPlayerInitialized once the player is initialized
* The event can be invoked in OnEnable if the player is already initialized
* otherwise it waits for the player to be initialized to invoke the event 
*/
public abstract class ALocalPlayerInitializer : MonoBehaviour
{
    bool _isRegistered = false;

    public abstract void OnLocalPlayerInitialized(GameObject player);

    void OnEnable()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkClientBase playerState = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkClientBase>();
            if (playerState.isInitialized)
            {
                OnLocalPlayerInitialized(NetworkManager.Singleton.LocalClient.PlayerObject.gameObject);
            }
            else
            {
                _isRegistered = true;
            }
        }
        else
        {
            _isRegistered = true;
        }

        if (_isRegistered)
        {
            LocalPlayer.instance.OnLocalPlayerInitialized.AddListener(OnLocalPlayerInitialized);
        }
    }

    void OnDisable()
    {
        if (LocalPlayer.instance != null)
        {
            if (_isRegistered)
            {
                LocalPlayer.instance.OnLocalPlayerInitialized.RemoveListener(OnLocalPlayerInitialized);
            }
        }
    }
}