using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GridCell
{
    public bool IsEmpty { get; set; }
}

public class GridManager : MonoBehaviour 
{
    [SerializeField]
    GameObject _ground;

    [SerializeField]
    int _width;
    public int width { get { return _width; } set { _width = value; } }

    [SerializeField]
    int _height;
    public int height { get { return _height; } set { _height = value; } }

    [SerializeField]
    float _size;
    public float size { get { return _size; } set { _size = value; } }

    Vector3 _min;
    Vector3 _max;
    GridCell[] _grid;
    GridGraph _gridGraph;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        _min = transform.position;
        _min.x -= (_width / 2.0f) * _size;
        _min.z -= (_height / 2.0f) * _size;
        _max = transform.position;
        _max.x += (_width / 2.0f) * _size + _size;
        _max.z += (_height / 2.0f) * _size + _size;

        _grid = new GridCell[_width * _height];
        for (int i = 0; i < _grid.Length; i++)
        {
            _grid[i] = new GridCell();
            _grid[i].IsEmpty = true;
        }

        _ground.transform.localScale = new Vector3(_width * _size, 1f, _height * _size);

        _gridGraph = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        _gridGraph.collision.heightMask = LayerMask.GetMask("Terrain");
        _gridGraph.collision.collisionCheck = false;
        _gridGraph.center = transform.position;
        _gridGraph.SetDimensions(_width, _height, _size);
        _gridGraph.Scan();
    }

    void OnDestroy()
    {
        if (_gridGraph != null)
        {
            AstarPath.active.data.RemoveGraph(_gridGraph);
        }
    }

    public bool IsEmpty(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _grid[y * _width + x].IsEmpty;
        }
        return false;
    }

    public void SetEmpty(int x, int y, bool empty)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _grid[y * _width + x].IsEmpty = empty;
            AstarPath.active.AddWorkItem(new AstarWorkItem(() => {
                _gridGraph.GetNode(x, y).Walkable = empty;
                _gridGraph.CalculateConnectionsForCellAndNeighbours(x, y);
            }));
        }
    }

    public Vector2Int GetCoordFromPosition(Vector3 position)
    {
        Vector2Int coord = Vector2Int.zero;
        coord.x = position.x < _min.x ? 0 : (position.x > _max.x ? _width : (int)((position.x - _min.x) / _size));
        coord.y = position.z < _min.z ? 0 : (position.z > _max.z ? _width : (int)((position.z - _min.z) / _size));
        return coord;
    }

    public Vector3 GetCellCenterFromPosition(Vector3 position)
    {
        Vector2Int coord = GetCoordFromPosition(position);
        return GetCellCenterFromCoord(coord);
    }

    public Vector3 GetCellCenterFromCoord(Vector2Int coord)
    {
        Vector3 cellCenterPos = Vector3.zero;
        cellCenterPos.x = _min.x + coord.x * _size + _size * 0.5f;
        cellCenterPos.z = _min.z + coord.y * _size + _size * 0.5f;
        return cellCenterPos;
    }

    public bool CanPlaceObject(GameObject gameObject)
    {
        var guo = new GraphUpdateObject(gameObject.GetComponentInChildren<Collider>().bounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false;

        // TODO: check object with all checkpoints
        var start = _gridGraph.GetNearest(GameObject.FindWithTag("SpawnStart").transform.position).node;
        var end = _gridGraph.GetNearest(GameObject.FindWithTag("SpawnEnd").transform.position).node;
        return GraphUpdateUtilities.UpdateGraphsNoBlock(guo, start, end, true);
    }

    void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        position.x -= (_width / 2.0f) * _size;
        position.y += 0.1f;
        position.z -= (_height / 2.0f) * _size;

        Gizmos.color = Color.red;
        for (int i = 0; i < _width + 1; i++)
        {
            Vector3 start = new Vector3(i * _size + position.x, position.y, position.z);
            Vector3 end = new Vector3(i * _size + position.x, position.y, _height * _size + position.z);

            Gizmos.DrawLine(start, end);
        }
        for (int i = 0; i < _height + 1; i++)
        {
            Vector3 start = new Vector3(position.x, position.y, i * _size + position.z);
            Vector3 end = new Vector3(_width * _size + position.x, position.y, i * _size + position.z);

            Gizmos.DrawLine(start, end);
        }
    }
}