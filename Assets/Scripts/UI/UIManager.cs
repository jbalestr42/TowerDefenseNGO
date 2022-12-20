using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType {
    None,
    Tower,
    Enemy,
}

public class UIManager : Singleton<UIManager>
{

    [SerializeField] UnityEngine.UI.Button _isReadyButton;
    [SerializeField] UnityEngine.UI.InputField _inputFieldName;
    [SerializeField] UnityEngine.UI.Text _goldText;
    [SerializeField] UnityEngine.UI.Text _scoreText;
    [SerializeField] UnityEngine.UI.Text _lifeText;
    [SerializeField] UnityEngine.UI.Text _timerText;

    [SerializeField] UIInventory _uiInventory;

    PanelType _selectedPanel = PanelType.None;
    MonoBehaviour _selectedMono = null;

    [SerializeField]
    private PanelTypeAPanelDictionary _panels = PanelTypeAPanelDictionary.New<PanelTypeAPanelDictionary>();
    private Dictionary<PanelType, APanel> Panels { get { return _panels.dictionary; } }

    public UIInventory GetUIInventory { get { return _uiInventory; } }

    void Awake()
    {
        _isReadyButton.onClick.AddListener(IsReadyOnClick);
    }

    #region Accessor to other UI systems

    public void ShowPanel(PanelType type, MonoBehaviour target)
    {
        if (_selectedPanel != type && type != PanelType.None)
        {
            if (_selectedPanel != PanelType.None)
            {
                Panels[_selectedPanel].HideUI();
            }
            _selectedPanel = type;
            _selectedMono = target;
            Panels[type].ShowUI(target);
        }
    }

    public void UpdatePanel(PanelType type, MonoBehaviour target)
    {
        if (type != PanelType.None && Panels[type].IsActive() && _selectedMono == target)
        {
            Panels[type].UpdateUI(target);
        }
    }

    public void HidePanel(PanelType type)
    {
        if (type != PanelType.None && Panels[type] != null)
        {
            Panels[type].HideUI();
            _selectedPanel = PanelType.None;
        }
    }

    #endregion

    #region Public methods exposed to update the UI

    public void SetGold(int gold)
    {
        _goldText.text = "Gold: " + gold.ToString();
    }

    public void SetScore(int score)
    {
        _scoreText.text = "Score: " + score.ToString();
    }

    public void SetLife(int life)
    {
        _lifeText.text = "Life: " + life.ToString();
    }

    public void SetTimer(float time)
    {
        _timerText.text = time.ToString();
    }

    public void ShowTimer(bool show)
    {
        _timerText.gameObject.SetActive(show);
    }

    public void OnReadyStateChanged(bool ready)
    {
        _isReadyButton.interactable = !ready;
    }

    #endregion

    #region Private UI Listeners

    void IsReadyOnClick()
    {
        PlayerBehaviour.current.SetReadyServerRpc(true);
    }

    #endregion
}
