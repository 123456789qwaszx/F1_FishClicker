using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Game Data/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public List<FishData> fishList = new List<FishData>();
}