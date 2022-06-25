using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTowerButton : MonoBehaviour
{
    public TowerData Data { get; set; }

    public void SelectTower()
    {
        PlayerBehavior player = LocalPlayer.instance.networkPlayer.GetComponent<PlayerBehavior>();
        if (player.gold >= Data.cost)
        {
            InteractionManager.instance.SetInteraction(new GridInteraction(Data));
        }
    }
}
