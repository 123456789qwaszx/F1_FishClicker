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
        StageName = isBoss
            ? $"{mapName} Boss Stage"
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
    }

    // 스테이지 클리어 시 호출
    public void ApplyUserClearedStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex > _maxStageIndex) return;

        if (stageIndex > HighestClearedIndex)
            HighestClearedIndex = stageIndex;
    }

    public void GoToStage(int stageIndex)
    {
        if (_maxStageIndex <= 0)
        {
            Debug.Log("Cannot set current stage: _maxStageIndex is negative.");
            return;
        }

        if (stageIndex < 0 || stageIndex > _maxStageIndex)
        {
            Debug.Log($"Requested stageIndex {stageIndex} is out of range. Clamping to valid range 0~{_maxStageIndex}.");
        }

        CurrentStageIndex = Mathf.Clamp(stageIndex, 0, _maxStageIndex);
    }

    public bool GoToNextStage()
    {
        if (CurrentStageIndex < _maxStageIndex)
        {
            CurrentStageIndex++;
            return true; // 스테이지 이동 완료
        }
        else Debug.Log("Already at the last stage of this map. Need to go to next map.");

        return false; // 다음 맵 이동 필요
    }

    public bool GoToPrevStage()
    {
        if (CurrentStageIndex > 0)
        {
            CurrentStageIndex--;
            return true; // 이전 스테이지 이동 완료
        }
        else Debug.Log("Already at the first stage of this map.");

        return false; // 이전 스테이지 없음
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

        InitializeFromPlayer();
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
        
        AssignBossDataToMaps();
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

    private void AssignBossDataToMaps()
    {
        BossMiniGameData[] allBosses = Resources.LoadAll<BossMiniGameData>("Data/Bosses");

        foreach (MapData map in _mapDB.mapList)
        {
            if (map.bossData == null)
            {
                BossMiniGameData foundBoss = System.Array.Find(
                    allBosses,
                    boss => boss.name.Equals($"Boss_{map.mapName}", System.StringComparison.OrdinalIgnoreCase)
                );

                if (foundBoss == null)
                {
                    Debug.LogWarning($"No Boss data found for map '{map.mapName}'.");
                }
                else
                {
                    map.bossData = foundBoss;
                }
            }
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
    
    private Dictionary<int, int> _mapClearedStageCache;
    
    public void InitializeFromPlayer()
    {
        // 1. Player 데이터 null 방어
        if (Player.Instance == null || Player.Instance.Data == null)
        {
            Debug.LogWarning("Player data not initialized yet. Creating empty progress cache.");
            _mapClearedStageCache = new Dictionary<int, int>();
            InitializeMapProgress(0); // 맵 0으로 기본 세팅
            return;
        }

        // 2. Player 데이터의 MapClearedStageIndices 가져오기
        var playerMapData = Player.Instance.Data.MapClearedStageIndices;
        if (playerMapData == null || playerMapData.Count == 0)
        {
            Debug.Log("No cleared map data found in player data. Initializing with default map.");
            _mapClearedStageCache = new Dictionary<int, int>();
            InitializeMapProgress(0);
            return;
        }

        // 3. 캐시 복사
        _mapClearedStageCache = new Dictionary<int, int>(playerMapData);
        
        int highestMapId = -1;
        foreach (int key in _mapClearedStageCache.Keys)
        {
            if (key > highestMapId) highestMapId = key;
        }

        // 초기 맵 세팅 (예: 맵 0)
        InitializeMapProgress(highestMapId);
    }
    
    
    public void InitializeMapProgress(int globalMapId)
    {
        int globalStageID = _mapClearedStageCache.GetValueOrDefault(globalMapId);
        
        int mapId = globalMapId;
        int stageId = globalStageID % 1000; 
        
        MapData mapData = GetMapById(mapId);

        if (_mapProgress == null)
            _mapProgress = new MapProgress();

        _mapProgress.SetStageCount(mapData);
        _mapProgress.ApplyUserClearedStage(stageId);
        _mapProgress.GoToStage(stageId);

        CurrentMapData = mapData;

        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
    }
    #endregion


    #region Getter

    public MapData CurrentMapData { get; private set; }
    public StageData CurrentStageData => GetStageByMapAndStage(CurrentMapData.id, _mapProgress.CurrentStageIndex);

    public MapData GetMapById(int mapId)
    {
        if (_mapByIdCache.TryGetValue(mapId, out var map))
            return map;

        Debug.LogWarning($"Map ID {mapId} not found in cache.");
        return null;
    }
    
    public StageData GetStageById(int stageId)
    {
        if (_stageByIdCache.TryGetValue(stageId, out var stage))
            return stage;

        Debug.LogWarning($"Stage {stageId} not founded.");
        return null;
    }

    public StageData GetStageByMapAndStage(int mapId, int stageId)
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
        MapData mapData = GetMapById(mapId);
        if (mapData == null)
        {
            Debug.LogError($"[MapManager] Cannot initialize MapProgress: Map ID {mapId} not found.");
            return;
        }

        _mapProgress.SetStageCount(mapData);

        int startStageIndex = 0;

        if (_mapClearedStageCache != null && _mapClearedStageCache.TryGetValue(mapId, out int globalStageId))
        {
            startStageIndex = globalStageId % 1000;
            Debug.Log($"[MapManager] Restoring progress for Map {mapId}, Stage {startStageIndex}");
        }
        else
        {
            Debug.Log($"[MapManager] No cached progress found for Map {mapId}. Starting from Stage 0");
        }

        MoveToStage(startStageIndex);

        CurrentMapData = mapData;

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