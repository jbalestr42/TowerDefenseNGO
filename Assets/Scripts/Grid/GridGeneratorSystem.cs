using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GridGeneratorSystem : MonoBehaviour
{
    [SerializeField] int _min = 2;
    public int min { get { return _min; } set { _min = value; } }

    [SerializeField] int _max = 5;
    public int max { get { return _max; } set { _max = value; } }

    [SerializeField] GameObject _prefab;

    [SerializeField] bool _isWalkable = false;

    List<GameObject> _spawnedObjects = new List<GameObject>();
    public List<GameObject> spawnedObjects { get { return _spawnedObjects; } }

    public int count { get { return _spawnedObjects.Count; } }

    public int GetRandomCount()
    {
        return Random.Range(_min, _max);
    }

    public virtual void Spawn(GridGenerator gridGenerator)
    {
        int count = GetRandomCount();
        for (int i = 0; i < count; i++)
        {
            GridCell cell = gridGenerator.GetRandomCell(_isWalkable);
            SpawnObject(cell.center);
        }
    }

    public virtual GameObject SpawnObject(Vector3 position)
    {
        GameObject go = Instantiate(_prefab, position, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        _spawnedObjects.Add(go);
        return go;
    }

    public virtual void DespawnAll()
    {
        foreach (GameObject go in _spawnedObjects)
        {
            go.GetComponent<NetworkObject>().Despawn();
        }
        _spawnedObjects.Clear();
    }
}