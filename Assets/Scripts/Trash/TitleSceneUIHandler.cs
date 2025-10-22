using UnityEngine;

public class TitleSceneUIHandler : IUIEventHandler
{
    public bool Handle(UIEventType type, object payload)
    {
        string uiKey = payload as string;
        
        switch (type)
        {
            case UIEventType.ChangeScene:
                UIManager.Instance.ChangeSceneUI(uiKey);
                return true;
        }
        return false;
    }
}
