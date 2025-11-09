using UnityEngine;

public static class GameEventHelper
{
    public static void TriggerBossSpawnEvent()
    {
        FishingManager.Instance.Controller.SpawnNewFish();
        UIManager.Instance.ChangeSceneUI<UI_BossStage>(stage =>
            stage.SetupBossUI(FishingManager.Instance.Controller.CurFish)
        );
        
        UIManager.Instance.CloseAllPopups();
    }

    public static void OnReturnToStage()
    {
        UIManager.Instance.ChangeSceneUI<UI_InGame>(popup =>
        {
            popup.UpdateRegionUI();
            popup.UpdateHUD();
        });
        UIManager.Instance.ShowPopup<UI_FishingGame>(popup =>
        {
            popup.RefreshFishUI();
        });
        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
    }
}