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
    private Coroutine timerCoroutine;

    void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.OnAttackBoss, OnBossAttacked);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.OnAttackBoss, OnBossAttacked);
    }

    /// <summary>
    /// 보스 UI 초기화 및 타이머 재시작
    /// </summary>
    public void SetupBossUI(BossMiniGameData bossData)
    {
        Txt_BossName.text = bossData.bossName;
        Img_Boss.sprite = bossData.bossImagePrefab;

        // 보스 HP 초기화
        Slider_BossHP.maxValue = bossData.maxHP;
        Slider_BossHP.value = BossMiniGameManager.Instance.currentHP;
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
        Debug.Log("Boss time over!");
    }
}
