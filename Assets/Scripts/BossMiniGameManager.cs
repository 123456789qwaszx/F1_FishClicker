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

    public int hitDamage = 1;

    void Start()
    {
        InitBoss();
    }

    void InitBoss()
    {
        if (BossData == null)
        {
            Debug.LogError("BossMiniGameData is missing!");
            return;
        }

        maxHP = BossData.maxHP;
        currentHP = maxHP;

        EventManager.Instance.TriggerEvent(EEventType.OnStartBossStage);
    }

    public void OnBossClicked()
    {
        if (currentHP <= 0) return;

        currentHP -= hitDamage;
        if (currentHP < 0) currentHP = 0;

        EventManager.Instance.TriggerEvent(EEventType.OnAttackBoss);

        if (currentHP == 0)
        {
            Debug.Log("Boss defeated!");
            // TODO: 보스 클리어 처리
        }
    }
}