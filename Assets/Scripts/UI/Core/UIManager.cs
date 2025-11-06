using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIEventType
{
    OpenPopup,
    ClosePopup,
    CloseAllPopup,
    UpdatePopup,
    FadeInStore,
    FadeOutStore,
    ChangeScene,
}
public static class UITypeCache<T>
{
    public static readonly string Name = typeof(T).Name;
}

public class UIManager : MonoBehaviour
{
    private readonly Dictionary<string, UI_Base> _uIEntry = new();
    private readonly Stack<UI_Popup> _popupStack = new ();

    public UI_Scene CurSceneUI { get; private set; }
    
    #region Singleton
    public static UIManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Init
    
    public void Init()
    {
        RegisterAllUIs();

        RegisterHandlers();
        
        ChangeSceneUI<UI_Title>();
        UpgradeManager.Instance.Init();
        GetUI<UI_UpgradePanel>().SetUp();
        
        MapManager.Instance.Init();
        GetUI<UI_SelectStageScene>().Init();
        FishingSystem.Instance.Init();
    }
    
    private void RegisterAllUIs()
    {
        _uIEntry.Clear();

        UI_Base[] list = GetComponentsInChildren<UI_Base>(includeInactive: true);

        foreach (UI_Base ui in list)
        {
            ui.Init();

            string key = ui.gameObject.name;

            if (_uIEntry.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate popup detected ' {key}");
                continue;
            }

            _uIEntry.Add(key, ui);
        }
    }

    #endregion
    
    #region Getter
    public UI_Popup GetTopPopup()
    {
        return _popupStack.Count > 0 ? _popupStack.Peek() : null;
    }
    
    public T GetUI<T>() where T : UI_Base
    {
        if (!_uIEntry.TryGetValue(UITypeCache<T>.Name, out UI_Base ui))
            return null;
        
        return ui as T;
    }
    #endregion
    
    #region SceneUI / Popup Control
    
    public void ChangeSceneUI<T>(Action<T> callback = null) where T : UI_Scene
    {
        string key = UITypeCache<T>.Name;
    
        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"UI Not registered : {key}");
        }
    
        CurSceneUI?.gameObject.SetActive(false);
        CurSceneUI = ui as T;
        
        ui?.gameObject.SetActive(true);
        callback?.Invoke(ui as T);
    }
    
    public void ShowPopup<T>(Action<T> callback = null, Transform parent = null) where T : UI_Popup
    {
        string key = UITypeCache<T>.Name;
    
        if (!_uIEntry.TryGetValue(key, out UI_Base ui))
        {
            Debug.Log($"Popup not registered: {key}");
            return;
        }
    
        T popupUI = ui as T;
    
        if (!_popupStack.Contains(popupUI))
            _popupStack.Push(popupUI);
        
        if (parent != null)
            popupUI?.transform.SetParent(parent);
        
        popupUI?.gameObject.SetActive(true);
    
        callback?.Invoke(popupUI);
    }

    
    public void CloseAllPopups()
    {
        while (_popupStack.Count > 0)
            CloseTopPopup();
    }

    public void CloseTopPopup()
    {
        if (_popupStack.Count == 0)
            return;
        
        _popupStack.Pop().gameObject.SetActive(false);

        if (_popupStack.Count > 0)
            _popupStack.Peek().gameObject.SetActive(true);
    }
    
    #endregion
    
    #region UIHandler

    private readonly List<IUIEventHandler> _uiHandlers = new();
    private Action<UIEventType, object> _onUIEvent;

    private void OnEnable() => _onUIEvent += HandleUIEvent;
    private void OnDisable() => _onUIEvent -= HandleUIEvent;

    public void Raise(UIEventType eventType, object payload = null)
    {
        _onUIEvent?.Invoke(eventType, payload);
    }

    private void HandleUIEvent(UIEventType ui, object payload)
    {
        foreach (var handler in _uiHandlers)
        {
            if (handler.Handle(ui, payload))
                break;
        }
    }

    private void RegisterHandlers()
    {
        _uiHandlers.Clear();

        _uiHandlers.Add(new TitleSceneUIHandler());
    }

    #endregion
}
