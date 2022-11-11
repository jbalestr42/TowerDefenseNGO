using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    CheckPoint _next;
    public CheckPoint next { get { return _next; } set { _next = value; } }
    public bool isLast { get { return _next == null; } }
}
