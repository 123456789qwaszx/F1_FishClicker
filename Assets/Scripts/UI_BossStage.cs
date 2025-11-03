using System.Collections;
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

    [Header("Timer UI")]
    public Slider Slider_Timer;
    public TextMeshProUGUI Txt_MaxTime;
    public TextMeshProUGUI Txt_RemainingTime;

    private float bossTimer;
    private float maxTime;

    void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.OnStartBossStage, InitUI);
        EventManager.Instance.AddEvent(EEventType.OnAttackBoss, OnBossAttacked);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.OnStartBossStage, InitUI);
        EventManager.Instance.RemoveEvent(EEventType.OnAttackBoss, OnBossAttacked);
    }

    void InitUI()
    {
        if (BossMiniGameManager.Instance.BossData == null) return;

        BossMiniGameData bossData = BossMiniGameManager.Instance.BossData;

        Txt_BossName.text = bossData.bossName;
        Slider_BossHP.maxValue = bossData.maxHP;
        Slider_BossHP.value = BossMiniGameManager.Instance.currentHP;

        UpdateBossHp();

        Img_Boss.sprite = bossData.bossImagePrefab;

        // 타이머 초기화
        maxTime = 60f; // 예시 최대 시간
        bossTimer = maxTime;

        Slider_Timer.maxValue = maxTime;
        Slider_Timer.value = bossTimer;
        Txt_MaxTime.text = Mathf.CeilToInt(maxTime).ToString();
        Txt_RemainingTime.text = Mathf.CeilToInt(bossTimer).ToString();

        StartCoroutine(BossTimerCoroutine());
    }
    

    void OnBossAttacked()
    {
        UpdateBossHp();
    }

    void UpdateBossHp()
    {
        int hp = BossMiniGameManager.Instance.currentHP;
        int maxHp = BossMiniGameManager.Instance.maxHP;

        Slider_BossHP.value = hp;
        Txt_BossHP.text = $"{hp}/{maxHp}";
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
    }
}
