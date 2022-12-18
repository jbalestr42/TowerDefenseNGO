using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [MinAttribute(1f)]
    [SerializeField] float _minRange;
    public float minRange { get { return _minRange; } set { _minRange = value; } }

    [MinAttribute(1f)]
    [SerializeField] float _maxRange;
    public float maxRange { get { return _maxRange; } set { _maxRange = value; } }

    float _range;
    public float range { get { return _range; } private set { } }

    [SerializeField] Transform _transform;

    void Start()
    {
        _range = Random.Range(_minRange, _maxRange);
        _transform.localScale = new Vector3(range, range, range);
    }
}
