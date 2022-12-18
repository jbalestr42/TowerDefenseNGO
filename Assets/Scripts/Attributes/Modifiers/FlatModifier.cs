using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatModifier : SKU.IAttributeModifier
{
    float _value = 0f;
    bool _isOver = false;
    public bool isOver { get { return _isOver; } set { _isOver = value; } }

    public FlatModifier(float value)
    {
        _value = value;
    }

    public float ApplyModifier()
    {
        return _value;
    }
}