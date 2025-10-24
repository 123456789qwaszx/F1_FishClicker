using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickSystem : Singleton<ClickSystem>
{
    public FloatingText floatingTextPrefab;
    public Canvas canvas; // Canvas 참조 추가
    
    private bool isInitialized = false;
    
    
    void Start()
    {
        StartCoroutine(WaitAndInitClickSystem());
    }
    
    private IEnumerator WaitAndInitClickSystem()
    {
        // FishingSystem 준비될 때까지 대기
        while (FishingSystem.Instance == null)
            yield return null;

        if (autoFishingCoroutine != null)
            StopCoroutine(autoFishingCoroutine);

        autoFishingCoroutine = StartCoroutine(AutoFishingRoutine());
        Debug.Log("자동낚시 실행");
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
        FishData caughtFish = FishingSystem.Instance.CatchFish();
        if (caughtFish == null)
            return;

        InventoryManager.Instance.AddFish(caughtFish);

        long totalClickValue = (long)GameManager.Instance.GetTotalClickStat(caughtFish.baseValue);

        CreateGoldText(totalClickValue);
        GameManager.Instance.ChangeMoney(totalClickValue);
        GameManager.Instance.IncreaseCaughtFishCount();
        EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);

        Debug.Log($"{caughtFish.fishName}");
    }


    private Coroutine autoFishingCoroutine;

    private float baseAutoInterval = 1.0f;

    private IEnumerator AutoFishingRoutine()
    {
        while (true)
        {
            // 안전하게 null 체크
            if (FishingSystem.Instance == null || GameManager.Instance == null || InventoryManager.Instance == null)
            {
                yield return null;
                continue;
            }
            Debug.Log("자동 낚시 실행");

            FishData caughtFish = null;
            try
            {
                caughtFish = FishingSystem.Instance.CatchFish();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("CatchFish error: " + ex);
            }

            if (caughtFish != null)
            {
                try
                {
                    InventoryManager.Instance.AddFish(caughtFish);
                    long value = (long)GameManager.Instance.GetTotalClickStat(caughtFish.baseValue);
                    GameManager.Instance.ChangeMoney(value);
                    EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);

                    Debug.Log($"자동낚시: {caughtFish.fishName} +{value}G");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("AutoFishing update error: " + ex);
                }
            }

            // 자동낚시 간격 계산
            float autoIntervalBonus = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);
            float autoInterval = Mathf.Max(0.1f, baseAutoInterval - autoIntervalBonus * 0.01f);

            yield return new WaitForSeconds(autoInterval);
        }
    }
    // private IEnumerator AutoFishingRoutine()
    // {
    //     while (true)
    //     {
    //         float autoIntervalBonus = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);
    //         float autoInterval = baseAutoInterval - autoIntervalBonus * 0.01f;
    //
    //         float efficiencyBonus = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);
    //
    //         FishData caughtFish = FishingSystem.Instance.CatchFish();
    //
    //         //InventoryManager.Instance.AddFish(caughtFish);
    //         //Debug.Log($"{caughtFish.name}");
    //
    //         GameManager.Instance.ChangeMoney(caughtFish.baseValue);
    //         EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);
    //
    //         yield return new WaitForSeconds(autoInterval); // 업그레이드 속도 반영
    //     }
    // }
}