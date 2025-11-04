using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SelectStageSceneTop : UI_Base
{
    enum Texts
    {
        CoinText,
        DiaText,
    }

    enum Buttons
    {
        CoinPlusButton,
        DiaPlusButton,
    }

    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.CoinPlusButton).gameObject.BindEvent(OnClickCoinPlusButton);
        GetButton((int)Buttons.DiaPlusButton).gameObject.BindEvent(OnClickDiaPlusButton);
        
        Refresh();
    }

    public void SetInfo(UI_SelectStageScene sceneUI)
    {
        _selectStageSceneUI = sceneUI;
        Refresh();
    }

    public void Refresh()
    {
        //GetText((int)Texts.CoinText).text = Managers.Game.Coin.ToString();
        //GetText((int)Texts.DiaText).text = Managers.Game.Dia.ToString();
    }

    #region EventHandler

    void OnClickCoinPlusButton(PointerEventData evt)
    {
        Debug.Log("OnClickCoinPlusButton");
    }

    void OnClickDiaPlusButton(PointerEventData evt)
    {
        Debug.Log("OnClickDiaPlusButton");
    }

    #endregion
}