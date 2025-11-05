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
    public BossMiniGameData bossData;
    
    public List<StageData> stages;
}


[System.Serializable]
public class StageData
{
    public int StageId { get; private set; }
    public string StageName { get; private set; }
    public int requiredCatchCount;
    public bool IsBossStage { get; private set; }

    public StageData(int id, string mapName, bool isBoss = false)
    {
        StageId = id;
        IsBossStage = isBoss;
        StageName = isBoss ? $"{mapName} Boss Stage"
            : $"{mapName} Stage {id + 1}";
        requiredCatchCount = Formula.GetStageCatchLinear(id);
    }
}


public class MapProgress
{
    private int _maxStageIndex = 0;

    public int HighestClearedIndex { get; private set; } = -1;

    public int CurrentStageIndex { get; private set; } = 0;
    public int NextStageIndex => Mathf.Clamp(CurrentStageIndex + 1, 0, _maxStageIndex);

    
    public void SetStageCount(MapData mapData)
    {
        int stageCount = mapData.stages.Count;
        
        _maxStageIndex = stageCount > 0 ? stageCount - 1 : 0;

        if (HighestClearedIndex > _maxStageIndex)
            HighestClearedIndex = _maxStageIndex;

        CurrentStageIndex = Mathf.Clamp(NextStageIndex, 0, _maxStageIndex);
    }

    // 스테이지 클리어 시 호출
    public void SetCleared(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex > _maxStageIndex) return;

        if (stageIndex > HighestClearedIndex)
            HighestClearedIndex = stageIndex;
    }

    public void GoToStage(int stageIndex)
    {
        if (_maxStageIndex <= 0) { Debug.Log("Cannot set current stage: _maxStageIndex is negative."); return; }
        if (stageIndex < 0 || stageIndex > _maxStageIndex) { Debug.Log($"Requested stageIndex {stageIndex} is out of range. Clamping to valid range 0~{_maxStageIndex}."); }

        CurrentStageIndex = Mathf.Clamp(stageIndex, 0, _maxStageIndex);
    }
    
    public bool GoToNextStage()
    {
        if (CurrentStageIndex < _maxStageIndex)
        {
            CurrentStageIndex++;
            return true; // 스테이지 이동 완료
        }
        else Debug.Log("Already at the last stage of this map. Need to go to next map."); return false; // 다음 맵 이동 필요
        
    }

    public bool GoToPrevStage()
    {
        if (CurrentStageIndex > 0)
        {
            CurrentStageIndex--;
            return true; // 이전 스테이지 이동 완료
        }
        else Debug.Log("Already at the first stage of this map."); return false; // 이전 스테이지 없음
    }
}


public class MapManager : Singleton<MapManager>
{
    private MapDatabase _mapDB;

    private readonly Dictionary<int, MapData> _mapByIdCache = new();
    private readonly Dictionary<int, StageData> _stageByIdCache = new();
    
    private MapProgress _mapProgress;


    #region Init

    public void Init()
    {
        LoadMapSheet();
        BuildMapCache();
        BuildStageCache();
        
        ChangeMap(0);
        InitProgress();
    }
    
    private void LoadMapSheet()
    {
        _mapDB = Resources.Load<MapDatabase>(StringNameSpace.ResourcePaths.MapDataPath);
        if (_mapDB == null)
        {
            Debug.LogError("MapDatabase asset not found in Resources");
            return;
        }

        foreach (MapData map in _mapDB.mapList)
        {
            GenerateStages(map);
        }
    }
    
    public void GenerateStages(MapData map, int stageCount = 10)
    {
        map.stages = new List<StageData>();
        for (int i = 0; i < stageCount; i++)
        {
            bool isBossStage = (i == stageCount - 1);
            map.stages.Add(new StageData(i, map.mapName, isBossStage));
        }
    }
    
    private void BuildMapCache()
    {
        _mapByIdCache.Clear();

        foreach (MapData map in _mapDB.mapList)
        {
            if (map == null) continue;
            if (!_mapByIdCache.ContainsKey(map.id))
            {
                _mapByIdCache.Add(map.id, map);
            }
        }
    }
    
