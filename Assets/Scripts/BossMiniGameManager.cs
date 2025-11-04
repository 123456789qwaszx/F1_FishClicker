using UnityEngine;

[CreateAssetMenu(fileName = "BossMiniGameData", menuName = "MiniGame/BossData")]
public class BossMiniGameData : ScriptableObject
{
    public string bossName;
    public int maxHP;
    public Sprite bossImagePrefab;
}

public class BossMiniGameManager : Singleton<BossMiniGameManager>
{
    public BossMiniGameData BossData;

    [HideInInspector] public int currentHP;
    [HideInInspector] public int maxHP;

    public int hitDamage = 4000;
    
    
    
    void OnEnable()
    {
        EventManager.Instance.AddEvent<BossMiniGameData>(EEventTypePayload.OnBossSpawn, SpawnBoss);
    }
    void OnDisable()
    {
        EventManager.Instance.RemoveEvent<BossMiniGameData>(EEventTypePayload.OnBossSpawn, SpawnBoss);
    }

    public void SpawnBoss(BossMiniGameData bossData)
    {
        if (bossData == null) { Debug.LogError("BossMiniGameData is missing!"); return; }

        maxHP = bossData.maxHP;
        currentHP = maxHP;

        //EventManager.Instance.TriggerEvent(EEventType.OnStartBossStage);
        
        UI_BossStage bossUI = UIManager.Instance.CurSceneUI as UI_BossStage;
        bossUI?.SetupBossUI(bossData);
    }

    /// <summary>
    /// 보스를 클릭했을 때
    /// </summary>
    public void OnBossClicked()
    {
        if (currentHP <= 0) return;

        currentHP -= hitDamage;
        if (currentHP < 0) currentHP = 0;

        EventManager.Instance.TriggerEvent(EEventType.OnAttackBoss);

        if (currentHP == 0)
        {
            Debug.Log("Boss defeated!");
            // UI 변경 등 처리
            UIManager.Instance.ChangeSceneUI<UI_InGame>();

            // 원하는 경우, 보스를 재시작하려면 StartBoss 호출 가능
            // StartBoss(); // 자동 재생성
        }
    }
}