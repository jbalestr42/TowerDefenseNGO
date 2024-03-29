﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenModifier : SKU.IAttributeModifier
{
    float _regenRate = 0f;
    float _value = 0f;
    float _start = 0f;
    bool _isOver = false;
    public bool isOver { get { return _isOver; } set { _isOver = value; } }

    public RegenModifier(float value, float regenRate)
    {
        _value = value;
        _regenRate = regenRate;
        _start = Time.realtimeSinceStartup;
    }

    public float ApplyModifier()
    {
        float diff = Time.realtimeSinceStartup - _start;
        if (diff >= _regenRate)
        {
            _start = Time.realtimeSinceStartup - (diff - _regenRate);
            return _value;
        }
        return 0f;
    }
}