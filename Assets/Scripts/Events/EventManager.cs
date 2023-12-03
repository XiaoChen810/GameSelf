using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializedDictionary("name", "event")]
    public SerializedDictionary<string, EventBase> preEventDict = new SerializedDictionary<string, EventBase>();

    private void Awake()
    {
        Instance = this;
    }
}
