using UnityEngine;

public static class GameEventHelper
{
    public static void TriggerBossSpawnEvent()
    {
        var bossData = MapManager.Instance.GetCurrentMap().bossData;
        if (bossData == null) { Debug.Log("GameEventHelper: BossMiniGameData is null!"); return; }

        EventManager.Instance.TriggerEvent(EEventTypePayload.OnBossSpawn, bossData, true);
    }
}