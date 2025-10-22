using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Phone : UI_Popup
{
    #region enum
    enum Texts
    {
    }


    enum Buttons
    {
        Btn_Home,
        Btn_Back,
    }


    enum Images
    {
        Img_Phone,
        Img_SafeAreaBottom,
        Img_RecentAppsButton,
    }


    enum Objects
    {
    }

    #endregion

    Image Img_Phone;
    Image Img_SafeAreaBottom;
    Image Img_RecentAppsButton;
    Button Btn_Home;
    Button Btn_Back;

    public Transform UpgradePanelBox;
    public Transform AppMenuPanelBox;


    protected override void Awake()
    {
        base.Awake();
        
        UpgradePanelBox = ComponentHelper.TryFindChild(this, "UpgradePanelBox");
        AppMenuPanelBox = ComponentHelper.TryFindChild(this, "AppMenuPanelBox");

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(Objects));

        Img_Phone = GetImage((int)Images.Img_Phone);
        Img_SafeAreaBottom = GetImage((int)Images.Img_SafeAreaBottom);
        Img_RecentAppsButton = GetImage((int)Images.Img_RecentAppsButton);
        Btn_Home = GetButton((int)Buttons.Btn_Home);
        Btn_Back = GetButton((int)Buttons.Btn_Back);
        
        BindEvent(Btn_Back.gameObject, OnBackPannel);
        BindEvent(Btn_Home.gameObject, OnHomeButton);
    }
    
    public void UpdateSlots()
    {
        
    }
    
    #region Button
    void OnBackPannel(PointerEventData eventData)
    {
        if (UIManager.Instance.PeekCurPopup() is UI_UpgradePanel)
        {
            UIManager.Instance.ClosePopupUI();
            UIManager.Instance.ShowPopup<UI_AppMenuPanel>(null, AppMenuPanelBox);
        }
        else
        {
            UIManager.Instance.CloseAllPopupUI();
        }
    }

    void OnHomeButton(PointerEventData eventData)
    {
        UIManager.Instance.CloseAllPopupUI();
    }
    #endregion
}
