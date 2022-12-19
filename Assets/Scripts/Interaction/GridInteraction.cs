using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GridInteraction : AInteraction
{

    TowerData _data = null;
    GameObject _tower = null;

    public GridInteraction(TowerData data)
    {
        _data = data;
        _tower = GameObject.Instantiate(_data.model);
    }

    public override int GetLayer()
    {
        return Layers.Terrain;
    }

    public override void OnMouseClick(Vector3 position)
    {
        EntityManager.instance.SpawnTowerServerRpc((TowerType)_data.towerType, NetworkManager.Singleton.LocalClientId, PlayerBehaviour.current.grid.GetNearestWalkablePosition(position));
        InteractionManager.instance.EndInteraction();
    }

    public override void OnMouseOver(Vector3 position)
    {
        if (_tower)
        {
            _tower.transform.position = PlayerBehaviour.current.grid.GetNearestWalkablePosition(position);
        }
    }

    public override void Cancel()
    {
        if (_tower)
        {
            GameObject.Destroy(_tower);
            _tower = null;
        }
    }

    public override void End()
    {
        if (_tower)
        {
            GameObject.Destroy(_tower);
            _tower = null;
        }
    }
}