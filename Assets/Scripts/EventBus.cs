using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public enum UIEventType
{
    //pause, Option
    OpenPause,
    ClosePause,
    OpenOption,
    CloseOption,
    CloseSound,
    CloseVideo,
    CloseControl,
    ShowSoundTab,
    ShowVideoTab,
    ShowControlTab,
    OnClickNewGame,

    //StartScene
    ShowStartWarningPanel,
    ShowStartMenuWithSave,
    ShowStartMenuNoSave,
    LoadMainScene,
    QuitGame,
    ShowPressAnyKey,
    LoadDayScene,
    LoadIntroScene,

    //MainSceneUI
    ShowRoundTimer,
    HideRoundTimer,
    UpdateTotalEarnings,
    UpdateTodayEarnings,
    ShowMenuPanel,
    UpdateMenuPanel,
    HideMenuPanel,
    ShowResultPanel,
    HideResultPanel,
    ShowOrderPanel,
    HideOrderPanel,
    UpdateBonusText,
    ShowWallPopup,
    HideWallPopup,
    ShowIngredientWarn,

    //Inventory
    ShowInventory,
    HideInventory,
    ShowRecipeBook,
    ShowStationPanel,
    ShowQuestPanel,
    FadeInInventory,
    FadeInRecipeBook,

    //store
    FadeInStore,
    FadeOutStore,
    ShowStationStore,
    ShowRecipeStore,

    //Dialogue
    ShowDialogueLine,
    HideDialoguePanel,
    ShowDialoguePanel,

    //Tutorial
    tu1,
    tu2,
    tu3,
    tu3_step2,
    tu3_step3,
    tu3_step4,
    tu3_step5,
    tu3_step6,
    tu3_step7,
    tu3_stop,
    tu4,
    tu5,
    tu6,
    tu7,
    tu8,
    tu9,
    tu8_stop,
    ShowDemoEnd
}


public enum GameEventType
{
    GamePhaseChanged, // 게임 상태 변경
    
    RoundTimerEnded,  // 영업 시간 종료
    AllCustomersLeft,
    DialogueEnded,
    QuestStatusChanged,
    
    NoMoreStoriesInPhase,
    
    // 튜토리얼용
    PlayerPickedUpItem,
    StationUsed,
    CustomerServed,
    StationLayoutChanged,
    UISceneReady,
}

public static class EventBus
{
    public static Action<UIEventType, object> OnUIEvent;
    
    public static Action<GameEventType, object> OnGameEvent;
    private static Dictionary<GameEventType, Action<object>> gameEventListeners = new();
    
    public static void Raise(UIEventType uiType, object payload = null)
    {
        OnUIEvent?.Invoke(uiType, payload);
    }
    
    public static void Raise(GameEventType eventType, object payload = null)
    {
        OnGameEvent?.Invoke(eventType, payload);

        if (gameEventListeners.TryGetValue(eventType, out var callback))
        {
            callback.Invoke(payload);
        }
    }
}