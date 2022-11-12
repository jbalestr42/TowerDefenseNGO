using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckPointSystem : GridGeneratorSystem
{
    public CheckPoint start { get { return spawnedObjects[0].GetComponent<CheckPoint>(); } }

    public override void Spawn(GridGenerator gridGenerator)
    {
        base.Spawn(gridGenerator);

        for (int i = 0; i < count - 1; i++)
        {
            spawnedObjects[i].GetComponent<CheckPoint>().next = spawnedObjects[i + 1].GetComponent<CheckPoint>();
        }
    }
}