using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : Singleton<ClickManager>
{
    public List<FishPool> allPools;       // 게임 내 모든 맵
    public string currentMapName = "FishPool000";

    void Awake()
    {
        //Init();
    }

    public void Init()
    {
        StartAutoFishing();
    }
    
    // 클릭 시 호출
    public void OnClickFishing()
    {
        FishData caughtFish = GetRandomFishFromCurrentMap();
        if (caughtFish != null)
        {
            InventoryManager.Instance.AddFish(caughtFish);
            // 결과 연출
            //UIManager.Instance.ShowCatchPopup(caughtFish);
            
            Debug.Log($"{caughtFish.name}");
        }
    }

    // 현재 맵에서 랜덤 물고기 선택
    private FishData GetRandomFishFromCurrentMap()
    {
        //FishPool pool = allPools.Find(pool => pool.mapName == currentMapName);
        FishPool pool = allPools[0];
        if (pool == null || pool.fishList.Count == 0)
            return null;

        float rand = Random.value; // 0~1 랜덤
        float cumulative = 0f;

        foreach (FishData fish in pool.fishList)
        {
            cumulative += fish.catchProbability;
            if (rand <= cumulative)
            {
                return fish;
            }
        }

        // 확률합이 1 미만일 경우 마지막 물고기 반환
        return pool.fishList[pool.fishList.Count - 1];
    }
    
    
    private Coroutine autoFishingCoroutine;

    public void StartAutoFishing()
    {
        if (autoFishingCoroutine != null)
            StopCoroutine(autoFishingCoroutine);
        
        autoFishingCoroutine = StartCoroutine(AutoFishingRoutine());
    }

    private IEnumerator AutoFishingRoutine()
    {
        while (true)
        {
            float autoInterval = UpgradeManager.Instance.GetStatValue(UpgradeType.CurrencyGain);
            float efficiencyBonus = UpgradeManager.Instance.GetStatValue(UpgradeType.RareOrAboveChanceBonus);
            
            FishData caughtFish = GetRandomFishFromCurrentMap();
            
            InventoryManager.Instance.AddFish(caughtFish);
            Debug.Log($"{caughtFish.name}");

            yield return new WaitForSeconds(autoInterval); // 업그레이드 속도 반영
        }
    }
}
