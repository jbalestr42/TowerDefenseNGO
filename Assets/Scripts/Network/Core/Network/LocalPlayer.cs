using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalPlayer : Singleton<LocalPlayer>
{
    [HideInInspector]
    public UnityEvent<GameObject> OnLocalPlayerInitialized = new UnityEvent<GameObject>();

    [HideInInspector]
    public UnityEvent<GameObject> OnClientPlayerInitialized = new UnityEvent<GameObject>();

    [HideInInspector]
    public UnityEvent<GameObject> OnServerPlayerInitialized = new UnityEvent<GameObject>();

    // Shortcut to access the player object (available after the initialization callback)
    [HideInInspector]
    public GameObject networkPlayer { get; set; } = null;
}