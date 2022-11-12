using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GridManager : MonoBehaviour 
{
    [SerializeField] GameObject _ground;
    [SerializeField] CheckPointSystem _checkPoints;

    [SerializeField] int _width;
    public int width { get { return _width; } set { _width = value; } }

    [SerializeField] int _height;
    public int height { get { return _height; } set { _height = value; } }

    [SerializeField] float _size;
    public float size { get { return _size; } set { _size = value; } }

    GridCell[] _cells;
    public GridCell[] cells { get { return _cells; } set { _cells = value; } }

    Vector3 _min;
    Vector3 _max;
    GridGraph _gridGraph;

    public void Generate()
    {
        _min = transform.position;
        _min.x -= (_width / 2.0f) * _size;
        _min.z -= (_height / 2.0f) * _size;
        _max = transform.position;
        _max.x += (_width / 2.0f) * _size + _size;
        _max.z += (_height / 2.0f) * _size + _size;

        _cells = new GridCell[_width * _height];
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i] = new GridCell();
            _cells[i].isEmpty = true;
            _cells[i].coord = new Vector2Int(i % _width, i / _width);
            _cells[i].center = GetCellCenterFromCoord(_cells[i].coord);
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
        if (AstarPath.active != null && _gridGraph != null)
        {
            AstarPath.active.data.RemoveGraph(_gridGraph);
        }
    }

    public bool IsEmpty(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _cells[y * _width + x].isEmpty;
        }
        return false;
    }

    public void SetEmpty(GridCell cell, bool empty)
    {
        SetEmpty(cell.coord.x, cell.coord.y, empty);
    }

    public void SetEmpty(int x, int y, bool empty)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _cells[y * _width + x].isEmpty = empty;
            AstarPath.active.AddWorkItem(new AstarWorkItem(() => {
                _gridGraph.GetNode(x, y).Walkable = empty;
                _gridGraph.CalculateConnectionsForCellAndNeighbours(x, y);
            }));
        }
    }

    public Vector2Int GetCoordFromPosition(Vector3 position)
    {
        Vector2Int coord = Vector2Int.one;
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
        Vector3 cellCenterPos = Vector3.one;
        cellCenterPos.x = _min.x + coord.x * _size + _size * 0.5f;
        cellCenterPos.y = _size * 0.5f;
        cellCenterPos.z = _min.z + coord.y * _size + _size * 0.5f;
        return cellCenterPos;
    }

    public bool CanPlaceObject(GameObject gameObject)
    {
        CheckPoint checkPoint = _checkPoints.start;
        while (checkPoint.next != null)
        {
            GraphUpdateObject guo = new GraphUpdateObject(gameObject.GetComponentInChildren<Collider>().bounds);
            guo.modifyWalkability = true;
            guo.setWalkability = false;

            Vector2Int nodeCoord = GetCoordFromPosition(checkPoint.transform.position);
            var node = _gridGraph.GetNode(nodeCoord.x, nodeCoord.y);
            Vector2Int nextNodeCoord = GetCoordFromPosition(checkPoint.next.transform.position);
            var nextNode = _gridGraph.GetNode(nextNodeCoord.x, nextNodeCoord.y);
            //var node = _gridGraph.GetNearest(checkPoint.transform.position).node;
            //var nextNode = _gridGraph.GetNearest(checkPoint.next.transform.position).node;
            checkPoint = checkPoint.next;
            if (!GraphUpdateUtilities.UpdateGraphsNoBlock(guo, node, nextNode, true))
            {
                return false;
            }
        }
        return true;
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