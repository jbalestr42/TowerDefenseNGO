using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

public enum GameState
{
    None,
    WaitingForPlayers,
    StartWave, // Start the coroutine to spawn enemies
    Spawning, // Spawn enemies
    WaitingForEndOfWave, // All enemies are spawned, waiting for them to be killed or reach the end
    NextWave, // Prepare the next wave
    GameOver, // Game lost
    GameEnd, // Game win
}

/*
 * Only instantiated by the server
 * */
public class GameWaveManager : NetworkSingleton<GameWaveManager>
{
    List<WaveData> _waves = null;

    GameState _state = GameState.None;
    int _currentWave = 0;
    EntityManager _entities = null;

    int _life = 10;
    public int life { get { return _life; } set { _life = value; } }

    void Start()
    {
        SetState(GameState.WaitingForPlayers);
        _entities = EntityManager.instance;
        _waves = DataManager.instance.waves;
    }

    void Update()
    {
        if (IsServer)
        {
            switch (_state)
            {
                case GameState.WaitingForPlayers:
                    if (CanStartGame())
                    {
                        StartGame();
                        SetState(GameState.Spawning);
                    }
                    break;
                case GameState.Spawning:
                    break;
                case GameState.WaitingForEndOfWave:
                    if (_entities.AreAllEnemyDead())
                    {
                        SetState(GameState.NextWave);
                    }
                    break;
                case GameState.NextWave:
                    SetAllPlayerReadyState(false);
                    _currentWave++;
                    if (_currentWave >= _waves.Count)
                    {
                        SetState(GameState.GameEnd);
                    }
                    else
                    {
                        SetState(GameState.WaitingForPlayers);
                    }
                    break;
                case GameState.GameEnd:
                    SetAllPlayerReadyState(false);
                    _currentWave = 0; //TODO: proper end, for now it's a restart
                    SetState(GameState.WaitingForPlayers);
                    break;
                case GameState.GameOver:
                    // Restart all, kill enemies, bullets, towers, reset score and gold
                    SetState(GameState.GameEnd);
                    break;
                default:
                    Debug.Log("State not implemented: " + _state);
                    break;
            }
        }
    }

    public void LooseLife(int amount)
    {
        _life -= amount;
        if (_life <= 0)
        {
            SetState(GameState.GameOver);
        }
    }

    public bool CanStartGame()
    {
        return AreAllPlayerReady();
    }

    public void StartGame()
    {
        Assert.IsNotNull(_waves);
        StartCoroutine(StartWave(_waves[_currentWave]));
    }

    public void SetAllPlayerReadyState(bool ready)
    {
        var players = NetworkManager.Singleton.ConnectedClients;
        foreach (var player in players)
        {
            PlayerBehaviour playerBehaviour = player.Value.PlayerObject.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                playerBehaviour.isReady = ready;
            }
        }
    }

    public bool AreAllPlayerReady()
    {
        var players = NetworkManager.Singleton.ConnectedClients;
        bool ready = players.Count > 0;
        foreach (var player in players)
        {
            ready = ready && player.Value.PlayerObject != null && player.Value.PlayerObject.GetComponent<PlayerBehaviour>().isReady;
        }
        return ready;
    }

    IEnumerator StartWave(WaveData wave)
    {
        var players = NetworkManager.Singleton.ConnectedClients;
        var wait = new WaitForSeconds(wave.spawnRate);

        int count = wave.count;
        while (count != 0)
        {
            foreach (var player in players)
            {
                EntityManager.instance.SpawnEnemy(_currentWave, wave.enemyData, player.Value.PlayerObject.GetComponent<PlayerBehaviour>());
            }
            count--;
            yield return wait;
        }
        SetState(GameState.WaitingForEndOfWave);
        yield return null;
    }

    void SetState(GameState newState)
    {
        Debug.Log($"[GameState] {_state} -> {newState}");
        _state = newState;
    }
}
