using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

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
        
        ChangeSceneUI<UI_Title>();
        UpgradeManager.Instance.Init();
        GameManager.Instance.SyncUpgrades(UpgradeManager.Instance.GetAllUpgrades());
        FindUI<UI_UpgradePanel>().SetUp();
        
        MapManager.Instance.Init();
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
    
    public UI_Popup PeekCurPopup()
    {
        return _popupStack.Count > 0 ? _popupStack.Peek() : null;
    }

    public T FindUI<T>() where T : UI_Base
    {
        return _uIEntry[typeof(T).Name] as T;
    }

    public UI_Popup GetPopup()
    {
        return _popupStack.Count > 0 ? _popupStack.Peek() : null;
    }

    public void ChangeSceneUI<T>(Action<T> callback = null) where T : UI_Scene
    {
        string key = typeof(T).Name;

        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"UI Not registered : {key}");
        }

        ui?.gameObject.SetActive(true);
        callback?.Invoke(ui as T);

        CurSceneUI?.gameObject.SetActive(false);
        CurSceneUI = ui as T;
    }
    
    
    public void ChangeSceneUI(string key, Action callback = null)
    {
        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"UI Not registered : {key}");
        }

        ui?.gameObject.SetActive(true);
        callback?.Invoke();

        CurSceneUI?.gameObject.SetActive(false);
        CurSceneUI = ui as UI_Scene;
    }

    public void AddPopupUI<T>() where T : UI_Popup
    {
        string key = typeof(T).Name;

        if (_uIEntry.TryGetValue(key, out UI_Base ui) == false)
        {
            Debug.LogError($"UI Not registered : {key}");
        }

        _popupStack.Push(ui as T);
    }

    public void ShowPopupUI(Action callback = null, Transform parent = null)
    {
        if (_popupStack.Count > 0)
        {
            UI_Popup top = _popupStack.Peek();
            callback?.Invoke();
            top?.transform.SetParent(parent);
        }
    }

    public void ShowPopup<T>(Action<T> callback = null, Transform parent = null) where T : UI_Popup
    {
        String key = typeof(T).Name;

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

    #endregion
}