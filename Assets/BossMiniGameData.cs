using UnityEngine;

[CreateAssetMenu(fileName = "BossMiniGameData", menuName = "MiniGame/BossData")]
public class BossMiniGameData : ScriptableObject
{
    public string bossName;
    public int maxHP;
    public Sprite bossImagePrefab;
}