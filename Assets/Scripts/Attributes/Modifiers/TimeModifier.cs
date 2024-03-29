﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeModifier : SKU.IAttributeModifier
{
    float _duration = 0f;
    float _value = 0f;
    float _start = 0f;
    public bool isOver { get { return (Time.realtimeSinceStartup - _start) >= _duration; } }

    public TimeModifier(float duration, float value)
    {
        _duration = duration;
        _value = value;
        _start = Time.realtimeSinceStartup;
    }

    public float ApplyModifier()
    {
        return _value * (1f - GetRatio());
    }

    float GetRatio()
    {
        float ratio = (Time.realtimeSinceStartup - _start) / _duration;
        ratio = Mathf.Clamp(ratio, 0f, 1f);
        return ratio;
    }
}
