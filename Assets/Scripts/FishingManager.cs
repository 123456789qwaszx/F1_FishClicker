using System.Collections.Generic;
using UnityEngine;

public class FishingManager : Singleton<FishingManager>
{
    private FishData _currentFish;
    public FishData CurrentFish => _currentFish;
    private double _maxHP;
    public double MaxHP => _maxHP;
    
    private double _currentHp;
    public double CurrentHp => _currentHp;

    public double _clickPower;
    public double ClickPower => _clickPower;

    public void SpawnNewFish()
    {
        _currentFish = FishingSystem.Instance.CatchFish();
        _maxHP = MapManager.Instance.CurrentStage().requiredCatchCount * 0.01;
        _currentHp = MaxHP;
        _clickPower = GameManager.Instance.GetTotalClickStat();
    }


    public void OnClickFish()
    {
        if (_currentFish == null || _currentHp <= 0) 
            return;
        Debug.Log(_maxHP);
        Debug.Log(_currentHp);
        Debug.Log(_clickPower);
        

        _currentHp -= GameManager.Instance.GetTotalClickStat();
        EventManager.Instance.TriggerEvent(EEventType.OnAttackFish);

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnFishCaught();
        }
    }

    void OnFishCaught()
    {
        Debug.Log($"물고기 {_currentFish.fishName} 처치!");
        
        long reward = _currentFish.baseValue;
        GameManager.Instance.ChangeMoney(reward);
        // EventManager.Instance.TriggerEvent(EEventType.OnFishDefeated);

        // 다음 물고기 소환
        SpawnNewFish();
    }
}