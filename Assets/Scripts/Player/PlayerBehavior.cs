using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    UIPlayerHUD _playerUI;
    Renderer _renderer;

    NetworkVariable<int> _gold = new NetworkVariable<int>();
    public int gold { get { return _gold.Value; } set { _gold.Value = value; } }

    NetworkVariable<int> _score = new NetworkVariable<int>();
    public int score { get { return _score.Value; } set { _score.Value = value; } }

    NetworkVariable<bool> _isReady = new NetworkVariable<bool>();
    public bool isReady { get { return _isReady.Value; } set { _isReady.Value = value; } }

    NetworkVariable<Color> _color = new NetworkVariable<Color>();
    public Color color { get { return _color.Value; } set { _color.Value = value; } }

    NetworkVariable<FixedString64Bytes> _playerName = new NetworkVariable<FixedString64Bytes>("");
    public string playerName { get { return _playerName.Value.ToString(); } set { _playerName.Value = value; } }

    void Start()
    {
        _color.OnValueChanged += (Color old, Color value) => { _renderer.material.color = value; };
        _playerName.OnValueChanged += (FixedString64Bytes old, FixedString64Bytes value) => { _playerUI.SetPlayerName(value.ToString()); };
        _gold.OnValueChanged += (int old, int value) => { UpdateGold(); };
        _score.OnValueChanged += (int old, int value) => { UpdateScore(); };
        _isReady.OnValueChanged += (bool old, bool value) => { UpdateIsReady(); };
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            UIManager.instance.currentPlayer = this;
            SetColorServerRpc(Random.ColorHSV());
            SetGoldServerRpc(100);
            UpdateGold();
        }

        _renderer = GetComponentInChildren<Renderer>();
        _playerUI = GetComponentInChildren<UIPlayerHUD>();
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
    public void SetColorServerRpc(Color color)
    {
        _color.Value = color;
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(string playerName)
    {
        _playerName.Value = playerName;
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
