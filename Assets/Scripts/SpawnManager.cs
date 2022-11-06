using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] List<Transform> _spawnPoint;

    int _index = 0;

    public Transform GetNextSpawnPoint()
    {
        Transform t = _spawnPoint[_index];
        _index++;
        return t;
    }
}
