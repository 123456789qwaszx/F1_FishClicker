using UnityEngine;

public class UI_Popup : UI_Base
{
    public virtual void ClosePopupUI()
    {
        UIManager.Instance.ClosePopupUI();
    }
}