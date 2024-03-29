﻿using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerBehaviour : NetworkBehaviour
{
    [SerializeField] GridManager _grid;
    public GridManager grid { get { return _grid; } set { _grid = value; } }

    [SerializeField] GridGenerator _gridGenerator;
    public GridGenerator gridGenerator { get { return _gridGenerator; } set { _gridGenerator = value; } }

    Renderer _renderer;

    NetworkVariable<int> _gold = new NetworkVariable<int>();
    public int gold { get { return _gold.Value; } set { _gold.Value = value; } }

    NetworkVariable<int> _score = new NetworkVariable<int>();
    public int score { get { return _score.Value; } set { _score.Value = value; } }

    NetworkVariable<bool> _isReady = new NetworkVariable<bool>();
    public bool isReady { get { return _isReady.Value; } set { _isReady.Value = value; } }

    public static PlayerBehaviour current = null;

    void Start()
    {
        _gold.OnValueChanged += (int old, int value) => { UpdateGold(); };
        _score.OnValueChanged += (int old, int value) => { UpdateScore(); };
        _isReady.OnValueChanged += (bool old, bool value) => { UpdateIsReady(); };
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner || IsServer)
        {
            _grid.Generate();
        }

        if (IsOwner)
        {
            current = this;
            SetGoldServerRpc(100);
            UpdateGold();
        }
    }

    public void Generate(int seed = 42)
    {
        _gridGenerator.Generate(_grid, transform, seed);
    }

    [ServerRpc]
    public void SetGoldServerRpc(int gold)
    {
        _gold.Value = gold;
    }

    [ServerRpc]
    public void SetScoreServerRpc(int score)
    {
        _score.Value = score;
    }

    [ServerRpc]
    public void SetReadyServerRpc(bool isReady)
    {
        _isReady.Value = isReady;
    }

    void UpdateGold()
    {
        if (IsOwner)
        {
            UIManager.instance.SetGold(gold);
            UIManager.instance.GetUIInventory.UpdateAffordableTowers(gold);
        }
    }

    void UpdateScore()
    {
        if (IsOwner)
        {
            UIManager.instance.SetScore(score);
        }
    }

    void UpdateIsReady()
    {
        if (IsOwner)
        {
            UIManager.instance.OnReadyStateChanged(isReady);
        }
    }
}
