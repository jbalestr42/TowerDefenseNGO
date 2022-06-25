using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServerBase : ClientRoleInitializer
{
    [SerializeField]
    GameObject _camera;

    private void Awake()
    {
        _camera.SetActive(false);
    }

    public override void OnServerPlayerSpawn()
    {
        _camera.SetActive(true);
        UIDebugManager.instance.SetCamera(_camera.GetComponent<Camera>());
    }
}