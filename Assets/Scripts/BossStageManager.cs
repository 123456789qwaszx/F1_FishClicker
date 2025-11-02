using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BossMiniGameData", menuName = "MiniGame/BossData")]
public class BossMiniGameData : ScriptableObject
{
    public string bossName;
    public int maxHP;
    public int hitDamage;
    public GameObject bossPrefab;
    public GameObject breakParticlePrefab;
}


public class BossMiniGameManager : Singleton<BossMiniGameManager>
{
    [FormerlySerializedAs("bossData")] public BossMiniGameData BossData;
    
    public int CurrentHP;
    private Animator bossAnimator;

    void Start()
    {
        InitBoss();
    }


    void InitBoss()
    {
        if (BossData == null) { Debug.LogError("BossMiniGameData is missing!"); return; }
        
        CurrentHP = BossData.maxHP;

        GameObject bossObj = Instantiate(BossData.bossPrefab);
        bossAnimator = bossObj.GetComponent<Animator>();

        EventManager.Instance.TriggerEvent(EEventType.OnStartBossStage);
    }

    public void OnBossClicked()
    {
        if (CurrentHP <= 0) return;

        CurrentHP -= BossData.hitDamage;
        if (CurrentHP < 0) CurrentHP = 0;

        EventManager.Instance.TriggerEvent(EEventType.OnAttackBoss);

        if (CurrentHP == 0)
        {
        }
    }
}
