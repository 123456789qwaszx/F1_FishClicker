using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeEffectType
{
    Additive,
    Multiplicative,
    RareFishChance,
    AutoFishingAmount,
}

[Serializable]
public class UpgradeData
{
    [Serializable]
    public class UpgradeType
    {
        public string id;
        public UpgradeEffectType effectType;

        public UpgradeType() { id = ""; effectType = UpgradeEffectType.Additive; }
        public UpgradeType(string id, UpgradeEffectType effectType)
        {
            this.id = id;
            this.effectType = effectType;
        }
    }

    public UpgradeType type = new ();
    public int level;
    public long baseStatValue;
    public long valueIncrease;
    public long baseCost;
    public long costIncrease;

    public long GetCurStatValue() => baseStatValue + level * valueIncrease;
    public long GetUpgradeCost() => (long)(baseCost * Math.Pow(1.5f, level));
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    private UpgradeDatabase _upgradeDB;
    private readonly List<UpgradeData> _upgradeDataList = new();
    private readonly List<UpgradeData.UpgradeType> _upgradeTypes = new();
    private readonly Dictionary<string, UpgradeData> _upgradeCache = new();
    
    public IEnumerable<UpgradeData> GetAllUpgrades() => _upgradeCache.Values;

    
    public void Init()
    {
        LoadUpgradeSheet();
        CollectUpgradeTypes();
        EnsureAllType();
        BuildUpgradeCache();
    }

    
    private void LoadUpgradeSheet()
    {
        _upgradeDB = Resources.Load<UpgradeDatabase>(StringNameSpace.ResourcePaths.UpgradeDataPath);
        if (!_upgradeDB) { Debug.LogWarning("UpgradeDatabase not found!"); }
    }

    
    public void CollectUpgradeTypes()
    {
        if (_upgradeDB == null) return;

        _upgradeTypes.Clear();
        var existingIds = new HashSet<string>();

        foreach (UpgradeData upgrade in _upgradeDB.upgradeList)
        {
            if (upgrade == null || upgrade.type == null)
                continue;

            if (existingIds.Contains(upgrade.type.id))
                continue;

            _upgradeTypes.Add(upgrade.type);
            existingIds.Add(upgrade.type.id);
        }
    }

    
    private void EnsureAllType()
    {
        HashSet<string> existIds = new HashSet<string>();
        foreach (UpgradeData ud in _upgradeDataList)
        {
            if (ud?.type != null && !string.IsNullOrEmpty(ud.type.id))
                existIds.Add(ud.type.id);
        }

        foreach (UpgradeData.UpgradeType type in _upgradeTypes)
        {
            if (string.IsNullOrEmpty(type.id) || existIds.Contains(type.id))
                continue;

            _upgradeDataList.Add(new UpgradeData
            {
                type = new UpgradeData.UpgradeType(type.id, type.effectType),
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
        foreach (UpgradeData upgrade in _upgradeDataList)
        {
            if (upgrade?.type != null && !string.IsNullOrEmpty(upgrade.type.id))
                _upgradeCache[upgrade.type.id] = upgrade;
        }
    }

    
    public void TryUpgrade(UpgradeData.UpgradeType upgradeType)
    {
        if (upgradeType == null || string.IsNullOrEmpty(upgradeType.id)) return;

        if (!_upgradeCache.TryGetValue(upgradeType.id, out UpgradeData upgradeData)) return;

        long cost = upgradeData.GetUpgradeCost();
        if (cost > GameManager.Instance.Money) return;

        GameManager.Instance.ChangeMoney(-cost);
        GameManager.Instance.IncreaseUsedMoneyAmount(cost);

        upgradeData.level++;
        GameManager.Instance.SetUpgradeResult(upgradeData);

        EventManager.Instance.TriggerEvent(EEventType.Upgraded);
    }
}
