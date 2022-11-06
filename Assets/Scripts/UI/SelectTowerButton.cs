using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectTowerButton : MonoBehaviour
{
    public TowerData Data { get; set; }

    public void SelectTower()
    {
        PlayerBehaviour player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerBehaviour>();
        if (player.gold >= Data.cost)
        {
            InteractionManager.instance.SetInteraction(new GridInteraction(Data));
        }
    }
}
