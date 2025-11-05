using System;
using System.Collections.Generic;
using UnityEngine;

public enum EEventType
{
    OnFishCaught,
    MoneyChanged,
    Upgraded,
    OnMapChanged,
    OnAttackBoss,
    OnAttackFish,
    OnStageChanged,
}


public enum EEventTypePayload
{
    OnBossSpawn,
    OnMapChanged,
    
}

public class EventManager
{
    #region Singleton
    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
                instance = new EventManager();
            return instance;
        }
    }
    #endregion

    private readonly Dictionary<EEventType, Action> _events = new();

    public void AddEvent(EEventType eventType, Action listener)
    {
        if (_events.ContainsKey(eventType) == false)
            _events.Add(eventType, new Action(() => { }));

        _events[eventType] += listener;
    }

    public void RemoveEvent(EEventType eventType, Action listener)
    {
        if (_events.ContainsKey(eventType))
            _events[eventType] -= listener;
    }

    public void TriggerEvent(EEventType eventType)
    {
        if (_events.TryGetValue(eventType, out Action action))
            action?.Invoke();
    }

    public void Clear()
    {
        _events.Clear();
        _payloadEvents.Clear();
    }
    
    private readonly Dictionary<EEventTypePayload, List<Action<object>>> _payloadEvents = new();
    private readonly Dictionary<EEventTypePayload, Dictionary<Delegate, Action<object>>> _wrappers = new();
    
    public void AddEvent<T>(EEventTypePayload eventType, Action<T> listener)
    {
        if (!_payloadEvents.TryGetValue(eventType, out List<Action<object>> list))
        {
            list = new List<Action<object>>();
            _payloadEvents[eventType] = list;
            _wrappers[eventType] = new Dictionary<Delegate, Action<object>>();
        }

        void wrapper(object obj)
        {
            if (obj is T tObj)
                listener(tObj);
            else Debug.LogError($"EventManager: Type mismatch! Expected {typeof(T)}, but got {(obj == null ? "null" : obj.GetType().ToString())}");
            
        }

        list.Add(wrapper);
        _wrappers[eventType][listener] = wrapper;
        
#if UNITY_EDITOR
        var method = new System.Diagnostics.StackTrace(false).GetFrame(1).GetMethod();
        Debug.Log($"AddEvent<{typeof(T)}> called for {eventType} by {method.DeclaringType.Name}.{method.Name}");
#endif
    }

    public void RemoveEvent<T>(EEventTypePayload eventType, Action<T> listener)
    {
        if (!_payloadEvents.TryGetValue(eventType, out List<Action<object>> list))
            return;

        if (_wrappers.TryGetValue(eventType, out var dict) && dict.TryGetValue(listener, out var wrapper))
        {
            list.Remove(wrapper);
            dict.Remove(listener);
        }
    }

    public void TriggerEvent<T>(EEventTypePayload eventType, T payload, bool debug = false)
    {
        if (!_payloadEvents.TryGetValue(eventType, out List<Action<object>> list))
            return; 

        foreach (Action<object> listener in list.ToArray())
        {
            listener?.Invoke(payload);
        }
        
        if (debug) { Debug.Log($"EventManager: Triggering event {eventType} with {list.Count} listener(s)"); }
    }
}
