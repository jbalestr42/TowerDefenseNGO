using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckPointSystem : GridGeneratorSystem
{
    public CheckPoint start { get { return spawnedObjects[0].gameObject.GetComponent<CheckPoint>(); } }

    public override IEnumerator Spawn(GridGenerator gridGenerator)
    {
        yield return base.Spawn(gridGenerator);

        for (int i = 0; i < count - 1; i++)
        {
            spawnedObjects[i].gameObject.GetComponent<CheckPoint>().next = spawnedObjects[i + 1].gameObject.GetComponent<CheckPoint>();
        }

        yield return null;
    }
}