    private void BuildStageCache()
    {
        _stageByIdCache.Clear();

        foreach (MapData map in _mapByIdCache.Values)
        {
            foreach (StageData stage in map.stages)
            {
                int globalStageId = map.id * 1000 + stage.StageId;

                if (!_stageByIdCache.ContainsKey(globalStageId))
                    _stageByIdCache.Add(globalStageId, stage);
            }
        }
    }
    
    private void InitProgress()
    {
        if (CurrentMapData == null)
        {
            Debug.LogError("Cannot initialize MapProgress: CurrentMap is null.");
            return;
        }

        _mapProgress = new MapProgress();
    }

    #endregion
    
    
    #region Getter

    public MapData CurrentMapData { get; private set; }
    public StageData CurrentStageData => GetStageById(CurrentMapData.id, _mapProgress.CurrentStageIndex);
    
    public MapData GetMapById(int mapId)
    {
        if (_mapByIdCache.TryGetValue(mapId, out var map))
            return map;

        Debug.LogWarning($"Map ID {mapId} not found in cache.");
        return null;
    }

    public StageData GetStageById(int mapId, int stageId)
    {
        int globalStageId = mapId * 1000 + stageId;
        if (_stageByIdCache.TryGetValue(globalStageId, out var stage))
            return stage;

        Debug.LogWarning($"Stage {stageId} not found in Map {mapId}.");
        return null;
    }

    #endregion
    
    #region Map/Stage Control
    
    public void ChangeMap(int mapId)
    {
        CurrentMapData = GetMapById(mapId);
        _mapProgress.SetStageCount(CurrentMapData);
        
        //유저데이터로 가져오기... 여기보단 Init 권장
        _mapProgress.SetCleared(5);
        _mapProgress.GoToStage(_mapProgress.HighestClearedIndex + 1);
        // 완료

        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
    }
    
    public void MoveToNextMap()
    {
        int nextMapId = CurrentMapData.id + 1;

        if (_mapByIdCache.ContainsKey(nextMapId))
        {
            ChangeMap(nextMapId);

            // 다음 맵의 첫 스테이지로 이동
            _mapProgress.GoToStage(0);

            EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
        }
        else
        {
            Debug.Log("Already at the last stage of the last map. Cannot move to next map.");
        }
    }
    
    public void MoveToPrevMap()
    {
        int prevMapId = CurrentMapData.id - 1;

        if (_mapByIdCache.ContainsKey(prevMapId))
        {
            ChangeMap(prevMapId);

            // 이전 맵의 마지막 스테이지로 이동
            _mapProgress.GoToStage(CurrentMapData.stages.Count - 1);

            EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
        }
        else
        {
            Debug.Log("Already at the first stage of the first map. Cannot move to previous map.");
        }
    }
    
    public void MoveToStage(int stageIndex)
    {
        _mapProgress.GoToStage(stageIndex);
        
        EventManager.Instance.TriggerEvent(EEventType.OnStageChanged);
    }

    
    public void MoveToNextStage()
    {
        bool moved = _mapProgress.GoToNextStage();
        
        if (moved)
            EventManager.Instance.TriggerEvent(EEventType.OnStageChanged);
        else
            MoveToNextMap(); // 필요 시 다음 맵 이동 처리
    }
    
    public void MoveToPrevStage()
    {
        bool moved = _mapProgress.GoToPrevStage();
        
        if (!moved)
        {
            // 이전 맵 존재 여부 확인
            int prevMapId = CurrentMapData.id - 1;
            if (_mapByIdCache.ContainsKey(prevMapId))
            {
                ChangeMap(prevMapId);
                _mapProgress.GoToStage(CurrentMapData.stages.Count - 1);
                EventManager.Instance.TriggerEvent(EEventType.OnStageChanged);
            }
            else
            {
                Debug.Log("Already at the first stage of the first map.");
            }
        }
        else
        {
            EventManager.Instance.TriggerEvent(EEventType.OnStageChanged);
        }
    }
    #endregion
}