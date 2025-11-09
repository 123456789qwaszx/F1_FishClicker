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


public class FishRarityProbability
{
    private readonly Dictionary<FishRarity, float> _basePercentages;
    private readonly Dictionary<FishRarity, float> _currentPercentages;

    public FishRarityProbability(float common, float rare, float epic, float legendary, float mythic)
    {
        _basePercentages = new Dictionary<FishRarity, float>
        {
            { FishRarity.Common, common },
            { FishRarity.Rare, rare },
            { FishRarity.Epic, epic },
            { FishRarity.Legendary, legendary },
            { FishRarity.Mythic, mythic }
        };

        _currentPercentages = new Dictionary<FishRarity, float>(_basePercentages);
    }

    public void ApplyRareBonus(float extra)
    {
        _currentPercentages[FishRarity.Common] = Mathf.Max(0, _basePercentages[FishRarity.Common] - extra);
        Redistribute();
    }

    private void Redistribute()
    {
        float remaining = 100f - _currentPercentages[FishRarity.Common];
        float sumOtherBase = _basePercentages[FishRarity.Rare] +
                             _basePercentages[FishRarity.Epic] +
                             _basePercentages[FishRarity.Legendary] +
                             _basePercentages[FishRarity.Mythic];

        foreach (FishRarity rarity in new[] { FishRarity.Rare, FishRarity.Epic, FishRarity.Legendary, FishRarity.Mythic })
        {
            _currentPercentages[rarity] = _basePercentages[rarity] / sumOtherBase * remaining;
        }
    }

    public FishRarity RollRarity()
    {
        float rand = Random.value * 100f;
        float cumulative = 0f;

        foreach (var kvp in _currentPercentages)
        {
            cumulative += kvp.Value;
            if (rand <= cumulative) return kvp.Key;
        }

        return FishRarity.Common;
    }
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
    public bool isBoss;
}


public class FishingManager : Singleton<FishingManager>
{
    private FishDatabase _fishDB;
    private readonly Dictionary<string, FishData> _fishCache = new();

    private readonly List<FishData> _commonFishes = new();
    private readonly List<FishData> _rareFishes = new();
    private readonly List<FishData> _epicFishes = new();
    private readonly List<FishData> _legendaryFishes = new();
    private readonly List<FishData> _mythicFishes = new();
    
    private readonly List<FishData> _bossFishes = new();
    
    [SerializeField] [Range(0, 100)] private float baseCommonPercent = 80f;
    [SerializeField] [Range(0, 100)] private float baseRarePercent = 15f;
    [SerializeField] [Range(0, 100)] private float baseEpicPercent = 4f;
    [SerializeField] [Range(0, 100)] private float baseLegendaryPercent = 0.7f;
    [SerializeField] [Range(0, 100)] private float baseMythicPercent = 0.3f;

    private FishRarityProbability _rarityProb;

    public CurrentFishController Controller { get; private set; }

    private void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.AddEvent(EEventType.OnMapChanged, RefreshFishListsFromCurrentMap);
        EventManager.Instance.AddEvent(EEventType.OnStageChanged, RefreshFishListsFromCurrentMap);
        EventManager.Instance.AddEvent(EEventType.OnMapChanged, Controller.SpawnNewFish);
        EventManager.Instance.AddEvent(EEventType.OnStageChanged, Controller.SpawnNewFish);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.Upgraded, ApplyRareOrAboveBonus);
        EventManager.Instance.RemoveEvent(EEventType.OnMapChanged, RefreshFishListsFromCurrentMap);
        EventManager.Instance.RemoveEvent(EEventType.OnStageChanged, RefreshFishListsFromCurrentMap);
        EventManager.Instance.RemoveEvent(EEventType.OnMapChanged, Controller.SpawnNewFish);
        EventManager.Instance.RemoveEvent(EEventType.OnStageChanged, Controller.SpawnNewFish);
    }

    #region Init

    public void Init()
    {
        LoadFishSheet();
        BuildFishCache();
        InitRarityProbability();
        ApplyRareOrAboveBonus();
        RefreshFishListsFromCurrentMap();
        EnsureControllerAssigned();
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
    
    private void InitRarityProbability()
    {
        if (_rarityProb == null)
            _rarityProb = new FishRarityProbability(baseCommonPercent, baseRarePercent, baseEpicPercent, baseLegendaryPercent, baseMythicPercent);
    }

    public void ApplyRareOrAboveBonus()
    {
        float extra = UpgradeManager.Instance.GetUpgradeAmount("Aria");
        _rarityProb.ApplyRareBonus(extra);
    }

    private void RefreshFishListsFromCurrentMap()
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
            
            if(fish.isBoss)
                _bossFishes.Add(fish);

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
    
    public void EnsureControllerAssigned()
    {
        if (Controller != null) 
            return;
        
        Controller = FindFirstObjectByType<CurrentFishController>();

        if (Controller == null)
            Debug.LogWarning("CurrentFishHandler가 씬에 없습니다!");
    }

    #endregion

    #region Getter

    public List<FishData> GetAvailableFishes()
    {
        var allFishes = new List<FishData>(
            _commonFishes.Count +
            _rareFishes.Count +
            _epicFishes.Count +
            _legendaryFishes.Count +
            _mythicFishes.Count
        );

        foreach (List<FishData> list in new[] { _commonFishes, _rareFishes, _epicFishes, _legendaryFishes, _mythicFishes })
            allFishes.AddRange(list);

        return allFishes;
    }

    public FishData GetRandomFishByRarity()
    {
        FishRarity rarity = _rarityProb.RollRarity();

        return rarity switch
        {
            FishRarity.Common => GetRandomFish(_commonFishes),
            FishRarity.Rare => GetRandomFish(_rareFishes),
            FishRarity.Epic => GetRandomFish(_epicFishes),
            FishRarity.Legendary => GetRandomFish(_legendaryFishes),
            FishRarity.Mythic => GetRandomFish(_mythicFishes),
            _ => GetRandomFish(_commonFishes)
        };
    }

    private FishData GetRandomFish(List<FishData> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
    
    
    public FishData GetRandomBossFish()
    {
        _bossFishes.Shuffle();
        
        return _bossFishes[0];
    }

    #endregion
}
