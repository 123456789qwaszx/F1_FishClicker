using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Aria,
    Ciel,
    Reina,
    Noel,
    Lumia,
    Kei,
    Mio,
}

public enum UpgradeEffectType
{
    Additive,
    Multiplicative,
    AutoFishingAmountAdditive,
    AutoFishingAmountMultiplicative,
    RareFishChance,
}

[Serializable]
public class UpgradeData
{
    public UpgradeType statType;              // 업그레이드 종류
    public int level;                         // 현재 레벨
    public long baseStatValue;                // 기본 수치
    public long valueIncrease;                // 증가량
    public long baseCost;                     // 기본 비용
    public long costIncrease;                 // 비용 증가량
    public UpgradeEffectType effectType;      // 효과 타입

    public long GetCurStatValue() => baseStatValue + level * valueIncrease;

    public long GetUpgradeCost() => (long)(baseCost * Math.Pow(1.5f, level));
}

public class UpgradeSystem : Singleton<UpgradeSystem>
{
    [SerializeField] private UpgradeDatabase _upgradeDB;
    [SerializeField] private Dictionary<UpgradeType, UpgradeData> _upgradeCache = new();
    public List<UpgradeData> upgradeData = new();

    private readonly Dictionary<UpgradeType, UpgradeData> _cache = new();

    private void Awake()
    {
        //Init();
    }

    public void Init()
    {
        LoadUpgradeDatabase();
        EnsureAllTypes();
        BuildCache();
    }

    
    private void LoadUpgradeDatabase()
    {
        _upgradeDB = Resources.Load<UpgradeDatabase>("UpgradeDatabase");
        if (_upgradeDB == null)
        {
            return;
        }

        _upgradeCache.Clear();
        upgradeData.Clear();
        
        foreach (UpgradeData upgrade in _upgradeDB.upgradeList)
        {
            if (!_upgradeCache.ContainsKey(upgrade.statType))
                _upgradeCache.Add(upgrade.statType, upgrade);
            
            upgradeData.Add(upgrade);
        }
    }

    
    private void EnsureAllTypes()
    {
        if (upgradeData == null)
            upgradeData = new List<UpgradeData>();

        HashSet<UpgradeType> exist = new HashSet<UpgradeType>();
        foreach (UpgradeData ud in upgradeData)
        {
            if (ud == null) continue;
            exist.Add(ud.statType);
        }

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
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
        GameManager.Instance.ClearUpgradeInfo();
        if (upgradeData == null) return;

        foreach (UpgradeData ud in upgradeData)
        {
            if (ud == null) continue;
            _cache[ud.statType] = ud;

            GameManager.Instance.SetUpgradeResult(ud.statType, ud);
        }
    }
    

    public void TryUpgrade(UpgradeType stat)
    {
        if (!_cache.TryGetValue(stat, out UpgradeData upgradeData))
        {
            return;
        }

        long cost = upgradeData.GetUpgradeCost();
        if (cost > GameManager.Instance.Money)
        {
            return;
        }

        GameManager.Instance.ChangeMoney(-cost);
        GameManager.Instance.IncreaseUsedMoneyAmount(-cost);

        upgradeData.level++;
        GameManager.Instance.SetUpgradeResult(upgradeData.statType, upgradeData);

        EventManager.Instance.TriggerEvent(EEventType.Upgraded);
    }
}
