using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    [Header("UI")]
    [SerializeField] private Image mapBackgroundImage; // 배경 이미지 UI

    [SerializeField]private MapDatabase _mapDB;            // CSV → ScriptableObject로 임포트된 맵 데이터
    [SerializeField] private MapData _currentMap;           // 현재 맵
    [SerializeField]private Dictionary<string, MapData> _mapCache = new ();

    void Awake()
    {
        Init();
    }


    public void Init()
    {
        LoadMapDatabase();
        FishDataCashing();
        SetCurrentMap("DeepSea");
    }

    
    private void LoadMapDatabase()
    {
        _mapDB = Resources.Load<MapDatabase>("MapDatabase");
        if (_mapDB == null)
        {
            Debug.LogError("MapDatabase asset not found in Resources!");
            return;
        }

        _mapCache.Clear();
        foreach (var map in _mapDB.mapList)
        {
            if (!_mapCache.ContainsKey(map.region))
                _mapCache.Add(map.region, map);
        }

        Debug.Log($"Loaded {_mapDB.mapList.Count} map entries!");
    }

    /// <summary>
    /// region 이름으로 현재 맵 설정 및 UI에 배경 표시
    /// </summary>
    public void SetCurrentMap(string region)
    {
        if (!_mapCache.TryGetValue(region, out MapData map))
        {
            Debug.LogWarning($"Map not found for region: {region}");
            return;
        }

        _currentMap = map;
        UpdateMapUI();
        Debug.Log($"Current map set to: {_currentMap.mapName}");
        
        
        List<FishData> mapFishes = GetFishForMap();
        
        FishingSystem fishingSystem = FishingSystem.Instance;
        if (fishingSystem == null)
        {
            Debug.LogError("FishingSystem instance not found!");
            return;
        }
        
        fishingSystem.commonFishes.Clear();
        fishingSystem.rareFishes.Clear();
        fishingSystem.epicFishes.Clear();
        fishingSystem.legendaryFishes.Clear();
        fishingSystem.mythicFishes.Clear();
        
        foreach (var fish in mapFishes)
        {
            switch (fish.rarity)
            {
                case "Common":
                    fishingSystem.commonFishes.Add(fish);
                    break;
                case "Rare":
                    fishingSystem.rareFishes.Add(fish);
                    break;
                case "Epic":
                    fishingSystem.epicFishes.Add(fish);
                    break;
                case "Legendary":
                    fishingSystem.legendaryFishes.Add(fish);
                    break;
                case "Mythic":
                    fishingSystem.mythicFishes.Add(fish);
                    break;
                default:
                    Debug.LogWarning($"Unknown fish rarity: {fish.rarity}");
                    break;
            }
        }

        // FishingSystem 확률 초기화
        fishingSystem.ResetPercentages();
        fishingSystem.ApplyRareOrAboveBonus();
    }

    /// <summary>
    /// 현재 맵의 배경 이미지 UI 업데이트
    /// </summary>
    private void UpdateMapUI()
    {
        if (_currentMap == null || mapBackgroundImage == null) return;

        Sprite bgSprite = Resources.Load<Sprite>(_currentMap.backgroundSprite);
        if (bgSprite == null)
        {
            Debug.LogWarning($"Background sprite not found at path: {_currentMap.backgroundSprite}");
            return;
        }

        mapBackgroundImage.sprite = bgSprite;
    }

    /// <summary>
    /// 현재 맵 정보 가져오기
    /// </summary>
    public MapData GetCurrentMap() => _currentMap;

    /// <summary>
    /// 현재 맵에서 낚시 가능한 물고기 풀 가져오기
    /// </summary>
    public List<string> GetCurrentMapFishPool()
    {
        if (_currentMap == null) return new List<string>();
        return _currentMap.fishPool;
    }
    
    
    private Dictionary<string, FishData> _fishCache = new Dictionary<string, FishData>();

    public void FishDataCashing()
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
    
    /// <summary>
    /// MapData의 region과 FishData의 region을 비교하여 해당 지역의 FishData 리스트 가져오기
    /// </summary>
    public List<FishData> GetFishForMap()
    {
        List<FishData> result = new List<FishData>();

        if (_currentMap == null) return result;

        // FishDatabase에 있는 모든 FishData를 순회하면서 지역이 같은 것만 추가
        foreach (var fish in _fishCache.Values)
        {
            if (fish.region == _currentMap.region)
            {
                result.Add(fish);
            }
        }

        return result;
    }

}
