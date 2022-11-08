using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    SKU.Attribute _speed = null;
    AILerp _aiLerp = null;
    CheckPoint _destination;
    public CheckPoint destination { get { return _destination; } }

    void Update()
    {
        if (_aiLerp != null)
        {
            _aiLerp.speed = _speed.Value;
        }
    }

    public void InitServer(PlayerBehaviour player, float speed)
    {
        _speed = new SKU.Attribute(speed);
        GetComponent<AttributeManager>().Add(AttributeType.Speed, _speed);
        
        CheckPoint start = player.grid.GetComponent<CheckPointList>().start;
        transform.position = start.transform.position;

        _aiLerp = gameObject.AddComponent<AILerp>();
        SetNextDestination(start.next);

        RaycastModifier modifier = gameObject.AddComponent<RaycastModifier>();
        modifier.quality = RaycastModifier.Quality.Highest;
    }

    public void SetNextDestination(CheckPoint destination)
    {
        _destination = destination;
        _aiLerp.destination = destination.transform.position;
    }
}
