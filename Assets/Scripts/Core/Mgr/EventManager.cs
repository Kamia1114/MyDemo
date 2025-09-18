// 示例 EventManager 类
using System;
using System.Collections.Generic;

public static class EventManager
{
    private static readonly Dictionary<string, Action<EventStruct>> eventTable = new();

    public static void AddListener(string eventName, Action<EventStruct> listener)
    {
        if (!eventTable.ContainsKey(eventName))
            eventTable[eventName] = listener;
        else
            eventTable[eventName] += listener;
    }

    public static void RemoveListener(string eventName, Action<EventStruct> listener)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] -= listener;
            if (eventTable[eventName] == null)
                eventTable.Remove(eventName);
        }
    }

    public static void TriggerEvent(string eventName, EventStruct eventData)
    {
        if (eventTable.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(eventData);
        }
    }
}