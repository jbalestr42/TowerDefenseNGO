using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GridGenerator : MonoBehaviour 
{
    [SerializeField] AGridGeneratorSystem _gridPathGeneratoor;
    [SerializeField] List<AGridGeneratorSystem> _gridGenerators;
    List<Vector2Int> _availablePositions = new List<Vector2Int>();

    public void Generate(GridManager grid, Transform ground, int seed)
    {
        Random.InitState(seed);
        DespawnSystem();
        InitAvailablePositions(grid);
        Spawn(grid, ground);
    }

    void SpawnSystem(GridManager grid, AGridGeneratorSystem gridGenerator, Transform ground)
    {
        int count = gridGenerator.GetRandomCount();
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, _availablePositions.Count - 1);
            Vector2Int coord = _availablePositions[index];
            _availablePositions.RemoveAt(index);

            // if position is not blocking path keep spawning, otherwise and get a new one
            GameObject go = gridGenerator.Spawn(grid, grid.GetCellCenterFromCoord(coord));
            go.transform.SetParent(ground);
        }
    }

    void InitAvailablePositions(GridManager grid)
    {
        _availablePositions.Clear();
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                _availablePositions.Add(new Vector2Int(x, y));
            }
        }
    }

    void Spawn(GridManager grid, Transform ground)
    {
        SpawnSystem(grid, _gridPathGeneratoor, ground);
        foreach (AGridGeneratorSystem gridGenerator in _gridGenerators)
        {
            SpawnSystem(grid, gridGenerator, ground);
        }
    }

    void DespawnSystem()
    {
        _gridPathGeneratoor.DespawnAll();
        foreach (AGridGeneratorSystem gridGenerator in _gridGenerators)
        {
            gridGenerator.DespawnAll();
        }
    }
}

// obstacle
// checkpoint
// empty zone
// teleportation
// gain gold x1.5 // overlay + need empty block
// damage zone // overlay + no need empty block
// slow zone // overlay + no need empty block
// fast zone // overlay + no need empty block