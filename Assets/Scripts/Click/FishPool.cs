using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishPool", menuName = "GameData/FishPool")]
public class FishPool : ScriptableObject
{
    public string mapName;
    public List<FishData> fishList;
}