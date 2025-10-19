using UnityEngine;
using System.Collections.Generic;


public class FishingSystem : MonoBehaviour
{
    [Header("물고기 리스트")]
    [SerializeField] private List<FishData> commonFishes;
    [SerializeField] private List<FishData> rareFishes;
    [SerializeField] private List<FishData> epicFishes;
    [SerializeField] private List<FishData> legendaryFishes;

    [Header("잡기 확률(%)")]
    [Range(0, 100)] public float commonPercent = 60f;
    [Range(0, 100)] public float rarePercent = 25f;
    [Range(0, 100)] public float epicPercent = 10f;
    [Range(0, 100)] public float legendaryPercent = 5f;

    private float baseCommonPercent, baseRarePercent, baseEpicPercent, baseLegendaryPercent;

    private void Awake()
    {
        baseCommonPercent = commonPercent;
        baseRarePercent = rarePercent;
        baseEpicPercent = epicPercent;
        baseLegendaryPercent = legendaryPercent;
    }

    private void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.Upgraded, ApplyExtraChance);
        ApplyExtraChance();
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.Upgraded, ApplyExtraChance);
    }

    void ApplyExtraChance()
    {
        float extra = UpgradeManager.Instance.GetStatValue(UpgradeType.ExtraChanceRate);

        // 예: Rare 확률을 조금 올리고 Common 감소
        rarePercent = baseRarePercent + extra;
        commonPercent = Mathf.Max(0f, baseCommonPercent - extra);

        // 합계 100% 보정
        float total = commonPercent + rarePercent + epicPercent + legendaryPercent;
        if (total != 100f)
        {
            float diff = 100f - total;
            commonPercent += diff; // 남은 퍼센트는 Common에 보정
        }
    }

    /// <summary>
    /// 물고기를 랜덤으로 잡음
    /// </summary>
    public FishData CatchFish()
    {
        float r = Random.value * 100f;
        float cumulative = 0f;

        if ((cumulative += commonPercent) > r) return GetRandomFishFromList(commonFishes);
        if ((cumulative += rarePercent) > r) return GetRandomFishFromList(rareFishes);
        if ((cumulative += epicPercent) > r) return GetRandomFishFromList(epicFishes);
        if ((cumulative += legendaryPercent) > r) return GetRandomFishFromList(legendaryFishes);

        // 실패 fallback
        return GetRandomFishFromList(commonFishes);
    }

    private FishData GetRandomFishFromList(List<FishData> list)
    {
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
