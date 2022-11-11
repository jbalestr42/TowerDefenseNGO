using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class UIServerManager : Singleton<UIServerManager>
{
    public UnityEngine.UI.Button _generateMap;

    void Awake()
    {
        _generateMap.onClick.AddListener(GenerateMapOnClick);
    }

    void GenerateMapOnClick()
    {
        int seed = Random.Range(1, int.MaxValue);
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
        {
            clientPair.Value.PlayerObject.GetComponent<PlayerBehaviour>().Generate(seed);
        } 
    }
}
