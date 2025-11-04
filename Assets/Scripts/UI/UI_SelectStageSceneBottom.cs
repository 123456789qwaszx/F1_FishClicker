using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SelectStageSceneBottom : UI_Base
{
    enum Buttons
    {
        Btn_Back,
    }

    UI_SelectStageScene _selectStageSceneUI;

    public override void Init()
    { }

    protected override void Awake()
    {
        BindButtons(typeof(Buttons));
        
        GetButton((int)Buttons.Btn_Back).gameObject.BindEvent(OnClickInventoryButton);
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
    }

    #region EventHandler
    void OnClickInventoryButton(PointerEventData eventData)
    {
        UIManager.Instance.ChangeSceneUI<UI_InGame>(popup => { popup.UpdateMapUI(); });
    }

    #endregion
}