using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Upgrade
    private readonly Dictionary<UpgradeType, (long value, int level, long cost)> _upgradeInfo = new();

    public long GetUpgradeAmount(UpgradeType type)
    {
        if (_upgradeInfo.TryGetValue(type, out var v))
            return v.value;
        
        return 0;
    }
    
    public int GetUpgradeLevel(UpgradeType type)
    {
        if (_upgradeInfo.TryGetValue(type, out var v))
            return v.level;
        
        return 0;
    }
    
    public long GetUpgradeCost(UpgradeType type)
    {
        if (_upgradeInfo.TryGetValue(type, out var v))
            return v.cost;
        
        return 0;
    }

    public void SetUpgradeResult(UpgradeType type, long value, int level, long cost)
    {
        _upgradeInfo[type] = (value, level, cost);
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

    
    public void IncreaseUsedMoneyAmount(long money)
    {
        if (money < 0)
        {
            usedMoney -= money; // 음수니 빼서 양수로
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
    }
}