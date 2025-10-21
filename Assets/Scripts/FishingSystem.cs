using UnityEngine;
using System.Collections.Generic;

public class FishingSystem : Singleton<FishingSystem>
{
    [Header("물고기 리스트")]
    [SerializeField] private List<FishData> commonFishes;
    [SerializeField] private List<FishData> rareFishes;
    [SerializeField] private List<FishData> epicFishes;
    [SerializeField] private List<FishData> legendaryFishes;

    [Header("기본 확률(%)")]
    [Range(0, 100)] public float baseCommonPercent = 60f;
    [Range(0, 100)] public float baseRarePercent = 25f;
    [Range(0, 100)] public float baseEpicPercent = 10f;
    [Range(0, 100)] public float baseLegendaryPercent = 5f;

    [Header("실제 확률(%)")]
    public float commonPercent;
    public float rarePercent;
    public float epicPercent;
    public float legendaryPercent;

    private void Awake()
    {
        ResetPercentages();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        ApplyRareOrAboveBonus(); // 초기 적용
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
    }

    /// <summary>
    /// 기본 확률로 초기화
    /// </summary>
    private void ResetPercentages()
    {
        commonPercent = baseCommonPercent;
        rarePercent = baseRarePercent;
        epicPercent = baseEpicPercent;
        legendaryPercent = baseLegendaryPercent;
    }

    /// <summary>
    /// UpgradeManager와 연동하여 Rare 이상 확률 보너스 적용
    /// </summary>
    private void ApplyRareOrAboveBonus()
    {
        float extra = UpgradeManager.Instance.GetStatValue(UpgradeType.RareOrAboveChanceBonus);

        // Common을 줄이고, 나머지 등급에 기존 비율을 유지하면서 분배
        Dictionary<string, float> newValues = new Dictionary<string, float>
        {
            { "Common", Mathf.Max(0f, baseCommonPercent - extra) }
        };

        RedistributeProbabilities(newValues);
    }

    
    private void RedistributeProbabilities(Dictionary<string, float> newValues)
    {
        // 기존 확률
        var baseValues = new Dictionary<string, float>
        {
            { "Common", baseCommonPercent },
            { "Rare", baseRarePercent },
            { "Epic", baseEpicPercent },
            { "Legendary", baseLegendaryPercent }
        };

        // 우선 newValues 적용
        var adjusted = new Dictionary<string, float>(baseValues);
        foreach (var kvp in newValues)
            if (adjusted.ContainsKey(kvp.Key))
                adjusted[kvp.Key] = Mathf.Clamp(kvp.Value, 0f, 100f);

        // 남은 확률 계산
        float totalNew = 0f;
        foreach (var kvp in adjusted)
            if (newValues.ContainsKey(kvp.Key))
                totalNew += kvp.Value;

        float remaining = 100f - totalNew;

        // 남은 등급 합계 (기존 baseValues 기준)
        float sumOtherBase = 0f;
        foreach (var kvp in baseValues)
            if (!newValues.ContainsKey(kvp.Key))
                sumOtherBase += kvp.Value;

        // 남은 확률 비율대로 재분배
        foreach (var kvp in baseValues)
        {
            if (!newValues.ContainsKey(kvp.Key))
            {
                adjusted[kvp.Key] = (kvp.Value / sumOtherBase) * remaining;
            }
        }

        // 최종 적용
        commonPercent = adjusted["Common"];
        rarePercent = adjusted["Rare"];
        epicPercent = adjusted["Epic"];
        legendaryPercent = adjusted["Legendary"];
    }


    /// <summary>
    /// 확률 기반 랜덤 물고기 반환
    /// </summary>
    public FishData CatchFish()
    {
        float r = Random.value * 100f;
        float cumulative = 0f;

        if ((cumulative += commonPercent) > r) return GetRandomFish(commonFishes);
        if ((cumulative += rarePercent) > r) return GetRandomFish(rareFishes);
        if ((cumulative += epicPercent) > r) return GetRandomFish(epicFishes);
        if ((cumulative += legendaryPercent) > r) return GetRandomFish(legendaryFishes);

        return GetRandomFish(commonFishes);
    }

    private FishData GetRandomFish(List<FishData> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
