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
    UpdatePopup,
    FadeInStore,
    FadeOutStore,
    ChangeScene,
}
public static class UITypeCache<T>
{
    // 제너릭 타입별로 단 한 번만 계산됨
    public static readonly string Name = typeof(T).Name;
}

public class UIManager : MonoBehaviour
{
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

    #region UI
    private Stack<UI_Popup> _popupStack = new ();

    private UI_Scene _sceneUI;
    public UI_Scene CurSceneUI
    {
        set { _sceneUI = value; }
        get { return _sceneUI; }
    }

    private readonly Dictionary<string, UI_Base> _uIEntry = new();

    
    public void Init()
    {
        RegisterAllUIs();

        RegisterHandlers();
        
        ChangeSceneUI<UI_Title>();
        UpgradeManager.Instance.Init();
        GameManager.Instance.SyncUpgrades(UpgradeManager.Instance.GetAllUpgrades());
        FindUI<UI_UpgradePanel>().SetUp();
        
        MapManager.Instance.Init();
        FindUI<UI_SelectStageScene>().Init();
        FishingSystem.Instance.Init();
    }
    
    
    #region UIHandler

    private readonly List<IUIEventHandler> uiHandlers = new();
    public Action<UIEventType, object> OnUIEvent;

    private void OnEnable() => OnUIEvent += HandleUIEvent;
    private void OnDisable() => OnUIEvent -= HandleUIEvent;

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

    private void RegisterHandlers()
    {
        uiHandlers.Clear();

        uiHandlers.Add(new TitleSceneUIHandler());
    }

    #endregion

    
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

    
    public T FindUI<T>() where T : UI_Base
    {
        return _uIEntry[typeof(T).Name] as T;
    }


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
    
    
    public void ChangeSceneUI(string key, Action callback = null)
    {
        CurSceneUI?.gameObject.SetActive(false);
        
        _uIEntry.TryGetValue(key, out UI_Base ui);
        CurSceneUI = ui as UI_Scene;
        
        CurSceneUI?.gameObject.SetActive(true);
        callback?.Invoke();
    }

    
    public void ShowPopup<T>(Action<T> callback = null, Transform parent = null) where T : UI_Popup
    {
        String key = UITypeCache<T>.Name;

        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"Popup not registered: {key}");
        }

        T popupUI = ui as T;

        
        _popupStack.Push(popupUI);

        if (_popupStack.Count <= 0) return;

        T top = (T)_popupStack.Peek();
        top.gameObject.SetActive(true);

        callback?.Invoke(top);

        if (parent != null)
            top.transform.SetParent(parent);
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0) return;
        _popupStack.Pop().gameObject.SetActive(false);

        if (_popupStack.Count > 0)
            _popupStack.Peek().gameObject.SetActive(true);
    }

    
    public UI_Popup PeekCurPopup()
    {
        return _popupStack.Count > 0 ? _popupStack.Peek() : null;
    }
    #endregion
    
    public void CloseUI<T>() where T : UI_Base
    {
        String key = UITypeCache<T>.Name;

        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"Popup not registered: {key}");
        }
        ui?.gameObject.SetActive(false);
    }
}
