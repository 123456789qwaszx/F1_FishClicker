using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossStage : UI_Scene
{
    [Header("Boss UI")]
    public TextMeshProUGUI Txt_BossName;
    public Image Img_Boss;
    public Slider Slider_BossHP;
    public TextMeshProUGUI Txt_BossHP;
    public Button Btn_Back;
    public Button Btn_Skip;

    [Header("Timer UI")]
    public Slider Slider_Timer;
    public TextMeshProUGUI Txt_MaxTime;
    public TextMeshProUGUI Txt_RemainingTime;

    private float bossTimer;
    private float maxTime;
    private Coroutine timerCoroutine;

    void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.OnAttackFish, UpdateBossHp);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.OnAttackFish, UpdateBossHp);
    }

    protected override void Awake()
    {
        
        gameObject.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 보스 UI 초기화 및 타이머 재시작
    /// </summary>
    public void SetupBossUI(FishData fishData)
    {
        Txt_BossName.text = FishingManager.Instance.Controller.CurFish.fishName;
        //Img_Boss.sprite = Resources.Load(fishData.spritePath);
        
        // 보스 HP 초기화
        UpdateBossHp();

        // 타이머 초기화
        maxTime = 60f; // 필요시 데이터 기반으로 설정 가능
        bossTimer = maxTime;

        Slider_Timer.maxValue = maxTime;
        Slider_Timer.value = bossTimer;
        Txt_MaxTime.text = Mathf.CeilToInt(maxTime).ToString();
        Txt_RemainingTime.text = Mathf.CeilToInt(bossTimer).ToString();

        // 기존 코루틴이 돌고 있으면 중지 후 재시작
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(BossTimerCoroutine());
    }

    void UpdateBossHp()
    {
        double curHp = FishingManager.Instance.Controller.CurFishHp;
        double maxHp = FishingManager.Instance.Controller.CurFishMaxHp;
        
        float ratio = (float)(curHp / maxHp);
        
        Slider_BossHP.maxValue = 1;
        Slider_BossHP.value = ratio;
        Txt_BossHP.text = $"{curHp}/{maxHp}";
    }

    private IEnumerator BossTimerCoroutine()
    {
        while (bossTimer > 0)
        {
            bossTimer -= Time.deltaTime;
            Slider_Timer.value = bossTimer;
            Txt_RemainingTime.text = Mathf.CeilToInt(bossTimer).ToString();
            yield return null;
        }

        bossTimer = 0;
        Slider_Timer.value = 0;
        Txt_RemainingTime.text = "0";

        // TODO: 타임 오버 처리
        Debug.Log("Boss time over!");
    }

    public void OnReturnToStage()
    {
        MapManager.Instance.MoveToPrevStage();
        GameEventHelper.OnReturnToStage();
    }
    
    
    public void OnSkipStage()
    {
        MapManager.Instance.MoveToNextMap();
        GameEventHelper.OnReturnToStage();
    }
}
