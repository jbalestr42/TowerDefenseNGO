using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Transform _start;
    public Transform start { get { return _start; } set { _start = value; } }

    [SerializeField] Transform _end;
    public Transform end { get { return _end; } set { _end = value; } }
}
