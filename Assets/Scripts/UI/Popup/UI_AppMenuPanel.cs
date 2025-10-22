using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AppMenuPanel : UI_Popup
{

    #region enum
    enum Buttons
    {
        // Btn_Icon00_Pressed,
        // Btn_Icon00_Normal,
        // Btn_Icon02_Pressed,
        // Btn_Icon02_Normal,

        Btn_Icon00,
        Btn_Icon01,
        Btn_Icon02
    }
    #endregion
    
    // Button Btn_Icon00_Pressed;
    // Button Btn_Icon00_Normal;
    // Button Btn_Icon02_Pressed;
    // Button Btn_Icon02_Normal;

    Button Btn_Icon00; //Shop
    Button Btn_Icon01; //UpgradePanel
    Button Btn_Icon02; //Setting

    protected override void Awake()
    {
        
        BindButtons(typeof(Buttons));

        // Btn_Icon00_Pressed = GetButton((int)Buttons.Btn_Icon00_Pressed);
        // Btn_Icon00_Normal = GetButton((int)Buttons.Btn_Icon00_Normal);
        // Btn_Icon02_Pressed = GetButton((int)Buttons.Btn_Icon02_Pressed);
        // Btn_Icon02_Normal = GetButton((int)Buttons.Btn_Icon02_Normal);

        Btn_Icon00 = GetButton((int)Buttons.Btn_Icon00);
        Btn_Icon01 = GetButton((int)Buttons.Btn_Icon01);
        Btn_Icon02 = GetButton((int)Buttons.Btn_Icon02);

        BindEvent(Btn_Icon00.gameObject, OnShowShop);
        BindEvent(Btn_Icon01.gameObject, OnShowUpgradePanel);
        BindEvent(Btn_Icon02.gameObject, OnShowSetting);
    }

    #region Button
    public void OnShowShop(PointerEventData eventData)
    {
        UIManager.Instance.CloseAllPopupUI();
        //UIManager.Instance.ChangeSceneUI<UI_Shop>();
    }

    void OnShowUpgradePanel(PointerEventData eventData)
    {
        ClosePopupUI();
        UIManager.Instance.ShowPopup<UI_UpgradePanel>(
            popup =>
            {
                popup.UpdateSlots();
            },
        UIManager.Instance.FindUI<UI_Phone>()?.UpgradePanelBox
            );
    }

    void OnShowSetting(PointerEventData eventData)
    {
        UIManager.Instance.CloseAllPopupUI();
        //MainMenuUIManager.Instance.ChangePanel(MainMenuUIManager.Instance.SettingPanel);
    }
    #endregion
}
