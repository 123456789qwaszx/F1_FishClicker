using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFishState
{
    public FishData Fish { get; private set; }
    public double MaxHp { get; private set; }
    public double CurrentHp { get; private set; }
    
    public void Initialize(FishData fish, double maxHp)
    {
        Fish = fish;
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }

    public void TakeDamage(double damage)
    {
        CurrentHp -= damage;
        if (CurrentHp < 0) CurrentHp = 0;
    }

    public bool IsDefeated() => CurrentHp <= 0;
}


public class CurrentFishController : MonoBehaviour
{
    public CurrentFishState CurrentFishState { get; private set; }

    
    public FishData CurFish => CurrentFishState?.Fish;
    
    public double CurFishHp => CurrentFishState.CurrentHp;

    public double CurFishMaxHp =>CurrentFishState.MaxHp;

    private void Awake()
    {
        CurrentFishState = new CurrentFishState();
    }
    
    
    #region Fetch

    private double FetchMaxHp(int difficultyLevel, double baseHp)
    {
        baseHp = 10.0;
        double hpMultiplier = 1.12;   // 단계당 12% 증가
        double linearBonus = 2.0;     // 추가 선형 보정

        double maxHp = baseHp * Math.Pow(hpMultiplier, difficultyLevel * linearBonus);
        
        return maxHp;
    }
    
    #endregion
    
    #region Fish Control
    
    public void SpawnNewFish()
    {
        FishData fish = FishingManager.Instance.GetRandomFishByRarity();
        if (fish == null) return;

        int difficultyLevel = Formula.GetDifficultyLevel_FromMapManager();
        
        double maxHp = Formula.GetCurFishHp(fish, difficultyLevel);

        CurrentFishState.Initialize(fish, maxHp);
    }

    public void AttackCurrentFish()
    {
        if (CurrentFishState == null || CurrentFishState.IsDefeated())
            return;

        CurrentFishState.TakeDamage(GameManager.Instance.ClickPower);
        EventManager.Instance.TriggerEvent(EEventType.OnAttackFish);

        if (CurrentFishState.IsDefeated())
        {
            HandleFishDefeated();
        }
    }
    
    #endregion

    #region Helpers
    
    private void HandleFishDefeated()
    {
        Debug.Log($"물고기 {CurrentFishState.Fish.fishName} 처치!");
        long reward = CurrentFishState.Fish.baseValue;
        GameManager.Instance.ChangeMoney(reward);
        
        EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);

        SpawnNewFish();

        UIManager.Instance.GetUI<UI_FishingGame>().RefreshFishUI();
    }
    
    #endregion
}