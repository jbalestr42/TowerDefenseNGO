using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkSingleton<GameManager>
{
    public enum GameState
    {
        None,
        StartGame,
        Running,
        Restart
    }

    [SerializeField] AGameType _gameType;
    GameState _state = GameState.None;

    void Start()
    {
        UIServerManager.instance.startGameButton.onClick.AddListener(StartGame);
    }

    void Update()
    {
        if (IsServer)
        {
            switch (_state)
            {
                case GameState.None:
                    break;

                case GameState.StartGame:
                    UIServerManager.instance.ShowUI(false);
                    _gameType.StartGame();
                    SetState(GameState.Running);
                    break;

                case GameState.Running:
                    if (_gameType.IsOver())
                    {
                        SetState(GameState.Restart);
                    }
                    break;

                case GameState.Restart:
                    UIServerManager.instance.ShowUI(true);
                    break;

                default:
                    Debug.Log("State not implemented: " + _state);
                    break;
            }
        }
    }

    void StartGame()
    {
        SetState(GameState.StartGame);
    }

    void SetState(GameState newState)
    {
        Debug.Log($"[GameManager] {_state} -> {newState}");
        _state = newState;
    }
}
