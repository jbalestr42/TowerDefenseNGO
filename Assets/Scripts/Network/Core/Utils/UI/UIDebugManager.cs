using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDebugManager : Singleton<UIDebugManager>
{
    [SerializeField]
    TMP_Text _persistentDebugText;

    [SerializeField]
    TMP_Text _timedDebugText;

    [SerializeField]
    Canvas _canvas;

    public struct TimedMessage
    {
        public float time;
        public string message;
    }
    List<TimedMessage> timedMessages { get; set; } = new List<TimedMessage>();

    List<Object> permanentMessages { get; set; } = new List<Object>();

    void Update()
    {
        _persistentDebugText.text = "";

        foreach (Object obj in permanentMessages)
        {
            _persistentDebugText.text += obj.ToString() + "\n";
        }

        _timedDebugText.text = "";
        for (int i = timedMessages.Count - 1; i >= 0; i--)
        {
            if (timedMessages[i].time <= Time.realtimeSinceStartup)
            {
                timedMessages.RemoveAt(i);
            }
            else
            {
                _timedDebugText.text += timedMessages[timedMessages.Count - 1 - i].message + "\n";
            }
        }
    }

    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }

    public static void AddMessage(Object obj)
    {
        instance.permanentMessages.Add(obj);
    }

    public static void RemoveMessage(Object obj)
    {
        if (instance)
        {
            instance.permanentMessages.Remove(obj);
        }
    }

    public static void AddTimedMessage(string message, float duration = 15f)
    {
        TimedMessage msg = new TimedMessage();
        msg.message = message;
        msg.time = Time.realtimeSinceStartup + duration;
        instance.timedMessages.Add(msg);
        Debug.Log(message);
    }
}