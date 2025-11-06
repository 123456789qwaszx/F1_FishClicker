using UnityEngine;
using System.Collections.Generic;

public enum FishRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}


[System.Serializable]
public class FishData
{
    public int id;
    public string fishName;
    public FishRarity rarity;
    public long baseValue;
    public string spritePath;
    public string description;
    public string region;
}


public class FishingSystem : Singleton<FishingSystem>
{
    private FishDatabase _fishDB;
    private readonly Dictionary<string, FishData> _fishCache = new();

    private readonly List<FishData> _commonFishes = new();
    private readonly List<FishData> _rareFishes = new();
    private readonly List<FishData> _epicFishes = new();
    private readonly List<FishData> _legendaryFishes = new();
    private readonly List<FishData> _mythicFishes = new();

    [SerializeField] [Range(0, 100)] private float baseCommonPercent = 60f;
    [SerializeField] [Range(0, 100)] private float baseRarePercent = 25f;
    [SerializeField] [Range(0, 100)] private float baseEpicPercent = 10f;
    [SerializeField] [Range(0, 100)] private float baseLegendaryPercent = 4f;
    [SerializeField] [Range(0, 100)] private float baseMythicPercent = 1f;

    private float _commonPercent;
    private float _rarePercent;
    private float _epicPercent;
    private float _legendaryPercent;
    private float _mythicPercent;


    private void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.AddEvent(EEventType.OnMapChanged, UpdateFishListsForCurrentMap);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.RemoveEvent(EEventType.OnMapChanged, UpdateFishListsForCurrentMap);
    }
    
    
    public void Init()
    {
        LoadFishSheet();
        BuildFishCache();
        ApplyRareOrAboveBonus();
        UpdateFishListsForCurrentMap();
    }

    
    private void LoadFishSheet()
    {
        _fishDB = Resources.Load<FishDatabase>(StringNameSpace.ResourcePaths.FishDataPath);
        if (!_fishDB) { Debug.LogWarning("FishDatabase not found!"); }
    }

    
    private void BuildFishCache()
    {
        _fishCache.Clear();
        foreach (FishData fish in _fishDB.fishList)
        {
            if (!_fishCache.ContainsKey(fish.fishName))
                _fishCache.Add(fish.fishName, fish);
        }
    }
    
    
    /// <summary>
    /// 게임매니저의 스탯과 연동하여 Rare 이상 확률 보너스 적용
    /// Common을 줄이고, 나머지 등급에 기존 비율을 유지하면서 분배
    /// </summary>
    public void ApplyRareOrAboveBonus()
    {
        _commonPercent = baseCommonPercent;
        _rarePercent = baseRarePercent;
        _epicPercent = baseEpicPercent;
        _legendaryPercent = baseLegendaryPercent;
        _mythicPercent = baseMythicPercent;

        float extra = UpgradeManager.Instance.GetUpgradeAmount("Aria");

        // Enum 기반으로 수정
        Dictionary<FishRarity, float> newValues = new Dictionary<FishRarity, float>
        {
            { FishRarity.Common, Mathf.Max(0f, baseCommonPercent - extra) }
        };

        RedistributeProbabilities(newValues);
    }

    private void RedistributeProbabilities(Dictionary<FishRarity, float> newValues)
    {
        // Enum 기반
        var baseValues = new Dictionary<FishRarity, float>
        {
            { FishRarity.Common, baseCommonPercent },
            { FishRarity.Rare, baseRarePercent },
            { FishRarity.Epic, baseEpicPercent },
            { FishRarity.Legendary, baseLegendaryPercent },
            { FishRarity.Mythic, baseMythicPercent }
        };

        var adjusted = new Dictionary<FishRarity, float>(baseValues);

        foreach (var kvp in newValues)
        {
            if (adjusted.ContainsKey(kvp.Key))
                adjusted[kvp.Key] = Mathf.Clamp(kvp.Value, 0f, 100f);
        }

        float totalNew = 0f;
        foreach (var kvp in adjusted)
        {
            if (newValues.ContainsKey(kvp.Key))
                totalNew += kvp.Value;
        }

        float remaining = 100f - totalNew;

        float sumOtherBase = 0f;
        foreach (var kvp in baseValues)
        {
            if (!newValues.ContainsKey(kvp.Key))
                sumOtherBase += kvp.Value;
        }

        foreach (var kvp in baseValues)
        {
            if (!newValues.ContainsKey(kvp.Key))
                adjusted[kvp.Key] = (kvp.Value / sumOtherBase) * remaining;
        }

        _commonPercent = adjusted[FishRarity.Common];
        _rarePercent = adjusted[FishRarity.Rare];
        _epicPercent = adjusted[FishRarity.Epic];
        _legendaryPercent = adjusted[FishRarity.Legendary];
        _mythicPercent = adjusted[FishRarity.Mythic];
    }

    
    private void UpdateFishListsForCurrentMap()
    {
        _commonFishes.Clear();
        _rareFishes.Clear();
        _epicFishes.Clear();
        _legendaryFishes.Clear();
        _mythicFishes.Clear();
    
        string currentRegion = MapManager.Instance.CurrentMapData.region;
    
        foreach (FishData fish in _fishCache.Values)
        {
            if (fish.region != currentRegion)
                continue;
        
            switch (fish.rarity)
            {
                case FishRarity.Common:
                    _commonFishes.Add(fish);
                    break;
                case FishRarity.Rare:
                    _rareFishes.Add(fish);
                    break;
                case FishRarity.Epic:
                    _epicFishes.Add(fish);
                    break;
                case FishRarity.Legendary:
                    _legendaryFishes.Add(fish);
                    break;
                case FishRarity.Mythic:
                    _mythicFishes.Add(fish);
                    break;
                default:
                    Debug.LogWarning($"Unknown fish rarity: {fish.rarity}");
                    break;
            }
        }
    }
    
    
    public List<FishData> GetCurrentMapFishList()
    {
        List<FishData> fishes = new List<FishData>(
            _commonFishes.Count +
            _rareFishes.Count +
            _epicFishes.Count +
            _legendaryFishes.Count +
            _mythicFishes.Count
        );

        fishes.AddRange(_commonFishes);
        fishes.AddRange(_rareFishes);
        fishes.AddRange(_epicFishes);
        fishes.AddRange(_legendaryFishes);
        fishes.AddRange(_mythicFishes);

        return fishes;
    }
    
    
    public FishData CatchFish()
    {
        float randomValue = Random.value * 100f;
        float cumulative = 0f;

        if ((cumulative += _commonPercent) > randomValue) return GetRandomFish(_commonFishes);
        if ((cumulative += _rarePercent) > randomValue) return GetRandomFish(_rareFishes);
        if ((cumulative += _epicPercent) > randomValue) return GetRandomFish(_epicFishes);
        if ((cumulative += _legendaryPercent) > randomValue) return GetRandomFish(_legendaryFishes);
        if ((cumulative += _mythicPercent) > randomValue) return GetRandomFish(_mythicFishes);

        return GetRandomFish(_commonFishes);
    }
    
    private FishData GetRandomFish(List<FishData> list)
    {
        if (list == null || list.Count == 0) return null;
        
        return list[Random.Range(0, list.Count)];
    }
}
