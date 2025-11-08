using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectStageScene : UI_Scene
{
    enum GameObjects
    {
        Player,
        SelectedChapter1,
        SelectedChapter2,
        SelectedChapter3,
        SelectedChapter4,
        SelectedChapter5,
        SelectedChapter6,
        SelectedChapter7,
    }

    enum Buttons
    {
        ChapterButton_1,
        ChapterButton_2,
        ChapterButton_3,
        ChapterButton_4,
        ChapterButton_5,
        ChapterButton_6,
        ChapterButton_7,
    }

    enum Images
    {
        MapTop,
        MapBottom,
        MapCenter1,
        MapCenter2,
    }

    [SerializeField] UI_StageBlock[] _stageBlockUI = new UI_StageBlock[10];
    ScrollRect scroll;

    UI_SelectStageSceneTop _topUI;
    UI_SelectStageSceneBottom _bottomUI;

    void OnEnable()
    {
        EventManager.Instance.AddEvent<MapData>(EEventTypePayload.OnMapChanged, OnMapChanged);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveEvent<MapData>(EEventTypePayload.OnMapChanged, OnMapChanged);
    }

    protected override void Awake()
    {
        base.Init();

        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        // 버튼 이벤트 바인딩
        for (int i = 1; i <= 7; i++)
        {
            int chapter = i; // 클로저 문제 방지
            GetButton((int)(Buttons)chapter - 1).gameObject
                .BindEvent((eventData) => OnClickChapterButton(eventData, chapter));
        }

        scroll = ComponentHelper.FindChildObject<ScrollRect>(gameObject, recursive: true);

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i] =
                ComponentHelper.FindChildObject<UI_StageBlock>(gameObject, $"UI_StageBlock{i + 1}", recursive: true);
        }
    }

    public void Init()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        MapData currentMap = MapManager.Instance.CurrentMapData;

        if (currentMap == null) return;

        // 스테이지 UI 갱신
        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i].SetInfo(i + 1, this);
        }

        // 챕터 선택 표시 갱신
        DisableButtonSelectedImage();
        EnableButtonSelectedImage(currentMap.id);

        // 플레이어 표시 갱신 (첫 스테이지)
        UpdatePlayerMarkerUI(1);
        SetScrollPosition(1);
    }

    void DisableButtonSelectedImage()
    {
        for (int i = 0; i < 7; i++)
        {
            GetObject((int)(GameObjects)i + 1).SetActive(false);
        }
    }

    void EnableButtonSelectedImage(int chapterId)
    {
        switch (chapterId)
        {
            case 1: GetObject((int)GameObjects.SelectedChapter1).SetActive(true); break;
            case 2: GetObject((int)GameObjects.SelectedChapter2).SetActive(true); break;
            case 3: GetObject((int)GameObjects.SelectedChapter3).SetActive(true); break;
            case 4: GetObject((int)GameObjects.SelectedChapter4).SetActive(true); break;
            case 5: GetObject((int)GameObjects.SelectedChapter5).SetActive(true); break;
            case 6: GetObject((int)GameObjects.SelectedChapter6).SetActive(true); break;
            case 7: GetObject((int)GameObjects.SelectedChapter7).SetActive(true); break;
        }
    }

    void SetScrollPosition(int stageNum)
    {
        scroll.verticalNormalizedPosition = (stageNum - 1f) / (_stageBlockUI.Length - 1f);
    }

    void UpdatePlayerMarkerUI(int stage)
    {
        var player = GetObject((int)GameObjects.Player);
        player.SetActive(false);
        int index = stage - 1;
        player.transform.position = _stageBlockUI[index].transform.position;
        player.SetActive(true);
    }

    #region EventHandler

    void OnClickChapterButton(PointerEventData eventData, int chapter)
    {
        MapManager.Instance.ChangeMap(chapter - 1); // MapManager에서 처리, UI는 이벤트로 갱신
        
        MapManager.Instance.MoveToStage(9);
        FishingManager.Instance.Controller.SpawnNewFish();
        UIManager.Instance.ChangeSceneUI<UI_BossStage>(stage =>
            stage.SetupBossUI(FishingManager.Instance.Controller.CurFish)
        );
    }

    void OnMapChanged(MapData mapData)
    {
        MapData currentMap = mapData;
        if (currentMap == null) return;

        RefreshUI();
    }

    public void OnSelectStage(int stageIndex)
    {
        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            if (i + 1 != stageIndex)
                _stageBlockUI[i].SelectStage(false);
        }

        UpdatePlayerMarkerUI(stageIndex);
        SetScrollPosition(stageIndex);
    }

    #endregion
}