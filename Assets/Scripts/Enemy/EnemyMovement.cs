using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    SKU.Attribute _speed = null;
    AILerp _aiLerp = null;

    void Update()
    {
        if (_aiLerp != null)
        {
            _aiLerp.speed = _speed.Value;
        }
    }

    public void InitServer(PlayerBehaviour player, float speed)
    {
        transform.position = player.grid.GetComponent<CheckpointManager>().start.position;
        
        _speed = new SKU.Attribute(speed);
        GetComponent<AttributeManager>().Add(AttributeType.Speed, _speed);

        _aiLerp = gameObject.AddComponent<AILerp>();
        _aiLerp.destination = player.grid.GetComponent<CheckpointManager>().end.position;

        RaycastModifier modifier = gameObject.AddComponent<RaycastModifier>();
        modifier.quality = RaycastModifier.Quality.Highest;
    }
}
