using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameData
{
    public int Map = 1;
    public int Stage = 1;
}


public class GameManager : Singleton<GameManager>
{
    public FloatingText floatingTextPrefab;
    public Canvas canvas; // Canvas 참조 추가
    
    GameData _gameData = new GameData();
    
    #region Stage
    public int UnlockedMap 
    {
        get { return _gameData.Map; }
        set { _gameData.Map = value; }
    }
	
    public int HighestStage
    {
        get { return _gameData.Stage; }
        set { _gameData.Stage = value; }
    }
    #endregion
    
    #region FishData
    public FishDatabase fishDatabase;
    

    #endregion
    
    #region Upgrade
    
    private double _cachedTotalClickStat = 0;
    
    
    public double ClickPower => GetTotalClickStat();
    
    // 외부에서 최종 수치 가져갈 때 사용
    public double GetTotalClickStat()
    {
        double total = 1.0; // 기본 클릭력

        // 물고기 수집 보너스
        total += GetClickPowerFromFishCollection();

        // 업그레이드 보너스
        if (UpgradeManager.Instance.IsUpgradeStatDirty)
            total += CalculateClickStatFromUpgrades();

        _cachedTotalClickStat = total;
        Debug.Log(_cachedTotalClickStat);
        return _cachedTotalClickStat;
    }
    
    public double GetClickPowerFromFishCollection()
    {
        double total = 1.0; // 기본 클릭력

        foreach (var (fishId, timesCaught) in _fishCatchCountsById)
        {
            if (timesCaught <= 0) continue;

            FishData fish = FishingManager.Instance.GetFishById(fishId);
            if (fish == null) continue;

            // 첫 잡은 보너스는 항상 1회만 적용: timesCaught >= 1이면 baseValue 적용
            total += fish.baseValue;

            // 추가로 잡은 횟수에 대한 0.1% 보너스
            if (timesCaught > 1)
            {
                total += (timesCaught - 1) * (fish.baseValue * 0.001);
            }
        }

        return total;
    }

    // 실제 계산을 담당하는 메서드
    private double CalculateClickStatFromUpgrades()
    {
        Dictionary<string, UpgradeData> upgradeCache = UpgradeManager.Instance.GetUpgradeCache(); 
        if (upgradeCache == null || upgradeCache.Count == 0)
        {
            UpgradeManager.Instance.MarkUpgradeStatClean();
            return 1.0;
        }

        double additiveSum = 0;
        double multiplicativeFactor = 1.0;

        foreach (UpgradeData data in upgradeCache.Values)
        {
            if (data.type.effectType == UpgradeEffectType.Additive)
                additiveSum += data.GetCurStatValue();
            else if (data.type.effectType == UpgradeEffectType.Multiplicative)
                multiplicativeFactor *= (1.0 + data.GetCurStatValue() / 100.0);
        }

        UpgradeManager.Instance.MarkUpgradeStatClean();
        return (1.0 + additiveSum) * multiplicativeFactor;
    }

    #endregion
    
    #region Money
    private long _money;
    
    public long Money
    {
        get { return _money; }
    }
    public void ChangeMoney(long money)
    {
        if (money < 0)
        {
            IncreaseUsedMoneyAmount(money);
        }

        _money += money;
    }
    #endregion

    #region FloatTexts

    void CreateFloatingText(long moneyToAdd)
    {
        {
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out anchoredPos
            );

            FloatingText ft = Instantiate(floatingTextPrefab, canvas.transform).GetComponent<FloatingText>();
            ft.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            ft.Setup(moneyToAdd);
        }
    }
    

    #endregion
    
    #region AutoFishing
    
    private Coroutine _autoFishingCoroutine;
    public void AutoFishingInit()
    {
        StartCoroutine(WaitAndInitClickSystem());
    }

    private IEnumerator WaitAndInitClickSystem()
    {
        while (FishingManager.Instance == null)
            yield return null;

        if (_autoFishingCoroutine != null)
            StopCoroutine(_autoFishingCoroutine);

        _autoFishingCoroutine = StartCoroutine(AutoFishingRoutine());
        Debug.Log("자동낚시 실행");
    }
    
    private IEnumerator AutoFishingRoutine()
    {
        while (true)
        {
            FishingManager.Instance.Controller.AttackCurrentFish();
            
            yield return new WaitForSeconds(1);
        }
    }
    
    #endregion

    #region FishCollection
    //_caughtFishCountsById.Keys.Select(id => fishDB[id])...
    private readonly Dictionary<int, int> _fishCatchCountsById   = new();

    public void RegisterCaughtFish(FishData fish)
    {
        if (fish == null) return;
        
        if (!_fishCatchCountsById .TryGetValue(fish.id, out int existingCount))
            _fishCatchCountsById [fish.id] = 1;
        else
            _fishCatchCountsById [fish.id] = existingCount + 1;
    }
    
    public bool HasCaughtFish(int fishId)
    {
        return _fishCatchCountsById.ContainsKey(fishId);
    }

// 특정 물고기 잡은 횟수 조회
    public int GetCaughtFishCount(int fishId)
    {
        _fishCatchCountsById.TryGetValue(fishId, out int count);
        return count;
    }

    #endregion
    
    
    public void IncreaseUsedMoneyAmount(long money)
    {
        if (money < 0)
        {
            usedMoney = money; // 음수니 빼서 양수로
        }
    }
    
    public void IncreaseCaughtFishCount()
    {
        fishCaughtCount++;
    }
    
    public void IncreaseUpgradeCount()
    {
        upgradeCount++;
    }
    
    public long usedMoney { get; private set; } // 전체 사용 돈
    public int fishCaughtCount { get; private set; } // 잡은 물고기 숫자
    public int upgradeCount { get; private set; } // 업그레이드 횟수

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _money = 0;
        
        fishCaughtCount = 0;
        upgradeCount = 0;
        usedMoney = 0;
        
        _gameData.Map = 1;
        _gameData.Stage = 1;
    }
}