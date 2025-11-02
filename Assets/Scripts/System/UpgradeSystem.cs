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
    private UpgradeDatabase _upgradeDB;
    //private Dictionary<UpgradeType, UpgradeData> _upgradeCache = new();
    public List<UpgradeData> upgradeDatas = new();

    private readonly Dictionary<UpgradeType, UpgradeData> _upgradeCache = new();

    private void Awake()
    {
        //Init();
    }

    public void Init()
    {
        LoadUpgradeSheet();
        AddMissingUpgradeTypes();
        BuildUpgradeCache();
        SyncUpgradesToGameManager();
    }

    
    private void LoadUpgradeSheet()
    {
        _upgradeDB = Resources.Load<UpgradeDatabase>(StringNameSpace.ResourcePaths.UpgradeDataPath);
        if (_upgradeDB == null) return;

        // _upgradeCache.Clear();
        // upgradeData.Clear();
        //
        // foreach (UpgradeData upgrade in _upgradeDB.upgradeList)
        // {
        //     if (!_upgradeCache.ContainsKey(upgrade.statType))
        //         _upgradeCache.Add(upgrade.statType, upgrade);
        //     
        //     upgradeData.Add(upgrade);
        // }
    }
    
    private void AddMissingUpgradeTypes()
    {
        if (upgradeDatas == null) upgradeDatas = new List<UpgradeData>();

        HashSet<UpgradeType> exist = new HashSet<UpgradeType>();
        foreach (UpgradeData ud in upgradeDatas)
        {
            if (ud == null) continue;
            exist.Add(ud.statType);
        }

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            if (exist.Contains(type)) continue;

            upgradeDatas.Add(new UpgradeData
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
    
    private void BuildUpgradeCache()
    {
        _upgradeCache.Clear();
        if (upgradeDatas == null) return;

        foreach (UpgradeData upgrade in upgradeDatas)
        {
            if (upgrade == null) continue;
            _upgradeCache[upgrade.statType] = upgrade;
        }
    }
    
    private void SyncUpgradesToGameManager()
    {
        GameManager.Instance.ClearUpgradeInfo();
        
        foreach (UpgradeData upgrade in _upgradeCache.Values)
        {
            GameManager.Instance.SetUpgradeResult(upgrade);
        }
    }
    

    public void TryUpgrade(UpgradeType upgradeType)
    {
        if (!_upgradeCache.TryGetValue(upgradeType, out UpgradeData upgradeData))
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
        GameManager.Instance.SetUpgradeResult(upgradeData);

        EventManager.Instance.TriggerEvent(EEventType.Upgraded);
    }
}
