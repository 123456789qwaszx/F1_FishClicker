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
    
    public List<StageData> stages;
    public int highestCleared = -1;
    
    public void GenerateStages(int stageCount = 10)
    {
        stages = new List<StageData>();
        for (int i = 0; i < stageCount; i++)
        {
            stages.Add(new StageData(i, mapName));
        }
    }
    
    public void MarkStageCleared(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stages.Count) return;

        if (stageIndex > highestCleared)
            highestCleared = stageIndex;
    }
}

[System.Serializable]
public class StageData
{
    public int StageId { get; private set; }
    public string StageName { get; private set; }
    public int requiredCatchCount;

    public StageData(int id, string mapName)
    {
        StageId = id;
        StageName = $"{mapName} Stage {id + 1}";
        requiredCatchCount = Formula.GetStageCatchLinear(id);
    }
}

public class MapManager : Singleton<MapManager>
{
    private MapDatabase _mapDB;
    private List<MapData> _mapCache = new();
    
    private int _currentMapIndex = 0;
    private MapData _currentMap;
    public MapData GetCurrentMap() => _currentMap;
    
    private int _currentStageIndex = 0;
    public StageData CurrentStage() => _currentMap.stages[_currentStageIndex];
    
    public bool CanGoToNextMap()
    {
        if (_currentMapIndex >= _mapCache.Count - 1) { Debug.Log($"Map index {_currentMapIndex} out of range!"); return false; }

        MapData nextMap = _mapCache[_currentMapIndex + 1];
        StageData firstStage = nextMap.stages[0];

        if (GameManager.Instance.fishCaughtCount < firstStage.requiredCatchCount)
        {
            Debug.Log($"Cannot go to next map: need at least {firstStage.requiredCatchCount} fish caught, current count is {GameManager.Instance.fishCaughtCount}");
            return false;
        }

        return true;
    }
    
    public bool CanGoToNextStage()
    {
        if (_currentStageIndex >= _currentMap.stages.Count - 1) { Debug.Log($"Stage index {_currentStageIndex} out of range!"); return false; }

        StageData nextStage = _currentMap.stages[_currentStageIndex + 1];

        if (GameManager.Instance.fishCaughtCount < nextStage.requiredCatchCount)
        {
            Debug.Log($"Cannot go to next stage: need at least {nextStage.requiredCatchCount} fish caught, current count is {GameManager.Instance.fishCaughtCount}");
            return false;
        }

        return true;
    }

    
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        LoadMapDatabase();
        BuildCache();
        ChangeMap(0); // 기본 맵
    }
    

    private void LoadMapDatabase()
    {
        _mapDB = Resources.Load<MapDatabase>("MapDatabase");
        if (_mapDB == null) { Debug.LogError ("MapDatabase asset not found in Resources!"); return; }

        int TT_StageCount = 10;
        int TT_StageCleared = 0;
        foreach (MapData m in _mapDB.mapList)
        {
            m.GenerateStages(TT_StageCount);
            m.MarkStageCleared(TT_StageCleared);
        }
    }
    
    private void BuildCache()
    {
        _mapCache.Clear();
        foreach (MapData m in _mapDB.mapList)
        {
            if (m == null) continue;
            if (!_mapCache.Exists(x => x.region == m.region))
                _mapCache.Add(m);
        }
    }

    
    public void ChangeMap(int index)
    {
        if (_mapCache == null || _mapCache.Count == 0) return;
        if (index < 0 || index >= _mapCache.Count) { Debug.Log($"Map index {index} out of range!"); return; }
        
        _currentMap = _mapCache[index];
        _currentMapIndex = index;
        _currentStageIndex = _currentMap.highestCleared + 1;
        
        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
        
        Debug.Log($"Current map set to: {_currentMap.mapName}");
    }
    

    public void ChangeStage(int index)
    {
        if (_currentMap == null) return;
        int targetIndex = index - 1;
        if (targetIndex < 0 || targetIndex >= _currentMap.stages.Count) { Debug.Log("Stage index out of range!"); return; }

        _currentStageIndex = targetIndex;
        
        Debug.Log($"Moved to stage: {CurrentStage().StageName}");
    }
    
    #region Button
    public void OnChangeMapToNext()
    {
        int nextIndex = _currentMapIndex + 1;
        if (nextIndex > _mapCache.Count) { Debug.Log("Already at last map!"); return; }

        ChangeMap(nextIndex);
    }

    public void OnChangeMapToPrev()
    {
        int prevIndex = _currentMapIndex - 1;
        if (prevIndex < 0) { Debug.Log("Already at first map!"); return; }

        ChangeMap(prevIndex);
    }
    
    public void OnChangeStageToNext()
    {
        int nextIndex = _currentStageIndex + 1;
        if (nextIndex >= _currentMap.stages.Count) { Debug.Log("Already at last stage!"); return; }

        ChangeStage(nextIndex);
    }

    public void OnChangeStageToPrev()
    {
        int prevIndex = _currentStageIndex;
        if (prevIndex <= 0) { Debug.Log("Already at first stage!"); return; }

        ChangeStage(prevIndex - 1);
    }
    #endregion
}
