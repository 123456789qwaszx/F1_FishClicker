using System.Collections.Generic;
using UnityEngine;

public class CurrentFishState
{
    public FishData Fish { get; private set; }
    public double MaxHP { get; private set; }
    public double CurrentHP { get; private set; }
    public double ClickPower { get; private set; }

    public CurrentFishState(FishData fish, double maxHP, double clickPower)
    {
        Fish = fish;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        ClickPower = clickPower;
    }

    public void TakeDamage(double damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;
    }

    public bool IsDefeated() => CurrentHP <= 0;
}


public class CurrentFishController : MonoBehaviour
{
    public CurrentFishState CurrentFishState { get; private set; }

    
    public FishData CurFish => CurrentFishState?.Fish;
    
    public double CurFishHp => CurrentFishState.CurrentHP;

    public double CurFishMaxHp =>CurrentFishState.MaxHP;
    
    public void SpawnNewFish()
    {
        FishData fish = FishingManager.Instance.GetRandomFishByRarity();
        if (fish == null) return;

        double maxHp = MapManager.Instance.CurrentStageData.requiredCatchCount * 0.01;
        double clickPower = GameManager.Instance.GetTotalClickStat();

        CurrentFishState = new CurrentFishState(fish, maxHp, clickPower);
    }

    public void AttackCurrentFish()
    {
        if (CurrentFishState == null || CurrentFishState.IsDefeated())
            return;

        CurrentFishState.TakeDamage(CurrentFishState.ClickPower);
        EventManager.Instance.TriggerEvent(EEventType.OnAttackFish);

        if (CurrentFishState.IsDefeated())
        {
            HandleFishDefeated();
        }
    }

    private void HandleFishDefeated()
    {
        Debug.Log($"물고기 {CurrentFishState.Fish.fishName} 처치!");
        long reward = CurrentFishState.Fish.baseValue;
        GameManager.Instance.ChangeMoney(reward);

        SpawnNewFish();
    }
}