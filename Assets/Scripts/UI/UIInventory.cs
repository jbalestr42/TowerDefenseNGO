﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {

    [SerializeField]
    GameObject _inventoryConainer;

    [SerializeField]
    GameObject _inventoryTower;

    List<SelectTowerButton> _towerButtons;

	void Awake()
    {
        _towerButtons = new List<SelectTowerButton>();
        var towersData = DataManager.instance.Towers;
		foreach (var data in towersData)
        {
            var towerButton = Instantiate(_inventoryTower);
            towerButton.GetComponent<SelectTowerButton>().Data = data.Value;
            towerButton.GetComponentInChildren<UnityEngine.UI.Text>().text = data.Value.text;
            towerButton.transform.SetParent(_inventoryConainer.transform);
            _towerButtons.Add(towerButton.GetComponent<SelectTowerButton>());
        }
	}

    public void UpdateAffordableTowers(int gold)
    {
        for (int i = 0; i < _towerButtons.Count; i++)
        {
            _towerButtons[i].GetComponent<UnityEngine.UI.Button>().interactable = _towerButtons[i].Data.cost <= gold;
        }
    }
}
