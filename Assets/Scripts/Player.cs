using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // 1. 기본 자원
    public long TotalGold = 0;

    // 2. 잡은 물고기 수 (fishName 기반)
    public Dictionary<string, int> CaughtFishCount = new Dictionary<string, int>();

    // 3. 도감: 잡아본 물고기
    public HashSet<string> FishDex = new HashSet<string>();

    // 4. 업그레이드 레벨 (UpgradeType.id 기준)
    public Dictionary<string, int> UpgradeLevels = new Dictionary<string, int>();

    // 5. 스테이지 진행도 (MapID -> MapProgress)
    public Dictionary<int, int> MapClearedStageIndices = new Dictionary<int, int>();

    // 6. 총 잡은 물고기 수 (통계)
    public int TotalFishCaught = 0;

    public void Reset()
    {
        TotalGold = 0;
        CaughtFishCount.Clear();
        FishDex.Clear();
        UpgradeLevels.Clear();
        TotalFishCaught = 0;
    }
}

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public PlayerData Data { get; private set; } = new PlayerData();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Fish

    // 물고기 잡기 (FishingSystem과 연동)
    public void CatchFish(string fishName, long goldReward)
    {
        if (!Data.CaughtFishCount.ContainsKey(fishName))
            Data.CaughtFishCount[fishName] = 0;

        Data.CaughtFishCount[fishName]++;
        Data.FishDex.Add(fishName);
        Data.TotalFishCaught++;

        // 골드 획득
        AddGold(goldReward);

        // 이벤트 발행
        //EventManager.Instance.TriggerEvent(EEventType.OnFishCaught, fishName);
    }

    public int GetCaughtFishCount(string fishName)
    {
        return Data.CaughtFishCount.TryGetValue(fishName, out int count) ? count : 0;
    }

    public bool HasCaughtFish(string fishName)
    {
        return Data.FishDex.Contains(fishName);
    }

    #endregion

    #region Gold

    public void AddGold(long amount)
    {
        Data.TotalGold += amount;
        EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);
    }

    public long GetTotalGold() => Data.TotalGold;

    #endregion
}
