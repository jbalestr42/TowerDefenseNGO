using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    SKU.Attribute _speed = null;
    AIPath _aiPath = null;

    void Update()
    {
        if (_aiPath != null)
        {
            _aiPath.maxSpeed = _speed.Value;
        }
    }

    public void InitServer(PlayerBehaviour player, float speed)
    {
        transform.position = player.grid.GetComponent<CheckpointManager>().start.position;
        
        _speed = new SKU.Attribute(speed);
        GetComponent<AttributeManager>().Add(AttributeType.Speed, _speed);

        _aiPath = gameObject.AddComponent<AIPath>();
        _aiPath.pickNextWaypointDist = 0.5f;
        AIDestinationSetter destination = gameObject.AddComponent<AIDestinationSetter>();
        destination.target = player.grid.GetComponent<CheckpointManager>().end;

        RaycastModifier modifier = gameObject.AddComponent<RaycastModifier>();
        modifier.thickRaycast = true;
        modifier.thickRaycastRadius = 0.5f;
    }
}
