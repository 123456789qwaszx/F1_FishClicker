using System.Collections;
using TMPro;
using UnityEngine;

public class UI_BossStage : UI_Scene
{
    [Header("UI")]
    public RectTransform hpBar;
    public TextMeshProUGUI hpText;
    public GameObject clickBlocker;
    public GameObject healText;
    public Canvas mainCanvas;
    public GameObject miniGameClearUI;

    private int maxHP;
    private int currentHP;
    private float originalHPBarWidth;

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

    // 초기 UI 설정
    void InitUI()
    {
        clickBlocker.SetActive(false);
        healText.SetActive(false);

        maxHP = BossMiniGameManager.Instance.BossData.maxHP;
        currentHP = maxHP;
        originalHPBarWidth = hpBar.sizeDelta.x;

        UpdateHPUI();
    }

    // 보스가 공격받았을 때
    void OnBossAttacked()
    {
        currentHP = BossMiniGameManager.Instance.CurrentHP;
        UpdateHPUI();
        PlayCrackEffect();
    }

    // 보스가 회복했을 때
    void OnBossHealed()
    {
        currentHP = BossMiniGameManager.Instance.CurrentHP;
        UpdateHPUI();
        // 필요 시 힐 이펙트
    }

    // HP Bar, Text 업데이트
    void UpdateHPUI()
    {
        float ratio = (float)currentHP / maxHP;
        hpBar.sizeDelta = new Vector2(originalHPBarWidth * ratio, hpBar.sizeDelta.y);
        hpText.text = $"{currentHP} / {maxHP}";
    }

    // 공격 이펙트 재생
    void PlayCrackEffect()
    {
        // 예: Particle, 사운드 등
    }

    void ShowClearUI()
    {
        GameObject clearUI = Instantiate(miniGameClearUI, mainCanvas.transform);
        clearUI.transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp(clearUI.GetComponent<RectTransform>(), 0.5f));
    }

    IEnumerator ScaleUp(RectTransform rect, float duration)
    {
        float time = 0f;
        Vector3 from = Vector3.zero;
        Vector3 to = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);
            rect.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        rect.localScale = to;
    }
}
