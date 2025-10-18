using System;
using System.Collections.Generic;
using UnityEngine;

public enum UIEventType
{
    OpenPopup,
    ClosePopup,
    UpdatePopup,
    FadeInStore,
    FadeOutStore
}

public enum GameEventType
{
    MoneyChanged,
    Upgraded,
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

    private Dictionary<GameEventType, Action> _events = new Dictionary<GameEventType, Action>();

    public void AddEvent(GameEventType eventType, Action listener)
    {
        if (_events.ContainsKey(eventType) == false)
            _events.Add(eventType, new Action(() => { }));

        _events[eventType] += listener;
    }

    public void RemoveEvent(GameEventType eventType, Action listener)
    {
        if (_events.ContainsKey(eventType))
            _events[eventType] -= listener;
    }

    public void TriggerEvent(GameEventType eventType)
    {
        if (_events.ContainsKey(eventType))
            _events[eventType].Invoke();
    }

    public void Clear()
    {
        _events.Clear();
    }
    
    
    #region UIHandler

    private List<IUIEventHandler> uiHandlers = new();
    public static Action<UIEventType, object> OnUIEvent;
    
    private void OnEnable()
    {
        OnUIEvent += HandleUIEvent;
    }

    private void OnDisable()
    {
        OnUIEvent -= HandleUIEvent;
    }

    private void RegisterHandlersForScene(string sceneName)
    {
        uiHandlers.Clear();
        
        //공통 핸들러 (항상 등록)
        
        uiHandlers.Add(new TitleSceneUIHandler());
        //추가

        switch (sceneName)
        {
            // 씬별 핸들러 추가
            // case 
        }
    }

    private void HandleUIEvent(UIEventType ui, object payload)
    {
        foreach (var handler in uiHandlers)
        {
            if (handler.Handle(ui, payload))
                break;
        }
    }

    #endregion
}