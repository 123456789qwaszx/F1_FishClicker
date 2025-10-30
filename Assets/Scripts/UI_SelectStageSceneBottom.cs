using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SelectStageSceneBottom : UI_Base
{
    enum Buttons
    {
        InventoryButton,
        PlayButton,
        ShopButton,
    }

    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventoryButton);
        GetButton((int)Buttons.PlayButton).gameObject.BindEvent(OnClickPlayButton);
        GetButton((int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
    }

    #region EventHandler
    void OnClickInventoryButton(PointerEventData eventData)
    {
        UIManager.Instance.ShowPopup<UI_InventoryPopup>(callback: (popup) =>
        {
            _selectStageSceneUI.InventoryPopupUI = popup;
        });
    }

    void OnClickPlayButton(PointerEventData evt)
    {
        _selectStageSceneUI.ShowStartStagePopup();
    }

    void OnClickShopButton(PointerEventData evt)
    {
        UIManager.Instance.ShowPopup<UI_ShopPopup>();
    }
    #endregion
}