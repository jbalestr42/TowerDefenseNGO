using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class UIServerManager : Singleton<UIServerManager>
{
    [SerializeField] UnityEngine.UI.Button _generateMapButton;
    [SerializeField] UnityEngine.UI.Button _startGameButton;
    public UnityEngine.UI.Button startGameButton { get { return _startGameButton; } set { _startGameButton = value; } }

    void Awake()
    {
        _generateMapButton.onClick.AddListener(GenerateMapOnClick);
    }

    void GenerateMapOnClick()
    {
        int seed = Random.Range(1, int.MaxValue);
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClients)
        {
            clientPair.Value.PlayerObject.GetComponent<PlayerBehaviour>().Generate(seed);
        } 
    }

    public void ShowUI(bool show)
    {
        _generateMapButton.gameObject.SetActive(show);
        _startGameButton.gameObject.SetActive(show);
    }
}