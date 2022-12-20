using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Unity.Netcode;
using UnityEngine;

public class WaveGameType : AGameType
{
    public enum State
    {
        None,
        Building,
        WaitingForEndOfWave,
        GameEnd,
    }

    List<WaveData> _waves = null;
    NetworkVariable<State> _state = new NetworkVariable<State>(State.None);

    EntityManager _entities = null;
    int _currentWave = 0;

    NetworkVariable<float> _timer = new NetworkVariable<float>(0f);
    [SerializeField] float _buildDuration = 120f;

    void Start()
    {
        _timer.OnValueChanged += (float oldValue, float newValue) => UIManager.instance.SetTimer(newValue);
        _entities = EntityManager.instance;
        _waves = DataManager.instance.waves;
    }

    void Update()
    {
        if (IsServer)
        {
            switch (_state.Value)
            {
                case State.None:
                    break;

                case State.Building:
                    _timer.Value += Time.deltaTime;
                    if (_timer.Value >= _buildDuration || AreAllPlayerReady())
                    {
                        _timer.Value = 0f;
                        SetAllPlayerReadyState(false);
                        // TODO: Disable ready button (player can only wait)
                        // TODO: Disable building
                        StartCoroutine(StartWave(_waves[_currentWave]));
                        UIManager.instance.ShowTimer(true);
                        SetWaitingStateClientRpc();
                        SetState(State.WaitingForEndOfWave);
                    }
                    break;

                case State.WaitingForEndOfWave:
                    // TODO: if enemies are dead for a player, enable building for him
                    if (_entities.AreAllEnemyDead())
                    {
                        _currentWave = (_currentWave + 1) % _waves.Count;
                        UIManager.instance.ShowTimer(false);
                        SetBuildingStateClientRpc();
                        SetState(State.Building);
                    }
                    break;

                case State.GameEnd:
                    break;

                default:
                    Debug.Log("State not implemented: " + _state);
                    break;
            }
        }
    }

    [ClientRpc]
    void SetBuildingStateClientRpc()
    {
        UIManager.instance.ShowTimer(true);
    }

    [ClientRpc]
    void SetWaitingStateClientRpc()
    {
        UIManager.instance.ShowTimer(false);
    }

    public override void StartGame()
    {
        SetState(State.Building);
    }

    public override bool IsOver()
    {
        return _state.Value == State.GameEnd;
    }

    void SetState(State newState)
    {
        Debug.Log($"[WaveGameType] {_state.Value} -> {newState}");
        _state.Value = newState;
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
        SetState(State.WaitingForEndOfWave);
        yield return null;
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
}