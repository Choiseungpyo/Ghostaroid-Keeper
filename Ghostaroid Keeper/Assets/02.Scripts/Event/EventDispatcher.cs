using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventDispatcher
{
    private static readonly Dictionary<Type, List<object>> listeners = new Dictionary<Type, List<object>>();

    public static void RegisterListener<T>(IEventListener<T> listener) where T : IEvent
    {
        if (listener == null) return;

        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var list))
        {
            list = new List<object>(8);
            listeners[type] = list;
        }

        if (!list.Contains(listener))
            list.Add(listener);
    }

    public static void UnregisterListener<T>(IEventListener<T> listener) where T : IEvent
    {
        if (listener == null) return;

        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var list))
            return;

        list.Remove(listener);

        if (list.Count == 0)
            listeners.Remove(type);
    }

    public static void Dispatch<T>(T gameEvent) where T : IEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var list))
            return;

        if (list.Count == 0)
            return;

        var snapshot = list.ToArray();

        for (int i = 0; i < snapshot.Length; ++i)
        {
            var obj = snapshot[i];

            if (obj is UnityEngine.Object uo && uo == null)
            {
                list.Remove(obj);
                continue;
            }

            var l = obj as IEventListener<T>;
            if (l == null) continue;

            l.OnEvent(gameEvent);
        }

        if (list.Count == 0)
            listeners.Remove(type);
    }
}