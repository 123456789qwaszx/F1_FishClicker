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

    public UpgradeType Type = new UpgradeType(); // 항상 null-safe
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
    private readonly List<UpgradeData> _upgradeData = new();
    private readonly List<UpgradeData.UpgradeType> _upgradeType = new();
    private readonly Dictionary<string, UpgradeData> _upgradeCache = new();
    public IEnumerable<UpgradeData> GetAllUpgrades() => _upgradeCache.Values;

    public void Init()
    {
        LoadUpgradeSheet();
        CollectUpgradeTypes();
        AddMissingUpgradeTypes();
        BuildUpgradeCache();
        Debug.Log($"Collected {_upgradeType.Count} upgrade types.");
    }

    private void LoadUpgradeSheet()
    {
        _upgradeDB = Resources.Load<UpgradeDatabase>(StringNameSpace.ResourcePaths.UpgradeDataPath);
        if (_upgradeDB == null)
            Debug.LogWarning("UpgradeDatabase is null or empty!");
    }

    public void CollectUpgradeTypes()
    {
        if (_upgradeDB == null) return;

        _upgradeType.Clear();
        var existingIds = new HashSet<string>();

        foreach (var upgradeData in _upgradeDB.upgradeList)
        {
            if (upgradeData == null || upgradeData.Type == null)
                continue;

            if (existingIds.Contains(upgradeData.Type.id))
                continue;

            _upgradeType.Add(upgradeData.Type);
            existingIds.Add(upgradeData.Type.id);
        }
    }

    private void AddMissingUpgradeTypes()
    {
        var existIds = new HashSet<string>();
        foreach (var ud in _upgradeData)
        {
            if (ud?.Type != null && !string.IsNullOrEmpty(ud.Type.id))
                existIds.Add(ud.Type.id);
        }

        foreach (var type in _upgradeType)
        {
            if (string.IsNullOrEmpty(type.id) || existIds.Contains(type.id))
                continue;

            _upgradeData.Add(new UpgradeData
            {
                Type = new UpgradeData.UpgradeType(type.id, type.effectType),
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
        foreach (var upgrade in _upgradeData)
        {
            if (upgrade?.Type != null && !string.IsNullOrEmpty(upgrade.Type.id))
                _upgradeCache[upgrade.Type.id] = upgrade;
        }
    }

    public void TryUpgrade(UpgradeData.UpgradeType upgradeType)
    {
        if (upgradeType == null || string.IsNullOrEmpty(upgradeType.id)) return;

        if (!_upgradeCache.TryGetValue(upgradeType.id, out var upgradeData)) return;

        long cost = upgradeData.GetUpgradeCost();
        if (cost > GameManager.Instance.Money) return;

        GameManager.Instance.ChangeMoney(-cost);
        GameManager.Instance.IncreaseUsedMoneyAmount(cost);

        upgradeData.level++;
        GameManager.Instance.SetUpgradeResult(upgradeData);

        EventManager.Instance.TriggerEvent(EEventType.Upgraded);
    }
}
