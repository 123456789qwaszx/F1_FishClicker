using UnityEngine;
using System.Collections.Generic;

public class FishingSystem : Singleton<FishingSystem>
{
    [Header("물고기 리스트")]
    [SerializeField] public List<FishData> commonFishes;
    [SerializeField] public List<FishData> rareFishes;
    [SerializeField] public List<FishData> epicFishes;
    [SerializeField] public List<FishData> legendaryFishes;
    [SerializeField] public List<FishData> mythicFishes; // Mythic 등급 추가

    [Header("기본 확률(%)")]
    [Range(0, 100)] public float baseCommonPercent = 60f;
    [Range(0, 100)] public float baseRarePercent = 25f;
    [Range(0, 100)] public float baseEpicPercent = 10f;
    [Range(0, 100)] public float baseLegendaryPercent = 4f;
    [Range(0, 100)] public float baseMythicPercent = 1f; // Mythic 기본 확률

    [Header("실제 확률(%)")]
    public float commonPercent;
    public float rarePercent;
    public float epicPercent;
    public float legendaryPercent;
    public float mythicPercent; // Mythic 실제 확률

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
    public void ResetPercentages()
    {
        commonPercent = baseCommonPercent;
        rarePercent = baseRarePercent;
        epicPercent = baseEpicPercent;
        legendaryPercent = baseLegendaryPercent;
        mythicPercent = baseMythicPercent;
    }

    /// <summary>
    /// UpgradeManager와 연동하여 Rare 이상 확률 보너스 적용
    /// </summary>
    public void ApplyRareOrAboveBonus()
    {
        float extra = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);

        // Common을 줄이고, 나머지 등급에 기존 비율을 유지하면서 분배
        Dictionary<string, float> newValues = new Dictionary<string, float>
        {
            { "Common", Mathf.Max(0f, baseCommonPercent - extra) }
        };

        RedistributeProbabilities(newValues);
    }

    private void RedistributeProbabilities(Dictionary<string, float> newValues)
    {
        var baseValues = new Dictionary<string, float>
        {
            { "Common", baseCommonPercent },
            { "Rare", baseRarePercent },
            { "Epic", baseEpicPercent },
            { "Legendary", baseLegendaryPercent },
            { "Mythic", baseMythicPercent }
        };

        var adjusted = new Dictionary<string, float>(baseValues);
        foreach (var kvp in newValues)
            if (adjusted.ContainsKey(kvp.Key))
                adjusted[kvp.Key] = Mathf.Clamp(kvp.Value, 0f, 100f);

        float totalNew = 0f;
        foreach (var kvp in adjusted)
            if (newValues.ContainsKey(kvp.Key))
                totalNew += kvp.Value;

        float remaining = 100f - totalNew;

        float sumOtherBase = 0f;
        foreach (var kvp in baseValues)
            if (!newValues.ContainsKey(kvp.Key))
                sumOtherBase += kvp.Value;

        foreach (var kvp in baseValues)
        {
            if (!newValues.ContainsKey(kvp.Key))
            {
                adjusted[kvp.Key] = (kvp.Value / sumOtherBase) * remaining;
            }
        }

        commonPercent = adjusted["Common"];
        rarePercent = adjusted["Rare"];
        epicPercent = adjusted["Epic"];
        legendaryPercent = adjusted["Legendary"];
        mythicPercent = adjusted["Mythic"];
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
        if ((cumulative += mythicPercent) > r) return GetRandomFish(mythicFishes);

        return GetRandomFish(commonFishes);
    }

    private FishData GetRandomFish(List<FishData> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
