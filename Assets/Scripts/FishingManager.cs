using System.Collections.Generic;
using UnityEngine;

public class FishManager : Singleton<FishManager>
{
    private FishData _currentFish;
    private double _currentHp;

    public void SpawnNewFish()
    {
        _currentFish = FishingSystem.Instance.CatchFish();
        _currentHp = MapManager.Instance.CurrentStage().requiredCatchCount;
    }


    public void OnClickFish()
    {
        if (_currentFish == null || _currentHp <= 0) 
            return;

        _currentHp -= GameManager.Instance.GetTotalClickStat();
        //EventManager.Instance.TriggerEvent(EEventType.OnAttackFish);

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnFishDefeated();
        }
    }

    void OnFishDefeated()
    {
        Debug.Log($"물고기 {_currentFish.fishName} 처치!");
        // GameManager.Instance.AddGold(_currentFish.reward);
        // EventManager.Instance.TriggerEvent(EEventType.OnFishDefeated);

        // 다음 물고기 소환
        SpawnNewFish();
    }
}