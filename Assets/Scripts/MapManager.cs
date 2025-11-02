using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class MapData
{
    public int id;
    public string mapName;
    public string region;
    public string backgroundSprite;
    public string description;
}

[System.Serializable]
public class StageData
{
    public int stageId;
    public string stageName;
    public int requiredCatchCount;

    public StageData(int id, string mapName)
    {
        stageId = id;
        stageName = $"{mapName} Stage {id + 1}";
        requiredCatchCount = 100 + id * 500;
    }
}

public class MapManager : Singleton<MapManager>
{
    private MapDatabase _mapDB;
    private List<MapData> _mapCache = new();
    
    
    
    [SerializeField] private MapData _currentMap;

    private int _currentMapIndex = 0;
    private int _currentStageIndex = 0;

    
    public List<StageData> stages;

    
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        LoadMapDatabase();
        SetMapByIndex(0); // 기본 맵
    }
    

    private void LoadMapDatabase()
    {
        MapDatabase _mapDB = Resources.Load<MapDatabase>("MapDatabase");
        if (_mapDB == null) { Debug.LogError ("MapDatabase asset not found in Resources!"); return; }

        _mapCache.Clear();
        foreach (MapData m in _mapDB.mapList)
        {
            if (!_mapCache.Exists(x => x.region == m.region))
                _mapCache.Add(m);
        }
    }

    public void GenerateStages(int stageCount = 10, string mapName = "")
    {
        stages = new List<StageData>();
        for (int i = 0; i < stageCount; i++)
        {
            stages.Add(new StageData(i, mapName));
        }
    }
    
    /// <summary>
    /// region 이름으로 현재 맵 설정 및 UI 갱신
    /// </summary>
    public void SetMapByIndex(int index)
    {
        string region = _mapCache[index].region;
        MapData map = _mapCache.Find(m => m.region == region);
        if (map == null) { Debug.LogWarning($"Map not found for region: {region}"); return; }

        _currentMap = map;
        _currentMapIndex = _mapCache.IndexOf(map);
        _currentStageIndex = 1;
        GenerateStages(10, region);

        Debug.Log($"Current map set to: {_currentMap.mapName}");

        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
    }


    public MapData GetCurrentMap() => _currentMap;

    // -----------------------
    // 🔽 맵 이동
    // -----------------------
    public void OnClickNextMap()
    {
        if (_mapDB == null || _mapDB.mapList.Count == 0)
            return;

        int nextIndex = _currentMapIndex + 1;
        if (nextIndex >= _mapDB.mapList.Count)
        {
            Debug.Log("Already at last map!");
            return;
        }

        SetMapByIndex(nextIndex);
    }

    public void OnClickPrevMap()
    {
        if (_mapDB == null || _mapDB.mapList.Count == 0)
            return;

        int prevIndex = _currentMapIndex - 1;
        if (prevIndex < 0)
        {
            Debug.Log("Already at first map!");
            return;
        }

        SetMapByIndex(prevIndex);
    }

    public bool HasNextMap() => _currentMapIndex < _mapDB.mapList.Count - 1;
    public bool HasPrevMap() => _currentMapIndex > 0;

    // -----------------------
    // 🔽 스테이지 기능
    // -----------------------
    public StageData GetCurrentStage()
    {
        if (_currentMap == null || stages.Count == 0) return null;
        return stages[_currentStageIndex];
    }

    public void ChangeToNextStage()
    {
        if (_currentMap == null || stages.Count == 0) return;

        if (_currentStageIndex < stages.Count - 1)
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

    public void ChangeToPrevStage()
    {
        if (_currentMap == null || stages.Count == 0) return;

        if (_currentStageIndex > 0)
        {
            _currentStageIndex--;
            Debug.Log($"Moved to stage: {GetCurrentStage().stageName}");
            EventManager.Instance.TriggerEvent(EEventType.OnStageChanged, GetCurrentStage());
        }
    }

    public bool HasNextStage() => _currentMap != null && _currentStageIndex < stages.Count - 1;
    public bool HasPrevStage() => _currentMap != null && _currentStageIndex > 0;

    public bool CanGoToNextStage()
    {
        var stage = GetCurrentStage();
        if (stage == null) return false;

        //if(GameManager.Instance.fishCaughtCount < stage.requiredCatchCount)
        //    return false;

        return HasNextStage();
    }

    public void OnTryNextStage()
    {
        if (CanGoToNextStage())
            ChangeToNextStage();
        else
            Debug.Log("조건을 만족해야 다음 스테이지로 이동 가능합니다.");
    }
}
