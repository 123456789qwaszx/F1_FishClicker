using System;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneUIHandler : IUIEventHandler
{
    // UITypeCache<T>를 활용한 안전한 이름-타입 매핑
    private readonly Dictionary<string, Action> _sceneActions;
    private readonly Dictionary<string, Action> _popupActions;

    public TitleSceneUIHandler()
    {
        _sceneActions = new Dictionary<string, Action>
        {
            { UITypeCache<UI_InGame>.Name, () => UIManager.Instance.ChangeSceneUI<UI_InGame>() },
            { UITypeCache<UI_Title>.Name, () => UIManager.Instance.ChangeSceneUI<UI_Title>() },
            { UITypeCache<UI_SelectStageScene>.Name, () => UIManager.Instance.ChangeSceneUI<UI_SelectStageScene>() },
            { UITypeCache<UI_BossStage>.Name, () => UIManager.Instance.ChangeSceneUI<UI_BossStage>() },
        };

        _popupActions = new Dictionary<string, Action>
        {
            { UITypeCache<UI_AppMenuPanel>.Name, () => UIManager.Instance.ShowPopup<UI_AppMenuPanel>() },
            { UITypeCache<UI_Phone>.Name, () => UIManager.Instance.ShowPopup<UI_Phone>() },
            { UITypeCache<UI_UpgradePanel>.Name, () => UIManager.Instance.ShowPopup<UI_UpgradePanel>() },
            { UITypeCache<UI_FishingGame>.Name, () => UIManager.Instance.ShowPopup<UI_FishingGame>() },
        };
    }

    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.ChangeScene:
                if (payload is string sceneKey && _sceneActions.TryGetValue(sceneKey, out var sceneAction))
                {
                    sceneAction.Invoke();
                    return true;
                }
                break;

            case UIEventType.OpenPopup:
                if (payload is string popupKey && _popupActions.TryGetValue(popupKey, out var popupAction))
                {
                    popupAction.Invoke();
                    return true;
                }
                break;

            case UIEventType.ClosePopup:
                UIManager.Instance.CloseTopPopup();
                return true;
            case UIEventType.CloseAllPopup:
                UIManager.Instance.CloseAllPopups();
                return true;
        }

        Debug.Log($"Unknown UI key or event: {payload}");
        return false;
    }
}