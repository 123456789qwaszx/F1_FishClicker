using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishData
{
    public int id;
    public string fishName;
    public string rarity;
    public long baseValue;
    public bool isBossFish;
    public string spritePath;
    public string description;
    public string region;
}

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Game Data/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public List<FishData> fishList = new List<FishData>();
}