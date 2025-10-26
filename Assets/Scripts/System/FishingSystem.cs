using UnityEngine;
using System.Collections.Generic;

public class FishingSystem : Singleton<FishingSystem>
{
    [Header("물고기 리스트")]
    [SerializeField] public List<FishData> commonFishes;
    [SerializeField] public List<FishData> rareFishes;
    [SerializeField] public List<FishData> epicFishes;
    [SerializeField] public List<FishData> legendaryFishes;
    [SerializeField] public List<FishData> mythicFishes;

    [Header("기본 확률(%)")]
    [Range(0, 100)] public float baseCommonPercent = 60f;
    [Range(0, 100)] public float baseRarePercent = 25f;
    [Range(0, 100)] public float baseEpicPercent = 10f;
    [Range(0, 100)] public float baseLegendaryPercent = 4f;
    [Range(0, 100)] public float baseMythicPercent = 1f;

    [Header("실제 확률(%)")]
    public float commonPercent;
    public float rarePercent;
    public float epicPercent;
    public float legendaryPercent;
    public float mythicPercent;

    private void Awake()
    {
        FishDataCashing();
        ResetPercentages();
        ApplyRareOrAboveBonus();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.AddEvent(EEventType.OnMapChanged, ApplyMapFishData);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.RemoveEvent(EEventType.OnMapChanged, ApplyMapFishData);
    }

    
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
    
    
    #region FishingProbability
    public void ResetPercentages()
    {
        commonPercent = baseCommonPercent;
        rarePercent = baseRarePercent;
        epicPercent = baseEpicPercent;
        legendaryPercent = baseLegendaryPercent;
        mythicPercent = baseMythicPercent;
    }

    
    /// <summary>
    /// 게임매니저의 스탯과 연동하여 Rare 이상 확률 보너스 적용
    /// Common을 줄이고, 나머지 등급에 기존 비율을 유지하면서 분배
    /// </summary>
    public void ApplyRareOrAboveBonus()
    {
        float extra = GameManager.Instance.GetUpgradeAmount(UpgradeType.Aria);

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
    #endregion
    

    #region FishData
    public void ApplyMapFishData()
    {
        List<FishData> mapFishes = GetFishForMap();
        
        commonFishes.Clear();
        rareFishes.Clear();
        epicFishes.Clear();
        legendaryFishes.Clear();
        mythicFishes.Clear();
        
        foreach (FishData fish in mapFishes)
        {
            switch (fish.rarity)
            {
                case "Common":
                    commonFishes.Add(fish);
                    break;
                case "Rare":
                    rareFishes.Add(fish);
                    break;
                case "Epic":
                    epicFishes.Add(fish);
                    break;
                case "Legendary":
                    legendaryFishes.Add(fish);
                    break;
                case "Mythic":
                    mythicFishes.Add(fish);
                    break;
                default:
                    Debug.LogWarning($"Unknown fish rarity: {fish.rarity}");
                    break;
            }
        }
        
        ResetPercentages();
        ApplyRareOrAboveBonus();
    }
    
    
    public List<FishData> GetFishForMap()
    {
        List<FishData> result = new List<FishData>();

        // FishDatabase에 있는 모든 FishData를 순회하면서 지역이 같은 것만 추가
        foreach (var fish in _fishCache.Values)
        {
            if (fish.region == MapManager.Instance.GetCurrentMap().region)
            {
                result.Add(fish);
            }
        }

        return result;
    }
    
    
    private Dictionary<string, FishData> _fishCache = new Dictionary<string, FishData>();

    private void FishDataCashing()
    {
        if (GameManager.Instance.fishDatabase == null)
        {
            Debug.LogError("FishDatabase is not assigned!");
            return;
        }

        // 이름 기준으로 캐시 생성
        _fishCache.Clear();
        foreach (var fish in GameManager.Instance.fishDatabase.fishList)
        {
            if (!_fishCache.ContainsKey(fish.fishName))
                _fishCache.Add(fish.fishName, fish);
        }
    }
    #endregion
}
