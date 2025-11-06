using System;
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
    //private readonly Dictionary<UpgradeType, UpgradeData> _upgradeInfo = new();
    private readonly Dictionary<string, UpgradeData> _upgradeInfo = new();
    
    
    public double GetTotalClickStat()
    {
        double result = 1;

        foreach (UpgradeData data in _upgradeInfo.Values)
        {
            if (data.type.effectType == UpgradeEffectType.Additive)
                result += data.GetCurStatValue();
        }

        foreach (UpgradeData data in _upgradeInfo.Values)
        {
            if (data.type.effectType == UpgradeEffectType.Multiplicative)
                result *= (1.0 + data.GetCurStatValue() / 100.0); // 퍼센트 적용 가정
        }

        return result;
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