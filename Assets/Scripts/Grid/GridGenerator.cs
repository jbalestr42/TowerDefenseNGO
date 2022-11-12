using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GridGenerator : MonoBehaviour 
{
    [SerializeField] GridGeneratorSystem _gridPathGenerator;
    [SerializeField] List<GridGeneratorSystem> _gridGenerators;

    List<GridCell> _availableCells = new List<GridCell>();
    GridManager _gridManager;

    public void Generate(GridManager grid, Transform ground, int seed)
    {
        _gridManager = grid;
        Random.InitState(seed);
        DespawnSystems();
        InitAvailablePositions(grid);
        SpawnSystems(grid, ground);
    }

    void SpawnSystem(GridManager grid, GridGeneratorSystem gridGenerator, Transform ground)
    {
        gridGenerator.Spawn(this);
        foreach (GameObject spawnedObject in gridGenerator.spawnedObjects)
        {
            spawnedObject.transform.SetParent(ground);
        }
    }

    public GridCell GetRandomCell(bool isWalkable)
    {
        int index = Random.Range(0, _availableCells.Count - 1);
        GridCell cell = _availableCells[index];
        _availableCells.RemoveAt(index);
        _gridManager.SetEmpty(cell, isWalkable);
        return cell;
    }

    void InitAvailablePositions(GridManager grid)
    {
        _availableCells.Clear();
        foreach (GridCell cell in grid.cells)
        {
            grid.SetEmpty(cell, true);
            _availableCells.Add(cell);
        }
    }

    void SpawnSystems(GridManager grid, Transform ground)
    {
        SpawnSystem(grid, _gridPathGenerator, ground);
        foreach (GridGeneratorSystem gridGenerator in _gridGenerators)
        {
            SpawnSystem(grid, gridGenerator, ground);
        }
    }

    void DespawnSystems()
    {
        _gridPathGenerator.DespawnAll();
        foreach (GridGeneratorSystem gridGenerator in _gridGenerators)
        {
            gridGenerator.DespawnAll();
        }
    }
}

            // Use fonction en grid cell enter/leave ? with on applyeffect() on enter


// obstacle
// checkpoint
// empty zone
// teleportation
// gain gold x1.5 // overlay + need empty block
// damage zone // overlay + no need empty block
// slow zone // overlay + no need empty block
// fast zone // overlay + no need empty block