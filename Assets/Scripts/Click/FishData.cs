using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public RarityType rarity;
    public float catchProbability; // 0~1
    public int baseValue;
}

public enum RarityType
{
    N,
    R,
    SR,
    LEGEND
}