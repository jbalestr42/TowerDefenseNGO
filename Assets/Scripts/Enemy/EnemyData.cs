﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyData : ScriptableObject
{
    public GameObject model;
    public float health;
    public float speed;
    public int score;
    public int gold;
    public int lifeCost;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Enemy Data")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<EnemyData>();
    }
#endif
}
