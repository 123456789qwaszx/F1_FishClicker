using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    [Header("UI")]
    [SerializeField] private Image mapBackgroundImage; // 배경 이미지 UI

    [SerializeField] private MapDatabase _mapDB; // CSV → ScriptableObject로 임포트된 맵 데이터
    [SerializeField] private MapData _currentMap; // 현재 맵
    [SerializeField] private Dictionary<string, MapData> _mapCache = new();

    private int _currentMapIndex = 0;
    private int _currentStageIndex = 0; // 현재 스테이지

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        LoadMapDatabase();
        SetCurrentMap("DeepSea"); // 기본 맵
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
    /// region 이름으로 현재 맵 설정 및 UI 갱신
    /// </summary>
    public void SetCurrentMap(string region)
    {
        if (!_mapCache.TryGetValue(region, out MapData map))
        {
            Debug.LogWarning($"Map not found for region: {region}");
            return;
        }

        _currentMap = map;
        _currentMapIndex = _mapDB.mapList.IndexOf(map);

        // 스테이지 기본값
        _currentStageIndex = 0;

        UpdateMapUI();
        Debug.Log($"Current map set to: {_currentMap.mapName}");

        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
        EventManager.Instance.TriggerEvent(EEventType.OnMapChangedWithData, _currentMap);
        EventManager.Instance.TriggerEvent(EEventType.OnStageChanged, GetCurrentStage());
    }

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

    public MapData GetCurrentMap() => _currentMap;

    // -----------------------
    // 🔽 맵 이동
    // -----------------------
    public void OnClickNextMap()
    {
        if (_mapDB == null || _mapDB.mapList.Count == 0) return;

        int nextIndex = _currentMapIndex + 1;
        if (nextIndex >= _mapDB.mapList.Count)
        {
            Debug.Log("Already at last map!");
            return;
        }

        SetCurrentMap(_mapDB.mapList[nextIndex].region);
    }

    public void OnClickPrevMap()
    {
        if (_mapDB == null || _mapDB.mapList.Count == 0) return;

        int prevIndex = _currentMapIndex - 1;
        if (prevIndex < 0)
        {
            Debug.Log("Already at first map!");
            return;
        }

        SetCurrentMap(_mapDB.mapList[prevIndex].region);
    }

    public bool HasNextMap() => _currentMapIndex < _mapDB.mapList.Count - 1;
    public bool HasPrevMap() => _currentMapIndex > 0;

    // -----------------------
    // 🔽 스테이지 기능 추가
    // -----------------------
    public StageData GetCurrentStage()
    {
        if (_currentMap == null || _currentMap.stages.Count == 0) return null;
        return _currentMap.stages[_currentStageIndex];
    }

    public void OnClickNextStage()
    {
        if (_currentMap == null || _currentMap.stages.Count == 0) return;

        if (_currentStageIndex < _currentMap.stages.Count - 1)
        {
            _currentStageIndex++;
            Debug.Log($"Moved to stage: {GetCurrentStage().stageName}");
            EventManager.Instance.TriggerEvent(EEventType.OnStageChanged, GetCurrentStage());
        }
        else
        {
            Debug.Log("Already at last stage in this map!");
        }
    }

    public void OnClickPrevStage()
    {
        if (_currentMap == null || _currentMap.stages.Count == 0) return;

        if (_currentStageIndex > 0)
        {
            _currentStageIndex--;
            Debug.Log($"Moved to stage: {GetCurrentStage().stageName}");
            EventManager.Instance.TriggerEvent(EEventType.OnStageChanged, GetCurrentStage());
        }
    }

    public bool HasNextStage() => _currentMap != null && _currentStageIndex < _currentMap.stages.Count - 1;
    public bool HasPrevStage() => _currentMap != null && _currentStageIndex > 0;
}
