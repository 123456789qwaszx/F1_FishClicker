using System;
using System.Collections.Generic;
using UnityEngine;

public enum UIEventType
{
    OpenPopup,
    ClosePopup,
    UpdatePopup,
    FadeInStore,
    FadeOutStore,
    
    ChangeScene,
}

public enum EEventType
{
    OnFishCaught,
    MoneyChanged,
    Upgraded,
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

    private Dictionary<EEventType, Action> _events = new Dictionary<EEventType, Action>();

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
        if (_events.ContainsKey(eventType))
            _events[eventType].Invoke();
    }

    public void Clear()
    {
        _events.Clear();
    }
    
    
    #region UIHandler

    private List<IUIEventHandler> uiHandlers = new();
    public Action<UIEventType, object> OnUIEvent;
    
    private void OnEnable()
    {
        OnUIEvent += HandleUIEvent;
    }

    private void OnDisable()
    {
        OnUIEvent -= HandleUIEvent;
    }
    
    
    public void Raise(UIEventType eventType, object payload = null)
    {
        OnUIEvent?.Invoke(eventType, payload);
    }


    private void HandleUIEvent(UIEventType ui, object payload)
    {
        foreach (var handler in uiHandlers)
        {
            if (handler.Handle(ui, payload))
                break;
        }
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
    #endregion
}