using UnityEngine;

public class TitleSceneUIHandler : IUIEventHandler
{
    public bool Handle(UIEventType type, object payload)
    {
        switch (type)
        {
            case UIEventType.OpenPopup:
                return true;
        }
        return false;
    }
}
