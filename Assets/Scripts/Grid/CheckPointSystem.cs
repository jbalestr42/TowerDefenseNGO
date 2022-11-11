using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckPointSystem : AGridGeneratorSystem
{
    [SerializeField] CheckPoint _checkpointPrefab;

    [SerializeField] List<CheckPoint> _checkPoints = new List<CheckPoint>();
    public List<CheckPoint> checkPoints { get { return _checkPoints; } set { _checkPoints = value; } }

    public int count { get { return _checkPoints.Count; } }
    public CheckPoint start { get { return _checkPoints[0]; } }

    public override GameObject Spawn(GridManager grid, Vector3 position)
    {
        CheckPoint checkPoint = Instantiate(_checkpointPrefab, position, Quaternion.identity);
        checkPoint.GetComponent<NetworkObject>().Spawn();
        if (_checkPoints.Count > 0)
        {
            _checkPoints[_checkPoints.Count - 1].next = checkPoint;
        }
        _checkPoints.Add(checkPoint);
        return checkPoint.gameObject;
    }

    public override void DespawnAll()
    {
        foreach (CheckPoint checkPoint in _checkPoints)
        {
            checkPoint.GetComponent<NetworkObject>().Despawn();
        }
        _checkPoints.Clear();
    }
}