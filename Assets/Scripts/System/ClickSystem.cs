using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickSystem : Singleton<ClickSystem>
{
    public List<FishPool> allPools; // 게임 내 모든 맵
    public string currentMapName = "FishPool000";

    public FloatingText floatingTextPrefab;
    public Canvas canvas; // Canvas 참조 추가

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        StartAutoFishing();
    }

    void CreateGoldText(long moneyToAdd)
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

    // 클릭 시 호출
    public void OnClickFishing()
    {
        FishData caughtFish = GetRandomFishFromCurrentMap();
        if (caughtFish == null)
            return;

        InventoryManager.Instance.AddFish(caughtFish);

        long totalClickValue = (long)GameManager.Instance.GetTotalClickStat(caughtFish.baseValue);

        CreateGoldText(totalClickValue);
        GameManager.Instance.ChangeMoney(totalClickValue);
        GameManager.Instance.IncreaseCaughtFishCount();
        EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);

        Debug.Log($"{caughtFish.name}");
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

    private float baseAutoInterval = 1.0f;

    private IEnumerator AutoFishingRoutine()
    {
        while (true)
        {
            float autoIntervalBonus = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);
            float autoInterval = baseAutoInterval - autoIntervalBonus * 0.01f;

            float efficiencyBonus = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);

            FishData caughtFish = GetRandomFishFromCurrentMap();

            //InventoryManager.Instance.AddFish(caughtFish);
            //Debug.Log($"{caughtFish.name}");

            GameManager.Instance.ChangeMoney(caughtFish.baseValue);
            EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);

            yield return new WaitForSeconds(autoInterval); // 업그레이드 속도 반영
        }
    }
}