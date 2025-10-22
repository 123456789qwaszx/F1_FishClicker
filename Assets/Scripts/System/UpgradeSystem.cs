using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    None,
    CurrencyGain,
    RareOrAboveChanceBonus,
    FeverGaugeFillRateUp,
    LotteryWinRate,
    LotteryDiscountRate
}

public class UpgradeData
{
    public UpgradeType statType;
    public int level;

    public long baseStatValue;
    public long valueIncrease;

    public long baseCost;
    public long costIncrease;

    public long GetCurStatValue() => baseStatValue + level * valueIncrease;
    public long GetUpgradeCost()
    {
        return (long)(baseCost * Math.Pow(1.5, level));
    }
}


public class UpgradeSystem : Singleton<UpgradeSystem>
{
    public List<UpgradeData> upgradeData;

    private readonly Dictionary<UpgradeType, UpgradeData> _cache = new Dictionary<UpgradeType, UpgradeData>();

    void Awake()
    {
        Init();
    }

    void Init()
    {
        EnsureAllTypes();
        BuildCache();
    }


    void EnsureAllTypes()
    {
        if (upgradeData == null) upgradeData = new List<UpgradeData>();

        HashSet<UpgradeType> exist = new HashSet<UpgradeType>();
        foreach (UpgradeData ud in upgradeData)
        {
            if (ud == null) continue;
            exist.Add(ud.statType);
        }

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            if (type == UpgradeType.None) continue;
            if (exist.Contains(type)) continue;

            upgradeData.Add(new UpgradeData
            {
                statType = type,
                level = 0,
                baseStatValue = 0,
                valueIncrease = 2,
                baseCost = 10,
                costIncrease = 30
            });
        }
    }

    private void BuildCache()
    {
        _cache.Clear();
        if (upgradeData == null) return;
        foreach (UpgradeData ud in upgradeData)
        {
            if (ud == null) continue;
            _cache[ud.statType] = ud;
            
            GameManager.Instance.SetUpgradeResult(ud.statType, ud.GetCurStatValue(), ud.level, ud.GetUpgradeCost());
        }
    }


    public int GetLevel(UpgradeType stat)
    {
        return _cache.TryGetValue(stat, out var u) ? u.level : 0;
    }
    

    public float GetStatValue(UpgradeType stat)
    {
        Debug.Log($"{_cache[stat].GetCurStatValue()}");
        return _cache.TryGetValue(stat, out UpgradeData ug) ? ug.GetCurStatValue() : -1;
    }


    public long GetUpgradeCost(UpgradeType stat)
    {
        return _cache.TryGetValue(stat, out UpgradeData ug) ? ug.GetUpgradeCost() : -1;
    }


    public void TryUpgrade(UpgradeType stat)
    {
        if (GetUpgradeCost(stat) > GameManager.Instance.Money)
        {
            Debug.Log("돈이 모자랍니다");
            return;
        }

        UpgradeData upgradeData = _cache[stat];
        
        GameManager.Instance.ChangeMoney(-upgradeData.GetUpgradeCost());
        GameManager.Instance.IncreaseUsedMoneyAmount(-upgradeData.GetUpgradeCost());

        upgradeData.level++;
        GameManager.Instance.SetUpgradeResult(stat, upgradeData.GetCurStatValue(), upgradeData.level, upgradeData.GetUpgradeCost());

        EventManager.Instance.TriggerEvent(EEventType.Upgraded);
    }
}