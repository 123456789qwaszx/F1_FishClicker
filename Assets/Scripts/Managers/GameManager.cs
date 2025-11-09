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
    
    private double _cachedTotalClickStat = 1;
    
    
    public double ClickPower => GetTotalClickStat();
    
    // 외부에서 최종 수치 가져갈 때 사용
    public double GetTotalClickStat()
    {
        if (UpgradeManager.Instance.IsUpgradeStatDirty)
            RecalculateTotalClickStat();

        return _cachedTotalClickStat;
    }

    // 실제 계산을 담당하는 메서드
    private void RecalculateTotalClickStat()
    {
        Dictionary<string, UpgradeData> upgradeCache = UpgradeManager.Instance.GetUpgradeCache(); 
        if (upgradeCache == null || upgradeCache.Count == 0)
        {
            _cachedTotalClickStat = 1;
            UpgradeManager.Instance.MarkUpgradeStatClean();
            return;
        }
        
        double result = 1;

        foreach (UpgradeData data in upgradeCache.Values)
        {
            if (data.type.effectType == UpgradeEffectType.Additive)
                result += data.GetCurStatValue();
            
            if (data.type.effectType == UpgradeEffectType.Multiplicative)
                result *= (1.0 + data.GetCurStatValue() / 100.0);
        }

        _cachedTotalClickStat = result;
        UpgradeManager.Instance.MarkUpgradeStatClean();
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