using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AppMenuPanel : UI_Popup
{

    #region enum
    enum Buttons
    {
        Btn_Icon00,
        Btn_Icon01,
        Btn_Icon02,
        Btn_Icon03,
    }
    #endregion
    

    Button Btn_Icon00; //Shop
    Button Btn_Icon01; //UpgradePanel
    Button Btn_Icon02; //Setting
    Button Btn_Icon03; //Stages

    protected override void Awake()
    {
        
        BindButtons(typeof(Buttons));

        Btn_Icon00 = GetButton((int)Buttons.Btn_Icon00);
        Btn_Icon01 = GetButton((int)Buttons.Btn_Icon01);
        Btn_Icon02 = GetButton((int)Buttons.Btn_Icon02);
        Btn_Icon03 = GetButton((int)Buttons.Btn_Icon03);

        BindEvent(Btn_Icon00.gameObject, OnShowShop);
        BindEvent(Btn_Icon01.gameObject, OnShowUpgradePanel);
        BindEvent(Btn_Icon02.gameObject, OnShowSetting);
        BindEvent(Btn_Icon03.gameObject, OnShowStageSelectPopup);
    }

    #region Button
    public void OnShowStageSelectPopup(PointerEventData eventData)
    {
        UIManager.Instance.CloseAllPopupUI();
        UIManager.Instance.ChangeSceneUI<UI_SelectStageScene>();
    }
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
