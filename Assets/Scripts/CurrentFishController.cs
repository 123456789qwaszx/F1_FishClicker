using System.Collections.Generic;
using UnityEngine;

public class CurrentFishState
{
    public FishData Fish { get; private set; }
    public double MaxHp { get; private set; }
    public double CurrentHp { get; private set; }
    public double ClickPower { get; private set; }

    public CurrentFishState(FishData fish, double maxHp, double clickPower)
    {
        Fish = fish;
        MaxHp = maxHp;
        CurrentHp = maxHp;
        ClickPower = clickPower;
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
    
    public void SpawnNewFish()
    {
        FishData fish = FishingManager.Instance.GetRandomFishByRarity();
        if (fish == null) return;

        double maxHp = MapManager.Instance.CurrentStageData.requiredCatchCount * 0.1;
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