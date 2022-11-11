using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGridGeneratorSystem : MonoBehaviour
{
    [SerializeField] int _min = 2;
    public int min { get { return _min; } set { _min = value; } }

    [SerializeField] int _max = 5;
    public int max { get { return _max; } set { _max = value; } }

    public int GetRandomCount()
    {
        return Random.Range(_min, _max);
    }

    public abstract GameObject Spawn(GridManager grid, Vector3 position);

    public abstract void DespawnAll();
}